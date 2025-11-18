namespace StudentTeacherManagment.Models.DTOs.Assignments
{
  
    public class AssignmentDetailsDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }

        public string TeacherId { get; set; }
        public string TeacherName { get; set; }

        // Collection of submissions (varies based on user role)
        public List<SubmissionDto> Submissions { get; set; } = new();
    }
}
