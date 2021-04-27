using Microsoft.VisualStudio.TestTools.UnitTesting;
using Routindo.Contract.Services;
using Routindo.Plugins.FTP.Tests.Mock;

namespace Routindo.Plugins.FTP.Tests
{
    [TestClass]
    public class TestAssemblyInit 
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext testContext)
        { 
            ServicesContainer.SetServicesProvider(new FakeServicesProvider());
        }
    }
}
