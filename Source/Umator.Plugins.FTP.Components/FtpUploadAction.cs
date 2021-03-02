using System;
using System.IO;
using System.Net;
using System.Text;
using Umator.Contract;
using Umator.Contract.Services;

namespace Umator.Plugins.FTP.Components
{
    public class FtpUploadAction: IAction
    {
        // private readonly ILoggingService _loggingService;

        private const int DEFAULT_FTP_PORT = 21;
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
        /// Gets or sets a value indicating whether [use binary].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use binary]; otherwise, <c>false</c>.
        /// </value>
        [Argument(FtpUploadActionInstanceArgs.UseBinary, true)] public bool UseBinary { get; set; } = false;

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

                string uploadFileName = Path.GetFileName(sourceFilePath);

                if (string.IsNullOrWhiteSpace(Host))
                    return ActionResult.Failed().WithAdditionInformation(ArgumentCollection.New().WithArgument("ERROR", $"Missing Instance Argument {FtpUploadActionInstanceArgs.Host}"));

                if (string.IsNullOrWhiteSpace(Username))
                    return ActionResult.Failed().WithAdditionInformation(ArgumentCollection.New().WithArgument("ERROR", $"Missing Instance Argument {FtpUploadActionInstanceArgs.Username}"));

                if (string.IsNullOrWhiteSpace(Password))
                    return ActionResult.Failed().WithAdditionInformation(ArgumentCollection.New().WithArgument("ERROR", $"Missing Instance Argument {FtpUploadActionInstanceArgs.Password}"));

                var uri = UriHelper.Combine(Host, Port > 0 ? Port : DEFAULT_FTP_PORT, DestinationFolderPath, uploadFileName);

                
                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
                // request.EnableSsl = true;
                //if (!string.IsNullOrWhiteSpace(DestinationFileName))
                //    request.RenameTo = DestinationFileName;

                request.Method = WebRequestMethods.Ftp.UploadFile;
                
                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential(Username, Password);

                // Copy the contents of the file to the request stream.
                byte[] fileContents;
                using (StreamReader sourceStream = new StreamReader(sourceFilePath))
                {
                    fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                }

                request.ContentLength = fileContents.Length;
                
                using (Stream requestStream = request.GetRequestStream())
                {
                    
                    requestStream.Write(fileContents, 0, fileContents.Length);
                }

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
                }

                //if (!string.IsNullOrWhiteSpace(DestinationFileName))
                //{
                //    using (Stream requestStream = request.GetRequestStream())
                //    {
                //        request.RenameTo = DestinationFileName;
                //    }
                    
                //    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                //    {
                //        Console.WriteLine($"Rename File Complete, status {response.StatusDescription}");
                //    }
                //}

                return ActionResult.Succeeded();
            }
            catch (Exception exception)
            {
                // _loggingService.Error(exception.ToString());
                return ActionResult.Failed().WithException(exception);
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
