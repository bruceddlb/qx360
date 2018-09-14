using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Business
{

    /// <summary>
    /// 业务操作发送对应消息
    /// </summary>
    public class SendSysMessageBLL
    {
        /// <summary>
        /// 发送教练通知
        /// </summary>
        /// <param name="notice"></param>
        /// <param name="accountid"></param>
        /// <param name="servicetime"></param>
        /// <param name="content"></param>
        /// <param name="orderno"></param>
        public static void SendMessageTeacher(QX360.Model.Enums.NoticeType notice, string accountid, string servicetime, string content, string orderno)
        {
            var account = TeacherBLL.Instance.GetEntity(accountid);
            switch (notice)
            {
                case Enums.NoticeType.预约提醒:
                    SendWxMessage.SendSuccessNotice(account.OpenId, account.Name, servicetime, content, orderno);
                    break;
                case Enums.NoticeType.取消提醒:
                    SendWxMessage.SendCancelNotice(account.OpenId, account.Name, servicetime, content, orderno);
                    break;
                case Enums.NoticeType.更改提醒:
                    SendWxMessage.SendChangeNotice(account.OpenId, account.Name, servicetime, content, orderno);
                    break;
                case Enums.NoticeType.完成通知:
                    SendWxMessage.SendFinishNotice(account.OpenId, account.Name, servicetime, content, orderno);
                    break;
                default:
                    break;
            }


        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="title"></param>
        /// <param name="accountid"></param>
        /// <param name="content"></param>
        public static void SendMessage(QX360.Model.Enums.MessageAlterType type, QX360.Model.Enums.NoticeType notice, string accountid,string name, string servicetime, string content, string orderno)
        {
            var account = MemberBLL.Instance.GetEntity(accountid);
            var messagesetup = MessageSetUpBLL.Instance.GetList(new MessageSetUpEntity() { AccountId = accountid, AlterType = (int)type });
            if (messagesetup.Count > 0)
            {
                var accountset = messagesetup.FirstOrDefault();
                if (accountset.Status == (int)QX360.Model.Enums.UseStatus.启用)
                {
                    switch (type)
                    {
                        case Enums.MessageAlterType.学车预约提示:
                            SendStudyNotice(notice, account.OpenId, name, servicetime, content, orderno);
                            break;
                        case Enums.MessageAlterType.实训预约提示:
                            SendTrainingNotice(notice, account.OpenId, name, servicetime, content, orderno);
                            break;
                        case Enums.MessageAlterType.看车预约提示:
                            SendSeeCarNotice(notice, account.OpenId, name, servicetime, content, orderno);
                            break;
                        case Enums.MessageAlterType.年检预约提示:
                            SendAuditNotice(notice, account.OpenId, name, servicetime, content, orderno);
                            break;
                        case Enums.MessageAlterType.代审预约提示:
                            SendTakeAuditNotice(notice, account.OpenId, name, servicetime, content, orderno);
                            break;
                        case Enums.MessageAlterType.陪驾预约提示:
                            SendWithDrivingNotice(notice, account.OpenId, name, servicetime, content, orderno);
                            break;
                        case Enums.MessageAlterType.保险预约提示:
                            SendInsuranceNotice(notice, account.OpenId, name, servicetime, content, orderno);
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                switch (type)
                {
                    case Enums.MessageAlterType.学车预约提示:
                        SendStudyNotice(notice, account.OpenId, name, servicetime, content, orderno);
                        break;
                    case Enums.MessageAlterType.实训预约提示:
                        SendTrainingNotice(notice, account.OpenId, name, servicetime, content, orderno);
                        break;
                    case Enums.MessageAlterType.看车预约提示:
                        SendSeeCarNotice(notice, account.OpenId, name, servicetime, content, orderno);
                        break;
                    case Enums.MessageAlterType.年检预约提示:
                        SendAuditNotice(notice, account.OpenId, name, servicetime, content, orderno);
                        break;
                    case Enums.MessageAlterType.代审预约提示:
                        SendTakeAuditNotice(notice, account.OpenId, name, servicetime, content, orderno);
                        break;
                    case Enums.MessageAlterType.陪驾预约提示:
                        SendWithDrivingNotice(notice, account.OpenId, name, servicetime, content, orderno);
                        break;
                    case Enums.MessageAlterType.保险预约提示:
                        SendInsuranceNotice(notice, account.OpenId, name, servicetime, content, orderno);
                        break;
                    default:
                        break;
                }
            }

        }

        /// <summary>
        /// 学车
        /// </summary>
        /// <param name="notice"></param>
        /// <param name="toUser"></param>
        /// <param name="username"></param>
        /// <param name="servcicetime"></param>
        /// <param name="content"></param>
        /// <param name="orderno"></param>
        public static void SendStudyNotice(QX360.Model.Enums.NoticeType notice, string toUser, string username, string servcicetime, string content, string orderno)
        {
            switch (notice)
            {
                case Enums.NoticeType.预约提醒:
                    SendWxMessage.SendSuccessNotice(toUser, username, servcicetime, content, orderno);
                    break;
                case Enums.NoticeType.取消提醒:
                    SendWxMessage.SendCancelNotice(toUser, username, servcicetime, content, orderno);
                    break;
                case Enums.NoticeType.更改提醒:
                    SendWxMessage.SendChangeNotice(toUser, username, servcicetime, content, orderno);
                    break;
                case Enums.NoticeType.完成通知:
                    SendWxMessage.SendFinishNotice(toUser, username, servcicetime, content, orderno);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 实训
        /// </summary>
        /// <param name="notice"></param>
        /// <param name="toUser"></param>
        /// <param name="username"></param>
        /// <param name="servcicetime"></param>
        /// <param name="content"></param>
        /// <param name="orderno"></param>
        public static void SendTrainingNotice(QX360.Model.Enums.NoticeType notice, string toUser, string username, string servcicetime, string content, string orderno)
        {
            switch (notice)
            {
                case Enums.NoticeType.预约提醒:
                    SendWxMessage.SendSuccessNotice(toUser, username, servcicetime, content, orderno);
                    break;
                case Enums.NoticeType.取消提醒:
                    SendWxMessage.SendCancelNotice(toUser, username, servcicetime, content, orderno);
                    break;
                case Enums.NoticeType.更改提醒:
                    SendWxMessage.SendChangeNotice(toUser, username, servcicetime, content, orderno);
                    break;
                case Enums.NoticeType.完成通知:
                    SendWxMessage.SendFinishNotice(toUser, username, servcicetime, content, orderno);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 看车
        /// </summary>
        /// <param name="notice"></param>
        /// <param name="toUser"></param>
        /// <param name="username"></param>
        /// <param name="servcicetime"></param>
        /// <param name="content"></param>
        /// <param name="orderno"></param>
        public static void SendSeeCarNotice(QX360.Model.Enums.NoticeType notice, string toUser, string username, string servcicetime, string content, string orderno)
        {
            switch (notice)
            {
                case Enums.NoticeType.预约提醒:
                    SendWxMessage.SendSuccessNotice(toUser, username, servcicetime, content, orderno);
                    break;
                case Enums.NoticeType.取消提醒:
                    SendWxMessage.SendCancelNotice(toUser, username, servcicetime, content, orderno);
                    break;
                case Enums.NoticeType.更改提醒:
                    SendWxMessage.SendChangeNotice(toUser, username, servcicetime, content, orderno);
                    break;
                case Enums.NoticeType.完成通知:
                    SendWxMessage.SendFinishNotice(toUser, username, servcicetime, content, orderno);
                    break;
                default:
                    break;
            }
        }

        public static void SendAuditNotice(QX360.Model.Enums.NoticeType notice, string toUser, string username, string servcicetime, string content, string orderno)
        {
            switch (notice)
            {
                case Enums.NoticeType.预约提醒:
                    SendWxMessage.SendSuccessNotice(toUser, username, servcicetime, content, orderno);
                    break;
                case Enums.NoticeType.取消提醒:
                    SendWxMessage.SendCancelNotice(toUser, username, servcicetime, content, orderno);
                    break;
                case Enums.NoticeType.更改提醒:
                    SendWxMessage.SendChangeNotice(toUser, username, servcicetime, content, orderno);
                    break;
                case Enums.NoticeType.完成通知:
                    SendWxMessage.SendFinishNotice(toUser, username, servcicetime, content, orderno);
                    break;
                default:
                    break;
            }
        }

        public static void SendTakeAuditNotice(QX360.Model.Enums.NoticeType notice, string toUser, string username, string servcicetime, string content, string orderno)
        {
            switch (notice)
            {
                case Enums.NoticeType.预约提醒:
                    SendWxMessage.SendSuccessNotice(toUser, username, servcicetime, content, orderno);
                    break;
                case Enums.NoticeType.取消提醒:
                    SendWxMessage.SendCancelNotice(toUser, username, servcicetime, content, orderno);
                    break;
                case Enums.NoticeType.更改提醒:
                    SendWxMessage.SendChangeNotice(toUser, username, servcicetime, content, orderno);
                    break;
                case Enums.NoticeType.完成通知:
                    SendWxMessage.SendFinishNotice(toUser, username, servcicetime, content, orderno);
                    break;
                default:
                    break;
            }
        }
        public static void SendWithDrivingNotice(QX360.Model.Enums.NoticeType notice, string toUser, string username, string servcicetime, string content, string orderno)
        {
            switch (notice)
            {
                case Enums.NoticeType.预约提醒:
                    SendWxMessage.SendSuccessNotice(toUser, username, servcicetime, content, orderno);
                    break;
                case Enums.NoticeType.取消提醒:
                    SendWxMessage.SendCancelNotice(toUser, username, servcicetime, content, orderno);
                    break;
                case Enums.NoticeType.更改提醒:
                    SendWxMessage.SendChangeNotice(toUser, username, servcicetime, content, orderno);
                    break;
                case Enums.NoticeType.完成通知:
                    SendWxMessage.SendFinishNotice(toUser, username, servcicetime, content, orderno);
                    break;
                default:
                    break;
            }
        }
        public static void SendInsuranceNotice(QX360.Model.Enums.NoticeType notice, string toUser, string username, string servcicetime, string content, string orderno)
        {
            switch (notice)
            {
                case Enums.NoticeType.预约提醒:
                    SendWxMessage.SendSuccessNotice(toUser, username, servcicetime, content, orderno);
                    break;
                case Enums.NoticeType.取消提醒:
                    SendWxMessage.SendCancelNotice(toUser, username, servcicetime, content, orderno);
                    break;
                case Enums.NoticeType.更改提醒:
                    SendWxMessage.SendChangeNotice(toUser, username, servcicetime, content, orderno);
                    break;
                case Enums.NoticeType.完成通知:
                    SendWxMessage.SendFinishNotice(toUser, username, servcicetime, content, orderno);
                    break;
                default:
                    break;
            }
        }
    }
}
