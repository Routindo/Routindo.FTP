using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Routindo.Contract;
using Routindo.Contract.Arguments;
using Routindo.Contract.Services;
using Routindo.Plugins.FTP.Components.Actions.Upload;

namespace Routindo.Plugins.FTP.Tests
{
    [TestClass]
    public class FtpUploadUnitTests
    {
        [TestMethod]
        [TestCategory("Integration Test")]
        public void SendSampleFileTest()
        {
            FtpUploadAction action = new FtpUploadAction()
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(FtpUploadAction)),
                Host = FtpTestCredentials.Host,
                Username = FtpTestCredentials.User,
                Password = FtpTestCredentials.Password,
                Port = FtpTestCredentials.Port,
                DestinationFolderPath = "Data",
                DestinationFileName = "renamed.txt",
                UseRemoteTemporaryExtension = true,
                RemoteTemporaryExtension = "remote",
                UseLocalTemporaryExtension = true,
                LocalTemporaryExtension = "local",
                Overwrite = true,
                CreateRemoteDirectory = true
            };

            string sourceFileName = Path.Combine(Path.GetTempPath(), "test.txt");
            File.WriteAllText(sourceFileName, "Hello world! This is a remote message!");

            var result =  action.Execute(ArgumentCollection.New()
                .WithArgument(FtpUploadActionExecutionArgs.SourceFilesCollection, new List<string> { sourceFileName }));
            Console.WriteLine(result.AttachedException);
            Assert.IsTrue(result.Result);
            File.Delete(sourceFileName);
        }
    }
}
