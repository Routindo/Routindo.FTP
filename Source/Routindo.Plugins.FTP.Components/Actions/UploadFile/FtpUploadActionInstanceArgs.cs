namespace Routindo.Plugins.FTP.Components.Actions.UploadFile
{
    public static class FtpUploadActionInstanceArgs
    {
        // Mandatory fields 
        public const string Host = nameof(Host);
        public const string Username = nameof(Username);
        public const string Password = nameof(Password);

        // Optional Fields 
        public const string Port = nameof(Port);
        public const string DestinationFolderPath = nameof(DestinationFolderPath);
        public const string DestinationFileName = nameof(DestinationFileName);

        // Extension 
        public const string UseRemoteTemporaryExtension = nameof(UseRemoteTemporaryExtension); 
        public const string RemoteTemporaryExtension = nameof(RemoteTemporaryExtension);

        public const string UseLocalTemporaryExtension = nameof(UseLocalTemporaryExtension);
        public const string LocalTemporaryExtension = nameof(LocalTemporaryExtension);

        public const string Overwrite = nameof(Overwrite);
        public const string CreateRemoteDirectory = nameof(CreateRemoteDirectory);
    }
}