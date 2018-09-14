using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Diagnostics;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities;

namespace QSMS.API.Weixin.Common
{
    /// <summary>
    /// 处理事件的方法
    /// </summary>
    public partial class MessageFunction
    {
        
        #region 处理关注/取消/默认回复方法===========================
        /// <summary>
        /// 定阅事件的统一处理
        /// </summary>
        public IResponseMessageBase EventSubscribe(int type, RequestMessageEventBase requestMessage)
        {
            int accountId = GetAccountId(); //取得公众账户ID
            string EventName = "";

            if (requestMessage.Event.ToString().Trim() != "")
            {
                EventName = requestMessage.Event.ToString();
            }          

            if (!ExistsOriginalId(accountId, requestMessage.ToUserName))
            {
                //验证接收方是否为我们系统配置的帐号，即验证微帐号与微信原始帐号id是否一致，如果不一致，说明【1】配置错误，【2】数据来源有问题
                //new BLL.weixin_response_content().Add(accountId, requestMessage.FromUserName, requestMessage.MsgType.ToString(), EventName, "none", "未取到关键词对应的数据", requestMessage.ToUserName);
                return GetResponseMessageTxtByContent(requestMessage, "验证微帐号与微信原始帐号id不一致，可能原因【1】系统配置错误，【2】非法的数据来源", accountId);
            }


            //int responseType = 0;
            //int ruleId = 0;// new BLL.weixin_request_rule().GetRuleIdAndResponseType(accountId, "request_type=" + type, out responseType);
            //if (ruleId <= 0 || responseType <= 0)
            //{
            //    //new BLL.weixin_response_content().Add(accountId, requestMessage.FromUserName, requestMessage.MsgType.ToString(), EventName, "none", "未取到关键词对应的数据", requestMessage.ToUserName);
            //    return null;
            //}
            IResponseMessageBase reponseMessage = null;
            reponseMessage = GetResponseMessageTxt(requestMessage, accountId);
           
            //switch (responseType)
            //{
            //    case 1:
            //        //发送纯文字
            //        reponseMessage = GetResponseMessageTxt(requestMessage, ruleId, accountId);
            //        break;
            //    case 2:
            //        //发送多图文
            //        reponseMessage = GetResponseMessageNews(requestMessage, ruleId, accountId);
            //        break;
            //    case 3:
            //        //发送语音
            //        reponseMessage = GetResponseMessageeMusic(requestMessage, ruleId, accountId);
            //        break;
            //    default:
            //        break;
            //}
            return reponseMessage;
        }
        #endregion

        #region 请求为文本的处理=====================================
        /// <summary>
        /// 推送纯文字
        /// </summary>
        public IResponseMessageBase GetResponseMessageTxt(RequestMessageText requestMessage, int accountId)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            string openid = requestMessage.FromUserName;
            string token = ConvertDateTimeInt(DateTime.Now).ToString();
            responseMessage.Content = "xxxxxxxxxxxxxxx";//new BLL.weixin_request_content().GetContent(ruleId);
            //new BLL.weixin_response_content().Add(accountId, requestMessage.FromUserName, requestMessage.MsgType.ToString(), requestMessage.Content, "text", responseMessage.Content, requestMessage.ToUserName);
            return responseMessage;
        }

        /// <summary>
        /// 推送纯文字
        /// </summary>
        public IResponseMessageBase GetResponseMessageTxtByContent(RequestMessageText requestMessage, string content, int accountId)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseMessage.Content = content;
            //new BLL.weixin_response_content().Add(accountId, requestMessage.FromUserName, requestMessage.MsgType.ToString(), requestMessage.Content, "text", "文字请求，推送纯粹文字，内容为：" + content, requestMessage.ToUserName);
            return responseMessage;
        }

        /// <summary>
        /// 处理语音请求
        /// </summary>
        public IResponseMessageBase GetResponseMessageeMusic(RequestMessageText requestMessage, int accountId)
        {
            string EventName = "";
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageMusic>(requestMessage);

            return responseMessage;
        }

        /// <summary>
        /// 推送多图文
        /// </summary>
        public IResponseMessageBase GetResponseMessageNews(RequestMessageText requestMessage, int accountId)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage);
            string openid = requestMessage.FromUserName;
            string token = ConvertDateTimeInt(DateTime.Now).ToString();

            Article article;
            List<Article> artList = new List<Article>();

            return responseMessage;
        }
        #endregion

        #region 请求为事件的处理=====================================
        /// <summary>
        /// 推送纯文字
        /// </summary>
        public IResponseMessageBase GetResponseMessageTxt(RequestMessageEventBase requestMessage, int accountId)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            string openid = requestMessage.FromUserName;
            string token = ConvertDateTimeInt(DateTime.Now).ToString();
            responseMessage.Content = "aaaaaaaaaaaaaaa";//new BLL.weixin_request_content().GetContent(ruleId);

            string EventName = "";
            if (requestMessage.Event.ToString().Trim() != "")
            {
                EventName = requestMessage.Event.ToString();
            }
           
            return responseMessage;
        }

        /// <summary>
        /// 推送纯文字
        /// </summary>
        public IResponseMessageBase GetResponseMessageTxtByContent(RequestMessageEventBase requestMessage, string content, int accountId)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            var locationService = new LocationService();
            responseMessage.Content = content;
            string EventName = "";
            if (requestMessage.Event.ToString().Trim() != "")
            {
                EventName = requestMessage.Event.ToString();
            }
           
            //new BLL.weixin_response_content().Add(accountId, requestMessage.FromUserName, requestMessage.MsgType.ToString(), EventName, "text", "事件：推送纯粹的文字，内容为:" + content, requestMessage.ToUserName);

            return responseMessage;
        }

        /// <summary>
        /// 处理语音请求
        /// </summary>
        public IResponseMessageBase GetResponseMessageeMusic(RequestMessageEventBase requestMessage, int ruleId, int accountId)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageMusic>(requestMessage);
            string EventName = "";
            if (requestMessage.Event.ToString().Trim() != "")
            {
                EventName = requestMessage.Event.ToString();
            }
          

            //
            return responseMessage;
        }

        /// <summary>
        /// 推送多图文
        /// </summary>
        public IResponseMessageBase GetResponseMessageNews(RequestMessageEventBase requestMessage, int ruleId, int accountId)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNews>(requestMessage);
            string openid = requestMessage.FromUserName;
            string token = ConvertDateTimeInt(DateTime.Now).ToString();

            Article article;
            List<Article> artList = new List<Article>();


            string EventName = "";
            if (requestMessage.Event.ToString().Trim() != "")
            {
                EventName = requestMessage.Event.ToString();
            }         

            //
            responseMessage.Articles.AddRange(artList);
            return responseMessage;
        }
        #endregion

        #region 获取验证公众账户ID===================================
        /// <summary>
        /// 获取公众账户的ID
        /// </summary>
        public int GetAccountId()
        {
            if (HttpContext.Current.Request["uid"] == null || HttpContext.Current.Request["uid"].ToString().Length < 1)
            {
                return 0;
            }
            int tmpInt = 0;
            if (!int.TryParse(HttpContext.Current.Request["uid"].ToString(), out tmpInt))
            {
                return 0;
            }
            int uid = int.Parse(HttpContext.Current.Request["uid"].ToString());
            return uid;
        }

        /// <summary>
        /// 验证公众账户原始ID是否一致
        /// </summary>
        public bool ExistsOriginalId(int accountId, string originalId)
        {
            return true;//验证id一致//new BLL.weixin_account().ExistsOriginalId(accountId, originalId);
        }
        #endregion

        #region 常用的方法封装=======================================
        /// <summary>
        /// 拼接微信URL地址参数
        /// </summary>
        public string GetWXApiUrl(string url, string token, string openid)
        {
            if (url.Contains("?"))
            {
                return url + "&token=" + token + "&openid=" + openid;
            }
            return url + "?token=" + token + "&openid=" + openid;
        }

        /// <summary>
        /// 设置微信url地址的后缀
        /// </summary>
        /// <returns></returns>
        public string GetWxUrlSuffix()
        {
            return "wxref=mp.weixin.qq.com";
        }

        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        public long ConvertDateTimeInt(System.DateTime time)
        {
            long intResult = 0;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            intResult = (long)(time - startTime).TotalSeconds;
            return intResult;
        }
        #endregion
    }
}
