using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using FluentFTP;
using Umator.Contract;
using Umator.Contract.Services;

namespace Umator.Plugins.FTP.Components
{
    [PluginItemInfo(ComponentUniqueId, "FTP Uploader",
         "Upload a File to remote host via FTP"),
     ExecutionArgumentsClass(typeof(FtpUploadActionExecutionArgs))]
    public class FtpUploadAction: IAction
    {
        public const string ComponentUniqueId = "D911DBBC-22D2-4E85-8880-D50D8618B049";

        private const int DEFAULT_FTP_PORT = 21;
        private const string DEFAULT_TEMP_EXTENSION = "temp";

        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        [Argument(FtpUploadActionInstanceArgs.Host, true)] public string Host { get; set; }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>
        /// The port.
        /// </value>
        [Argument(FtpUploadActionInstanceArgs.Port, true)] public int Port { get; set; } = DEFAULT_FTP_PORT;

        

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        [Argument(FtpUploadActionInstanceArgs.Username, true)] public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [Argument(FtpUploadActionInstanceArgs.Password, true)] public string Password { get; set; }

        /// <summary>
        /// Gets or sets the destination folder path.
        /// </summary>
        /// <value>
        /// The destination folder path.
        /// </value>
        [Argument(FtpUploadActionInstanceArgs.DestinationFolderPath, false)] public string DestinationFolderPath { get; set; }

        /// <summary>
        /// Gets or sets the name of the destination file.
        /// </summary>
        /// <value>
        /// The name of the destination file.
        /// </value>
        [Argument(FtpUploadActionInstanceArgs.DestinationFileName, false)] public string DestinationFileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use local temporary extension].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use local temporary extension]; otherwise, <c>false</c>.
        /// </value>
        [Argument(FtpUploadActionInstanceArgs.UseLocalTemporaryExtension, false)]  public bool UseLocalTemporaryExtension { get; set; } = false;
        /// <summary>
        /// Gets or sets the local temporary extension.
        /// </summary>
        /// <value>
        /// The local temporary extension.
        /// </value>
        [Argument(FtpUploadActionInstanceArgs.LocalTemporaryExtension, false)] public string LocalTemporaryExtension { get; set; } = DEFAULT_TEMP_EXTENSION;

        /// <summary>
        /// Gets or sets a value indicating whether [use remote temporary extension].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use remote temporary extension]; otherwise, <c>false</c>.
        /// </value>
        [Argument(FtpUploadActionInstanceArgs.UseRemoteTemporaryExtension, false)] public bool UseRemoteTemporaryExtension { get; set; } = false;
        /// <summary>
        /// Gets or sets the remote temporary extension.
        /// </summary>
        /// <value>
        /// The remote temporary extension.
        /// </value>
        [Argument(FtpUploadActionInstanceArgs.RemoteTemporaryExtension, false)] public string RemoteTemporaryExtension { get; set; } = DEFAULT_TEMP_EXTENSION;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="FtpUploadAction"/> is overwrite.
        /// </summary>
        /// <value>
        ///   <c>true</c> if overwrite; otherwise, <c>false</c>.
        /// </value>
        [Argument(FtpUploadActionInstanceArgs.Overwrite, false)] public bool Overwrite { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [create remote directory].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [create remote directory]; otherwise, <c>false</c>.
        /// </value>
        [Argument(FtpUploadActionInstanceArgs.CreateRemoteDirectory, false)] public bool CreateRemoteDirectory { get; set; }

        /// <summary>
        /// Executes the specified arguments.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        public ActionResult Execute(ArgumentCollection arguments)
        {
            // string uploadFileName = null; // , sourceFilePath = null;
            try
            {

                // Must provide arguments
                if (arguments == null)
                    throw new ArgumentNullException(nameof(arguments));

                if (!arguments.HasArgument(FtpUploadActionExecutionArgs.SourceFilesCollection))
                    throw new MissingArgumentException(FtpUploadActionExecutionArgs.SourceFilesCollection);

                if (!(arguments[FtpUploadActionExecutionArgs.SourceFilesCollection] is List<string> filePaths))
                    throw new ArgumentsValidationException(
                        $"unable to cast argument value into list of string. key({FtpUploadActionExecutionArgs.SourceFilesCollection})");


                foreach (var sourceFilePath in filePaths)
                {
                    string uploadFileName = null;
                    try
                    {
                        if (!File.Exists(sourceFilePath))
                        {
                            // File not found 
                            return ActionResult.Failed().WithAdditionInformation(ArgumentCollection.New()
                                .WithArgument("ERROR", $"File Not Exist : {sourceFilePath}"));
                        }

                        if (string.IsNullOrWhiteSpace(Host))
                            return ActionResult.Failed().WithAdditionInformation(ArgumentCollection.New()
                                .WithArgument("ERROR",
                                    $"Missing Instance Argument {FtpUploadActionInstanceArgs.Host}"));

                        if (string.IsNullOrWhiteSpace(Username))
                            return ActionResult.Failed().WithAdditionInformation(ArgumentCollection.New()
                                .WithArgument("ERROR",
                                    $"Missing Instance Argument {FtpUploadActionInstanceArgs.Username}"));

                        if (string.IsNullOrWhiteSpace(Password))
                            return ActionResult.Failed().WithAdditionInformation(ArgumentCollection.New()
                                .WithArgument("ERROR",
                                    $"Missing Instance Argument {FtpUploadActionInstanceArgs.Password}"));

                        uploadFileName = sourceFilePath;
                        if (UseLocalTemporaryExtension && !string.IsNullOrWhiteSpace(LocalTemporaryExtension))
                        {
                            uploadFileName =
                                Path.ChangeExtension(sourceFilePath, LocalTemporaryExtension.TrimStart('.'));
                            File.Move(sourceFilePath, uploadFileName);
                        }

                        string uploadRemoteFileName = null;
                        if (UseRemoteTemporaryExtension && !string.IsNullOrWhiteSpace(RemoteTemporaryExtension))
                        {
                            if (!string.IsNullOrWhiteSpace(DestinationFileName))
                            {
                                uploadRemoteFileName =
                                    $"{Path.ChangeExtension(DestinationFileName, null)}.{RemoteTemporaryExtension.TrimStart('.')}";
                            }
                            else
                            {
                                uploadRemoteFileName =
                                    $"{Path.GetFileNameWithoutExtension(sourceFilePath)}.{RemoteTemporaryExtension.TrimStart('.')}";
                            }
                        }

                        using (FtpClient ftpClient = new FtpClient(Host, Port, Username, Password))
                        {
                            var tempRemotePath = UriHelper.BuildPath(DestinationFolderPath, uploadRemoteFileName);

                            // Upload the file
                            var uploadStatus = ftpClient.UploadFile(uploadFileName, tempRemotePath,
                                Overwrite ? FtpRemoteExists.Overwrite : FtpRemoteExists.NoCheck, CreateRemoteDirectory);
                            // TODO Log Upload STatus

                            // Rename file after upload
                            string finalFileName = string.IsNullOrWhiteSpace(DestinationFileName)
                                ? Path.GetFileName(sourceFilePath)
                                : DestinationFileName;

                            string finalRemotePath = UriHelper.BuildPath(DestinationFolderPath, finalFileName);
                            ftpClient.Rename(tempRemotePath, finalRemotePath);
                        }
                    }
                    catch (Exception exception)
                    {
                        // TODO Log 
                    }
                    finally
                    {
                        // Rename source file to original extension 
                        if (UseLocalTemporaryExtension && !string.IsNullOrWhiteSpace(uploadFileName) && !string.IsNullOrWhiteSpace(sourceFilePath))
                        {
                            if (!string.Equals(uploadFileName, sourceFilePath, StringComparison.CurrentCultureIgnoreCase) && File.Exists(uploadFileName) && !File.Exists(sourceFilePath))
                            {
                                File.Move(uploadFileName, sourceFilePath);
                            }
                        }
                    }
                }

                

                return ActionResult.Succeeded();
            }
            catch (Exception exception)
            {
                // _loggingService.Error(exception.ToString());
                return ActionResult.Failed().WithException(exception);
            }
        }
    }

    public static class FtpUploadActionExecutionArgs 
    {
        public const string SourceFilesCollection = nameof(SourceFilesCollection);
    }
}
