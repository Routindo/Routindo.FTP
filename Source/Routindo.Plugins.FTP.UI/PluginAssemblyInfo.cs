using Routindo.Contract;
using Routindo.Contract.Attributes;
using Routindo.Plugins.FTP.Components;
using Routindo.Plugins.FTP.UI.Views;

[assembly: ComponentConfigurator(typeof(UploadFileActionView), FtpUploadAction.ComponentUniqueId, "Configurator for Ftp File Uploader")]