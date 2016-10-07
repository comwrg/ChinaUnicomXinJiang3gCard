// ----------------------------------------------------------------
// Author      : Wrg
// Email       : comwrg@qq.com
// Date        : 2016/10/07 13:53
// Description : 
// ----------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Wpf.XinJiang3gCard
{
    public class PriceList
    {
        private XinJiang3GCardHttp http = new XinJiang3GCardHttp();
        Dictionary<string, decimal> _goodsIdDictionary = new Dictionary<string, decimal>();
        public void Add(string goodsId)
        {
            if(!_goodsIdDictionary.ContainsKey(goodsId))
                _goodsIdDictionary.Add(goodsId, decimal.MaxValue);
        }

        public void Refresh()
        {
            foreach (string goodsId in _goodsIdDictionary.Keys.ToArray())
            {
                string mToken, hName, hValue;
                decimal price;
                http.PromptlyOrder(goodsId, "乌鲁木齐", out mToken, out hName, out hValue, out price);
                _goodsIdDictionary[goodsId] = price;
            }
        }

        public decimal this[string goodsId] => _goodsIdDictionary[goodsId];
    }
}