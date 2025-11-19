using StudentTeacherManagment.Models.DTOs.Submissions;
using StudentTeacherManagment.Models.Domain;

namespace StudentTeacherManagment.Services
{
    public interface ISubmissionService
    {
        Task<Submission> CreateSubmissionAsync(string studentId, SubmissionCreateDto dto);
        Task<Submission> GetByIdAsync(Guid id, string userId);
        Task<IEnumerable<Submission>> GetForAssignmentAsync(Guid assignmentId, string userId);
        Task<Submission> GradeSubmissionAsync(Guid id, int grade, string userId);
        Task<IEnumerable<Submission>> GetMySubmissionsAsync(string studentId);

        Task<(byte[] fileBytes, string mime, string fileName)> DownloadFileAsync(Guid id, string userId);
    }
}
