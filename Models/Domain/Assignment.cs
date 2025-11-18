namespace StudentTeacherManagment.Models.Domain
{
    public class Assignment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string TeacherId { get; set; }
        public ApplicationUser Teacher { get; set; }

        public List<Submission> Submissions { get; set; } = new();
    }
}
