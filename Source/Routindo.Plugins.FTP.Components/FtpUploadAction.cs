using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using FluentFTP;
using Routindo.Contract;
using Routindo.Contract.Actions;
using Routindo.Contract.Arguments;
using Routindo.Contract.Attributes;
using Routindo.Contract.Exceptions;
using Routindo.Contract.Services;

namespace Routindo.Plugins.FTP.Components
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

        public ILoggingService LoggingService { get; set; }

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
                    LoggingService.Debug($"Uploading file {sourceFilePath}");
                    string uploadFileName = null;
                    try
                    {
                        if (!File.Exists(sourceFilePath))
                        {
                            LoggingService.Error($"File doesn't not exist {sourceFilePath}");
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(Host))
                        {
                            LoggingService.Error($"Missing Instance Argument {FtpUploadActionInstanceArgs.Host}");
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(Username))
                        {
                            LoggingService.Error($"Missing Instance Argument {FtpUploadActionInstanceArgs.Username}");
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(Password))
                        {
                            LoggingService.Error($"Missing Instance Argument {FtpUploadActionInstanceArgs.Password}");
                            continue;
                        }

                        uploadFileName = sourceFilePath;
                        if (UseLocalTemporaryExtension && !string.IsNullOrWhiteSpace(LocalTemporaryExtension))
                        {
                            LoggingService.Debug($"Renaming file with temporary extension {LocalTemporaryExtension} => From {sourceFilePath} to {uploadFileName}");
                            uploadFileName =
                                Path.ChangeExtension(sourceFilePath, LocalTemporaryExtension.TrimStart('.'));
                            if (File.Exists(uploadFileName))
                            {
                                LoggingService.Error($"File with the same name already exist! {uploadFileName}");
                                continue;
                            }

                            File.Move(sourceFilePath, uploadFileName);
                            LoggingService.Debug($"File renamed to {uploadFileName}");
                        }

                        string uploadRemoteFileName = null;
                        if (UseRemoteTemporaryExtension && !string.IsNullOrWhiteSpace(RemoteTemporaryExtension))
                        {
                            LoggingService.Debug($"Setting a temporary remote filename extension to {RemoteTemporaryExtension}");
                            if (!string.IsNullOrWhiteSpace(DestinationFileName))
                            {
                                LoggingService.Debug($"Using a static name for remote file name: {DestinationFileName}");
                                uploadRemoteFileName =
                                    $"{Path.ChangeExtension(DestinationFileName, null)}.{RemoteTemporaryExtension.TrimStart('.')}";
                                LoggingService.Debug($"Remote file name with Temporary extension and a static name {uploadRemoteFileName}");
                            }
                            else
                            {
                                uploadRemoteFileName =
                                    $"{Path.GetFileNameWithoutExtension(sourceFilePath)}.{RemoteTemporaryExtension.TrimStart('.')}";
                                LoggingService.Debug($"Remote file name with Temporary extension {uploadRemoteFileName}");
                            }
                        }

                        LoggingService.Debug($"Connecting to Host {Host}");
                        using (FtpClient ftpClient = new FtpClient(Host, Port, Username, Password))
                        {
                            var tempRemotePath = UriHelper.BuildPath(DestinationFolderPath, uploadRemoteFileName);

                            LoggingService.Debug($"Uploading file with options : {FtpUploadActionInstanceArgs.Overwrite}={Overwrite} and {FtpUploadActionInstanceArgs.CreateRemoteDirectory}={CreateRemoteDirectory}");
                            // Upload the file
                            var uploadStatus = ftpClient.UploadFile(uploadFileName, tempRemotePath,
                                Overwrite ? FtpRemoteExists.Overwrite : FtpRemoteExists.NoCheck, CreateRemoteDirectory);
                            LoggingService.Debug($"File Upload completed with status {uploadStatus:G}");

                            // Rename file after upload
                            string finalFileName = string.IsNullOrWhiteSpace(DestinationFileName)
                                ? Path.GetFileName(sourceFilePath)
                                : DestinationFileName;

                            LoggingService.Debug($"Renaming the uploaded remote file to its final name: {finalFileName}");
                            string finalRemotePath = UriHelper.BuildPath(DestinationFolderPath, finalFileName);
                            
                            ftpClient.Rename(tempRemotePath, finalRemotePath);
                        }
                    }
                    catch (Exception exception)
                    {
                        LoggingService.Error(exception, $"File: {sourceFilePath}");
                    }
                    finally
                    {
                        // Rename source file to original extension 
                        if (UseLocalTemporaryExtension && !string.IsNullOrWhiteSpace(uploadFileName) && !string.IsNullOrWhiteSpace(sourceFilePath))
                        {
                            LoggingService.Debug($"Renaming the local file to with its original extension");
                            if (!string.Equals(uploadFileName, sourceFilePath, StringComparison.CurrentCultureIgnoreCase))
                            {
                                bool sourceFileExist = File.Exists(sourceFilePath);
                                bool uploadFileExist = File.Exists(uploadFileName);
                                if (uploadFileExist && !sourceFileExist)
                                {
                                    File.Move(uploadFileName, sourceFilePath);
                                }
                                else
                                {
                                    LoggingService.Error($"The file with temporary name was not found [{nameof(uploadFileExist)}={uploadFileExist}] or another file with the same original name was created during the upload process [{nameof(sourceFileExist)}={sourceFileExist}].");
                                }
                            }
                        }
                    }
                }

                return ActionResult.Succeeded();
            }
            catch (Exception exception)
            {
                LoggingService.Error(exception);
                return ActionResult.Failed().WithException(exception);
            }
        }
    }

    public static class FtpUploadActionExecutionArgs 
    {
        public const string SourceFilesCollection = nameof(SourceFilesCollection);
    }
}
