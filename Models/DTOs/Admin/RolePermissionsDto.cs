namespace StudentTeacherManagment.Models.DTOs.Admin
{
    public class RolePermissionsDto
    {
        public string RoleName { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
    }
}
