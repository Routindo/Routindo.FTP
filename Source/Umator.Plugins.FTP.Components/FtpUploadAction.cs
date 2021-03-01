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
        private readonly ILoggingService _loggingService;
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        [Argument(FtpUploadActionInstanceArgs.Host, true)] public string Host { get; set; }

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

        [Argument(FtpUploadActionInstanceArgs.DestinationFolderPath, false)] public string DestinationFolderPath { get; set; }

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
                
                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://www.contoso.com/test.htm");
                
                request.Method = WebRequestMethods.Ftp.UploadFile;
                
                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential("anonymous", "janeDoe@contoso.com");

                // Copy the contents of the file to the request stream.
                byte[] fileContents;
                using (StreamReader sourceStream = new StreamReader("testfile.txt"))
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

                return ActionResult.Succeeded();
            }
            catch (Exception exception)
            {
                _loggingService.Error(exception.ToString());
                return ActionResult.Failed();
            }
        }

        private string CombineFullUri(string host, string folder, string filename)
        {
            return Combine(Combine(host, folder), filename);
        }

        private string Combine(string uri1, string uri2)
        {
            uri1 = uri1.TrimEnd('/');
            uri2 = uri2.TrimStart('/');
            return $"{uri1}/{uri2}";
        }

    }

    public static class FtpUploadActionInstanceArgs
    {
        public const string Host = nameof(Host);
        public const string Username = nameof(Username);
        public const string Password = nameof(Password);
        public const string DestinationFolderPath = nameof(DestinationFolderPath);
        public const string DestinationFileName = nameof(DestinationFileName); 
    }

    public static class FtpUploadActionExecutionArgs 
    {
        public const string SourceFilePath = nameof(SourceFilePath);
    }
}
