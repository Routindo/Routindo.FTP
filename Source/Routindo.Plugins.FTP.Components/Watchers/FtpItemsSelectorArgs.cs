namespace Routindo.Plugins.FTP.Components.Watchers
{
    public static class FtpItemsSelectorArgs
    { 
        // Mandatory fields 
        public const string Host = nameof(Host);
        public const string Username = nameof(Username);
        public const string Password = nameof(Password);

        // Optional Fields 
        public const string Port = nameof(Port);
        public const string RemoteWorkingDir = nameof(RemoteWorkingDir);

        public const string SelectFiles = nameof(SelectFiles);
        public const string SelectDirectories = nameof(SelectDirectories);

        public const string MaximumFiles = nameof(MaximumFiles);
        public const string CreatedBefore = nameof(CreatedBefore);
        public const string CreatedAfter = nameof(CreatedAfter);
        public const string EditedBefore = nameof(EditedBefore);
        public const string EditedAfter = nameof(EditedAfter);
        public const string SortingCriteria = nameof(SortingCriteria);
    }
}