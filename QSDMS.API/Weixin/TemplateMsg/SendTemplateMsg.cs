using iFramework.Framework.Log;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSMS.API.Weixin.TemplateMsg
{
    /// <summary>
    /// 发送模板消息
    /// </summary>
    public class SendTemplateMsg
    {
        private Log _logger;
        /// <summary>
        /// 日志操作
        /// </summary>
        public  Log Logger
        {
            get { return _logger ?? (_logger = LogFactory.GetLogger(this.GetType().ToString())); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="openId">用户openId  </param>
        /// <param name="templateId">模版id</param>
        /// <param name="linkUrl">点击详情后跳转后的链接地址，为空则不跳转 </param>
        public static ErrorMessage SendMsg(WxMsgModel model, Dictionary<string, string> param)
        {
            ErrorMessage errorMessage = new ErrorMessage(ErrorMessage.ExceptionCode, "消息发送失败");
            try
            {
                //校验参数
                if (string.IsNullOrWhiteSpace(model.OpenId))
                {
                    errorMessage.errmsg = "接收消息的账号不能为空。";
                    return errorMessage;
                }
                if (string.IsNullOrWhiteSpace(model.TemplateId))
                {
                    errorMessage.errmsg = "模板id不能为空。";
                    return errorMessage;
                }
                if (string.IsNullOrWhiteSpace(Config.APPID) && string.IsNullOrWhiteSpace(Config.APPSECRET))
                {
                    errorMessage.errmsg = "微信appid，AppSecret不能为空";
                    return errorMessage;
                }
                //string appId, appSecret;
                //appId = "wx65aceb537005fb0d";
                //appSecret = "a7eddd99f2e1ab7acb1fe6aa09152941";
                //获取许可令牌
                AccessToken token = AccessToken.Get(Config.APPID, Config.APPSECRET);
                if (token == null)
                {
                    errorMessage.errmsg = "获取Token失败。";
                }

                Dictionary<string, TemplateDataItem> templateData = new Dictionary<string, TemplateDataItem>();

                foreach (var item in param.Keys)
                {
                    templateData.Add(item, new TemplateDataItem(param[item]));
                }

                SendTemplateMessageResult sendResult = TemplateApi.SendTemplateMessage(token.access_token, model.OpenId, model.TemplateId, "#ccc", model.LinkUrl, templateData);

                //发送成功  
                if (sendResult.errcode.ToString() == "请求成功")
                {
                    //...  
                }
                else
                {

                    //Logger.Error(sendResult.errmsg);
                }
                errorMessage.errcode = 0;
                errorMessage.errmsg = "成功";
                return errorMessage;
            }
            catch (Exception ex)
            {
                return errorMessage;
            }
        }
    }
}

