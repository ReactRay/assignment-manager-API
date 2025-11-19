using StudentTeacherManagment.Models.Domain;

namespace StudentTeacherManagment.Services.Token
{
    public interface ITokenService
    {
        string GenerateToken(ApplicationUser user, IList<string> roles);
    }
}
