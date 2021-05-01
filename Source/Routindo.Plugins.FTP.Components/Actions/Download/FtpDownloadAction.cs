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

namespace Routindo.Plugins.FTP.Components.Actions.Download
{
    [PluginItemInfo(ComponentUniqueId, nameof(FtpDownloadAction),
         "Download one or multiple files from remote host via FTP", Category = "FTP", FriendlyName = "Download Files"),
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

        [Argument(FtpDownloadActionArgs.Host, true)] public string Host { get; set; }

        [Argument(FtpDownloadActionArgs.Port, true)] public int Port { get; set; } = DEFAULT_FTP_PORT;

        [Argument(FtpDownloadActionArgs.Username, true)] public string Username { get; set; }

        [Argument(FtpDownloadActionArgs.Password, true)] public string Password { get; set; }

        [Argument(FtpDownloadActionArgs.RemoteWorkingDir, false)] public string RemoteWorkingDir { get; set; }

        [Argument(FtpDownloadActionArgs.DirectoryPath, true)] public string DirectoryPath { get; set; }

        [Argument(FtpDownloadActionArgs.Overwrite, false)] public bool Overwrite { get; set; }

        [Argument(FtpDownloadActionArgs.Append, false)] public bool Append { get; set; }

        [Argument(FtpDownloadActionArgs.UseTemporaryName, false)] public bool UseTemporaryName { get; set; }

        [Argument(FtpDownloadActionArgs.DeleteDownloaded, false)] public bool DeleteDownloaded { get; set; }
        [Argument(FtpDownloadActionArgs.MoveDownloaded, false)] public bool MoveDownloaded { get; set; }
        [Argument(FtpDownloadActionArgs.MoveDownloadedPath, false)] public string MoveDownloadedPath { get; set; }

        [Argument(FtpDownloadActionArgs.RenameDownloaded, false)] public bool RenameDownloaded { get; set; }

        [Argument(FtpDownloadActionArgs.RenameDownloadedExtension, false)] public string RenameDownloadedExtension { get; set; }

        [Argument(FtpDownloadActionArgs.RenameDownloadedPrefix, false)] public string RenameDownloadedPrefix { get; set; }

        [Argument(FtpDownloadActionArgs.RenameDownloadedNewName, false)] public string RenameDownloadedNewName { get; set; }

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

                    if(MoveDownloaded || DeleteDownloaded || RenameDownloaded)
                        foreach (var downloadedFile in downloadedFiles)
                        {
                            if (DeleteDownloaded) ftpClient.DeleteFile(downloadedFile);
                            else if (MoveDownloaded && !string.IsNullOrWhiteSpace(MoveDownloadedPath))
                                ftpClient.MoveFile(downloadedFile, UriHelper.BuildPath(MoveDownloadedPath, Path.GetFileName(downloadedFile)));
                            else if (RenameDownloaded)
                            {
                                LoggingService.Debug($"Trying to rename remote file {downloadedFile}");
                                var remoteParentPath = UriHelper.GetParentUriString(downloadedFile);
                                if (!string.IsNullOrWhiteSpace(RenameDownloadedNewName))
                                {
                                    LoggingService.Debug($"Renaming remote file {downloadedFile} to {RenameDownloadedNewName}");
                                    var newName = UriHelper.BuildPath(remoteParentPath, RenameDownloadedNewName);
                                    ftpClient.Rename(downloadedFile, newName);
                                    LoggingService.Debug($"Renamed file {downloadedFile} to {newName}");
                                }
                                else
                                {
                                    LoggingService.Debug($"Trying to apply prefix or extension to remote file {downloadedFile}");
                                    var fileName = Path.GetFileName(downloadedFile);
                                    if (!string.IsNullOrWhiteSpace(RenameDownloadedExtension))
                                    {
                                        LoggingService.Debug($"Changing extension to {RenameDownloadedExtension}");
                                        fileName = Path.ChangeExtension(fileName, RenameDownloadedExtension);
                                    }

                                    if (!string.IsNullOrWhiteSpace(RenameDownloadedPrefix))
                                    {
                                        LoggingService.Debug($"Applying prefix to {RenameDownloadedPrefix}");
                                        fileName = $"{RenameDownloadedPrefix}{fileName}";
                                    }
                                    LoggingService.Debug($"Renaming remote file {downloadedFile} to {fileName}");
                                    var newName = UriHelper.BuildPath(remoteParentPath, fileName);
                                    ftpClient.Rename(downloadedFile, newName); 
                                    LoggingService.Debug($"Renamed file {downloadedFile} to {newName}");
                                }
                            }
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
