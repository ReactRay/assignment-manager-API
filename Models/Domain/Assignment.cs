namespace StudentTeacherManagment.Models.Domain
{
    public class Assignment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }

        // FK — only the teacher who created it
        public string TeacherId { get; set; }
        public ApplicationUser Teacher { get; set; }

        // Every assignment can have many student submissions
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    }
}
