using System;
using System.IO;
using Routindo.Contract.Services;

namespace Routindo.Plugins.FTP.Tests.Mock
{
    public class FakeServicesProvider: IServicesProvider
    {
        public ILoggingService GetLoggingService(string name, Type type = null)
        {
            return new FakeLoggingService(name, type);
        }

        public IEnvironmentService GetEnvironmentService()
        {
            return new FakeEnvironmentService(Path.Combine(Path.GetTempPath(), "Tests", "data"),
                Path.Combine(Path.GetTempPath(), "Tests", "logs"), Path.Combine(Path.GetTempPath(), "Tests", "config"));
        }
    }
}