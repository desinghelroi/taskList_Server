using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskList_Server.Data;
using TaskList_Server.Models.DTOs;

namespace TaskList_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly Tasklist25Context _context;

        public TasksController(Tasklist25Context context)
        {
            _context = context;
        }

        [HttpGet]       
        public async Task<ActionResult<object>> GetTasks(int page = 1, int pageSize = 20)
        {
            try
            {
                var query = from t in _context.Tasks.AsNoTracking()
                            join c in _context.TblCustomers on t.CustomerId equals c.IntId into cust
                            from c in cust.DefaultIfEmpty()
                            join s in _context.Statuses on t.StatusId equals s.StatusId into stat
                            from s in stat.DefaultIfEmpty()
                            join p in _context.Priorities on t.PriorityId equals p.PriorityId into pri
                            from p in pri.DefaultIfEmpty()
                            join a in _context.TblApplications on t.ApplicationId equals a.IntId into app
                            from a in app.DefaultIfEmpty()
                            join u in _context.Users on t.UserId equals u.UserId into user
                            from us in user.DefaultIfEmpty()
                            orderby t.TaskId descending
                            select new TaskDto
                            {
                                TaskId = t.TaskId,
                                IntDisplayNo = t.IntDisplayNo ?? 0,
                                UserId = t.UserId ?? 0,
                                UserName = us.FirstName,
                                RegistrationDate = t.RegistrationDate ?? DateTime.Now,
                                LastChangeDate = t.LastChangeDate,
                                DelegatedTo = "",
                                Description = t.Description,
                                Visible = t.Visible ?? false,
                                SeriousBug = t.SeriousBug ?? false,
                                SmallBug = t.SmallBug ?? false,
                                CustomerId = c.IntId,
                                CustomerName = c.ChrCustomerName,
                                CustomerCode = c.ChrCustomerCode,
                                StatusId = s.StatusId,
                                StatusName = s.Name,
                                PriorityId = p.PriorityId,
                                PriorityName = p.Name,
                                ApplicationName = a.ChrApplicationName ?? "",
                                AppId = a.IntId
                            };

                var totalRecords = await query.Where(s=>s.StatusName != "Deleted" && s.StatusName != "Uploaded" && s.StatusName != "Approved" && s.StatusName != "Completed" && s.Visible).OrderByDescending(s=>s.RegistrationDate).ToListAsync();

                //var tasks = await query
                //    .Skip((page - 1) * pageSize)
                //    .Take(pageSize)
                //    .ToListAsync();

                //return Ok(new
                //{
                //    Data = tasks,
                //    TotalRecords = totalRecords,
                //    Page = page,
                //    PageSize = pageSize
                //});
                return Ok(totalRecords);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }               
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<TaskList_Server.Models.Task>> GetTask(int id)
        {
            try
            {
                var query = from t in _context.Tasks.AsNoTracking()
                            join c in _context.TblCustomers on t.CustomerId equals c.IntId into cust
                            from c in cust.DefaultIfEmpty()
                            join s in _context.Statuses on t.StatusId equals s.StatusId into stat
                            from s in stat.DefaultIfEmpty()
                            join p in _context.Priorities on t.PriorityId equals p.PriorityId into pri
                            from p in pri.DefaultIfEmpty()
                            join a in _context.TblApplications on t.ApplicationId equals a.IntId into app
                            from a in app.DefaultIfEmpty()
                            join u in _context.Users on t.UserId equals u.UserId into user
                            from us in user.DefaultIfEmpty()
                            orderby t.TaskId descending
                            select new TaskDto
                            {
                                TaskId = t.TaskId,
                                IntDisplayNo = t.IntDisplayNo ?? 0,
                                UserId = t.UserId ?? 0,
                                UserName = us.FirstName,
                                RegistrationDate = t.RegistrationDate ?? DateTime.Now,
                                LastChangeDate = t.LastChangeDate,
                                DelegatedTo = "",
                                Description = t.Description,
                                Visible = t.Visible ?? false,
                                SeriousBug = t.SeriousBug ?? false,
                                SmallBug = t.SmallBug ?? false,
                                CustomerId = c.IntId,
                                CustomerName = c.ChrCustomerName,
                                CustomerCode = c.ChrCustomerCode,
                                StatusId = s.StatusId,
                                StatusName = s.Name,
                                PriorityId = p.PriorityId,
                                PriorityName = p.Name,
                                ApplicationName = a.ChrApplicationName ?? "",
                                AppId = a.IntId, 

                            };

                var totalRecords = await query.Where(s =>s.TaskId == id).FirstOrDefaultAsync();


                return Ok(totalRecords);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateTaskDto dto)
        {
            var task = new TaskList_Server.Models.Task
            {
                Description = dto.Description,
                SeriousBug = dto.SeriousBug,
                SmallBug = dto.MinorBug,
                Visible = dto.Visible == "Yes",
                RegistrationDate = DateTime.Now,
                LastChangeDate = DateTime.Now,
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            //// Handle optional file upload
            //if (dto.File != null)
            //{
            //    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            //    if (!Directory.Exists(uploads))
            //        Directory.CreateDirectory(uploads);

            //    var filePath = Path.Combine(uploads, dto.File.FileName);
            //    using (var stream = new FileStream(filePath, FileMode.Create))
            //    {
            //        await dto.File.CopyToAsync(stream);
            //    }
            //}

            return Ok(new { message = "Task created successfully" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskDto taskDto)
        {
            if (id != taskDto.TaskId)
                return BadRequest("Task ID mismatch");

            var existingTask = await _context.Tasks.FindAsync(id);
            if (existingTask == null)
                return NotFound();

            existingTask.Description = taskDto.Description;
            existingTask.StatusId = taskDto.StatusId;
            existingTask.PriorityId = taskDto.PriorityId;
            existingTask.UserId = taskDto.UserId;
            existingTask.Visible = taskDto.Visible;
            existingTask.SeriousBug = taskDto.SeriousBug;
            existingTask.SmallBug = taskDto.SmallBug;
            existingTask.Description = taskDto.Description;
            existingTask.LastChangeDate = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists(id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("statuses")]
        public async Task<ActionResult<IEnumerable<StatusDto>>> GetStatuses()
        {
            var statuses = await _context.Statuses
                .Select(s => new StatusDto
                {
                    StatusId = s.StatusId,
                    Name = s.Name
                })
                .ToListAsync();

            return Ok(statuses);
        }


        private bool TaskExists(int id) => _context.Tasks.Any(t => t.TaskId == id);
    }
}
