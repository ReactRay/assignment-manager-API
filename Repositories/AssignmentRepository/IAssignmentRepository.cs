using StudentTeacherManagment.Models.Domain;

namespace StudentTeacherManagment.Repositories.AssignmentRepository
{
    public interface IAssignmentRepository
    {
        Task<Assignment> CreateAsync(Assignment assignment);
        Task<Assignment?> GetByIdAsync(Guid id);
        Task<IEnumerable<Assignment>> GetAllAsync();
        Task<Assignment?> UpdateAsync(Assignment assignment);
        Task<bool> DeleteAsync(Guid id);
    }
}
