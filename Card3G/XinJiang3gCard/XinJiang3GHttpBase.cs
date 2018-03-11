// ----------------------------------------------------------------
// Author      : Wrg
// Email       : comwrg@qq.com
// Date        : 2016/11/02 19:36
// Description : 
// ----------------------------------------------------------------

using System;
using System.Linq;
using System.Text.RegularExpressions;
using Http;
using Newtonsoft.Json.Linq;

namespace Wpf.XinJiang3gCard
{
    public class XinJiang3GHttpBase
    {
        protected readonly Requester Req = new Requester();

        protected bool IsReceivePay(string attribution, string goodsId)
        {
            var articleCity =
                Regex.Match(XinJiang3gCardHttpRes.Attribution, $"(\\d{{3}}){attribution}").Groups[1].Value;
            var item = new RequestItem
                       {
                           Url = "http://mall.10010.com/mall-web/OrderInputAjaxNew/hasReceivePay",
                           Method = Method.POST,
                           PostData = $"AddrCityCode={articleCity}&NumberCityCode={articleCity}&goodsId={goodsId}"
                       };
            var res = Req.GetResponse(item);
            return bool.Parse(res.String);
        }
        /// <summary>
        /// 初始化Cookies,需要在一切网页访问之前进行
        /// </summary>
        /// <param name="goodsId">商品编号</param>
        protected void InitCookies(string goodsId)
        {
            if (!Req.CookieString.Contains("JUT2="))
            {
                Req.GetResponse(new RequestItem
                                       {
                                           Url = "http://mall.10010.com/mall-web/NoLogin/init?goodsId=" + goodsId
                                       });
                Console.WriteLine(Req.CookieString);
            }
        }
        /// <summary>
        /// 获取CityCode   out cityCode,essCityCode
        /// </summary>
        /// <param name="cityName">城市名称</param>
        /// <param name="cityCode">城市编号</param>
        /// <param name="essCityCode">ess城市编号</param>
        protected void GetCityCode(string cityName, out string cityCode, out string essCityCode)
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

        protected void GetDistrictCode(string cityCode, string districtName, out string districtCode)
        {
            var jss = JObject.Parse(XinJiang3gCardHttpRes.Code);
            var items = from item in jss["CITY_MAP"][cityCode]
                        where (string) item["DISTRICT_NAME"] == districtName
                        select item;
            districtCode = null;
            foreach (var item in items)
                districtCode = item["DISTRICT_CODE"].ToString();
        }

        protected string GetAddrCode(string essCityCode, string districtCode, string addr)
        {
            var item = new RequestItem
                       {
                           Url = "http://mall.10010.com/mall-web/OrderInputAjaxNew/toGetSelfFetchInfoList",
                           Method = Method.POST,
                           PostData =
                               $"merchantProvice=89&cityCode={essCityCode}&countyCode={districtCode}&goodsId=891606209102"
                           // merchantProvice 商户所属省份
                       };

            var res = Req.GetResponse(item);
            var json = Regex.Unescape(res.String);
            json = json.Substring(1, json.Length - 2);
            var jss = JObject.Parse(json);
            try
            {
                var items = from js in jss["selfFetchInfo"] where (string) js["SELFGET_NAME"] == addr select js;
                foreach (var ite in items)
                    return ite["ADDRESS_ID"].ToString();
            }
            catch (Exception)
            {
                return addr;
            }

            return addr;
        }
        /// <summary>
        /// 正则获取网页中的价格
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        protected decimal GetPrice(string html)
            => Convert.ToDecimal(Regex.Match(html, "billingResult\" class=\"moneyQ\">(.*?)</").Groups[1].Value);
        /// <summary>
        /// 获取城市代码
        /// </summary>
        /// <param name="attribution"></param>
        /// <returns></returns>
        protected string GetArticleCity(string attribution)
            => Regex.Match(XinJiang3gCardHttpRes.Attribution, $"(\\d{{3}}){attribution}").Groups[1].Value;

        protected void GetMToken(string html,out string mToken, out string hName, out string hValue)
        {
            var m = Regex.Match(html,
                    "name=\"_m_token\" value=\"(.*?)\"[\\S\\s]*?name=\"(.*?)\" value=\"(.*?)\"");
            mToken = m.Groups[1].Value;
            hName = m.Groups[2].Value;
            hValue = m.Groups[3].Value;
        }
    }
}