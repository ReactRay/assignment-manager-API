namespace StudentTeacherManagment.Models.DTOs.Assignments
{
    public class CreateAssignmentDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
    }
}
