using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Routindo.Contract;
using Routindo.Contract.Services;
using Routindo.Plugins.FTP.Components;
using Routindo.Plugins.FTP.Components.Watchers.Files;

namespace Routindo.Plugins.FTP.Tests
{
    [TestClass]
    public class FtpFilesWatcherTests
    {
        private const string LocalWorkingDir = @"%userprofile%\DATA\Temps\ftp";
        private const string LocalDownloadDir = @"%userprofile%\DATA\Temps\ftp\test";
        [TestCleanup]
        public void Cleanup()
        {
            var files = Directory.GetFiles(Environment.ExpandEnvironmentVariables(LocalWorkingDir));
            foreach (var file in files)
            {
                File.Delete(file);
            }

            files = Directory.GetFiles(Environment.ExpandEnvironmentVariables(LocalDownloadDir));
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void WatchForFilesTest()
        {
            string remoteWorkingDir = "/";
            FtpWatcher watcher = new FtpWatcher
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(FtpWatcher)),
                Host = FtpTestCredentials.Host,
                Username = FtpTestCredentials.User,
                Password = FtpTestCredentials.Password,
                SelectFiles = true
            };


            var localWriteDir = Environment.ExpandEnvironmentVariables(LocalWorkingDir);
            var fileNamePath = CreateTestFile(localWriteDir);
            var watcherResult = watcher.Watch();
            Assert.IsNotNull(watcherResult);
            Assert.AreEqual(true, watcherResult.Result);
            Assert.IsNotNull(watcherResult.WatchingArguments);
            Assert.IsTrue(watcherResult.WatchingArguments.HasArgument(FtpWatcherResultArgs.RemoteFilesCollection));
            Assert.IsTrue(watcherResult.WatchingArguments.GetValue<List<string>>(FtpWatcherResultArgs.RemoteFilesCollection).Any());
            Assert.AreEqual(1, watcherResult.WatchingArguments.GetValue<List<string>>(FtpWatcherResultArgs.RemoteFilesCollection).Count);

            var watchedFile = watcherResult.WatchingArguments.GetValue<List<string>>(FtpWatcherResultArgs
                .RemoteFilesCollection).Single();

            Assert.AreEqual(remoteWorkingDir +Path.GetFileName(fileNamePath), watchedFile);
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void WatchForFilesInDirTest()
        {
            string remoteWorkingDir = "/test";
            FtpWatcher watcher = new FtpWatcher
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(FtpWatcher)),
                Host = FtpTestCredentials.Host,
                Username = FtpTestCredentials.User,
                Password = FtpTestCredentials.Password,
                Port = FtpTestCredentials.Port,
                RemoteWorkingDir = remoteWorkingDir,
                SelectFiles = true
            };


            var localWriteDir = Path.Combine(Environment.ExpandEnvironmentVariables(LocalWorkingDir), remoteWorkingDir.Trim('/'));
            var fileNamePath = CreateTestFile(localWriteDir);
            var watcherResult = watcher.Watch();
            Assert.IsNotNull(watcherResult);
            Assert.AreEqual(true, watcherResult.Result);
            Assert.IsNotNull(watcherResult.WatchingArguments);
            Assert.IsTrue(watcherResult.WatchingArguments.HasArgument(FtpWatcherResultArgs.RemoteFilesCollection));
            Assert.IsTrue(watcherResult.WatchingArguments.GetValue<List<string>>(FtpWatcherResultArgs.RemoteFilesCollection).Any());
            Assert.AreEqual(1, watcherResult.WatchingArguments.GetValue<List<string>>(FtpWatcherResultArgs.RemoteFilesCollection).Count);

            var watchedFile = watcherResult.WatchingArguments.GetValue<List<string>>(FtpWatcherResultArgs
                .RemoteFilesCollection).Single();

            Assert.AreEqual(UriHelper.BuildPath("/", remoteWorkingDir, Path.GetFileName(fileNamePath)), watchedFile);
        }

        private static string CreateTestFile(string localWriteDir)
        {
            var fileNamePath = Path.Combine(localWriteDir,
                $"File{DateTime.Now.ToString("HHmmssfff")}.txt");
            File.WriteAllText(fileNamePath, DateTime.Now.ToString("G"));
            return fileNamePath;
        }

        [TestMethod]
        [TestCategory("Integration Test")]
        public void WatchForDirectoryTest()
        {
            string remoteWorkingDir = "/";
            FtpWatcher watcher = new FtpWatcher
            {
                Id = PluginUtilities.GetUniqueId(),
                LoggingService = ServicesContainer.ServicesProvider.GetLoggingService(nameof(FtpWatcher)),
                Host = FtpTestCredentials.Host,
                Username = FtpTestCredentials.User,
                Password = FtpTestCredentials.Password,
                SelectDirectories = true
            };


            var watcherResult = watcher.Watch();
            Assert.IsNotNull(watcherResult);
            Assert.AreEqual(true, watcherResult.Result);
            Assert.IsNotNull(watcherResult.WatchingArguments);
            Assert.IsTrue(watcherResult.WatchingArguments.HasArgument(FtpWatcherResultArgs.RemoteFilesCollection));
            Assert.IsTrue(watcherResult.WatchingArguments.GetValue<List<string>>(FtpWatcherResultArgs.RemoteFilesCollection).Any());
            Assert.AreEqual(1, watcherResult.WatchingArguments.GetValue<List<string>>(FtpWatcherResultArgs.RemoteFilesCollection).Count);
        }
    }
}
