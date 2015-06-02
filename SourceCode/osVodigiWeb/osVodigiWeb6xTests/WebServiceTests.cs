using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace osVodigiWeb6xTests
{
    [TestClass]
    public class WebServiceTests
    {
        [TestMethod]
        public void Test_DatabaseVersion_Get()
        {
            osVodigiWS.osVodigiServiceSoapClient ws = new osVodigiWS.osVodigiServiceSoapClient();
            osVodigiWS.DatabaseVersion version = ws.DatabaseVersion_Get();
        }

        [TestMethod]
        public void Player_GetCurrentSchedule()
        {
            osVodigiWS.osVodigiServiceSoapClient ws = new osVodigiWS.osVodigiServiceSoapClient();
            string schedulexml = ws.Player_GetCurrentSchedule(1000000);
        }

    }
}
