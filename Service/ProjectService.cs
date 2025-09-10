using Microsoft.EntityFrameworkCore;
using TaskList_Server.Data;
using TaskList_Server.Interface;
using TaskList_Server.Models;
using TaskList_Server.Models.DTOs;

namespace TaskList_Server.Service
{
    public class ProjectService : IProjectService
    {
        private readonly Tasklist25Context _context;

        public ProjectService(Tasklist25Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TblApplication>> GetAllProjectsAsync()
        {
            return await _context.TblApplications
                .Select(s => new TblApplication
                {
                    IntId = s.IntId,
                    ChrApplicationName = s.ChrApplicationName,
                    IntCustomerId = s.IntCustomerId
                })
                .ToListAsync();
        }

        public async Task<TblApplication> CreateProjectAsync(TblApplication app, string? customerId)
        {
            if (!string.IsNullOrEmpty(customerId))
                app.IntCustomerId = Convert.ToInt32(customerId);

            _context.TblApplications.Add(app);
            await _context.SaveChangesAsync();
            return app;
        }

        public async Task<TblApplication?> UpdateProjectAsync(int id, TblApplication app, string? customerId)
        {
            var existing = await _context.TblApplications.FindAsync(id);
            if (existing == null) return null;

            if (!string.IsNullOrEmpty(customerId))
                existing.IntCustomerId = Convert.ToInt32(customerId);

            existing.ChrApplicationName = app.ChrApplicationName;
            existing.IntCustomerId = app.IntCustomerId;

            _context.Entry(existing).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return existing;
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            var project = await _context.TblApplications.FindAsync(id);
            if (project == null) return false;

            _context.TblApplications.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }



        public async Task<IEnumerable<EmployeeTaskStatsDto>> GetEmployeeTaskStatsAsync(DateTime fromDate, DateTime toDate)
        {
            var query = _context.Tasks
                .Include(t => t.Users)
                .Where(t => t.LastChangeDate >= fromDate && t.LastChangeDate <= toDate && t.Users.BitShowUser == true);

            var data = await query
                .GroupBy(t => new { t.UserId, t.Users.FirstName })
                .Select(g => new EmployeeTaskStatsDto
                {
                    DeveloperId = g.Key.UserId ?? 0,
                    DeveloperName = g.Key.FirstName ?? "Unknown",
                    TaskCount = g.Count()
                })
                .OrderByDescending(x => x.TaskCount)
                .ToListAsync();

            return data;
        }



    }
}
