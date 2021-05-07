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

namespace Routindo.Plugins.FTP.Components.Actions.Move
{
    [PluginItemInfo(ComponentUniqueId, nameof(FtpMoveFilesAction),
         "Move one or multiple files from one location to another on remote host via FTP", Category = "FTP", FriendlyName = "Move Files"),
     ExecutionArgumentsClass(typeof(FtpMoveFilesActionExecutionArgs)),
     ResultArgumentsClass(typeof(FtpMoveFilesActionResultArgs))
    ]
    public class FtpMoveFilesAction: IAction
    {
        public const string ComponentUniqueId = "DA11A4CE-E2EE-4F23-9A5B-6BF0E6750547";

        public const int DEFAULT_FTP_PORT = 21;

        public string Id { get; set; }
        public ILoggingService LoggingService { get; set; }

        [Argument(FtpMoveFilesActionArgs.Host, true)] public string Host { get; set; }

        [Argument(FtpMoveFilesActionArgs.Port, true)] public int Port { get; set; } = DEFAULT_FTP_PORT;

        [Argument(FtpMoveFilesActionArgs.Username, true)] public string Username { get; set; }

        [Argument(FtpMoveFilesActionArgs.Password, true)] public string Password { get; set; }

        [Argument(FtpMoveFilesActionArgs.RemoteWorkingDir, false)] public string RemoteWorkingDir { get; set; }

        [Argument(FtpMoveFilesActionArgs.DestinationPath, false)] public string DestinationPath { get; set; }

        public ActionResult Execute(ArgumentCollection arguments)
        {
            List<string> movedFiles = new List<string>();
            List<string> failedFiles = new List<string>(); 
            try
            {
                // Must provide arguments
                if (arguments == null)
                    throw new ArgumentNullException(nameof(arguments));

                if (!arguments.HasArgument(FtpMoveFilesActionExecutionArgs.RemoteFilesCollection))
                    throw new MissingArgumentException(FtpMoveFilesActionExecutionArgs.RemoteFilesCollection);

                if (!(arguments[FtpMoveFilesActionExecutionArgs.RemoteFilesCollection] is List<string> remotePaths))
                    throw new ArgumentsValidationException(
                        $"unable to cast argument value into list of string. key({FtpMoveFilesActionExecutionArgs.RemoteFilesCollection})");


                using (FtpClient ftpClient = new FtpClient(Host, Port, Username, Password))
                {
                    if (!string.IsNullOrWhiteSpace(RemoteWorkingDir))
                        ftpClient.SetWorkingDirectory(RemoteWorkingDir);
                    foreach (var remotePath in remotePaths)
                    {
                        try
                        {
                            LoggingService.Debug($"Moving remote file {remotePath} to {DestinationPath}");
                            if (ftpClient.MoveFile(remotePath,
                                UriHelper.BuildPath(DestinationPath, Path.GetFileName(remotePath))))
                            {
                                movedFiles.Add(remotePath);
                            }
                            failedFiles.Add(remotePath);
                        }
                        catch (Exception exception)
                        {
                            LoggingService.Error(exception);
                            failedFiles.Add(remotePath);
                        }
                    }
                }

                if (movedFiles.Any())
                {
                    return ActionResult.Succeeded()
                            .WithAdditionInformation(ArgumentCollection.New()
                                .WithArgument(FtpMoveFilesActionResultArgs.MovedFiles, movedFiles)
                                .WithArgument(FtpMoveFilesActionResultArgs.FailedFiles, failedFiles)
                            )
                        ;
                }

                return ActionResult.Failed().WithAdditionInformation(
                    ArgumentCollection.New()
                        .WithArgument(FtpMoveFilesActionResultArgs.MovedFiles, movedFiles)
                        .WithArgument(FtpMoveFilesActionResultArgs.FailedFiles, failedFiles)
                    );
            }
            catch (Exception exception)
            {
                LoggingService.Error(exception);
                return ActionResult.Failed(exception)
                    .WithAdditionInformation(ArgumentCollection.New()
                        .WithArgument(FtpMoveFilesActionResultArgs.MovedFiles, movedFiles)
                        .WithArgument(FtpMoveFilesActionResultArgs.FailedFiles, failedFiles));
            }
        }
    }
}
