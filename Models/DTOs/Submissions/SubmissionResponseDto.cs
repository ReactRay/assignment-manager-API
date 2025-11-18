namespace StudentTeacherManagment.Models.DTOs.Submissions
{
    public class SubmissionResponseDto
    {
        public Guid Id { get; set; }
        public DateTime SubmittedAt { get; set; }

        public int? Grade { get; set; }
        public string Status { get; set; }

        public string StudentId { get; set; }
        public string StudentName { get; set; }

        public Guid AssignmentId { get; set; }

        // 🔽 new
        public string? FileUrl { get; set; }
        public string? FileName { get; set; }
    }
}
