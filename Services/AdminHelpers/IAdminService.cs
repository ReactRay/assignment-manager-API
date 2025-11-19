using StudentTeacherManagment.Models.DTOs.Admin;

namespace StudentTeacherManagment.Services.AdminHelpers
{
    public interface IAdminService
    {
        Task<List<UserWithRolesDto>> GetAllUsersAsync();
        Task<string> CreateUserWithRoleAsync(CreateUserRequestDto dto, string role);
        Task AssignRoleAsync(string userId, string roleName);
        Task RemoveRoleAsync(string userId, string roleName);
      
    }

}
