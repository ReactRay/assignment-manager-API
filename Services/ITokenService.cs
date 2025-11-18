using StudentTeacherManagment.Models.Domain;

namespace StudentTeacherManagment.Services
{
    public interface ITokenService
    {
        string GenerateToken(ApplicationUser user, IList<string> roles);

    }
}
