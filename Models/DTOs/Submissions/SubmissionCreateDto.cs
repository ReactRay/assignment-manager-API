namespace StudentTeacherManagment.Models.DTOs.Submissions
{
    public class SubmissionCreateDto
    {
        public Guid AssignmentId { get; set; }
        public string Content { get; set; }
    }
}
