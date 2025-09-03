using Microsoft.AspNetCore.Mvc;
using TaskList_Server.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using TaskList_Server.Interface;

namespace TaskList_Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskListService _taskService;

        public TasksController(ITaskListService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        [EnableRateLimiting("GeneralLimiter")]

        public async Task<ActionResult<object>> GetTasks(int page = 1, int pageSize = 20, string filter = "true", string search = "", string staus = "")
        {
            var customerId = User.FindFirst("customerId")?.Value;
            if (string.IsNullOrEmpty(customerId)) return BadRequest();
            var result = await _taskService.GetTasksAsync(filter, search, staus, page, pageSize, customerId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                    return NotFound();

                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [EnableRateLimiting("WriteLimiter")]
        public async Task<IActionResult> CreateTask([FromForm] CreateTaskDto dto)
        {
            var customerId = Convert.ToInt32(User.FindFirst("customerId")?.Value);
            var result = await _taskService.CreateTaskAsync(dto, customerId);
            if (result.Success)
                return Ok(new { message = result.Message, taskId = result.TaskId });
            return StatusCode(500, new { message = result.Message });
        }


        [HttpPut("{id}")]
        [EnableRateLimiting("WriteLimiter")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateTask(int id, [FromForm] TaskDto dto)
        {
            if (id != dto.TaskId)
                return BadRequest("Task ID mismatch");

            var result = await _taskService.UpdateTaskAsync(id, dto);

            if (!result.Success)
                return StatusCode(500, new { message = result.Message });

            return Ok(new { message = result.Message });
        }


        [HttpDelete("{id}")]
        [EnableRateLimiting("WriteLimiter")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var result = await _taskService.DeleteTaskAsync(id);

            if (!result.Success)
                return StatusCode(500, new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpGet("statuses")]
        public async Task<ActionResult<IEnumerable<StatusDto>>> GetStatuses()
        {
            var statuses = await _taskService.GetStatusesAsync();
            return Ok(statuses);
        }

        [HttpGet("application")]
        public async Task<ActionResult<IEnumerable<ProjectsDto>>> GetProjectList()
        {
            var projects = await _taskService.GetProjectListAsync();
            return Ok(projects);
        }

        [HttpGet("priority")]
        public async Task<ActionResult<IEnumerable<PriorityDto>>> GetPriorityList()
        {
            var priorities = await _taskService.GetPriorityListAsync();
            return Ok(priorities);
        }

        [HttpGet("get_developers")]
        public async Task<ActionResult<IEnumerable<DeveloperDto>>> GetDevelopers()
        {
            var developers = await _taskService.GetDevelopersAsync();
            return Ok(developers);
        }

        [HttpGet("get_taskCounts")]
        public async Task<ActionResult<TaskCountsDto>> GetCounts()
        {
            var customerId = User.FindFirst("customerId")?.Value;
            if (string.IsNullOrEmpty(customerId))
                return Unauthorized("CustomerId not found in token.");

            var counts = await _taskService.GetCountsAsync(customerId);
            return Ok(counts);
        }


        [HttpGet("report")]
        public async Task<ActionResult<IEnumerable<TasksReportDto>>> GetTasksReport([FromQuery] ReportFilters filters)
        {
            try
            {
                var result = await _taskService.GetTasksReportAsync(filters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while generating the report.", detail = ex.Message });
            }
        }

        [HttpGet("file/{id}")]
        public async Task<ActionResult<TaskFileDto>> GetFileContent(int id)
        {
            var fileDto = await _taskService.GetFileContentAsync(id);
            if (fileDto == null)
                return NotFound();

            return Ok(fileDto);
        }
    }
}
