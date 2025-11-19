using Microsoft.EntityFrameworkCore;
using StudentTeacherManagment.Data;
using StudentTeacherManagment.Models.Domain;

namespace StudentTeacherManagment.Repositories.SubmissionRepository
{
    public class SQLsubmissionRepository : ISubmissionRepository
    {
        private readonly AppDbContext _context;

        public SQLsubmissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Submission> CreateAsync(Submission submission)
        {
            await _context.Submissions.AddAsync(submission);
            await _context.SaveChangesAsync();
            return submission;
        }

        public async Task<IEnumerable<Submission>> GetByAssignmentIdAsync(Guid assignmentId)
        {
            return await _context.Submissions
                .Where(s => s.AssignmentId == assignmentId)
                .Include(s => s.Student)
                .Include(s => s.Assignment)   // 🔥 THIS FIXES EVERYTHING
                .ToListAsync();
        }


        public async Task<IEnumerable<Submission>> GetByStudentIdAsync(string studentId)
        {
            return await _context.Submissions
                .Where(s => s.StudentId == studentId)
                .Include(s => s.Assignment)
                .ToListAsync();
        }

        public async Task<Submission?> GetByIdAsync(Guid id)
        {
            return await _context.Submissions
                .Include(s => s.Student)
                .Include(s => s.Assignment)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Submission?> GradeAsync(Guid submissionId, int grade)
        {
            var submission = await _context.Submissions.FindAsync(submissionId);
            if (submission == null)
                return null;

            submission.Grade = grade;
            submission.Status = "Graded";

            await _context.SaveChangesAsync();
            return submission;
        }
    }
}
