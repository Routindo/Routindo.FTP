using Umator.Contract;
using Umator.Plugins.FTP.Components;
using Umator.Plugins.FTP.UI.Views;

[assembly: ComponentConfigurator(typeof(UploadFileActionView), FtpUploadAction.ComponentUniqueId, "Configurator for Ftp File Uploader")]