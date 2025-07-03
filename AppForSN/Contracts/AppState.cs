namespace AppForSNForUsers.Contracts
{
    public static class AppState
    {
        public static string JwtToken { get; set; }
        public static string UserFullName { get; set; }
        public static string UserRole { get; set; }
        public static int UserId { get; set; }

        public static void Clear()
        {
            JwtToken = null;
            UserFullName = null;
            UserRole = null;
            UserId = 0;
        }
    }

}
