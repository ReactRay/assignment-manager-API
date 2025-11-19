using StudentTeacherManagment.Models.DTOs.Assignments;
using System.Security.Claims;

namespace StudentTeacherManagment.Services.AssignmentHelpers

{
    public interface IAssignmentService
    {
        Task<AssignmentResponseDto> CreateAsync(ClaimsPrincipal user, CreateAssignmentDto dto);
        Task<IEnumerable<AssignmentResponseDto>> GetAllAsync(ClaimsPrincipal user);
        Task<object> GetByIdAsync(ClaimsPrincipal user, Guid id);
        Task<AssignmentResponseDto> UpdateAsync(ClaimsPrincipal user, Guid id, UpdateAssignmentDto dto);
        Task DeleteAsync(ClaimsPrincipal user, Guid id);
    }
}
