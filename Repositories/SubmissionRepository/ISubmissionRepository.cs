using StudentTeacherManagment.Models.Domain;

namespace StudentTeacherManagment.Repositories.SubmissionRepository
{
    public interface ISubmissionRepository
    {
        Task<Submission> CreateAsync(Submission submission);

        Task<IEnumerable<Submission>> GetByAssignmentIdAsync(Guid assignmentId);

        Task<IEnumerable<Submission>> GetByStudentIdAsync(string studentId);

        Task<Submission?> GetByIdAsync(Guid id);

        Task<Submission?> GradeAsync(Guid submissionId, int grade);


    }
}
