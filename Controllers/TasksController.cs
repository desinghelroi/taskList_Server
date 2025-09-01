using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskList_Server.Data;
using TaskList_Server.Models;
using TaskList_Server.Models.DTOs;
using System.IO;
using Microsoft.Data.SqlClient.DataClassification;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Authorization;

namespace TaskList_Server.Controllers
{
    [Authorize]
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
        public async Task<ActionResult<object>> GetTasks(int page = 1, int pageSize = 20, string filter = "true", string search = "")
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

                // Apply filter
                IQueryable<TaskDto> filteredQuery;
                if (filter.Equals("true", StringComparison.OrdinalIgnoreCase))
                    filteredQuery = query.Where(s => s.Visible == true && s.CustomerId == 1 && s.StatusId < 6);
                else
                    filteredQuery = query.Where(s => s.Visible == false && s.CustomerId == 1 && s.StatusId < 6);

                if (!string.IsNullOrEmpty(search))
                    filteredQuery = filteredQuery.Where(s => s.Description.Contains(search));

                var totalRecords = await filteredQuery.CountAsync();

                var tasks = await filteredQuery
                    .OrderByDescending(t => t.TaskId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (!string.IsNullOrEmpty(search))
                {
                    tasks = tasks.Select(t =>
                    {
                        t.Description = t.Description.Replace(search, $"<span style='color:#E80F0F;'>{search}</span>");
                        return t;
                    }).ToList();
                }

                return Ok(new
                {
                    Data = tasks,
                    TotalRecords = totalRecords,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
                });
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
                var task = await (from t in _context.Tasks.AsNoTracking()
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
                                  where t.TaskId == id
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
                                  }).FirstOrDefaultAsync();


                if (task != null)
                {
                    var uploadsFolder = Path.Combine(AppContext.BaseDirectory, "Uploads"); 

                    var files = _context.TblUploadedFiles
                        .Where(f => f.IntTaskId == id)
                        .AsEnumerable() 
                        .Select(f =>
                        {
                            var filePath = Path.Combine(uploadsFolder, f.ChrSavedFileName);
                            string base64 = null;

                            try
                            {
                                if (System.IO.File.Exists(filePath))
                                {
                                    var bytes = System.IO.File.ReadAllBytes(filePath);
                                    var ext = Path.GetExtension(f.ChrSavedFileName).ToLower();
                                    var mimeType = ext switch
                                    {
                                        ".jpg" or ".jpeg" => "image/jpeg",
                                        ".png" => "image/png",
                                        ".gif" => "image/gif",
                                        ".bmp" => "image/bmp",
                                        ".pdf" => "application/pdf",
                                        _ => "application/octet-stream"
                                    };
                                    base64 = $"data:{mimeType};base64,{Convert.ToBase64String(bytes)}";
                                }
                            }
                            catch
                            {
                                base64 = null;
                            }

                            return new TaskFileDto
                            {
                                FileName = f.ChrOriginalFileName,
                                FileUrl = base64
                            };
                        })
                        .ToList();

                    task.Files = files;
                }



                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateTaskDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                int lastNumber = _context.Tasks
                    .OrderByDescending(t => t.IntDisplayNo)
                    .Select(t => t.IntDisplayNo)
                    .FirstOrDefault() ?? 1;

                var task = new TaskList_Server.Models.Task
                {
                    Description = dto.Description,
                    SeriousBug = dto.SeriousBug,
                    SmallBug = dto.SmallBug,
                    Visible = dto.Visible,
                    RegistrationDate = DateTime.Now,
                    LastChangeDate = DateTime.Now,
                    UserId = dto.UserId,
                    DelegatedTo = dto.UserId,
                    CustomerId = 1,
                    PriorityId = dto.PriorityId,
                    ApplicationId = dto.AppId,
                    StatusId = dto.StatusId,
                    IntDisplayNo = lastNumber + 1
                };

                _context.Tasks.Add(task);
                await _context.SaveChangesAsync(); 

                if (dto.File != null && dto.File.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(dto.File.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.File.CopyToAsync(stream);
                    }

                    var fileUpload = new TblUploadedFile
                    {
                        ChrOriginalFileName = dto.File.FileName,
                        ChrSavedFileName = uniqueFileName, 
                        IntTaskId = task.TaskId
                    };

                    _context.TblUploadedFiles.Add(fileUpload);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return Ok(new { message = "Task created successfully", taskId = task.TaskId });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Error creating task", error = ex.Message });
            }
        }



        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateTask(int id, [FromForm] TaskDto taskDto)
        {
            if (id != taskDto.TaskId)
                return BadRequest("Task ID mismatch");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existingTask = await _context.Tasks.FindAsync(id);
                if (existingTask == null)
                    return NotFound();

                existingTask.Description = taskDto.Description;
                existingTask.StatusId = taskDto.StatusId;
                existingTask.ApplicationId = taskDto.AppId;
                existingTask.PriorityId = taskDto.PriorityId;
                existingTask.UserId = taskDto.UserId;
                existingTask.Visible = taskDto.Visible;
                existingTask.SeriousBug = taskDto.SeriousBug;
                existingTask.SmallBug = taskDto.SmallBug;
                existingTask.LastChangeDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                if (taskDto.File != null && taskDto.File.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(taskDto.File.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await taskDto.File.CopyToAsync(stream);
                    }

                    var fileUpload = new TblUploadedFile
                    {
                        ChrOriginalFileName = taskDto.File.FileName,
                        ChrSavedFileName = uniqueFileName,
                        IntTaskId = existingTask.TaskId
                    };

                    _context.TblUploadedFiles.Add(fileUpload);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return Ok(new { message = "Task updated successfully" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Error updating task", error = ex.Message });
            }
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


        [HttpGet("application")]
        public async Task<ActionResult<IEnumerable<ProjectsDto>>> GetProjectList()
        {
            var projects = await _context.TblApplications
                .Select(s => new ProjectsDto
                {
                    AppId = s.IntId,
                    ApplicationName = s.ChrApplicationName
                })
                .ToListAsync();

            return Ok(projects);
        }

        [HttpGet("priority")]
        public async Task<ActionResult<IEnumerable<PriorityDto>>> GetPriorityList()
        {
            var priority = await _context.Priorities
                .Select(s => new PriorityDto
                {
                    PriorityId = s.PriorityId,
                    PriorityName = s.Name
                })
                .ToListAsync();

            return Ok(priority);
        }

        [HttpGet("get_developers")]
        public async Task<ActionResult<IEnumerable<DeveloperDto>>> GetDeveloper()
        {
            var developer = await _context.Users
                .Where(s=>s.BitShowUser == true)
                .Select(s => new DeveloperDto
                {
                    UserId = s.UserId,
                    UserName = s.FirstName
                })
                .ToListAsync();

            return Ok(developer);
        }

        [HttpGet("get_taskCounts")]
        public async Task<ActionResult<object>> GetCounts()
        {
            int GetCurrentTaskCount = await _context.Tasks
                .Where(s => s.Visible == true && s.CustomerId == 1 && s.StatusId < 6)
                .CountAsync();

            int GetClosedTaskCount = await _context.Tasks
                .Where(s => s.Visible == false && s.CustomerId == 1 && s.StatusId < 6)
                .CountAsync();

            return Ok(new { OpenTask = GetCurrentTaskCount, ClosedTask = GetClosedTaskCount });
        }



        private bool TaskExists(int id) => _context.Tasks.Any(t => t.TaskId == id);
    }
}
