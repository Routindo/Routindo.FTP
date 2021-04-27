namespace Routindo.Plugins.FTP.Components.Actions.DownloadFile
{
    public static class FtpDownloadActionArgs
    {
        // Mandatory fields 
        public const string Host = nameof(Host);
        public const string Username = nameof(Username);
        public const string Password = nameof(Password);

        // Optional Fields 
        public const string Port = nameof(Port);

        // Extension 
        public const string DirectoryPath = nameof(DirectoryPath);

        public const string Overwrite = nameof(Overwrite);
        public const string Append = nameof(Append);

        public const string UseTemporaryName = nameof(UseTemporaryName);

        public const string RemoteWorkingDir = nameof(RemoteWorkingDir);
    }
}