using System;
using Routindo.Contract.Arguments;
using Routindo.Contract.UI;
using Routindo.Plugins.FTP.Components.Actions.Move;

namespace Routindo.Plugins.FTP.UI.ViewModels
{
    public class MoveFilesActionViewModel: PluginConfiguratorViewModelBase
    {
        private string _host;
        private string _username;
        private string _password;
        private int _port = FtpMoveFilesAction.DEFAULT_FTP_PORT;
        private string _remoteWorkingDir;
        private string _destinationPath;

        public string Host
        {
            get => _host;
            set
            {
                _host = value;
                ClearPropertyErrors();
                ValidateNonNullOrEmptyString(_host);
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                ClearPropertyErrors();
                ValidateNonNullOrEmptyString(_username);
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                ClearPropertyErrors();
                ValidateNonNullOrEmptyString(_password);
                OnPropertyChanged();
            }
        }

        public int Port
        {
            get => _port;
            set
            {
                _port = value;
                ClearPropertyErrors();
                ValidatePortNumber(_port);
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

        public string DestinationPath
        {
            get => _destinationPath;
            set
            {
                _destinationPath = value;
                ClearPropertyErrors();
                ValidateNonNullOrEmptyString(_destinationPath);
                OnPropertyChanged();
            }
        }

        public override void Configure()
        {
            this.InstanceArguments = ArgumentCollection.New()
                .WithArgument(FtpMoveFilesActionArgs.Host, Host)
                .WithArgument(FtpMoveFilesActionArgs.Port, Port)
                .WithArgument(FtpMoveFilesActionArgs.Username, Username)
                .WithArgument(FtpMoveFilesActionArgs.Password, Password)
                .WithArgument(FtpMoveFilesActionArgs.RemoteWorkingDir, RemoteWorkingDir)
                .WithArgument(FtpMoveFilesActionArgs.DestinationPath, DestinationPath);
        }

        public override void SetArguments(ArgumentCollection arguments)
        {
            if (arguments.HasArgument(FtpMoveFilesActionArgs.Host))
                Host = arguments.GetValue<string>(FtpMoveFilesActionArgs.Host);

            if (arguments.HasArgument(FtpMoveFilesActionArgs.Username))
                Username = arguments.GetValue<string>(FtpMoveFilesActionArgs.Username);

            if (arguments.HasArgument(FtpMoveFilesActionArgs.Password))
                Password = arguments.GetValue<string>(FtpMoveFilesActionArgs.Password);

            if (arguments.HasArgument(FtpMoveFilesActionArgs.Port))
            {
                var port = arguments.GetValue<int>(FtpMoveFilesActionArgs.Port);
                if (port > 0)
                    Port = port;
            }

            if (arguments.HasArgument(FtpMoveFilesActionArgs.RemoteWorkingDir))
                RemoteWorkingDir = arguments.GetValue<string>(FtpMoveFilesActionArgs.RemoteWorkingDir);

            if (arguments.HasArgument(FtpMoveFilesActionArgs.DestinationPath))
                DestinationPath = arguments.GetValue<string>(FtpMoveFilesActionArgs.DestinationPath);
        }
    }
}
