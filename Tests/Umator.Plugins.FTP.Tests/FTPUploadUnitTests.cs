using System;
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
                DestinationFileName = "renamed.txt",
            };

            string sourceFileName = Path.Combine(Path.GetTempPath(), "test.txt");
            File.WriteAllText(sourceFileName, "Hello world!");

            var result =  action.Execute(ArgumentCollection.New()
                .WithArgument(FtpUploadActionExecutionArgs.SourceFilePath, sourceFileName));
            Console.WriteLine(result.AttachedException);
            Assert.IsTrue(result.Result);
        }
    }
}
