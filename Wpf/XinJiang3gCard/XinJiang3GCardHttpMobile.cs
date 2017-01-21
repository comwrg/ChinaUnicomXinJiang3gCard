using Http;
namespace Wpf.XinJiang3gCard
{
    class XinJiang3GCardHttpMobile:XinJiang3GHttpBase
    {
        private void PromtlyBuy()
        {
            RequestItem item = new RequestItem
                               {
                                   Url = "http://www.10010.com/mall-web/GoodsDetail/promtlyBuy",
                                   Method = Method.POST,
                                   PostData =$"goodsId=891609140208&activityId=90073100&activityType=4&activityProtper=24&productId=99999828&productValue=136&productType=4GMain&provinceCode=89&merchantId=8900000&cityCode=890&number=18599218573&numGroup=&numFrom=&numFee=0&oldPreFee=&preFeeVal=&usimPrice=0&difPlace=&localFee=&remoteFee=&isOnlinepay=&isReceivepay=&totalPrice=&custTag=2&tmpId=60000009&inventoryType=1&brandCode=AP&moduleCode=iPhone732G&colorCode=9810072100598994&privilegePack=&smsCode=&captchaCode=&staging=1&seckill=&iousFlag=&subCardNum=0&hasSalActNew="
                               };

            Response res = Req.GetResponse(item);


        }
    }
}
