namespace StudentTeacherManagment.Permissions
{
    public static class RolePermissions
    {
        public static readonly Dictionary<string, List<string>> PermissionsByRole =
            new()
            {
                ["Teacher"] = new List<string>
                {
                    Permissions.Assignments.Create,
                    Permissions.Assignments.Read,
                    Permissions.Assignments.Update,
                    Permissions.Assignments.Delete,
                    Permissions.Assignments.Grade,

                    Permissions.Submissions.Read,
                    Permissions.Submissions.Download,
                    Permissions.Submissions.Grade
                },

                ["Student"] = new List<string>
                {
                    Permissions.Assignments.Read,
                    Permissions.Submissions.Create,
                    Permissions.Submissions.Read
                },

                ["Admin"] = new List<string>
                {
                    Permissions.Assignments.Create,
                    Permissions.Assignments.Read,
                    Permissions.Assignments.Update,
                    Permissions.Assignments.Delete,
                    Permissions.Assignments.Grade,

                    Permissions.Submissions.Create,
                    Permissions.Submissions.Read,
                    Permissions.Submissions.Download,
                    Permissions.Submissions.Grade
                }
            };
    }
}
