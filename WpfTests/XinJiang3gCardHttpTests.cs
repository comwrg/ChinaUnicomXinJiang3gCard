using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Wpf.Tests
{
    [TestClass()]
    public class XinJiang3gCardHttpTests
    {
        XinJiang3GCardHttp http = new XinJiang3GCardHttp();
        [TestMethod()]
        public void PromptlyOrderTest()
        {
            string mtoken, hName, hValue;
            decimal price;
            //http.PromptlyBuyNetworkCard(out mtoken, out hName, out hValue, out price);
        }

        [TestMethod()]
        public void OrderTest()
        {
            //http.BuyNetworkCard("伍晴虹", "431381198109106573", "13301489219");
        }

        [TestMethod()]
        public void GetCodeTest()
        {
            string districtCode;
            string cityCode;
            //http.GetCode("乌鲁木齐市", out cityCode, out districtCode);
        }
    }
}