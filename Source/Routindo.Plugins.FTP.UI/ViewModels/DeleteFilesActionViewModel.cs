using Routindo.Contract.Arguments;
using Routindo.Contract.UI;
using Routindo.Plugins.FTP.Components.Actions.Delete;

namespace Routindo.Plugins.FTP.UI.ViewModels
{
    public class DeleteFilesActionViewModel: PluginConfiguratorViewModelBase
    {
        private string _host;
        private string _username;
        private string _password;
        private int _port = FtpDeleteAction.DEFAULT_FTP_PORT;
        private string _remoteWorkingDir;

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
                .WithArgument(FtpDeleteActionArgs.Host, Host)
                .WithArgument(FtpDeleteActionArgs.Port, Port)
                .WithArgument(FtpDeleteActionArgs.Username, Username)
                .WithArgument(FtpDeleteActionArgs.Password, Password)
                .WithArgument(FtpDeleteActionArgs.RemoteWorkingDir, RemoteWorkingDir);
        }

        public override void SetArguments(ArgumentCollection arguments)
        {
            if (arguments.HasArgument(FtpDeleteActionArgs.Host))
                Host = arguments.GetValue<string>(FtpDeleteActionArgs.Host);

            if (arguments.HasArgument(FtpDeleteActionArgs.Username))
                Username = arguments.GetValue<string>(FtpDeleteActionArgs.Username);

            if (arguments.HasArgument(FtpDeleteActionArgs.Password))
                Password = arguments.GetValue<string>(FtpDeleteActionArgs.Password);

            if (arguments.HasArgument(FtpDeleteActionArgs.Port))
                Port = arguments.GetValue<int>(FtpDeleteActionArgs.Port);

            if (arguments.HasArgument(FtpDeleteActionArgs.RemoteWorkingDir))
                RemoteWorkingDir = arguments.GetValue<string>(FtpDeleteActionArgs.RemoteWorkingDir);
        }
    }
}
