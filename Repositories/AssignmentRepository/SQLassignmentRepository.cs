using Microsoft.EntityFrameworkCore;
using StudentTeacherManagment.Data;
using StudentTeacherManagment.Models.Domain;

namespace StudentTeacherManagment.Repositories.AssignmentRepository
{
    public class SQLAssignmentRepository : IAssignmentRepository
    {
        private readonly AppDbContext _context;

        public SQLAssignmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Assignment> CreateAsync(Assignment assignment)
        {
            await _context.Assignments.AddAsync(assignment);
            await _context.SaveChangesAsync();
            return assignment;
        }

        //for student  does not load submissions here (only for students)
        public async Task<Assignment?> GetByIdAsync(Guid id)
        {
            return await _context.Assignments
                .Include(a => a.Teacher)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        // for teacher/admin we must load submissions + student names
        public async Task<Assignment?> GetFullByIdAsync(Guid id)
        {
            return await _context.Assignments
                .Include(a => a.Teacher)
                .Include(a => a.Submissions)
                    .ThenInclude(s => s.Student)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        // load submissions for all assignments
        public async Task<IEnumerable<Assignment>> GetAllAsync()
        {
            return await _context.Assignments
                .Include(a => a.Teacher)
                .Include(a => a.Submissions)
                    .ThenInclude(s => s.Student)
                .ToListAsync();
        }

        // Teachers only see their assignments (with submissions)
        public async Task<IEnumerable<Assignment>> GetByTeacherIdAsync(string teacherId)
        {
            return await _context.Assignments
                .Where(a => a.TeacherId == teacherId)
                .Include(a => a.Teacher)
                .Include(a => a.Submissions)
                    .ThenInclude(s => s.Student)
                .ToListAsync();
        }

        public async Task<Assignment?> UpdateAsync(Assignment assignment)
        {
            var existing = await _context.Assignments.FindAsync(assignment.Id);

            if (existing == null)
                return null;

            existing.Title = assignment.Title;
            existing.Description = assignment.Description;
            existing.DueDate = assignment.DueDate;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _context.Assignments.FindAsync(id);
            if (existing == null)
                return false;

            _context.Assignments.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
