using Microsoft.AspNetCore.Mvc;
using TaskList_Server.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using TaskList_Server.Interface;
using Microsoft.EntityFrameworkCore;
using TaskList_Server.Models;
using TaskList_Server.Data;
using TaskList_Server.Service;

namespace TaskList_Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskListService _taskService;
        private readonly IProjectService _projectService;


        public TasksController(ITaskListService taskService, IProjectService projectService)
        {
            _taskService = taskService;
            _projectService = projectService;
        }

        [HttpGet]
        [EnableRateLimiting("GeneralLimiter")]

        public async Task<ActionResult<object>> GetTasks(int page = 1, int pageSize = 20, string filter = "true", string search = "", string staus = "", int developerId = 0, int projectId=0)
        {
            var customerId = User.FindFirst("customerId")?.Value;
            if (string.IsNullOrEmpty(customerId)) return BadRequest();
            var result = await _taskService.GetTasksAsync(filter, search, staus, page, pageSize, customerId,developerId,projectId);
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
        public async Task<ActionResult<IEnumerable<StatusDto>>> GetStatuses() => Ok(await _taskService.GetStatusesAsync());

        [HttpGet("application")]
        public async Task<ActionResult<IEnumerable<ProjectsDto>>> GetProjectList()
        {
            var customerId = User.FindFirst("customerId")?.Value;
            if (customerId == null) return NotFound();
            var projects = await _taskService.GetProjectListAsync(customerId);
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
            var customerId = User.FindFirst("customerId")?.Value;
            if (customerId == null) return NotFound();
            var developers = await _taskService.GetDevelopersAsync(customerId);
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

        [HttpGet("get_allProjects")]
        public async Task<IEnumerable<TblApplication>> GetAllProjectList()
        {
            return await _projectService.GetAllProjectsAsync();
        }

        [HttpPost("createProject")]
        public async Task<ActionResult<TblApplication>> CreateProject([FromBody] TblApplication app)
        {
            if (string.IsNullOrWhiteSpace(app.ChrApplicationName))
                return BadRequest("Project name is required");

            var customerId = User.FindFirst("customerId")?.Value;
            var created = await _projectService.CreateProjectAsync(app, customerId);

            return CreatedAtAction(nameof(GetAllProjectList), new { id = created.IntId }, created);
        }

        [HttpPut("updateProject/{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] TblApplication app)
        {
            var customerId = User.FindFirst("customerId")?.Value;
            var updated = await _projectService.UpdateProjectAsync(id, app, customerId);

            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("deleteProject/{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var deleted = await _projectService.DeleteProjectAsync(id);
            if (!deleted) return NotFound();

            return Ok(new { message = "Project deleted successfully" });
        }

        [HttpGet("employeeTaskStats")]
        public async Task<ActionResult<IEnumerable<EmployeeTaskStatsDto>>> GetEmployeeTaskStats([FromQuery] string fromDate, [FromQuery] string toDate)
        {
            DateTime from = Convert.ToDateTime(fromDate);
            DateTime to = Convert.ToDateTime(toDate);
            var data = await _projectService.GetEmployeeTaskStatsAsync(from, to);
            return Ok(data);
        }


    }
}
