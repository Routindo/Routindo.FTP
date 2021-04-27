using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Routindo.Plugins.FTP.Components;

namespace Routindo.Plugins.FTP.Tests
{
    [TestClass]
    public class UriHelperTests
    {
        [TestMethod]
        public void CombineUriTest()
        {
            var uri = UriHelper.Combine("routindo.com", 121, "data", "file.txt");
            Console.WriteLine(uri);
        }

        [TestMethod]
        public void BuildPathTest()
        {
            var uri = UriHelper.BuildPath("Directory", "SubDirectory", "File.txt");
            Assert.AreEqual("Directory/SubDirectory/File.txt", uri);
            Console.WriteLine(uri);
        }

        [TestMethod]
        public void GetFileNameFromPathTest()
        {
            var fileName = UriHelper.GetFileName("Directory/File.txt");
            Assert.AreEqual("File.txt", fileName);
            Console.WriteLine(fileName);
        }
    }
}