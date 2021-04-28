﻿using Routindo.Contract.Attributes;
using Routindo.Plugins.FTP.Components.Actions.DownloadFile;
using Routindo.Plugins.FTP.Components.Actions.UploadFile;
using Routindo.Plugins.FTP.Components.Watchers.Files;
using Routindo.Plugins.FTP.UI.Views;

[assembly: ComponentConfigurator(typeof(UploadFileActionView), FtpUploadAction.ComponentUniqueId, "Configurator for Ftp File Uploader")]
[assembly: ComponentConfigurator(typeof(DownloadFileActionView), FtpDownloadAction.ComponentUniqueId, "Configurator for Ftp File Downloader")]
[assembly: ComponentConfigurator(typeof(FtpWatcherView), FtpWatcher.ComponentUniqueId, "Configurator for Ftp Watcher")]