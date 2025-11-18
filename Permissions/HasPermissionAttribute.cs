using Microsoft.AspNetCore.Authorization;

namespace StudentTeacherManagment.Permissions
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(string permission)
        {
            Policy = permission;
        }
    }
}
