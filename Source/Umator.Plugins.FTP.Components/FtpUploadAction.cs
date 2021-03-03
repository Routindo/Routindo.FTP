using System;
using System.IO;
using System.Net;
using System.Text;
using FluentFTP;
using Umator.Contract;
using Umator.Contract.Services;

namespace Umator.Plugins.FTP.Components
{
    public class FtpUploadAction: IAction
    {
        // private readonly ILoggingService _loggingService;

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
        /// Executes the specified arguments.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        public ActionResult Execute(ArgumentCollection arguments)
        {
            try
            {
                if(!arguments.HasArgument(FtpUploadActionExecutionArgs.SourceFilePath) || string.IsNullOrWhiteSpace(arguments.GetValue<string>(FtpUploadActionExecutionArgs.SourceFilePath)))
                    return ActionResult.Failed().WithAdditionInformation(ArgumentCollection.New().WithArgument("ERROR", $"Missing Execution Argument {FtpUploadActionExecutionArgs.SourceFilePath}"));

                var sourceFilePath = arguments.GetValue<string>(FtpUploadActionExecutionArgs.SourceFilePath);
                if (!File.Exists(sourceFilePath))
                {
                    // File not found 
                    return ActionResult.Failed().WithAdditionInformation(ArgumentCollection.New().WithArgument("ERROR", $"File Not Exist : {sourceFilePath}"));
                }

                if (string.IsNullOrWhiteSpace(Host))
                    return ActionResult.Failed().WithAdditionInformation(ArgumentCollection.New().WithArgument("ERROR", $"Missing Instance Argument {FtpUploadActionInstanceArgs.Host}"));

                if (string.IsNullOrWhiteSpace(Username))
                    return ActionResult.Failed().WithAdditionInformation(ArgumentCollection.New().WithArgument("ERROR", $"Missing Instance Argument {FtpUploadActionInstanceArgs.Username}"));

                if (string.IsNullOrWhiteSpace(Password))
                    return ActionResult.Failed().WithAdditionInformation(ArgumentCollection.New().WithArgument("ERROR", $"Missing Instance Argument {FtpUploadActionInstanceArgs.Password}"));

                using (FtpClient ftpClient = new FtpClient(Host, Port, Username, Password))
                {
                    // Create temporary file name for uploading the file
                    string uploadTempFileName =
                        $"{Path.GetFileNameWithoutExtension(sourceFilePath)}.{DEFAULT_TEMP_EXTENSION}";
                    var tempRemotePath = UriHelper.BuildPath(DestinationFolderPath, uploadTempFileName);

                    // Upload the file
                    ftpClient.UploadFile(sourceFilePath, tempRemotePath, FtpRemoteExists.Overwrite, true);
                    // Rename file after upload
                    string finalFileName = string.IsNullOrWhiteSpace(DestinationFileName)
                        ? Path.GetFileName(sourceFilePath)
                        : DestinationFileName;

                    string finalRemotePath = UriHelper.BuildPath(DestinationFolderPath, finalFileName);
                    ftpClient.Rename(tempRemotePath, finalRemotePath);
                }

                return ActionResult.Succeeded();
            }
            catch (Exception exception)
            {
                // _loggingService.Error(exception.ToString());
                return ActionResult.Failed().WithException(exception);
            }
        }

        private bool RenameFile(string remoteFileUri, string renameTo)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(remoteFileUri);
                request.Credentials = new NetworkCredential(Username, Password);
                request.Method = WebRequestMethods.Ftp.Rename;
                request.RenameTo = renameTo;
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    // TODO : Use logging service
                    Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
                }

                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return false;
            }
        }
    }


    public static class FtpUploadActionInstanceArgs
    {
        // Mandatory fields 
        public const string Host = nameof(Host);
        public const string Username = nameof(Username);
        public const string Password = nameof(Password);
        public const string UseBinary = nameof(UseBinary);

        // Optional Fields 
        public const string Port = nameof(Port);
        public const string DestinationFolderPath = nameof(DestinationFolderPath);
        public const string DestinationFileName = nameof(DestinationFileName);

        // Extension 
        public const string RemoteTemporaryExtension = nameof(RemoteTemporaryExtension);
        public const string LocalTemporaryExtension = nameof(LocalTemporaryExtension);
    }

    public static class FtpUploadActionExecutionArgs 
    {
        public const string SourceFilePath = nameof(SourceFilePath);
    }
}
