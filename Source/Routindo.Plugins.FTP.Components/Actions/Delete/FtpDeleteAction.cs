using System;
using System.Collections.Generic;
using System.Linq;
using FluentFTP;
using Routindo.Contract.Actions;
using Routindo.Contract.Arguments;
using Routindo.Contract.Attributes;
using Routindo.Contract.Exceptions;
using Routindo.Contract.Services;

namespace Routindo.Plugins.FTP.Components.Actions.Delete
{
    [PluginItemInfo(ComponentUniqueId, nameof(FtpDeleteAction),
         "Delete one or multiple files from remote host via FTP", Category = "FTP", FriendlyName = "Delete Files"),
     ExecutionArgumentsClass(typeof(FtpDeleteActionExecutionArgs)),
     ResultArgumentsClass(typeof(FtpDeleteActionResultArgs))
    ]
    public class FtpDeleteAction: IAction
    {
        public const string ComponentUniqueId = "1FE5316A-126D-4D25-83B3-DC556E2F1B6D";

        public const int DEFAULT_FTP_PORT = 21;

        public string Id { get; set; }
        public ILoggingService LoggingService { get; set; }

        [Argument(FtpDeleteActionArgs.Host, true)] public string Host { get; set; }

        [Argument(FtpDeleteActionArgs.Port, true)] public int Port { get; set; } = DEFAULT_FTP_PORT;

        [Argument(FtpDeleteActionArgs.Username, true)] public string Username { get; set; }

        [Argument(FtpDeleteActionArgs.Password, true)] public string Password { get; set; }

        [Argument(FtpDeleteActionArgs.RemoteWorkingDir, false)] public string RemoteWorkingDir { get; set; }

        public ActionResult Execute(ArgumentCollection arguments)
        {
            List<string> deletedFiles = new List<string>();
            List<string> failedFiles = new List<string>();
            try
            {
                // Must provide arguments
                if (arguments == null)
                    throw new ArgumentNullException(nameof(arguments));

                if (!arguments.HasArgument(FtpDeleteActionExecutionArgs.RemoteFilesCollection))
                    throw new MissingArgumentException(FtpDeleteActionExecutionArgs.RemoteFilesCollection);

                if (!(arguments[FtpDeleteActionExecutionArgs.RemoteFilesCollection] is List<string> remotePaths))
                    throw new ArgumentsValidationException(
                        $"unable to cast argument value into list of string. key({FtpDeleteActionExecutionArgs.RemoteFilesCollection})");


                using (FtpClient ftpClient = new FtpClient(Host, Port, Username, Password))
                {
                    if (!string.IsNullOrWhiteSpace(RemoteWorkingDir))
                        ftpClient.SetWorkingDirectory(RemoteWorkingDir);
                    foreach (var remotePath in remotePaths)
                    {
                        try
                        {
                            LoggingService.Debug($"Deleting remote file {remotePath}");
                            ftpClient.DeleteFile(remotePath);
                            deletedFiles.Add(remotePath);
                        }
                        catch (Exception exception)
                        {
                            LoggingService.Error(exception);
                            failedFiles.Add(remotePath);
                        }
                    }
                }

                if (deletedFiles.Any())
                {
                    return ActionResult.Succeeded()
                            .WithAdditionInformation(ArgumentCollection.New()
                                .WithArgument(FtpDeleteActionResultArgs.DeletedFiles, deletedFiles)
                                .WithArgument(FtpDeleteActionResultArgs.FailedFiles, failedFiles)
                            )
                        ;
                }

                return ActionResult.Failed().WithAdditionInformation(
                    ArgumentCollection.New()
                        .WithArgument(FtpDeleteActionResultArgs.DeletedFiles, deletedFiles)
                        .WithArgument(FtpDeleteActionResultArgs.FailedFiles, failedFiles)
                    );
            }
            catch (Exception exception)
            {
                LoggingService.Error(exception);
                return ActionResult.Failed(exception)
                    .WithAdditionInformation(ArgumentCollection.New()
                        .WithArgument(FtpDeleteActionResultArgs.DeletedFiles, deletedFiles)
                        .WithArgument(FtpDeleteActionResultArgs.FailedFiles, failedFiles));
            }
        }
    }
}
