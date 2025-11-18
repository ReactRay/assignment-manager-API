namespace StudentTeacherManagment.Permissions
{
    public static class RolePermissions
    {
        public static readonly Dictionary<string, List<string>> PermissionsByRole =
            new()
            {
                // TEACHER PERMISSIONS
                ["Teacher"] = new List<string>
                {
                    Permissions.CreateAssignment,
                    Permissions.EditAssignment,
                    Permissions.DeleteAssignment,
                    Permissions.GradeAssignment,
                    Permissions.ViewAssignments,
                    Permissions.ViewStudentSubmissions
                },

                // STUDENT PERMISSIONS
                ["Student"] = new List<string>
                {
                    Permissions.ViewAssignments,
                    Permissions.SubmitAssignment
                },

                // ADMIN (BONUS)
                ["Admin"] = new List<string>
                {
                    Permissions.CreateAssignment,
                    Permissions.EditAssignment,
                    Permissions.DeleteAssignment,
                    Permissions.GradeAssignment,
                    Permissions.ViewAssignments,
                    Permissions.SubmitAssignment,
                    Permissions.ViewStudentSubmissions
                }
            };
    }
}
