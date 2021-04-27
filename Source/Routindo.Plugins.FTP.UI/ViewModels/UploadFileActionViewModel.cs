using System;
using Routindo.Contract;
using Routindo.Contract.Arguments;
using Routindo.Contract.UI;
using Routindo.Plugins.FTP.Components;
using Routindo.Plugins.FTP.Components.Actions.UploadFile;

namespace Routindo.Plugins.FTP.UI.ViewModels
{
    public class UploadFileActionViewModel : PluginConfiguratorViewModelBase
    {
        private string _host;
        private string _username;
        private string _password;
        private int _port = 21;
        private string _destinationFolderPath;
        private string _destinationFileName;
        private bool _useRemoteTemporaryExtension;
        private string _remoteTemporaryExtension;
        private bool _useLocalTemporaryExtension;
        private string _localTemporaryExtension;
        private bool _overwrite;
        private bool _createRemoteDirectory;

        // Mandatory fields 
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

        public string DestinationFolderPath
        {
            get => _destinationFolderPath;
            set
            {
                _destinationFolderPath = value;
                OnPropertyChanged();
            }
        }

        public string DestinationFileName
        {
            get => _destinationFileName;
            set
            {
                _destinationFileName = value;
                OnPropertyChanged();
            }
        }

        public bool UseRemoteTemporaryExtension
        {
            get => _useRemoteTemporaryExtension;
            set
            {
                _useRemoteTemporaryExtension = value;
                OnPropertyChanged();
            }
        }

        public string RemoteTemporaryExtension
        {
            get => _remoteTemporaryExtension;
            set
            {
                _remoteTemporaryExtension = value; OnPropertyChanged();
            }
        }

        public bool UseLocalTemporaryExtension
        {
            get => _useLocalTemporaryExtension;
            set
            {
                _useLocalTemporaryExtension = value;
                OnPropertyChanged();
            }
        }

        public string LocalTemporaryExtension
        {
            get => _localTemporaryExtension;
            set
            {
                _localTemporaryExtension = value; OnPropertyChanged();
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

        public bool CreateRemoteDirectory
        {
            get => _createRemoteDirectory;
            set
            {
                _createRemoteDirectory = value;
                OnPropertyChanged();
            }
        }

        public override void Configure()
        {
            this.InstanceArguments = ArgumentCollection.New()
                .WithArgument(FtpUploadActionInstanceArgs.Host, Host)
                .WithArgument(FtpUploadActionInstanceArgs.Port, Port)
                .WithArgument(FtpUploadActionInstanceArgs.Username, Username)
                .WithArgument(FtpUploadActionInstanceArgs.Password, Password)
                .WithArgument(FtpUploadActionInstanceArgs.DestinationFolderPath, DestinationFolderPath)
                .WithArgument(FtpUploadActionInstanceArgs.DestinationFileName, DestinationFileName)
                .WithArgument(FtpUploadActionInstanceArgs.UseRemoteTemporaryExtension, UseRemoteTemporaryExtension)
                .WithArgument(FtpUploadActionInstanceArgs.RemoteTemporaryExtension, RemoteTemporaryExtension)
                .WithArgument(FtpUploadActionInstanceArgs.UseLocalTemporaryExtension, UseLocalTemporaryExtension)
                .WithArgument(FtpUploadActionInstanceArgs.LocalTemporaryExtension, LocalTemporaryExtension)
                .WithArgument(FtpUploadActionInstanceArgs.Overwrite, Overwrite)
                .WithArgument(FtpUploadActionInstanceArgs.CreateRemoteDirectory, CreateRemoteDirectory);
        }

        public override void SetArguments(ArgumentCollection arguments)
        {
            if (arguments.HasArgument(FtpUploadActionInstanceArgs.Host))
                Host = arguments.GetValue<string>(FtpUploadActionInstanceArgs.Host);

            if (arguments.HasArgument(FtpUploadActionInstanceArgs.Username))
                Username = arguments.GetValue<string>(FtpUploadActionInstanceArgs.Username);

            if (arguments.HasArgument(FtpUploadActionInstanceArgs.Password))
                Password = arguments.GetValue<string>(FtpUploadActionInstanceArgs.Password);

            if (arguments.HasArgument(FtpUploadActionInstanceArgs.Port))
                Port = arguments.GetValue<int>(FtpUploadActionInstanceArgs.Port);

            if (arguments.HasArgument(FtpUploadActionInstanceArgs.DestinationFileName))
                DestinationFileName = arguments.GetValue<string>(FtpUploadActionInstanceArgs.DestinationFileName);

            if (arguments.HasArgument(FtpUploadActionInstanceArgs.DestinationFolderPath))
                DestinationFolderPath = arguments.GetValue<string>(FtpUploadActionInstanceArgs.DestinationFolderPath);

            if (arguments.HasArgument(FtpUploadActionInstanceArgs.UseRemoteTemporaryExtension))
                UseRemoteTemporaryExtension = arguments.GetValue<bool>(FtpUploadActionInstanceArgs.UseRemoteTemporaryExtension);

            if (arguments.HasArgument(FtpUploadActionInstanceArgs.RemoteTemporaryExtension))
                RemoteTemporaryExtension = arguments.GetValue<string>(FtpUploadActionInstanceArgs.RemoteTemporaryExtension);

            if (arguments.HasArgument(FtpUploadActionInstanceArgs.UseLocalTemporaryExtension))
                UseLocalTemporaryExtension = arguments.GetValue<bool>(FtpUploadActionInstanceArgs.UseLocalTemporaryExtension);

            if (arguments.HasArgument(FtpUploadActionInstanceArgs.LocalTemporaryExtension))
                LocalTemporaryExtension = arguments.GetValue<string>(FtpUploadActionInstanceArgs.LocalTemporaryExtension);

            if (arguments.HasArgument(FtpUploadActionInstanceArgs.Overwrite))
                Overwrite = arguments.GetValue<bool>(FtpUploadActionInstanceArgs.Overwrite);

            if (arguments.HasArgument(FtpUploadActionInstanceArgs.CreateRemoteDirectory))
                CreateRemoteDirectory = arguments.GetValue<bool>(FtpUploadActionInstanceArgs.CreateRemoteDirectory);
        }
    }
}