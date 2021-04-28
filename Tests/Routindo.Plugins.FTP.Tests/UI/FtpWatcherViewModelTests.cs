using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Routindo.Contract.Arguments;
using Routindo.Plugins.FTP.Components.Watchers;
using Routindo.Plugins.FTP.UI.Enums;
using Routindo.Plugins.FTP.UI.ViewModels;

namespace Routindo.Plugins.FTP.Tests.UI
{
    [TestClass]
    public class FtpWatcherViewModelTests
    {
        [TestMethod]
        [TestCategory("Unit Test")]
        public void ConfigureArgumentsTest()
        {
            FtpWatcherViewModel viewModel = new FtpWatcherViewModel()
            {
                Port = FtpTestCredentials.Port,
                Host = FtpTestCredentials.Host,
                Username = FtpTestCredentials.User,
                Password = FtpTestCredentials.Password,
                RemoteWorkingDir = "/remote",
                MaximumFiles = 100,
                SortingCriteria = ItemsSelectionSortingCriteria.CreationTimeAscending,
                CreatedBefore = 30,
                CreatedAfter = 40,
                EditedBefore = 50,
                EditedAfter = 60,
                CreatedAfterPeriod = TimePeriod.Days,
                CreatedBeforePeriod = TimePeriod.Hours,
                EditedAfterPeriod = TimePeriod.Minutes,
                EditedBeforePeriod = TimePeriod.Seconds,
                FilterByCreatedAfter = true,
                FilterByCreatedBefore = true,
                FilterByEditedAfter = true,
                FilterByEditedBefore = true
            };

            viewModel.Configure();
            Assert.AreEqual(viewModel.Port, viewModel.InstanceArguments.GetValue<int>(FtpItemsSelectorArgs.Port));
            Assert.AreEqual(viewModel.Host, viewModel.InstanceArguments.GetValue<string>(FtpItemsSelectorArgs.Host));
            Assert.AreEqual(viewModel.Username, viewModel.InstanceArguments.GetValue<string>(FtpItemsSelectorArgs.Username));
            Assert.AreEqual(viewModel.Password, viewModel.InstanceArguments.GetValue<string>(FtpItemsSelectorArgs.Username));
            Assert.AreEqual(viewModel.RemoteWorkingDir, viewModel.InstanceArguments.GetValue<string>(FtpItemsSelectorArgs.RemoteWorkingDir));
            Assert.AreEqual(viewModel.MaximumFiles, viewModel.InstanceArguments.GetValue<int>(FtpItemsSelectorArgs.MaximumFiles));
            Assert.AreEqual(viewModel.SortingCriteria, viewModel.InstanceArguments.GetValue<ItemsSelectionSortingCriteria>(FtpItemsSelectorArgs.SortingCriteria));

            Assert.IsNotNull(viewModel.InstanceArguments.GetValue<ulong?>(FtpItemsSelectorArgs.CreatedBefore));
            Assert.IsNotNull(viewModel.InstanceArguments.GetValue<ulong?>(FtpItemsSelectorArgs.CreatedAfter));
            Assert.IsNotNull(viewModel.InstanceArguments.GetValue<ulong?>(FtpItemsSelectorArgs.EditedBefore));
            Assert.IsNotNull(viewModel.InstanceArguments.GetValue<ulong?>(FtpItemsSelectorArgs.EditedAfter));
        }

        [TestMethod]
        [TestCategory("Unit Test")]
        public void SetArgumentsTest()
        {
            var arguments = ArgumentCollection.New()
                .WithArgument(FtpItemsSelectorArgs.Port, FtpTestCredentials.Port)
                .WithArgument(FtpItemsSelectorArgs.Host, FtpTestCredentials.Host)
                .WithArgument(FtpItemsSelectorArgs.Username, FtpTestCredentials.User)
                .WithArgument(FtpItemsSelectorArgs.Password, FtpTestCredentials.Password)
                .WithArgument(FtpItemsSelectorArgs.RemoteWorkingDir, "/")
                .WithArgument(FtpItemsSelectorArgs.MaximumFiles, 100)
                .WithArgument(FtpItemsSelectorArgs.SortingCriteria, ItemsSelectionSortingCriteria.CreationTimeDescending)
                .WithArgument(FtpItemsSelectorArgs.CreatedBefore, 1)
                .WithArgument(FtpItemsSelectorArgs.CreatedAfter, 1000)
                .WithArgument(FtpItemsSelectorArgs.EditedBefore, 60000)
                .WithArgument(FtpItemsSelectorArgs.EditedAfter, 120000);


            FtpWatcherViewModel viewModel = new FtpWatcherViewModel()
            {
                
            };
            viewModel.SetArguments(arguments);

            Assert.AreEqual(viewModel.Port, arguments.GetValue<int>(FtpItemsSelectorArgs.Port));
            Assert.AreEqual(viewModel.Host, arguments.GetValue<string>(FtpItemsSelectorArgs.Host));
            Assert.AreEqual(viewModel.Username, arguments.GetValue<string>(FtpItemsSelectorArgs.Username));
            Assert.AreEqual(viewModel.Password, arguments.GetValue<string>(FtpItemsSelectorArgs.Username));
            Assert.AreEqual(viewModel.RemoteWorkingDir, arguments.GetValue<string>(FtpItemsSelectorArgs.RemoteWorkingDir));
            Assert.AreEqual(viewModel.MaximumFiles, arguments.GetValue<int>(FtpItemsSelectorArgs.MaximumFiles));
            Assert.AreEqual(viewModel.SortingCriteria, arguments.GetValue<ItemsSelectionSortingCriteria>(FtpItemsSelectorArgs.SortingCriteria));

            Assert.AreEqual(1,viewModel.CreatedBefore);
            Assert.AreEqual(1, viewModel.CreatedAfter);
            Assert.AreEqual(1, viewModel.EditedBefore);
            Assert.AreEqual(2, viewModel.EditedAfter);

            Assert.AreEqual(TimePeriod.Milliseconds, viewModel.CreatedBeforePeriod);
            Assert.AreEqual(TimePeriod.Seconds, viewModel.CreatedAfterPeriod);
            Assert.AreEqual(TimePeriod.Minutes, viewModel.EditedBeforePeriod);
            Assert.AreEqual(TimePeriod.Minutes, viewModel.EditedAfterPeriod);

            Assert.IsTrue(viewModel.FilterByCreatedBefore);
            Assert.IsTrue(viewModel.FilterByCreatedAfter);
            Assert.IsTrue(viewModel.FilterByEditedBefore);
            Assert.IsTrue(viewModel.FilterByEditedAfter);

            Assert.IsTrue(viewModel.FilterByCreationTime);
            Assert.IsTrue(viewModel.FilterByEditionTime);

        }
    }
}
