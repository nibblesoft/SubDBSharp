using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SubDBSharp.Test
{
    [TestClass]
    public class UtilsTest
    {
        [TestMethod]
        public void TestGetHashString()
        {
            // dexter
            const string expected = "ffd8d4aa68033dc03d1c8ef373b9028c";
            // http://thesubdb.com/api/samples/dexter.mp4
            string path = @"dexter.mp4";
            string hashString = Utils.GetHashString(path);
            Assert.AreEqual(expected, hashString);
        }
    }
}
