using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QSDMS.Util;
using QSMS.API.Payment.WxPay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QX360.Model;
using iFramework.Framework;
namespace QX360.WeiXinWeb.Controllers
{
    /// <summary>
    /// 支付
    /// </summary>
    public class PurseController : BaseController
    {
        //
        // GET: /Purse/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 微信在线付款
        /// </summary>
        /// <param name="money"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult WxPay(string money, string tradetitle = "订单支付")
        {
            var result = new ReturnMessage(false) { Message = "订单支付失败!" };
            try
            {
                if (string.IsNullOrWhiteSpace(money))
                {
                    result.Message = "请输入金额";
                    return Json(result);
                }
                if (LoginUser == null)
                {
                    result.Message = "获取当前用户信息失败";
                    return Json(result);
                }
                var openid = LoginUser.OpenId;
                JsApiPay jsApiPay = new JsApiPay(this);
                jsApiPay.openid = openid;
                jsApiPay.total_fee = (int)(Converter.ParseDecimal(money) * 100);//Converter.ParseDecimal(money, 0) * 100;//单位为分 所以在此*100
                //JSAPI支付预处理
                var tradeNo = WxPayApi.GenerateOutTradeNo();
                Log.Debug(this.GetType().Name, "用户订单支付!openid:" + LoginUser.OpenId);
                WxPayData unifiedOrderResult = jsApiPay.GetUnifiedOrderResult(tradeNo, "订单支付", "订单支付", openid, jsApiPay.total_fee, "ticket");
                WriteDebug("aaa");
                var appid = unifiedOrderResult.GetValue("appid").ToString();
                WriteDebug("bbb"+appid);
                var prepayid = unifiedOrderResult.GetValue("prepay_id").ToString();
                var param = jsApiPay.GetJsApiParameters(appid, prepayid);
                result.IsSuccess = true;
                result.Message = "支付成功!";
                result.ResultData["data"] = new { tradeNo = tradeNo, param = param };

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "PurseController>>WxPay";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

        /// <summary>
        /// 记录支付日志
        /// </summary>
        /// <param name="money"></param>
        /// <param name="orderno"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult WritePayLog(string money, string objectid, int operate, string remark)
        {
            var result = new ReturnMessage(false) { Message = "支付日志记录失败!" };
            try
            {
                TradeLogEntity trade = new TradeLogEntity();
                trade.TradeLogId = QSDMS.Util.Util.NewUpperGuid();
                trade.ObjectId = objectid;
                trade.Operate = (byte)Converter.ParseInt32(operate);
                trade.Remark = remark;
                trade.Money = Converter.ParseDecimal(money);
                trade.AddTime = DateTime.Now;
                trade.AccountId = LoginUser.UserId;
                result.IsSuccess = true;
                result.Message = "订单支付成功";

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "PurseController>>WritePayLog";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }
    }
}
