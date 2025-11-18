using Microsoft.AspNetCore.Identity;

namespace StudentTeacherManagment.Models.Domain
{
    public class ApplicationUser :IdentityUser
    {
        public string FullName { get; set; }

        public ICollection<Assignment> CreatedAssignments { get; set; } = new List<Assignment>();
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    }
}
