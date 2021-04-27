using Routindo.Contract.Services;

namespace Routindo.Plugins.FTP.Tests.Mock
{
    public class FakeEnvironmentService: IEnvironmentService
    {
        public FakeEnvironmentService(string dataDirectory, string logsDirectory, string configDirectory)
        {
            DataDirectory = dataDirectory;
            LogsDirectory = logsDirectory;
            ConfigDirectory = configDirectory;
        }

        public string DataDirectory { get; }
        public string LogsDirectory { get; }
        public string ConfigDirectory { get; }
    }
}
