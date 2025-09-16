using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TaskList_Server.Data;
using TaskList_Server.Interface;
using TaskList_Server.Models;
using TaskList_Server.Models.DTOs;

namespace TaskList_Server.Service
{
    public class TaskListService : ITaskListService
    {
        private readonly Tasklist25Context _context;

        public TaskListService(Tasklist25Context context) => _context = context;

        public async Task<PagedResult<TaskDto>> GetTasksAsync(string filter,string search,string status, int page,int pageSize, string customerId,int developerId,int projectId)
        {
            var query = _context.Tasks
                .AsNoTracking()
                .GroupJoin(_context.TblCustomers, t => t.CustomerId, c => c.IntId, (t, cust) => new { t, cust })
                .SelectMany(x => x.cust.DefaultIfEmpty(), (x, c) => new { x.t, c })
                .GroupJoin(_context.Statuses, x => x.t.StatusId, s => s.StatusId, (x, stat) => new { x.t, x.c, stat })
                .SelectMany(x => x.stat.DefaultIfEmpty(), (x, s) => new { x.t, x.c, s })
                .GroupJoin(_context.Priorities, x => x.t.PriorityId, p => p.PriorityId, (x, pri) => new { x.t, x.c, x.s, pri })
                .SelectMany(x => x.pri.DefaultIfEmpty(), (x, p) => new { x.t, x.c, x.s, p })
                .GroupJoin(_context.TblApplications, x => x.t.ApplicationId, a => a.IntId, (x, app) => new { x.t, x.c, x.s, x.p, app })
                .SelectMany(x => x.app.DefaultIfEmpty(), (x, a) => new { x.t, x.c, x.s, x.p, a })
                .GroupJoin(_context.Users, x => x.t.UserId, u => u.UserId, (x, user) => new { x.t, x.c, x.s, x.p, x.a, user })
                .SelectMany(x => x.user.DefaultIfEmpty(), (x, us) => new TaskDto
                {
                    TaskId = x.t.TaskId,
                    IntDisplayNo = x.t.IntDisplayNo ?? 0,
                    UserId = x.t.UserId ?? 0,
                    UserName = us.FirstName ?? "",
                    RegistrationDate = x.t.RegistrationDate ?? DateTime.Now,
                    LastChangeDate = x.t.LastChangeDate,
                    DelegatedTo = "",
                    Description = x.t.Description ?? "",
                    Visible = x.t.Visible ?? false,
                    SeriousBug = x.t.SeriousBug ?? false,
                    SmallBug = x.t.SmallBug ?? false,
                    CustomerId = x.c.IntId,
                    CustomerName = x.c.ChrCustomerName ?? "",
                    CustomerCode = x.c.ChrCustomerCode ?? "",
                    StatusId = x.s.StatusId,
                    StatusName = x.s.Name,
                    PriorityId = x.p.PriorityId,
                    PriorityName = x.p.Name,
                    ApplicationName = x.a.ChrApplicationName ?? "",
                    AppId = x.a.IntId,
                    //StartDate = x.t.StartDate,
                    //TotalHours = x.t.TotalHours
                });

            var filteredQuery = query
                .Where(s => s.CustomerId == Convert.ToInt32(customerId) && s.StatusId < 6)
                .Where(s => filter.Equals("true", StringComparison.OrdinalIgnoreCase) ? s.Visible : !s.Visible);

            if (!string.IsNullOrEmpty(search))
                filteredQuery = filteredQuery.Where(s => s.Description.Contains(search));

            if (!string.IsNullOrEmpty(status))
                filteredQuery = filteredQuery.Where(s => s.Visible && s.StatusName.Contains(status));

            if (developerId != 0)
                filteredQuery = filteredQuery.Where(s => s.Visible && s.UserId == developerId);

            if (projectId != 0)
                filteredQuery = filteredQuery.Where(s => s.Visible && s.AppId == projectId);

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
                    t.Description = Regex.Replace(
                        t.Description,
                        Regex.Escape(search),
                        m => $"<span style='background-color:#E80F0F;'>{m.Value}</span>",
                        RegexOptions.IgnoreCase
                    );
                    return t;
                }).ToList();
            }

            return new PagedResult<TaskDto>
            {
                Data = tasks,
                TotalRecords = totalRecords,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
            };
        }


        public async Task<TaskDto?> GetTaskByIdAsync(int id)
        {
            var task = await _context.Tasks.AsNoTracking().Where(t => t.TaskId == id)
                            .GroupJoin(_context.TblCustomers, t => t.CustomerId, c => c.IntId, (t, cust) => new { t, cust })
                            .SelectMany(x => x.cust.DefaultIfEmpty(), (x, c) => new { x.t, c })
                            .GroupJoin(_context.Statuses, x => x.t.StatusId, s => s.StatusId, (x, stat) => new { x, stat })
                            .SelectMany(x => x.stat.DefaultIfEmpty(), (x, s) => new { x.x.t, x.x.c, s })
                            .GroupJoin(_context.Priorities, x => x.t.PriorityId, p => p.PriorityId, (x, pri) => new { x, pri })
                            .SelectMany(x => x.pri.DefaultIfEmpty(), (x, p) => new { x.x.t, x.x.c, x.x.s, p })
                            .GroupJoin(_context.TblApplications, x => x.t.ApplicationId, a => a.IntId, (x, app) => new { x, app })
                            .SelectMany(x => x.app.DefaultIfEmpty(), (x, a) => new { x.x.t, x.x.c, x.x.s, x.x.p, a })
                            .GroupJoin(_context.Users, x => x.t.UserId, u => u.UserId, (x, user) => new { x, user })
                            .SelectMany(x => x.user.DefaultIfEmpty(), (x, us) => new { x.x.t, x.x.c, x.x.s, x.x.p, x.x.a, us })
                            .Select(x => new TaskDto
                            {
                                TaskId = x.t.TaskId,
                                IntDisplayNo = x.t.IntDisplayNo ?? 0,
                                UserId = x.t.UserId ?? 0,
                                UserName = x.us.FirstName ?? "",
                                RegistrationDate = x.t.RegistrationDate ?? DateTime.Now,
                                LastChangeDate = x.t.LastChangeDate,
                                DelegatedTo = "",
                                Description = x.t.Description ?? "",
                                Visible = x.t.Visible ?? false,
                                SeriousBug = x.t.SeriousBug ?? false,
                                SmallBug = x.t.SmallBug ?? false,
                                CustomerId = x.c.IntId,
                                CustomerName = x.c.ChrCustomerName ?? "",
                                CustomerCode = x.c.ChrCustomerCode ?? "",
                                StatusId = x.s.StatusId,
                                StatusName = x.s.Name,
                                PriorityId = x.p.PriorityId,
                                PriorityName = x.p.Name,
                                ApplicationName = x.a.ChrApplicationName ?? "",
                                AppId = x.a.IntId,
                                //StartDate = x.t.StartDate,
                                //TotalHours = x.t.TotalHours
                            })
                            .FirstOrDefaultAsync();


            if (task != null)
            {
                var files = _context.TblUploadedFiles
                    .Where(f => f.IntTaskId == id)
                    .AsEnumerable()
                    .Select(f => new TaskFileDto
                    {
                        FileName = f.ChrOriginalFileName ?? "",
                        UrlId = f.IntId
                    })
                    .ToList();

                task.Files = files;
            }

            return task;
        }


        public async Task<(bool Success, string Message, int? TaskId)> CreateTaskAsync(CreateTaskDto dto, int customerId)
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
                    CustomerId = customerId,
                    PriorityId = dto.PriorityId,
                    ApplicationId = dto.AppId,
                    StatusId = dto.StatusId,
                    IntDisplayNo = lastNumber + 1,
                    //TotalHours = "",
                    
                };
                //if (dto.StatusId == 2) 
                //{
                //    task.StartDate = DateTime.Now; 
                //}

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

                return (true, "Task created successfully", task.TaskId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Error creating task: {ex.Message}", null);
            }
        }


        public async Task<(bool Success, string Message)> UpdateTaskAsync(int id, TaskDto dto)
        {
            try
            {
                var affected = await _context.Tasks.Where(t => t.TaskId == id).ExecuteUpdateAsync(t => t
                        .SetProperty(task => task.Description, task => dto.Description)
                        .SetProperty(task => task.StatusId, task => dto.StatusId)
                        .SetProperty(task => task.ApplicationId, task => dto.AppId)
                        .SetProperty(task => task.PriorityId, task => dto.PriorityId)
                        .SetProperty(task => task.UserId, task => dto.UserId)
                        .SetProperty(task => task.Visible, task => dto.Visible)
                        .SetProperty(task => task.SeriousBug, task => dto.SeriousBug)
                        .SetProperty(task => task.SmallBug, task => dto.SmallBug)
                        .SetProperty(task => task.LastChangeDate, task => DateTime.UtcNow)
                        //.SetProperty(task => task.StartDate, task => dto.StatusId == 2 ? DateTime.Now : task.StartDate)
                    );

                if (affected == 0)
                    return (false, "Task not found");

                //if (dto.StatusId == 3)
                //{
                //    var taskData = await _context.Tasks
                //        .Where(t => t.TaskId == id)
                //        .Select(t => new { t.StartDate })
                //        .FirstOrDefaultAsync();

                //    if (taskData?.StartDate != null)
                //    {
                //        var ts = DateTime.Now - taskData.StartDate.Value;
                //        var totalHours = $"{(int)ts.TotalHours}:{ts.Minutes:D2}:{ts.Seconds:D2} hrs";

                //        await _context.Tasks
                //            .Where(t => t.TaskId == id)
                //            .ExecuteUpdateAsync(t => t.SetProperty(task => task.TotalHours, task => totalHours));
                //    }
                //}

                if (dto.File != null && dto.File.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(dto.File.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    await using var stream = new FileStream(filePath, FileMode.Create);
                    await dto.File.CopyToAsync(stream);

                    var fileUpload = new TblUploadedFile
                    {
                        ChrOriginalFileName = dto.File.FileName,
                        ChrSavedFileName = uniqueFileName,
                        IntTaskId = id
                    };

                    _context.TblUploadedFiles.Add(fileUpload);
                    await _context.SaveChangesAsync();
                }

                return (true, "Task updated successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Error updating task: {ex.Message}");
            }
        }


        public async Task<(bool Success, string Message)> DeleteTaskAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var task = await _context.Tasks.FindAsync(id);
                if (task == null)
                    return (false, "Task not found");

                var files = await _context.TblUploadedFiles
                    .Where(f => f.IntTaskId == id)
                    .ToListAsync();

                foreach (var file in files)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", file.ChrSavedFileName ?? "");
                    if (File.Exists(filePath))
                        File.Delete(filePath);

                    _context.TblUploadedFiles.Remove(file);
                }

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return (true, "Task deleted successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Error deleting task: {ex.Message}");
            }
        }

        public async Task<IEnumerable<StatusDto>> GetStatusesAsync()
        {
            var status = await _context.Statuses
                .GroupBy(a => a.Name)
                .Select(s => new StatusDto
                {
                    StatusId = s.First().StatusId,
                    Name = s.Key ?? ""
                })
                .ToListAsync();


            status.Insert(0, new StatusDto
            {
                StatusId = 0,
                Name = "-- Välj Status --"
            });
            return status;
        }

        public async Task<IEnumerable<PriorityDto>> GetPriorityListAsync()
        {
            var Priorities = await _context.Priorities
                .Select(p => new PriorityDto
                {
                    PriorityId = p.PriorityId,
                    PriorityName = p.Name
                })
                .ToListAsync();


            Priorities.Insert(0, new PriorityDto
            {
                PriorityId = 0,
                PriorityName = "-- Välj Prioritet --"
            });
            return Priorities;
        }


        public async Task<IEnumerable<DeveloperDto>> GetDevelopersAsync(string CustomerId)
        {
            int CusId = Convert.ToInt32(CustomerId);
            var Users =  await _context.Users
                .Where(u => u.BitShowUser == true && u.IntCustomerId == CusId)
                .Select(u => new DeveloperDto
                {
                    UserId = u.UserId,
                    UserName = u.FirstName ?? ""
                })
                .ToListAsync();


            Users.Insert(0, new DeveloperDto
            {
                UserId = 0,
                UserName = "-- Välj Utvecklare --"
            });
            return Users;
        }


        public async Task<IEnumerable<ProjectsDto>> GetProjectListAsync(string customerId)
        {
            int cusId = Convert.ToInt32(customerId);

            var apps = await _context.TblApplications
                .Where(s => s.IntCustomerId == cusId)
                .GroupBy(app => app.ChrApplicationName)
                .Select(g => new ProjectsDto
                {
                    AppId = g.First().IntId,
                    ApplicationName = g.Key ?? ""
                })
                .OrderBy(p => p.ApplicationName)
                .ToListAsync();

            apps.Insert(0, new ProjectsDto
            {
                AppId = 0,
                ApplicationName = "-- Välj Projekt --"
            });

            return apps;
        }

        public async Task<TaskCountsDto> GetCountsAsync(string customerId)
        {
            int customerIdInt = Convert.ToInt32(customerId);

            int openTaskCount = await _context.Tasks
                .Where(s => s.Visible == true && s.CustomerId == customerIdInt && s.StatusId < 6)
                .CountAsync();

            int closedTaskCount = await _context.Tasks
                .Where(s => s.Visible == false && s.CustomerId == customerIdInt && s.StatusId < 6)
                .CountAsync();

            int completedTaskCount = await _context.Tasks
                .Where(s => s.Visible == true && s.Status.Name == "Completed")
                .CountAsync();

            int uploadedTaskCount = await _context.Tasks
                .Where(s => s.Visible == true && s.Status.Name == "Uploaded")
                .CountAsync();

            return new TaskCountsDto
            {
                OpenTask = openTaskCount,
                ClosedTask = closedTaskCount,
                CompletedTask = completedTaskCount,
                UploadedTask = uploadedTaskCount
            };
        }


        public async Task<IEnumerable<TasksReportDto>> GetTasksReportAsync(ReportFilters filters)
        {
            var query = _context.Tasks
                .Include(t => t.Status)
                .Include(t => t.Project)
                .Include(t => t.Users)
                .AsQueryable();

            if (filters.Status.HasValue)
                query = query.Where(t => t.StatusId == filters.Status.Value);

            if (!string.IsNullOrEmpty(filters.TaskName))
                query = query.Where(t => t.Description.Contains(filters.TaskName));

            if (filters.ProjectId.HasValue)
                query = query.Where(t => t.ApplicationId == filters.ProjectId.Value);

            if (filters.FromDate.HasValue)
                query = query.Where(t => t.LastChangeDate >= filters.FromDate.Value);

            if (filters.ToDate.HasValue)
                query = query.Where(t => t.LastChangeDate <= filters.ToDate.Value);

            if (filters.DeveloperId.HasValue)
                query = query.Where(t => t.UserId == filters.DeveloperId.Value);

            var data = await query
                .Select(t => new TasksReportDto
                {
                    Id = t.TaskId,
                    DeveloperName = t.Users.FirstName ?? "",
                    TaskName =  t.Description ?? "",
                    StatusName = t.Status.Name,
                    ProjectName = t.Project.ChrApplicationName ?? "",
                    StartDate = t.RegistrationDate,
                    EndDate = t.LastChangeDate
                })
                .ToListAsync();

            if (!string.IsNullOrEmpty(filters.TaskName))
            {
                data = data.Select(t =>
                {
                    t.TaskName = Regex.Replace(
                        t.TaskName,
                        Regex.Escape(filters.TaskName),
                        m => $"<span style='background-color:#E80F0F;'>{m.Value}</span>",
                        RegexOptions.IgnoreCase
                    );
                    return t;
                }).ToList();
            }

            return data;
        }

        public async Task<TaskFileDto?> GetFileContentAsync(int id)
        {
            var file = await _context.TblUploadedFiles.FindAsync(id);
            if (file == null) return null;

            var filePath = Path.Combine("Uploads", file.ChrSavedFileName ?? "");
            if (!System.IO.File.Exists(filePath)) return null;

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var base64 = Convert.ToBase64String(fileBytes);

            return new TaskFileDto
            {
                FileName = file.ChrOriginalFileName ?? "",
                FileUrl = $"data:application/octet-stream;base64,{base64}"
            };
        }

      
    }
}
