using System.Windows.Forms;
using System.Windows.Input;
using Routindo.Contract.Arguments;
using Routindo.Contract.UI;
using Routindo.Plugins.FTP.Components.Actions.Download;

namespace Routindo.Plugins.FTP.UI.ViewModels
{
    public class DownloadFileActionViewModel: PluginConfiguratorViewModelBase
    {
        private string _host;
        private string _username;
        private string _password;
        private int _port = FtpDownloadAction.DEFAULT_FTP_PORT;
        private string _directoryPath;
        private bool _overwrite;
        private bool _append;
        private bool _useTemporaryName;
        private string _remoteWorkingDir;
        private bool _deleteDownloaded;
        private bool _moveDownloaded;
        private string _moveDownloadedPath;

        public DownloadFileActionViewModel()
        {
            SelectLocalDirectoryCommand = new RelayCommand(SelectLocalDirectory);
        }

        public ICommand SelectLocalDirectoryCommand { get; }
         
        private void SelectLocalDirectory()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (!string.IsNullOrWhiteSpace(DirectoryPath))
                {
                    dialog.SelectedPath = DirectoryPath;
                }

                dialog.Description = "Directory where to download the files";
                dialog.ShowNewFolderButton = true;
                dialog.UseDescriptionForTitle = true;
                var dialogResult = dialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    DirectoryPath = dialog.SelectedPath;
                }
            }
        }

        public string Host
        {
            get => _host;
            set
            {
                _host = value;
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public int Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged();
            }
        }

        public string DirectoryPath
        {
            get => _directoryPath;
            set
            {
                _directoryPath = value;
                OnPropertyChanged();
            }
        }

        public bool Overwrite
        {
            get => _overwrite;
            set
            {
                _overwrite = value;
                OnPropertyChanged();
            }
        }

        public bool Append
        {
            get => _append;
            set
            {
                _append = value;
                OnPropertyChanged();
            }
        }

        public bool UseTemporaryName
        {
            get => _useTemporaryName;
            set
            {
                _useTemporaryName = value;
                OnPropertyChanged();
            }
        }

        public bool DeleteDownloaded
        {
            get => _deleteDownloaded;
            set
            {
                _deleteDownloaded = value;
                OnPropertyChanged();
            }
        }

        public bool MoveDownloaded
        {
            get => _moveDownloaded;
            set
            {
                _moveDownloaded = value;
                if (_moveDownloaded)
                {
                    ClearPropertyErrors(nameof(MoveDownloadedPath));
                    ValidateNonNullOrEmptyString(MoveDownloadedPath, nameof(MoveDownloadedPath));
                }
                else
                {
                    MoveDownloadedPath = string.Empty;
                }
                OnPropertyChanged();
            }
        }

        public string MoveDownloadedPath
        {
            get => _moveDownloadedPath;
            set
            {
                _moveDownloadedPath = value;
                ClearPropertyErrors();
                if (MoveDownloaded)
                    ValidateNonNullOrEmptyString(_moveDownloadedPath);
                OnPropertyChanged();
            }
        }

        public string RemoteWorkingDir
        {
            get => _remoteWorkingDir;
            set
            {
                _remoteWorkingDir = value;
                OnPropertyChanged();
            }
        }

        public override void Configure()
        {
            this.InstanceArguments = ArgumentCollection.New()
                    .WithArgument(FtpDownloadActionArgs.Host, Host)
                    .WithArgument(FtpDownloadActionArgs.Port, Port)
                    .WithArgument(FtpDownloadActionArgs.Username, Username)
                    .WithArgument(FtpDownloadActionArgs.Password, Password)
                    .WithArgument(FtpDownloadActionArgs.DirectoryPath, DirectoryPath)
                    .WithArgument(FtpDownloadActionArgs.RemoteWorkingDir, RemoteWorkingDir)
                    .WithArgument(FtpDownloadActionArgs.UseTemporaryName, UseTemporaryName)
                    .WithArgument(FtpDownloadActionArgs.Overwrite, Overwrite)
                    .WithArgument(FtpDownloadActionArgs.Append, Append)
                    .WithArgument(FtpDownloadActionArgs.DeleteDownloaded, DeleteDownloaded)
                    .WithArgument(FtpDownloadActionArgs.MoveDownloaded, MoveDownloaded)
                    .WithArgument(FtpDownloadActionArgs.MoveDownloadedPath, MoveDownloadedPath)
                ;
        }

        public override void SetArguments(ArgumentCollection arguments)
        {
            if (arguments.HasArgument(FtpDownloadActionArgs.Host))
                Host = arguments.GetValue<string>(FtpDownloadActionArgs.Host);

            if (arguments.HasArgument(FtpDownloadActionArgs.Username))
                Username = arguments.GetValue<string>(FtpDownloadActionArgs.Username);

            if (arguments.HasArgument(FtpDownloadActionArgs.Password))
                Password = arguments.GetValue<string>(FtpDownloadActionArgs.Password);

            if (arguments.HasArgument(FtpDownloadActionArgs.Port))
                Port = arguments.GetValue<int>(FtpDownloadActionArgs.Port);

            if (arguments.HasArgument(FtpDownloadActionArgs.DirectoryPath))
                DirectoryPath = arguments.GetValue<string>(FtpDownloadActionArgs.DirectoryPath);

            if (arguments.HasArgument(FtpDownloadActionArgs.RemoteWorkingDir))
                RemoteWorkingDir = arguments.GetValue<string>(FtpDownloadActionArgs.RemoteWorkingDir);

            if (arguments.HasArgument(FtpDownloadActionArgs.UseTemporaryName))
                UseTemporaryName = arguments.GetValue<bool>(FtpDownloadActionArgs.UseTemporaryName);

            if (arguments.HasArgument(FtpDownloadActionArgs.Overwrite))
                Overwrite = arguments.GetValue<bool>(FtpDownloadActionArgs.Overwrite);

            if (arguments.HasArgument(FtpDownloadActionArgs.Append))
                Append = arguments.GetValue<bool>(FtpDownloadActionArgs.Append);

            if (arguments.HasArgument(FtpDownloadActionArgs.DeleteDownloaded))
                DeleteDownloaded = arguments.GetValue<bool>(FtpDownloadActionArgs.DeleteDownloaded);

            if (arguments.HasArgument(FtpDownloadActionArgs.MoveDownloaded))
                MoveDownloaded = arguments.GetValue<bool>(FtpDownloadActionArgs.MoveDownloaded);

            if (arguments.HasArgument(FtpDownloadActionArgs.MoveDownloadedPath))
                MoveDownloadedPath = arguments.GetValue<string>(FtpDownloadActionArgs.MoveDownloadedPath);
        }
    }
}
