namespace StudentTeacherManagment.Permissions
{
    public static class RolePermissions
    {
        public static readonly Dictionary<string, List<string>> PermissionsByRole =
            new()
            {
                ["Teacher"] = new List<string>
                {
                    AppPermissions.Assignments.Create,
                    AppPermissions.Assignments.Read,
                    AppPermissions.Assignments.Update,
                    AppPermissions.Assignments.Delete,
                    AppPermissions.Assignments.Grade,
                    AppPermissions.Submissions.Read,
                    AppPermissions.Submissions.Download,
                    AppPermissions.Submissions.Grade
                },

                ["Student"] = new List<string>
                {
                    AppPermissions.Assignments.Read,
                    AppPermissions.Submissions.Create,
                    AppPermissions.Submissions.Read
                },

                ["Admin"] = new List<string>
                {
                    AppPermissions.Assignments.Create,
                    AppPermissions.Assignments.Read,
                    AppPermissions.Assignments.Update,
                    AppPermissions.Assignments.Delete,
                    AppPermissions.Assignments.Grade,
                    AppPermissions.Submissions.Create,
                    AppPermissions.Submissions.Read,
                    AppPermissions.Submissions.Download,
                    AppPermissions.Submissions.Grade
                }
            };
    }
}
