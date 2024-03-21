using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using Kofax.Base64ConnectorV1;
using static System.Net.WebRequestMethods;

namespace Base64UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestBase64Flow()
        {
            string KTASDKURL = "https://<Your tenant here>/services/sdk/";
            string SESSIONID = "<Your Sesion ID Here>";
            string Base64token = "ApiKey <Your Base64.ai Token Here>";
            Base64Connector Base64Connector = new Base64Connector();

            string extraction = Base64Connector.Base64GetExtractionResult("<Document.InstanceID>", "<FileExtension>", KTASDKURL, SESSIONID, Base64token);
            Assert.IsNotNull(extraction);


        }
    }
}
