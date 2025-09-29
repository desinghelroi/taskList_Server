using Microsoft.AspNetCore.Mvc;
using TaskList_Server.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using TaskList_Server.Interface;
using TaskList_Server.Models;
namespace TaskList_Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController(ITaskListService taskService, IProjectService projectService) : ControllerBase
    {
        private readonly ITaskListService _taskService = taskService;
        private readonly IProjectService _projectService = projectService;

        [HttpGet]
        [EnableRateLimiting("GeneralLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]       
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<object>> GetTasks(int page = 1, int pageSize = 20, string filter = "true", string search = "", string staus = "", int developerId = 0, int projectId = 0) => User.FindFirst("customerId")?.Value is string customerId && !string.IsNullOrEmpty(customerId) ? Ok(await _taskService.GetTasksAsync(filter, search, staus, page, pageSize, customerId, developerId, projectId)) : BadRequest();

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]       
        public async Task<IActionResult> GetTaskById(int id) => await _taskService.GetTaskByIdAsync(id) is var task && task != null ? Ok(task) : NotFound();

        [HttpPost]
        [Consumes("multipart/form-data")]
        [EnableRateLimiting("WriteLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]        
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateTask([FromForm] CreateTaskDto dto) => (await _taskService.CreateTaskAsync(dto, Convert.ToInt32(User.FindFirst("customerId")?.Value))) is var result && result.Success ? Ok(new { message = result.Message, taskId = result.TaskId }) : StatusCode(500, new { message = result.Message });

        [HttpPut("{id}")]
        [EnableRateLimiting("WriteLimiter")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateTask(int id, [FromForm] TaskDto dto) => id != dto.TaskId ? BadRequest("Task ID mismatch") : (await _taskService.UpdateTaskAsync(id, dto)) is var result && result.Success ? Ok(new { message = result.Message }) : StatusCode(500, new { message = result.Message });
        
        [HttpDelete("{id}")]
        [EnableRateLimiting("WriteLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]     
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteTask(int id) => await _taskService.DeleteTaskAsync(id) is var result && result.Success ? Ok(new { message = result.Message }) : StatusCode(500, new { message = result.Message });

        [HttpGet("statuses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<StatusDto>>> GetStatuses() => Ok(await _taskService.GetStatusesAsync());

        [HttpGet("application")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ProjectsDto>>> GetProjectList() => User.FindFirst("customerId")?.Value is string customerId && !string.IsNullOrEmpty(customerId) ? Ok(await _taskService.GetProjectListAsync(customerId)) : NotFound();
        
        [HttpGet("priority")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PriorityDto>>> GetPriorityList() => Ok(await _taskService.GetPriorityListAsync());
       
        [HttpGet("get_developers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DeveloperDto>>> GetDevelopers() => User.FindFirst("customerId")?.Value is string customerId && !string.IsNullOrEmpty(customerId) ? Ok(await _taskService.GetDevelopersAsync(customerId)) : NotFound();

        [HttpGet("get_taskCounts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TaskCountsDto>> GetCounts() => User.FindFirst("customerId")?.Value is string customerId && !string.IsNullOrEmpty(customerId)
        ? Ok(await _taskService.GetCountsAsync(customerId)) : Unauthorized("CustomerId not found in token.");

        [HttpGet("report")]
        [ProducesResponseType(StatusCodes.Status200OK)]   
        public async Task<ActionResult<IEnumerable<TasksReportDto>>> GetTasksReport([FromQuery] ReportFilters filters) => Ok(await _taskService.GetTasksReportAsync(filters));
        

        [HttpGet("file/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskFileDto>> GetFileContent(int id) => await _taskService.GetFileContentAsync(id) is TaskFileDto file ? Ok(file) : NotFound();

        [HttpGet("get_allProjects")]
        public async Task<IEnumerable<TblApplication>> GetAllProjectList() => await _projectService.GetAllProjectsAsync();

        [HttpPost("createProject")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TblApplication>> CreateProject([FromBody] TblApplication app) =>  app is null || string.IsNullOrEmpty(app.ChrApplicationName)
        ? BadRequest("Project name is required") : CreatedAtAction(nameof(GetAllProjectList), new { id = (await _projectService.CreateProjectAsync(app, User.FindFirst("customerId")?.Value)).IntId }, await _projectService.CreateProjectAsync(app, User.FindFirst("customerId")?.Value));


        [HttpPut("updateProject/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] TblApplication app) => (await _projectService.UpdateProjectAsync(id, app, User.FindFirst("customerId")?.Value)) is TblApplication updated ? Ok(updated) : NotFound();

        [HttpDelete("deleteProject/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProject(int id) => await _projectService.DeleteProjectAsync(id) ? Ok(new { message = "Project deleted successfully" }) : NotFound();

        [HttpGet("employeeTaskStats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmployeeTaskStatsDto>>> GetEmployeeTaskStats([FromQuery] string fromDate, [FromQuery] string toDate)  => Ok(await _projectService.GetEmployeeTaskStatsAsync(Convert.ToDateTime(fromDate),Convert.ToDateTime(toDate)));
    }
}
