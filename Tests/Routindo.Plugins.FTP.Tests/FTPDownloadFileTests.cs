using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Routindo.Contract;
using Routindo.Contract.Arguments;
using Routindo.Contract.Services;
using Routindo.Plugins.FTP.Components.Actions.Download;

namespace Routindo.Plugins.FTP.Tests
{
    [TestClass]
    public class FtpDownloadFileTests
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
        public void DownloadFileTest()
        {
            var localWriteDir = Path.GetDirectoryName(Environment.ExpandEnvironmentVariables(LocalWorkingDir));
            FtpDownloadAction ftpDownloadAction = new FtpDownloadAction()
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(FtpDownloadAction))
            };
            ftpDownloadAction.Host = FtpTestCredentials.Host;
            ftpDownloadAction.Username = FtpTestCredentials.User;
            ftpDownloadAction.Password = FtpTestCredentials.Password;
            ftpDownloadAction.Port = FtpTestCredentials.Port;
            ftpDownloadAction.DirectoryPath = LocalDownloadDir;

            var fileNamePath = CreateTestFile(localWriteDir);

            var actionResult = ftpDownloadAction.Execute(ArgumentCollection.New()
                .WithArgument(FtpDownloadActionExecutionArgs.RemoteFilesCollection, new List<string>() {Path.GetFileName(fileNamePath)})
            );

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void DownloadFileWithOverwriteTest()
        {
            var localWriteDir = Path.GetDirectoryName(Environment.ExpandEnvironmentVariables(LocalWorkingDir));
            FtpDownloadAction ftpDownloadAction = new FtpDownloadAction()
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(FtpDownloadAction))
            };
            ftpDownloadAction.Host = FtpTestCredentials.Host;
            ftpDownloadAction.Username = FtpTestCredentials.User;
            ftpDownloadAction.Password = FtpTestCredentials.Password;
            ftpDownloadAction.Port = FtpTestCredentials.Port;
            ftpDownloadAction.DirectoryPath = LocalDownloadDir;
            ftpDownloadAction.Overwrite = true;

            var fileNamePath = CreateTestFile(localWriteDir);

            var actionResult = ftpDownloadAction.Execute(ArgumentCollection.New()
                .WithArgument(FtpDownloadActionExecutionArgs.RemoteFilesCollection, new List<string>() { Path.GetFileName(fileNamePath) })
            );

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);

            actionResult = ftpDownloadAction.Execute(ArgumentCollection.New()
                .WithArgument(FtpDownloadActionExecutionArgs.RemoteFilesCollection, new List<string>() { Path.GetFileName(fileNamePath) })
            );

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);

            var fileLines = File.ReadAllLines(Path.Combine(
                Environment.ExpandEnvironmentVariables(ftpDownloadAction.DirectoryPath),
                Path.GetFileName(fileNamePath)));
            Assert.AreEqual(1, fileLines.Count());
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void DownloadFileWithAppendTest()
        {
            var localWriteDir = Path.GetDirectoryName(Environment.ExpandEnvironmentVariables(LocalWorkingDir));
            FtpDownloadAction ftpDownloadAction = new FtpDownloadAction()
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(FtpDownloadAction))
            };
            ftpDownloadAction.Host = FtpTestCredentials.Host;
            ftpDownloadAction.Username = FtpTestCredentials.User;
            ftpDownloadAction.Password = FtpTestCredentials.Password;
            ftpDownloadAction.Port = FtpTestCredentials.Port;
            ftpDownloadAction.DirectoryPath = LocalDownloadDir;
            ftpDownloadAction.Append = true;

            var fileNamePath = CreateTestFile(localWriteDir);

            var actionResult = ftpDownloadAction.Execute(ArgumentCollection.New()
                .WithArgument(FtpDownloadActionExecutionArgs.RemoteFilesCollection, new List<string>() { Path.GetFileName(fileNamePath) })
            );

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);

            File.AppendAllLines(Path.Combine(Environment.ExpandEnvironmentVariables(localWriteDir), fileNamePath), new List<string>()
            {
                // Environment.NewLine,
                $"{Environment.NewLine}Hello world!"
            });

            actionResult = ftpDownloadAction.Execute(ArgumentCollection.New()
                .WithArgument(FtpDownloadActionExecutionArgs.RemoteFilesCollection, new List<string>() { Path.GetFileName(fileNamePath) })
            );

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);

            var fileLines = File.ReadAllLines(Path.Combine(
                Environment.ExpandEnvironmentVariables(ftpDownloadAction.DirectoryPath),
                Path.GetFileName(fileNamePath)));
            Assert.AreEqual(2, fileLines.Count());
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void DownloadFileWithTempNameTest()
        {
            var localWriteDir = Path.GetDirectoryName(Environment.ExpandEnvironmentVariables(LocalWorkingDir));
            FtpDownloadAction ftpDownloadAction = new FtpDownloadAction()
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(FtpDownloadAction))
            };
            ftpDownloadAction.Host = FtpTestCredentials.Host;
            ftpDownloadAction.Username = FtpTestCredentials.User;
            ftpDownloadAction.Password = FtpTestCredentials.Password;
            ftpDownloadAction.Port = FtpTestCredentials.Port;
            ftpDownloadAction.DirectoryPath = LocalDownloadDir;
            ftpDownloadAction.UseTemporaryName = true;

            var fileNamePath = CreateTestFile(localWriteDir);

            var actionResult = ftpDownloadAction.Execute(ArgumentCollection.New()
                .WithArgument(FtpDownloadActionExecutionArgs.RemoteFilesCollection, new List<string>() { Path.GetFileName(fileNamePath) })
            );

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
            Assert.IsTrue(File.Exists(Path.Combine(Environment.ExpandEnvironmentVariables(LocalDownloadDir), Path.GetFileName(fileNamePath))));
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void DownloadFileAndChangeExtensionTest()
        {
            var localWriteDir = Path.GetDirectoryName(Environment.ExpandEnvironmentVariables(LocalWorkingDir));
            FtpDownloadAction ftpDownloadAction = new FtpDownloadAction
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(FtpDownloadAction)),
                Host = FtpTestCredentials.Host,
                Username = FtpTestCredentials.User,
                Password = FtpTestCredentials.Password,
                Port = FtpTestCredentials.Port,
                DirectoryPath = LocalDownloadDir
            };

            ftpDownloadAction.RenameDownloaded = true;
            ftpDownloadAction.RenameDownloadedExtension = "rtnd";
            var fileNamePath = CreateTestFile(localWriteDir);

            var actionResult = ftpDownloadAction.Execute(ArgumentCollection.New()
                .WithArgument(FtpDownloadActionExecutionArgs.RemoteFilesCollection, new List<string>() { Path.GetFileName(fileNamePath) })
            );

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void DownloadFileAndChangePrefixTest()
        {
            var localWriteDir = Path.GetDirectoryName(Environment.ExpandEnvironmentVariables(LocalWorkingDir));
            FtpDownloadAction ftpDownloadAction = new FtpDownloadAction
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(FtpDownloadAction)),
                Host = FtpTestCredentials.Host,
                Username = FtpTestCredentials.User,
                Password = FtpTestCredentials.Password,
                Port = FtpTestCredentials.Port,
                DirectoryPath = LocalDownloadDir
            };

            ftpDownloadAction.RenameDownloaded = true;
            ftpDownloadAction.RenameDownloadedPrefix = "ROUTINDO_";
            var fileNamePath = CreateTestFile(localWriteDir);

            var actionResult = ftpDownloadAction.Execute(ArgumentCollection.New()
                .WithArgument(FtpDownloadActionExecutionArgs.RemoteFilesCollection, new List<string>() { Path.GetFileName(fileNamePath) })
            );

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void DownloadFileAndChangePrefixAndExtensionTest()
        {
            var localWriteDir = Path.GetDirectoryName(Environment.ExpandEnvironmentVariables(LocalWorkingDir));
            FtpDownloadAction ftpDownloadAction = new FtpDownloadAction
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(FtpDownloadAction)),
                Host = FtpTestCredentials.Host,
                Username = FtpTestCredentials.User,
                Password = FtpTestCredentials.Password,
                Port = FtpTestCredentials.Port,
                DirectoryPath = LocalDownloadDir
            };

            ftpDownloadAction.RenameDownloaded = true;
            ftpDownloadAction.RenameDownloadedPrefix = "ROUTINDO_";
            ftpDownloadAction.RenameDownloadedExtension = "rtnd";
            var fileNamePath = CreateTestFile(localWriteDir);

            var actionResult = ftpDownloadAction.Execute(ArgumentCollection.New()
                .WithArgument(FtpDownloadActionExecutionArgs.RemoteFilesCollection, new List<string>() { Path.GetFileName(fileNamePath) })
            );

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void DownloadFileFromWorkingDirAndChangePrefixAndExtensionTest()
        {
            var localWriteDir = Environment.ExpandEnvironmentVariables(LocalWorkingDir);
            FtpDownloadAction ftpDownloadAction = new FtpDownloadAction
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(FtpDownloadAction)),
                Host = FtpTestCredentials.Host,
                Username = FtpTestCredentials.User,
                Password = FtpTestCredentials.Password,
                Port = FtpTestCredentials.Port,
                DirectoryPath = LocalDownloadDir,
                RemoteWorkingDir = "test"
            };

            ftpDownloadAction.RenameDownloaded = true;
            ftpDownloadAction.RenameDownloadedPrefix = "ROUTINDO_";
            ftpDownloadAction.RenameDownloadedExtension = "rtnd";
            var fileNamePath = CreateTestFile(localWriteDir);

            var actionResult = ftpDownloadAction.Execute(ArgumentCollection.New()
                .WithArgument(FtpDownloadActionExecutionArgs.RemoteFilesCollection, new List<string>() { Path.GetFileName(fileNamePath) })
            );

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void DownloadFileAndRenameTest()
        {
            var localWriteDir = Path.GetDirectoryName(Environment.ExpandEnvironmentVariables(LocalWorkingDir));
            FtpDownloadAction ftpDownloadAction = new FtpDownloadAction
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(FtpDownloadAction)),
                Host = FtpTestCredentials.Host,
                Username = FtpTestCredentials.User,
                Password = FtpTestCredentials.Password,
                Port = FtpTestCredentials.Port,
                DirectoryPath = LocalDownloadDir
            };

            ftpDownloadAction.RenameDownloaded = true;
            ftpDownloadAction.RenameDownloadedNewName = "ROUTINDO_";
            var fileNamePath = CreateTestFile(localWriteDir);

            var actionResult = ftpDownloadAction.Execute(ArgumentCollection.New()
                .WithArgument(FtpDownloadActionExecutionArgs.RemoteFilesCollection, new List<string>() { Path.GetFileName(fileNamePath) })
            );

            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Result);
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void DownloadFileFromWorkingDirAndRenameTest()
        {
            var localWriteDir = Environment.ExpandEnvironmentVariables(LocalWorkingDir);
            FtpDownloadAction ftpDownloadAction = new FtpDownloadAction
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(FtpDownloadAction)),
                Host = FtpTestCredentials.Host,
                Username = FtpTestCredentials.User,
                Password = FtpTestCredentials.Password,
                Port = FtpTestCredentials.Port,
                DirectoryPath = LocalDownloadDir,
                RemoteWorkingDir = "test"
            };

            ftpDownloadAction.RenameDownloaded = true;
            ftpDownloadAction.RenameDownloadedNewName = "ROUTINDO_";
            var fileNamePath = CreateTestFile(localWriteDir);

            var actionResult = ftpDownloadAction.Execute(ArgumentCollection.New()
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
