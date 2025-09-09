using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task<PagedResult<TaskDto>> GetTasksAsync(string filter, string search, string status, int page, int pageSize, string customerId, int developerId, int projectId)
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
                            UserName = us.FirstName ?? "",
                            RegistrationDate = t.RegistrationDate ?? DateTime.Now,
                            LastChangeDate = t.LastChangeDate,
                            DelegatedTo = "",
                            Description = t.Description ?? "",
                            Visible = t.Visible ?? false,
                            SeriousBug = t.SeriousBug ?? false,
                            SmallBug = t.SmallBug ?? false,
                            CustomerId = c.IntId,
                            CustomerName = c.ChrCustomerName ?? "",
                            CustomerCode = c.ChrCustomerCode ?? "",
                            StatusId = s.StatusId,
                            StatusName = s.Name,
                            PriorityId = p.PriorityId,
                            PriorityName = p.Name,
                            ApplicationName = a.ChrApplicationName ?? "",
                            AppId = a.IntId
                        };

            IQueryable<TaskDto> filteredQuery;
            if (filter.Equals("true", StringComparison.OrdinalIgnoreCase))
                filteredQuery = query.Where(s => s.Visible == true && s.CustomerId == Convert.ToInt32(customerId) && s.StatusId < 6);
            else
                filteredQuery = query.Where(s => s.Visible == false && s.CustomerId == Convert.ToInt32(customerId) && s.StatusId < 6);

            if (!string.IsNullOrEmpty(search))
                filteredQuery = query.Where(s => s.Description.Contains(search));

            if (!string.IsNullOrEmpty(status))
                filteredQuery = query.Where(s => s.Visible && s.StatusName.Contains(status));
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
                    t.Description = t.Description.Replace(search, $"<span style='color:#E80F0F;'>{search}</span>");
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
                                  UserName = us.FirstName ?? "",
                                  RegistrationDate = t.RegistrationDate ?? DateTime.Now,
                                  LastChangeDate = t.LastChangeDate,
                                  DelegatedTo = "",
                                  Description = t.Description ?? "",
                                  Visible = t.Visible ?? false,
                                  SeriousBug = t.SeriousBug ?? false,
                                  SmallBug = t.SmallBug ?? false,
                                  CustomerId = c.IntId,
                                  CustomerName = c.ChrCustomerName ?? "",
                                  CustomerCode = c.ChrCustomerCode ?? "",
                                  StatusId = s.StatusId,
                                  StatusName = s.Name,
                                  PriorityId = p.PriorityId,
                                  PriorityName = p.Name,
                                  ApplicationName = a.ChrApplicationName ?? "",
                                  AppId = a.IntId
                              }).FirstOrDefaultAsync();

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
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var existingTask = await _context.Tasks.FindAsync(id);
                if (existingTask == null)
                    return (false, "Task not found");

                existingTask.Description = dto.Description;
                existingTask.StatusId = dto.StatusId;
                existingTask.ApplicationId = dto.AppId;
                existingTask.PriorityId = dto.PriorityId;
                existingTask.UserId = dto.UserId;
                existingTask.Visible = dto.Visible;
                existingTask.SeriousBug = dto.SeriousBug;
                existingTask.SmallBug = dto.SmallBug;
                existingTask.LastChangeDate = DateTime.UtcNow;

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
                        IntTaskId = existingTask.TaskId
                    };

                    _context.TblUploadedFiles.Add(fileUpload);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return (true, "Task updated successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
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
                Name = "--Select--"
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
                PriorityName = "--Select--"
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
                UserName = "--Select--"
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
                ApplicationName = "--Select--"
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

            return await query
                .Select(t => new TasksReportDto
                {
                    Id = t.TaskId,
                    DeveloperName = t.Users.FirstName ?? "",
                    TaskName = t.Description ?? "",
                    StatusName = t.Status.Name,
                    ProjectName = t.Project.ChrApplicationName ?? "",
                    StartDate = t.RegistrationDate,
                    EndDate = t.LastChangeDate
                })
                .ToListAsync();
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
