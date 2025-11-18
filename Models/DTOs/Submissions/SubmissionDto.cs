namespace StudentTeacherManagment.Models.DTOs.Submissions
{
    public class SubmissionDto
    {
        public Guid Id { get; set; }

        public string StudentId { get; set; }
        public string StudentName { get; set; }

        public DateTime SubmittedAt { get; set; }
        public int? Grade { get; set; }
        public string Status { get; set; }
    }
}
