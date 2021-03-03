using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Umator.Plugins.FTP.Components;

namespace Umator.Plugins.FTP.Tests
{
    [TestClass]
    public class UriHelperTests
    {
        [TestMethod]
        public void CombineUriTest()
        {
            var uri = UriHelper.Combine("umator.com", 121, "data", "file.txt");
            Console.WriteLine(uri);
        }

        [TestMethod]
        public void BuildPathTest()
        {
            var uri = UriHelper.BuildPath("Directory", "SubDirectory", "File.txt");
            Assert.AreEqual("Directory/SubDirectory/File.txt", uri);
            Console.WriteLine(uri);
        }
    }
}