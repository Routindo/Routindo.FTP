using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Routindo.Contract;
using Routindo.Contract.Arguments;
using Routindo.Contract.Services;
using Routindo.Plugins.FTP.Components.Actions.Download;
using Routindo.Plugins.FTP.Components.Actions.Move;

namespace Routindo.Plugins.FTP.Tests
{
    [TestClass]
    public class MoveFilesActionTests
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
        public void MoveFileTest()
        {
            var localWriteDir = Path.GetDirectoryName(Environment.ExpandEnvironmentVariables(LocalWorkingDir));
            FtpMoveFilesAction ftpMoveAction = new FtpMoveFilesAction()
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(FtpDownloadAction))
            };
            ftpMoveAction.Host = FtpTestCredentials.Host;
            ftpMoveAction.Username = FtpTestCredentials.User;
            ftpMoveAction.Password = FtpTestCredentials.Password;
            ftpMoveAction.Port = FtpTestCredentials.Port;
            ftpMoveAction.RemoteWorkingDir = "/";
            ftpMoveAction.DestinationPath = "/test";
            var fileNamePath = CreateTestFile(localWriteDir);

            var actionResult = ftpMoveAction.Execute(ArgumentCollection.New()
                .WithArgument(FtpDownloadActionExecutionArgs.RemoteFilesCollection, new List<string>() { Path.GetFileName(fileNamePath) })
            );

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
            Assert.IsTrue(File.Exists(Path.Combine(Environment.ExpandEnvironmentVariables(LocalWorkingDir), Path.GetFileName(fileNamePath))));
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
