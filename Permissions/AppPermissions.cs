namespace StudentTeacherManagment.Permissions
{
    public static class AppPermissions
    {
        public static class Assignments
        {
            public const string Create = "Permissions.Assignments.Create";
            public const string Read = "Permissions.Assignments.Read";
            public const string Update = "Permissions.Assignments.Update";
            public const string Delete = "Permissions.Assignments.Delete";
            public const string Grade = "Permissions.Assignments.Grade";
        }

        public static class Submissions
        {
            public const string Create = "Permissions.Submissions.Create";
            public const string Read = "Permissions.Submissions.Read";
            public const string Download = "Permissions.Submissions.Download";
            public const string Grade = "Permissions.Submissions.Grade";
        }
    }
}