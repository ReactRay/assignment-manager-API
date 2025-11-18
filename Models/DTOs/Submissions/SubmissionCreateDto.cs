namespace StudentTeacherManagment.Models.DTOs.Submissions
{
    public class SubmissionCreateDto
    {
        public Guid AssignmentId { get; set; }
        public IFormFile File { get; set; }   // PDF upload
    }
}

