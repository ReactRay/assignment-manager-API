using StudentTeacherManagment.Models.DTOs.Submissions;

namespace StudentTeacherManagment.Models.DTOs.Assignments
{
    public class AssignmentResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }

        public string TeacherId { get; set; }
        public string TeacherName { get; set; }

        public List<SubmissionDto> Submissions { get; set; } = new();

    }
}
