// ----------------------------------------------------------------
// Author      : Wrg
// Email       : comwrg@qq.com
// Date        : 2016/10/03 9:51
// Description : http://www.10010.com/goodsdetail/891606209102.html
// ----------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Http;
using MaterialDesignThemes.Wpf.Transitions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wpf.XinJiang3gCard;


namespace Wpf
{
    public class XinJiang3GCardHttp
    {
        private readonly Requester _requester = new Requester();


        public bool Order(ExportInfo ei)
        {
            string mToken, hName, hValue;
            decimal price;
            //GetCookies(ei.GoodsId);
            PromptlyOrder(ei.GoodsId, ei.Attribution, out mToken, out hName, out hValue, out price);
            if (Convert.ToDecimal(ei.Price) < price)
                return false;
            string cityCode, districtCode, essCityCode;
            GetCityCode(ei.City, out cityCode, out essCityCode);
            GetDistrictCode(cityCode, ei.District, out districtCode);
            string addrCode = GetAddrCode(essCityCode, districtCode, ei.BussinessHall);
            string paramStr = null;
            if (IsReceivePay(ei.Attribution, ei.GoodsId))
            {
                paramStr =
                    $"{{\"payment\":{{\"payWayCode\":\"01\",\"payTypeCode\":\"02\",\"payInstalmentBankCode\":\"\",\"payInstalmentTerm\":\"\"}},\"netWorkType\":\"new\",\"networkData\":{{\"hostName\":\"{ei.Name}\",\"idCard\":\"{ei.IdCard}\",\"psptTypeCode\":\"02\",\"idCardAddress\":\"\"}},\"postAddr\":{{\"ReceiverName\":\"{ei.Name}\",\"MobilePhone\":\"{ei.Mobile}\",\"ProvinceCode\":\"650000\",\"EssProvinceCode\":\"89\",\"ProvinceName\":\"新疆\",\"CityCode\":\"{cityCode}\",\"EssCityCode\":\"{essCityCode}\",\"CityName\":\"{ei.City}\",\"DistrictCode\":\"{districtCode}\",\"DistrictName\":\"{ei.District}\",\"Selftype\":\"0\",\"PostAddr\":\"{ei.BussinessHall}\"}},\"delivery\":{{\"dispachCode\":\"03\",\"deliveryCompanyCode\":\"\",\"selfFetchCode\":\"{addrCode}\",\"dlvTypeCode\":\"\"}},\"billingInfo\":{{\"invoiceTitle\":\"{ei.Name}\",\"invoiceContent\":\"\",\"orderRemarks\":\"\",\"moneyCardNum\":\"\",\"referrerName\":\"\",\"referrerCode\":\"\"}},\"province\":\"11\",\"districtName\":\"110\"}}";
            }
            else
            {
                paramStr =
                    $"{{\"payment\":{{\"payWayCode\":\"\",\"payTypeCode\":\"01\",\"payInstalmentBankCode\":\"\",\"payInstalmentTerm\":\"\"}},\"netWorkType\":\"new\",\"networkData\":{{\"hostName\":\"{ei.Name}\",\"idCard\":\"{ei.IdCard}\",\"psptTypeCode\":\"02\",\"idCardAddress\":\"\"}},\"postAddr\":{{\"ReceiverName\":\"{ei.Name}\",\"MobilePhone\":\"{ei.Mobile}\",\"ProvinceCode\":\"650000\",\"EssProvinceCode\":\"{essCityCode}\",\"ProvinceName\":\"新疆\",\"CityCode\":\"{cityCode}\",\"EssCityCode\":\"{essCityCode}\",\"CityName\":\"{ei.City}\",\"DistrictCode\":\"{districtCode}\",\"DistrictName\":\"{ei.District}\",\"Selftype\":\"0\",\"PostAddr\":\"{ei.BussinessHall}\"}},\"delivery\":{{\"dispachCode\":\"03\",\"deliveryCompanyCode\":\"\",\"selfFetchCode\":\"{addrCode}\",\"dlvTypeCode\":\"\"}},\"billingInfo\":{{\"invoiceTitle\":\"{ei.Name}\",\"invoiceContent\":\"\",\"orderRemarks\":\"\",\"moneyCardNum\":\"\",\"referrerName\":\"\",\"referrerCode\":\"\"}},\"province\":\"11\",\"city\":\"110\"}}";
            }



            //ProvinceCode:650000 -> 新疆

            paramStr = Uri.EscapeDataString(paramStr);
            //Console.WriteLine(paramStr);

            string inventoryType = "1";

            RequestItem item = new RequestItem()
            {

                Url = "http://www.10010.com/mall-web/OrderSubmit/submitOrder",
                Method = Method.POST,
                PostData =
                    $"paramStr={paramStr}&inventoryType={inventoryType}&uploadTokenUuid=&_m_token={mToken}&{Uri.EscapeDataString(hName)}={Uri.EscapeDataString(hValue)}"
            };

            var res = _requester.GetResponse(item);
            Console.WriteLine(res.Location);
            return true;
        }

        public bool IsReceivePay(string attribution, string goodsId)
        {
            string articleCity =
                Regex.Match(XinJiang3gCardHttpRes.Attribution, $"(\\d{{3}}){attribution}").Groups[1].Value;
            RequestItem item = new RequestItem
            {
                Url = "http://www.10010.com/mall-web/OrderInputAjaxNew/hasReceivePay",
                Method = Method.POST,
                PostData = $"AddrCityCode={articleCity}&NumberCityCode={articleCity}&goodsId={goodsId}",
            };
            var res = _requester.GetResponse(item);
            return bool.Parse(res.String);
        }

        private void GetCookies(string goodsId)
        {
            if (!_requester.CookieString.Contains("JUT2="))
            {
                _requester.GetResponse(new RequestItem()
                {
                    Url = "http://www.10010.com/mall-web/NoLogin/init?goodsId=" + goodsId
                });
                Console.WriteLine(_requester.CookieString);
            }
        }

        public void PromptlyOrder(string goodsId, string attribution, out string mToken, out string hName,
            out string hValue, out decimal price)
        {
            string articleCity =
                Regex.Match(XinJiang3gCardHttpRes.Attribution, $"(\\d{{3}}){attribution}").Groups[1].Value;

            GetCookies(goodsId);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            RequestItem item = new RequestItem
            {
                Url = "http://www.10010.com/mall-web/GoodsDetail/promptlyBuyNetworkCard",
                Method = Method.POST,
                PostData =
                    $"goodsId={goodsId}&tmplId=10000011&merchantId=8900000&provinceCode=89&cityCode=&diffPlace=0&localFee=0&remoteFee=0&isOnlinepay=1&isReceivepay=0&inventoryType=1&articleProvince=89&articleCity={articleCity}&manageType=3&articleSymbol=2372&productId=99005969&productPackDesc=%E5%90%AB%E9%A2%84%E5%AD%98%E6%AC%BE200%E5%85%83%E3%80%82%0D%0A%E8%B5%84%E8%B4%B9%E6%A0%87%E5%87%86%EF%BC%9A200%E5%85%83%E5%8C%8515GB%E7%9C%81%E5%86%85%E6%B5%81%E9%87%8F%EF%BC%8C%E6%9C%89%E6%95%88%E6%9C%9F%E4%B8%BA90%E5%A4%A9%EF%BC%8C%E6%9C%89%E6%95%88%E6%9C%9F%E4%B8%BA%E4%BA%A7%E5%93%81%E6%BF%80%E6%B4%BB%E6%97%A5%E8%B5%B7%E8%AE%A1%E7%AE%97%E7%9A%84%E7%B4%AF%E8%AE%A1%E5%A4%A9%E6%95%B0%E3%80%82&packageType=2372&brandCode=&modelCode=&colorCode=",
            };
            
            var res = _requester.GetResponse(item);
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            Match m = Regex.Match(res.String,
                "name=\"_m_token\" value=\"(.*?)\"[\\S\\s]*?name=\"(.*?)\" value=\"(.*?)\"");
            mToken = m.Groups[1].Value;
            hName = m.Groups[2].Value;
            hValue = m.Groups[3].Value;
            // Console.WriteLine(res.String);
//            Console.WriteLine(mToken);
//            Console.WriteLine(hName);
//            Console.WriteLine(hValue);
            //Console.WriteLine(res.String.IndexOf("_m_token"));


            price =
                Convert.ToDecimal(Regex.Match(res.String, "billingResult\" class=\"moneyQ\">(.*?)</").Groups[1].Value);
        }

        private void GetCityCode(string cityName, out string cityCode, out string essCityCode)
        {
//            Match m = Regex.Match(XinJiang3gCardHttpRes.Code,
//                $"\"CITY_CODE\": \"(\\d{{6}})\",\"DISTRICT_CODE\": \"(\\d{{6}})\",\"DISTRICT_NAME\": \"{district}\"");
//            cityCode = m.Groups[1].Value;
//            districtCode = m.Groups[2].Value;

            var jss = JObject.Parse(XinJiang3gCardHttpRes.Code);
            var items = from item in jss["PROVINCE_MAP"]["650000"]
                where (string) item["CITY_NAME"] == cityName
                select item;
            cityCode = essCityCode = null;
            foreach (var item in items)
            {
                cityCode = item["CITY_CODE"].ToString();
                essCityCode = item["ESS_CITY_CODE"].ToString();
            }

        }

        private void GetDistrictCode(string cityCode, string districtName, out string districtCode)
        {
            var jss = JObject.Parse(XinJiang3gCardHttpRes.Code);
            var items = from item in jss["CITY_MAP"][cityCode]
                where (string) item["DISTRICT_NAME"] == districtName
                select item;
            districtCode = null;
            foreach (var item in items)
            {
                districtCode = item["DISTRICT_CODE"].ToString();
            }

        }

        private string GetAddrCode(string essCityCode, string districtCode, string addr)
        {
            RequestItem item = new RequestItem
            {
                Url = "http://www.10010.com/mall-web/OrderInputAjaxNew/toGetSelfFetchInfoList",
                Method = Method.POST,
                PostData = $"merchantProvice=89&cityCode={essCityCode}&countyCode={districtCode}&goodsId=891606209102"
                // merchantProvice 商户所属省份
            };

            var res = _requester.GetResponse(item);
            string json = Regex.Unescape(res.String);
            json = json.Substring(1, json.Length - 2);
            JObject jss = JObject.Parse(json);
            try
            {
                var items = from js in jss["selfFetchInfo"] where (string)js["SELFGET_NAME"] == addr select js;
                foreach (var ite in items)
                {
                    return ite["ADDRESS_ID"].ToString();
                }
            }
            catch (Exception)
            {
                return addr;
            }

            return addr;

        }

    }
}