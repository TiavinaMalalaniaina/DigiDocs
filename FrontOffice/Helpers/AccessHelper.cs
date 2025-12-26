namespace FrontOffice.Helpers
{
    public static class AccessHelper
    {
        public static int GetRoleLevel(string role)
        {
            return role switch
            {
                "standard" => 1,
                "premium" => 2,
                "vip" => 3,
                _ => 0
            };
        }

        public static bool CanDownload(string userRole, string documentAccess)
        {
            return GetRoleLevel(userRole) >= GetRoleLevel(documentAccess);
        }
    }
}
