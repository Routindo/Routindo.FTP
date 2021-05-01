using Routindo.Contract.Attributes;
using Routindo.Plugins.FTP.Components.Actions.Delete;
using Routindo.Plugins.FTP.Components.Actions.Download;
using Routindo.Plugins.FTP.Components.Actions.Upload;
using Routindo.Plugins.FTP.Components.Watchers.Files;
using Routindo.Plugins.FTP.UI.Views;

[assembly: ComponentConfigurator(typeof(UploadFileActionView), FtpUploadAction.ComponentUniqueId, "Configurator for Ftp File Uploader")]
[assembly: ComponentConfigurator(typeof(DownloadFileActionView), FtpDownloadAction.ComponentUniqueId, "Configurator for Ftp File Downloader")]
[assembly: ComponentConfigurator(typeof(DeleteFilesActionView), FtpDeleteAction.ComponentUniqueId, "Configurator for Ftp File Deleter")]
[assembly: ComponentConfigurator(typeof(FtpWatcherView), FtpWatcher.ComponentUniqueId, "Configurator for Ftp Watcher")]