namespace VoxDocs.DTO
{
    // ViewModel para informações de conta de Administrador
    public class AdminInfoAccount
    {
        public required DTOUserInfo UserInfo { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public List<string> AvailablePermissions { get; } = new List<string> { "admin", "user", "manager" };
        public List<DTOUserInfo> RecentUsers { get; set; } = new List<DTOUserInfo>();
        public string CurrentPermission { get; } = "admin";
        public bool CanManageUsers { get; } = true;
        public bool CanEditPlans { get; } = true;
        public bool CanAccessReports { get; } = true;
    }

    // ViewModel para informações de conta de Usuário normal
    public class UserInfoAccount
    {
        public required DTOUserInfo UserInfo { get; set; }
        public string CurrentPermission { get; } = "user";
        public bool CanUploadFiles { get; } = true;
        public bool CanCreateFolders { get; set; }
        public int StorageUsage { get; set; } // em MB
        public int StorageLimit { get; set; } // em MB
        public List<string> AvailableActions { get; } = new List<string> { "upload", "download", "view" };
    }
}