namespace Routindo.Plugins.FTP.Components.Actions.Delete
{
    public static class FtpDeleteActionArgs
    {
        // Mandatory fields 
        public const string Host = nameof(Host);
        public const string Username = nameof(Username);
        public const string Password = nameof(Password);

        // Optional Fields 
        public const string Port = nameof(Port);
        public const string RemoteWorkingDir = nameof(RemoteWorkingDir);
    }
}