using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Umator.Contract;
using Umator.Plugins.FTP.Components;

namespace Umator.Plugins.FTP.Tests
{
    [TestClass]
    public class FTPUploadUnitTests
    {
        [TestMethod]
        public void SendSampleFileTest()
        {
            FtpUploadAction action = new FtpUploadAction()
            {
                Host = "192.168.175.128",
                Username = "user",
                Password = "user",
                DestinationFolderPath = "Data",
                DestinationFileName = "renamed.txt",
                Port = 21, UseRemoteTemporaryExtension = true,
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
