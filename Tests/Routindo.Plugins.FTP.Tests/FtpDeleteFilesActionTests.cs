using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Routindo.Contract;
using Routindo.Contract.Arguments;
using Routindo.Contract.Services;
using Routindo.Plugins.FTP.Components.Actions.Delete;
using Routindo.Plugins.FTP.Components.Actions.Download;

namespace Routindo.Plugins.FTP.Tests
{
    [TestClass]
    public class FtpDeleteFilesActionTests
    {
        private const string LocalWorkingDir = @"%userprofile%\DATA\Temps\ftp\test";
        private const string LocalDownloadDir = @"%userprofile%\DATA\Temps\ftp\downloaded";

        [TestCleanup]
        public void Cleanup()
        {
            var files = Directory.GetFiles(Path.GetDirectoryName(Environment.ExpandEnvironmentVariables(LocalWorkingDir)), "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void DeleteFilesTest()
        {
            var localWriteDir = Path.GetDirectoryName(Environment.ExpandEnvironmentVariables(LocalWorkingDir));
            FtpDeleteAction ftpDeleteAction = new FtpDeleteAction() 
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(FtpDownloadAction))
            };
            ftpDeleteAction.Host = FtpTestCredentials.Host;
            ftpDeleteAction.Username = FtpTestCredentials.User;
            ftpDeleteAction.Password = FtpTestCredentials.Password;
            ftpDeleteAction.Port = FtpTestCredentials.Port;
            ftpDeleteAction.RemoteWorkingDir = "/";

            var fileNamePath = CreateTestFile(localWriteDir);

            var actionResult = ftpDeleteAction.Execute(ArgumentCollection.New()
                .WithArgument(FtpDownloadActionExecutionArgs.RemoteFilesCollection, new List<string>() { Path.GetFileName(fileNamePath) })
            );

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
        }

        private static string CreateTestFile(string localWriteDir)
        {
            var fileNamePath = Path.Combine(localWriteDir,
                $"File{DateTime.Now.ToString("HHmmssfff")}.txt");
            File.WriteAllText(fileNamePath, DateTime.Now.ToString("G"));
            return fileNamePath;
        }
    }
}
