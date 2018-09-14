using QSMS.API.AppSmsClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSMS.API.WCFAppSms
{
    public class WCFAppSmsHelper
    {

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static bool SendSms(Dictionary<string, string> dic, SmsType type)
        {
            try
            {
                Dictionary<string, string> nmc = new Dictionary<string, string>();
                string sign = "【人车行】";
                SmsPropertyEnum smstype = SmsPropertyEnum.SmsEvent;
                if (type == SmsType.SmsAD)
                {
                    sign = "回T退订 【人车行】";
                    smstype = SmsPropertyEnum.SmsAD;
                }
                foreach (var item in dic)
                {
                    nmc.Add(item.Key, string.Format("{0}{1}", dic[item.Key], sign));
                }
                AppSmsClient.AppSmsClient client = new AppSmsClient.AppSmsClient();
                string msg = "";
                int compid = int.Parse(QSDMS.Util.Config.GetValue("SMSCompId"));
                string account = QSDMS.Util.Config.GetValue("SMSAccount");
                string pwd = QSDMS.Util.Config.GetValue("SMSPwd");
                var ret = client.SendSms(smstype, account, pwd, nmc, 0, compid, 0, out msg);
                if (ret == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {

                return false;
            }
            return false;
        }

    }
    /// <summary>
    /// 短信类型
    /// </summary>
    public enum SmsType
    {
        /// <summary>
        /// 消息提醒类短信
        /// </summary>

        SmsEvent = 0,

        /// <summary>
        /// 营销类短信内容
        /// </summary>

        SmsAD = 1
    }
}
