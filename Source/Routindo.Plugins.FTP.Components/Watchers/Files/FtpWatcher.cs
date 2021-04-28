using System;
using System.Linq;
using Routindo.Contract.Arguments;
using Routindo.Contract.Attributes;
using Routindo.Contract.Services;
using Routindo.Contract.Watchers;

namespace Routindo.Plugins.FTP.Components.Watchers.Files
{
    [PluginItemInfo(ComponentUniqueId, "FTP Remote Watcher",
         "Watch Remote directory and check if contains items - files or directories -"),
     ResultArgumentsClass(typeof(FtpWatcherResultArgs))
    ]
    public class FtpWatcher: FtpItemsSelector, IWatcher 
    {
        // ReSharper disable once InconsistentNaming
        
        public const string ComponentUniqueId = "DA3BC9D3-5F1E-4FFD-93AB-E0FC08FEF596";

        public string Id { get; set; }
        public ILoggingService LoggingService { get; set; }

        public WatcherResult Watch()
        {
            try
            {
                var files = Select();
                if (files.Any())
                    return WatcherResult.Succeed(ArgumentCollection.New()
                        .WithArgument(FtpWatcherResultArgs.RemoteFilesCollection, files)
                    );

                return WatcherResult.NotFound;
            }
            catch (Exception exception)
            {
                LoggingService.Error(exception);
                return WatcherResult.NotFound.WithException(exception);
            }
        }

        protected override ILoggingService Logger => LoggingService;
    }
}
