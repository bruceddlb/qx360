using QSDMS.Util;
using QSMS.API.AliyunSms;
using QX360.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.WeiXinWeb
{
    public class SmsVerifyHelper
    {
        private static Random random = new Random();
        private static string fixed_mobile_verify_string = "sms_mobile_verify_{0}";


        public static bool SendMobileSms(string mobile)
        {
            string key = string.Format(fixed_mobile_verify_string, mobile);
            return SendRegisterSms(key, mobile, null);
        }

        public static string GetMobileSmsCode(string mobile)
        {
            string key = string.Format(fixed_mobile_verify_string, mobile);
            return CacheHelper.Instance.Get<string>(key);
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="mobile"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        private static bool SendRegisterSms(string key, string mobile, string code)
        {
            CacheHelper.Instance.Remove(key);
            if (string.IsNullOrWhiteSpace(code))
                code = random.Next(100000, 999999).ToString();
            //没即时发送，先注释
            //bool isSend = SendSmsMessageBLL.SendWCFRegisterSms(mobile, code);
            bool isSend = AliSmsHelper.SendSms(mobile, "SMS_120500273", "{\"code\":\"" + code + "\"}");
            if (isSend)
            {
                //缓存短信验证码信息,时间为3分钟
                CacheHelper.Instance.Update(key, code, 3);
            }
            return isSend;
        }
    }
}
