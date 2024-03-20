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
            string KTASDKURL = "https://ktacloudeco-dev.ktaprt.dev.kofaxcloud.com/services/sdk/";
            string SESSIONID = "D2A967C768C7854B91C210DF77F118A4";
            string Base64token = "ApiKey rafael.castro@kofax.com:53887204-af5d-412e-805a-1b748b0262aa";
            //string Base64token = "ApiKey bill.mariani @kofax.com: 0d9083c9-c097-4ffa-873d-0d165f023b1c";
            Base64Connector Base64Connector = new Base64Connector();
            //string fileid = Base64Connector.Base64ScanFile("b578cb5e-9aa9-4fd1-b7c0-b05e00dd92ea", "pdf, KTA, "2BDC955ED880C84B9CB52287D59EBF37", Base64token);

            //Assert.IsNotNull(fileid);


            string extraction = Base64Connector.Base64GetExtractionResult("2e729f33-c3a7-4f50-8fbd-b13900ccdbbe", "pdf", KTASDKURL, SESSIONID, Base64token);
            //string extraction = Base64Connector.Base64GetExtractionResultFromFile("C:\\Program Files\\Kofax\\TotalAgility\\Sample Processes\\Capture SDK Sample Package\\Sample Images\\file1.tif", ".tif", Base64token);// ("94ceabcb-e198-418e-9ede-b00d00e84670", "pdf", KTASDKURL, "2BDC955ED880C84B9CB52287D59EBF37", Base64token);
            Assert.IsNotNull(extraction);


        }
    }
}
