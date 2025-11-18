namespace StudentTeacherManagment.Models.Domain
{
    public class Submission
    {

        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public string? FileMimeType { get; set; }


        public int? Grade { get; set; }
        public string Status { get; set; } = "Pending";

        public string StudentId { get; set; }
        public ApplicationUser Student { get; set; }

        public Guid AssignmentId { get; set; }
        public Assignment Assignment { get; set; }
    }
}
