using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Newtonsoft.Json;
using QSDMS.Util;
using iFramework.Framework;

namespace QX360.WeiXinWeb
{
    public class SmsHelper
    {
        private static readonly string appkey = ConfigurationManager.AppSettings["SmsAppKey"];
        private static readonly string secret = ConfigurationManager.AppSettings["SmsAppSecret"];
        private static readonly string url = "http://106.ihuyi.com/webservice/sms.php?method=Submit";

        private static IDictionary<SignMode, string> signDic = new Dictionary<SignMode, string>() {
            {SignMode.注册验证,"SMS_12785306"},
            {SignMode.身份验证,"SMS_12785310"},
            {SignMode.变更验证,"SMS_12785304"},
            {SignMode.系统通知,"SMS_42830035"},
        };

        public static bool SendSms(string phone, string code,string product) {          
            bool isSend = false;
            try
            {
                string account = appkey;//用户名是登录ihuyi.com账号名（例如：cf_demo123）
                string password = secret; //请登录用户中心->验证码、通知短信->帐户及签名设置->APIKEY
                string mobile = phone;               
                string content = "您的验证码是：" + code + " 。请不要把验证码泄露给其他人。";

                string postStrTpl = "account={0}&password={1}&mobile={2}&content={3}";               
                string postData = string.Format(postStrTpl, account, password, mobile, content);
                string Cookie = "";
                string resp = new HttpMethod().HttpPost(url, postData, Encoding.UTF8, ref Cookie);
                
                //解析结构
                int len1 = resp.IndexOf("</code>");
                int len2 = resp.IndexOf("<code>");
                string rtcode = resp.Substring((len2 + 6), (len1 - len2 - 6));
               
                int len3 = resp.IndexOf("</msg>");
                int len4 = resp.IndexOf("<msg>");
                string msg = resp.Substring((len4 + 5), (len3 - len4 - 5));
                if (rtcode == "2")//2表示发送成功
                {                   
                    isSend = true;
                }            
              
            }
            catch (Exception ex) {
                ex.Data["Method"] = "SmsHelper>>SendSms";
                new ExceptionHelper().LogException(ex);
            }

            return isSend;
        }
    }

    public enum SignMode
    {
        注册验证=1,
        变更验证=2,
        身份验证=3,
        系统通知=4,
    }
}
