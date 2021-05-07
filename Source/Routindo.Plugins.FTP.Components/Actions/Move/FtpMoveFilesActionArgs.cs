namespace Routindo.Plugins.FTP.Components.Actions.Move
{
    public static class FtpMoveFilesActionArgs
    {
        // Mandatory fields 
        public const string Host = nameof(Host);
        public const string Username = nameof(Username);
        public const string Password = nameof(Password);

        // Optional Fields 
        public const string Port = nameof(Port);
        public const string RemoteWorkingDir = nameof(RemoteWorkingDir);
        public const string DestinationPath = nameof(DestinationPath);
    }
}