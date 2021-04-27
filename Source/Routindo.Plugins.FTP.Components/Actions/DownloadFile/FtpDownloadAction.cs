using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentFTP;
using Routindo.Contract.Actions;
using Routindo.Contract.Arguments;
using Routindo.Contract.Attributes;
using Routindo.Contract.Exceptions;
using Routindo.Contract.Services;

namespace Routindo.Plugins.FTP.Components.Actions.DownloadFile
{
    [PluginItemInfo(ComponentUniqueId, "FTP File Downloader",
         "Download one or multiple files from remote host via FTP"),
     ExecutionArgumentsClass(typeof(FtpDownloadActionExecutionArgs)), 
    ResultArgumentsClass(typeof(FtpDownloadActionResultArgs))
    ]
    public class FtpDownloadAction : IAction
    {

        public const string ComponentUniqueId = "80901D31-1766-4E2F-A369-0305DC2DDC76";

        public const int DEFAULT_FTP_PORT = 21;
        public const string DEFAULT_TEMP_EXTENSION = "temp";

        public string Id { get; set; }
        public ILoggingService LoggingService { get; set; }


        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        [Argument(FtpDownloadActionArgs.Host, true)]
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        [Argument(FtpDownloadActionArgs.Port, true)]
        public int Port { get; set; } = DEFAULT_FTP_PORT;



        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        [Argument(FtpDownloadActionArgs.Username, true)]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [Argument(FtpDownloadActionArgs.Password, true)]
        public string Password { get; set; }

        [Argument(FtpDownloadActionArgs.RemoteWorkingDir, true)]
        public string RemoteWorkingDir { get; set; }

        [Argument(FtpDownloadActionArgs.DirectoryPath, true)]
        public string DirectoryPath { get; set; }

        [Argument(FtpDownloadActionArgs.Overwrite, true)]
        public bool Overwrite { get; set; }

        [Argument(FtpDownloadActionArgs.Append, true)]
        public bool Append { get; set; }

        [Argument(FtpDownloadActionArgs.UseTemporaryName, true)]
        public bool UseTemporaryName { get; set; }

        public ActionResult Execute(ArgumentCollection arguments)
        {
            try
            {
                // Must provide arguments
                if (arguments == null)
                    throw new ArgumentNullException(nameof(arguments));

                if (!arguments.HasArgument(FtpDownloadActionExecutionArgs.RemoteFilesCollection))
                    throw new MissingArgumentException(FtpDownloadActionExecutionArgs.RemoteFilesCollection);

                if (!(arguments[FtpDownloadActionExecutionArgs.RemoteFilesCollection] is List<string> remotePaths))
                    throw new ArgumentsValidationException(
                        $"unable to cast argument value into list of string. key({FtpDownloadActionExecutionArgs.RemoteFilesCollection})");

                FtpLocalExists existMode = Overwrite
                    ? FtpLocalExists.Overwrite
                    : Append
                        ? FtpLocalExists.Append
                        : FtpLocalExists.Skip;

                LoggingService.Trace($"ExistMode: {existMode:G}");

                var directoryPath = Environment.ExpandEnvironmentVariables(DirectoryPath);

                List<string> downloadedFiles = new List<string>();
                List<string> skippedFiles = new List<string>();
                List<string> failedFiles = new List<string>();
                if (UseTemporaryName && existMode != FtpLocalExists.Append)
                    LoggingService.Warn($"Cannot use temporary name for download in mode Append.");
                using (FtpClient ftpClient = new FtpClient(Host, Port, Username, Password))
                {
                    if (!string.IsNullOrWhiteSpace(RemoteWorkingDir))
                        ftpClient.SetWorkingDirectory(RemoteWorkingDir);
                    foreach (var remotePath in remotePaths)
                    {
                        var remoteFileName = UriHelper.GetFileName(remotePath);
                        var localPath = Path.Combine(directoryPath, remoteFileName);
                        string temporaryPath = string.Empty;
                        bool useTemporaryName = UseTemporaryName && existMode != FtpLocalExists.Append;

                        if (useTemporaryName)
                            temporaryPath = Path.ChangeExtension(Path.Combine(directoryPath, Path.GetRandomFileName()),
                                DEFAULT_TEMP_EXTENSION);

                        var downloadPath = useTemporaryName ? temporaryPath : localPath;

                        LoggingService.Debug($"Downloading file {remotePath} to temporary path {downloadPath}");

                        var ftpStatus = ftpClient.DownloadFile(downloadPath, remotePath, existMode);

                        LoggingService.Debug($"File download completed with status {ftpStatus:G}");

                        if (ftpStatus == FtpStatus.Success && useTemporaryName)
                        {
                            LoggingService.Trace($"Renaming file {downloadPath} to {localPath}");
                            File.Move(downloadPath, localPath, Overwrite);
                        }

                        if (ftpStatus == FtpStatus.Success)
                            downloadedFiles.Add(remotePath);

                        if (ftpStatus == FtpStatus.Skipped)
                            skippedFiles.Add(remotePath);

                        if (ftpStatus == FtpStatus.Failed)
                            failedFiles.Add(remotePath);
                    }
                }

                if (downloadedFiles.Any())
                    return ActionResult.Succeeded()
                        .WithAdditionInformation(ArgumentCollection.New()
                            .WithArgument(FtpDownloadActionResultArgs.DownloadedFiles, downloadedFiles)
                            .WithArgument(FtpDownloadActionResultArgs.SkippedFiles, skippedFiles)
                            .WithArgument(FtpDownloadActionResultArgs.FailedFiles, failedFiles)
                        );

                return ActionResult.Failed()
                    .WithAdditionInformation(ArgumentCollection.New()
                        .WithArgument(FtpDownloadActionResultArgs.DownloadedFiles, downloadedFiles)
                        .WithArgument(FtpDownloadActionResultArgs.SkippedFiles, skippedFiles)
                        .WithArgument(FtpDownloadActionResultArgs.FailedFiles, failedFiles)
                    );
            }
            catch (Exception exception)
            {
                LoggingService.Error(exception);
                return ActionResult.Failed(exception)
                    .WithAdditionInformation(ArgumentCollection.New()
                        .WithArgument(FtpDownloadActionResultArgs.DownloadedFiles, new List<string>())
                        .WithArgument(FtpDownloadActionResultArgs.SkippedFiles, new List<string>())
                        .WithArgument(FtpDownloadActionResultArgs.FailedFiles, new List<string>())
                    );
            }
        }
    }
}
