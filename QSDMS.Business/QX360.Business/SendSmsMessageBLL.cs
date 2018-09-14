using QSMS.API.AliyunSms;
using QSMS.API.WCFAppSms;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Business
{
    public class SendSmsMessageBLL
    {

        public static bool SendWCFRegisterSms(string mobile, string content)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add(mobile, content);
            var flag = WCFAppSmsHelper.SendSms(dic, SmsType.SmsEvent);
            if (flag)
            {
                SmsLogBLL.Instance.Add(new SmsLogEntity()
                {
                    SmsLogId = QSDMS.Util.Util.NewUpperGuid(),
                    Caption = content,
                    RecivMobile = mobile,
                    Status = 1,
                    SmsTempId = "WCFAppSms",
                    CreateTime = DateTime.Now,
                    Exception = "成功"

                });
                return true;
            }
            else
            {
                SmsLogBLL.Instance.Add(new SmsLogEntity()
                {
                    SmsLogId = QSDMS.Util.Util.NewUpperGuid(),
                    Caption = content,
                    RecivMobile = mobile,
                    Status = 0,
                    SmsTempId = "WCFAppSms",
                    CreateTime = DateTime.Now,
                    Exception = "发送失败"

                });
            }
            return false;
        }
        /// <summary>
        /// 调用wcf接口发送短信
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="content"></param>
        public static bool SendWCFSms(string mobile, string content)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add(mobile, content);
            var flag = WCFAppSmsHelper.SendSms(dic, SmsType.SmsAD);
            return flag;

        }

        /// <summary>
        /// 消息提醒
        /// </summary>
        /// <param name="mobile">openid</param>
        /// <param name="username">name</param>
        /// <param name="servcicetime">time</param>
        /// eg:尊敬的${name} 先生/女士,服务内容：${content},服务时间：${servcicetime},预约订单号：${orderno}
        public static void SendSubricNotice(string memberid, string mobile, string username, string servcicetime, string content, string orderno)
        {
            string message = string.Format("尊敬的{0}先生/女士,您的预约已成功,服务内容：{1},服务时间：{2},预约订单号：{3}", username, content, servcicetime, orderno);
            var flag = SendWCFSms(mobile, message);
            if (flag)
            {
                SmsLogBLL.Instance.Add(new SmsLogEntity()
                {
                    SmsLogId = QSDMS.Util.Util.NewUpperGuid(),
                    MemberId = memberid,
                    NoticeType = (int)QX360.Model.Enums.SMNoticeType.提示短信,
                    Caption = content,
                    RecivMobile = mobile,
                    Status = 1,
                    SmsTempId = "WCFAppSms",
                    CreateTime = DateTime.Now,
                    Exception = "成功"

                });
            }
            else
            {
                SmsLogBLL.Instance.Add(new SmsLogEntity()
                {
                    SmsLogId = QSDMS.Util.Util.NewUpperGuid(),
                    MemberId = memberid,
                    NoticeType = (int)QX360.Model.Enums.SMNoticeType.提示短信,
                    Caption = content,
                    RecivMobile = mobile,
                    Status = 0,
                    SmsTempId = "WCFAppSms",
                    CreateTime = DateTime.Now,
                    Exception = "发送失败"

                });
            }
        }

      
        /// <summary>
        /// 年检到期消息提醒
        /// </summary>
        /// <param name="mobile">openid</param>
        /// <param name="username">name</param>
        /// <param name="servcicetime">time</param>
        /// eg:尊敬的${name} 先生/女士,车辆信息：${content}，年检即将到期，请前往机构进行年检服务
        public static void SendAuditNotice(string memberid, string mobile, string username, string content, DateTime? pretime)
        {
            //您好！您的爱车鄂EB6F48应于6月份年检，年检可提前三个月（含当月）进行，请您备好资料，安排时间年检。咨询电话：6361199 退订回T
            //您的爱车鄂EW512M应于06月份年检，年检可提前三个月（含当月）进行（逾期未年检将会面临罚200元扣3分的处罚），请备好资料，提前安排时间年检。预约请关注微信公众号"人车行"http://url.cn/5H4z3qR，咨询电话：6361199
            //string message = string.Format("您好！您的爱车{0}，年检可提前三个月（含当月）进行，请您备好资料，安排时间年检。咨询电话：6361199 ", content);
            string message = string.Format("您好！您的爱车{0}，年检可提前三个月（含当月）进行（逾期未年检将会面临罚200元扣3分的处罚），请备好资料，提前安排时间年检。预约请关注微信公众号【人车行】，咨询电话：6361199 ", content);
            var flag = SendWCFSms(mobile, message);
            if (flag)
            {
                SmsLogBLL.Instance.Add(new SmsLogEntity()
                {
                    SmsLogId = QSDMS.Util.Util.NewUpperGuid(),
                    Caption = content,
                    MemberId = memberid,
                    NoticeType = (int)QX360.Model.Enums.SMNoticeType.年审短信,
                    RecivMobile = mobile,
                    Status = 1,
                    SmsTempId = "WCFAppSms",
                    CreateTime = DateTime.Now,
                    Exception = "成功"

                });
            }
            else
            {
                SmsLogBLL.Instance.Add(new SmsLogEntity()
                {
                    SmsLogId = QSDMS.Util.Util.NewUpperGuid(),
                    Caption = content,
                    MemberId = memberid,
                    NoticeType = (int)QX360.Model.Enums.SMNoticeType.年审短信,
                    RecivMobile = mobile,
                    Status = 0,
                    SmsTempId = "WCFAppSms",
                    CreateTime = DateTime.Now,
                    Exception = "发送失败"

                });
            }
        }
    }
}
