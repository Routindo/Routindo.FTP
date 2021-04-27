using System;
using System.Linq;
using FluentFTP;
using Routindo.Contract.Arguments;
using Routindo.Contract.Attributes;
using Routindo.Contract.Services;
using Routindo.Contract.Watchers;

namespace Routindo.Plugins.FTP.Components.Watchers.Files
{
    public class FtpFilesWatcher: IWatcher
    {
        public const int DEFAULT_FTP_PORT = 21;
        public const string ComponentUniqueId = "DA3BC9D3-5F1E-4FFD-93AB-E0FC08FEF596";

        public string Id { get; set; }
        public ILoggingService LoggingService { get; set; }

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        [Argument(FtpFilesWatcherArgs.Host, true)] public string Host { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        [Argument(FtpFilesWatcherArgs.Port, true)] public int Port { get; set; } = DEFAULT_FTP_PORT;



        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        [Argument(FtpFilesWatcherArgs.Username, true)] public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [Argument(FtpFilesWatcherArgs.Password, true)] public string Password { get; set; }

        [Argument(FtpFilesWatcherArgs.RemoteWorkingDir, false)] public string RemoteWorkingDir { get; set; }

        public WatcherResult Watch()
        {
            try
            {
                using (FtpClient ftpClient = new FtpClient(Host, Port, Username, Password))
                {
                    if (!string.IsNullOrWhiteSpace(RemoteWorkingDir))
                        ftpClient.SetWorkingDirectory(RemoteWorkingDir);

                    var items = ftpClient.GetListing();
                    var files = items.Where(i => i.Type == FtpFileSystemObjectType.File).ToList();
                    if(files.Any())
                        return WatcherResult.Succeed(ArgumentCollection.New()
                                .WithArgument(FtpFilesWatcherResultArgs.RemoteFilesCollection, files.Select(item => item.FullName).ToList())
                        );

                    return WatcherResult.NotFound;
                }
            }
            catch (Exception exception)
            {
                LoggingService.Error(exception);
                return WatcherResult.NotFound;
            }
        }
    }

    public static class FtpFilesWatcherArgs
    {
        // Mandatory fields 
        public const string Host = nameof(Host);
        public const string Username = nameof(Username);
        public const string Password = nameof(Password);

        // Optional Fields 
        public const string Port = nameof(Port);
        public const string RemoteWorkingDir = nameof(RemoteWorkingDir);
    }

    public static class FtpFilesWatcherResultArgs
    {
        public const string RemoteFilesCollection = nameof(RemoteFilesCollection);
    }
}
