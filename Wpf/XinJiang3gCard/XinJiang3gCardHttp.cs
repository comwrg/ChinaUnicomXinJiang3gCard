// ----------------------------------------------------------------
// Author      : Wrg
// Email       : comwrg@qq.com
// Date        : 2016/10/03 9:51
// Description : http://www.10010.com/goodsdetail/891606209102.html
// ----------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Http;
using Wpf.XinJiang3gCard;

namespace Wpf
{
    public class XinJiang3GCardHttp : XinJiang3GHttpBase
    {
        public bool BuyNetworkCard(BuyCardInfo ei)
        {
            string mToken, hName, hValue;
            decimal price;
            //InitCookies(ei.GoodsId);
            PromptlyBuyNetworkCard(ei.GoodsId, ei.Attribution, out mToken, out hName, out hValue, out price);
            if (Convert.ToDecimal(ei.Price) < price)
                return false;
            string cityCode, districtCode, essCityCode;
            GetCityCode(ei.City, out cityCode, out essCityCode);
            GetDistrictCode(cityCode, ei.District, out districtCode);
            var addrCode = GetAddrCode(essCityCode, districtCode, ei.BussinessHall);
            string paramStr = null;
            if (IsReceivePay(ei.Attribution, ei.GoodsId))
                paramStr =
                    $"{{\"payment\":{{\"payWayCode\":\"01\",\"payTypeCode\":\"02\",\"payInstalmentBankCode\":\"\",\"payInstalmentTerm\":\"\"}},\"netWorkType\":\"new\",\"networkData\":{{\"hostName\":\"{ei.Name}\",\"idCard\":\"{ei.IdCard}\",\"psptTypeCode\":\"02\",\"idCardAddress\":\"\"}},\"postAddr\":{{\"ReceiverName\":\"{ei.Name}\",\"MobilePhone\":\"{ei.Mobile}\",\"ProvinceCode\":\"650000\",\"EssProvinceCode\":\"89\",\"ProvinceName\":\"新疆\",\"CityCode\":\"{cityCode}\",\"EssCityCode\":\"{essCityCode}\",\"CityName\":\"{ei.City}\",\"DistrictCode\":\"{districtCode}\",\"DistrictName\":\"{ei.District}\",\"Selftype\":\"0\",\"PostAddr\":\"{ei.BussinessHall}\"}},\"delivery\":{{\"dispachCode\":\"03\",\"deliveryCompanyCode\":\"\",\"selfFetchCode\":\"{addrCode}\",\"dlvTypeCode\":\"\"}},\"billingInfo\":{{\"invoiceTitle\":\"{ei.Name}\",\"invoiceContent\":\"\",\"orderRemarks\":\"\",\"moneyCardNum\":\"\",\"referrerName\":\"\",\"referrerCode\":\"\"}},\"province\":\"11\",\"districtName\":\"110\"}}";
            else
                paramStr =
                    $"{{\"payment\":{{\"payWayCode\":\"\",\"payTypeCode\":\"01\",\"payInstalmentBankCode\":\"\",\"payInstalmentTerm\":\"\"}},\"netWorkType\":\"new\",\"networkData\":{{\"hostName\":\"{ei.Name}\",\"idCard\":\"{ei.IdCard}\",\"psptTypeCode\":\"02\",\"idCardAddress\":\"\"}},\"postAddr\":{{\"ReceiverName\":\"{ei.Name}\",\"MobilePhone\":\"{ei.Mobile}\",\"ProvinceCode\":\"650000\",\"EssProvinceCode\":\"{essCityCode}\",\"ProvinceName\":\"新疆\",\"CityCode\":\"{cityCode}\",\"EssCityCode\":\"{essCityCode}\",\"CityName\":\"{ei.City}\",\"DistrictCode\":\"{districtCode}\",\"DistrictName\":\"{ei.District}\",\"Selftype\":\"0\",\"PostAddr\":\"{ei.BussinessHall}\"}},\"delivery\":{{\"dispachCode\":\"03\",\"deliveryCompanyCode\":\"\",\"selfFetchCode\":\"{addrCode}\",\"dlvTypeCode\":\"\"}},\"billingInfo\":{{\"invoiceTitle\":\"{ei.Name}\",\"invoiceContent\":\"\",\"orderRemarks\":\"\",\"moneyCardNum\":\"\",\"referrerName\":\"\",\"referrerCode\":\"\"}},\"province\":\"11\",\"city\":\"110\"}}";


            //ProvinceCode:650000 -> 新疆

            paramStr = Uri.EscapeDataString(paramStr);
            //Console.WriteLine(paramStr);

            var inventoryType = "1";

            var item = new RequestItem
                       {
                           Url = "http://www.10010.com/mall-web/OrderSubmit/submitOrder",
                           Method = Method.POST,
                           PostData =
                               $"paramStr={paramStr}&inventoryType={inventoryType}&uploadTokenUuid=&_m_token={mToken}&{Uri.EscapeDataString(hName)}={Uri.EscapeDataString(hValue)}"
                       };

            var res = Req.GetResponse(item);
            Console.WriteLine(res.Location);
            // res.Location 
            // http://www.10010.com/mall-web/OrderSuccess/showOrderInfo?enPara=uSod4mavQfRxixgGzPzHTUbQQT6oNGFiH%2BdfpjaxQx8%3D

            return res.Location.Contains("OrderSuccess");
        }

        public void PromptlyBuyNetworkCard(string goodsId, string attribution, out string mToken, out string hName,
                                            out string hValue, out decimal price)
        {
            var articleCity = GetArticleCity(attribution);

            InitCookies(goodsId);

//            var sw = new Stopwatch();
//            sw.Start();
            var item = new RequestItem
                       {
                           Url = "http://www.10010.com/mall-web/GoodsDetail/promptlyBuyNetworkCard",
                           Method = Method.POST,
                           PostData =
                               $"goodsId={goodsId}&tmplId=10000011&merchantId=8900000&provinceCode=89&cityCode=&diffPlace=0&localFee=0&remoteFee=0&isOnlinepay=1&isReceivepay=0&inventoryType=1&articleProvince=89&articleCity={articleCity}&manageType=3&articleSymbol=2372&productId=99005969&productPackDesc=%E5%90%AB%E9%A2%84%E5%AD%98%E6%AC%BE200%E5%85%83%E3%80%82%0D%0A%E8%B5%84%E8%B4%B9%E6%A0%87%E5%87%86%EF%BC%9A200%E5%85%83%E5%8C%8515GB%E7%9C%81%E5%86%85%E6%B5%81%E9%87%8F%EF%BC%8C%E6%9C%89%E6%95%88%E6%9C%9F%E4%B8%BA90%E5%A4%A9%EF%BC%8C%E6%9C%89%E6%95%88%E6%9C%9F%E4%B8%BA%E4%BA%A7%E5%93%81%E6%BF%80%E6%B4%BB%E6%97%A5%E8%B5%B7%E8%AE%A1%E7%AE%97%E7%9A%84%E7%B4%AF%E8%AE%A1%E5%A4%A9%E6%95%B0%E3%80%82&packageType=2372&brandCode=&modelCode=&colorCode="
                       };

            var res = Req.GetResponse(item);
//            sw.Stop();
//            Console.WriteLine(sw.Elapsed);
            GetMToken(res.String, out mToken, out hName, out hValue);
            // Console.WriteLine(res.String);
//            Console.WriteLine(mToken);
//            Console.WriteLine(hName);
//            Console.WriteLine(hValue);
            //Console.WriteLine(res.String.IndexOf("_m_token"));


            price = GetPrice(res.String);
        }
    }
}