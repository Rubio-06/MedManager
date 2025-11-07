namespace MedManager.Web.Helpers
{
    public static class RoleConstants
    {
        public const string Admin = "Admin";
        public const string Doctor = "Doctor";
        public const string Patient = "Patient";

        public const string AdminOrDoctor = "Admin,Doctor";
        public const string All = "Admin,Doctor,Patient";
    }
}
