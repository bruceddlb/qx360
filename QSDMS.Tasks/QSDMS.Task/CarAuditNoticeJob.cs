using Common.Logging;
using Quartz;
using QX360.Business;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSDMS.Task
{
    /// <summary>
    /// 车辆年检提醒-短信提醒
    /// 晚上：9：00
    /// </summary>
    public class CarAuditNoticeJob : IJob
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CarAuditNoticeJob));
        int noticemonth = -2;//提前3个月
        string noticeDay = SettingsBLL.Instance.GetValue("audit_notice_time") == "" ? "1" : SettingsBLL.Instance.GetValue("audit_notice_time");
        public void Execute(IJobExecutionContext context)
        {
            //处理当前的数据
            logger.Info("开始执行发送年检通知:start....");
            AuditNotice();
            logger.Info("开始执行发送年检通知:end....");
            logger.Info("开始执行发送年检通知..时间:" + DateTime.Now);
        }

        public void AuditNotice()
        {
            try
            {
                noticeDay = noticeDay.PadLeft(2, '0');
                var list = OwnerBLL.Instance.GetList(null);
                foreach (var o in list)
                {
                    if (o.RegisterTime == null)
                    {
                        continue;
                    }
                    if (o.LastCheckTime == null)
                    {
                        o.LastCheckTime = o.RegisterTime;
                    }
                    //当前注册日期的月份
                    string currentMonth = o.RegisterTime.Value.Month.ToString().PadLeft(2, '0');
                    string currentYear = DateTime.Now.Year.ToString();
                    //非营运/警车 类型 载客（小型、微型）
                    if ((o.UseType == (int)QX360.Model.Enums.UseType.非营运 && o.CarType == (int)QX360.Model.Enums.CarType.微_小型客车) || (o.UseType == (int)QX360.Model.Enums.UseType.警车_救护 && o.CarType == (int)QX360.Model.Enums.CarType.微_小型客车))
                    {
                        if (DateTime.Now > o.RegisterTime.Value.AddYears(15))//大于15
                        {
                            if (DateTime.Now.ToString("yyyy-MM-dd") == Convert.ToDateTime(string.Format("{0}-{1}-{2}", currentYear, currentMonth, noticeDay)).AddMonths(noticemonth).ToString("yyyy-MM-dd") || DateTime.Now.ToString("yyyy-MM-dd") == Convert.ToDateTime(string.Format("{0}-{1}-{2}", currentYear, currentMonth, noticeDay)).AddMonths(6).AddMonths(noticemonth).ToString("yyyy-MM-dd"))
                            {
                                //发送短信
                                var content = string.Format("{0}应于{1}月份年检", o.CarNumber, currentMonth);
                                SendSmsMessageBLL.SendAuditNotice(o.MemberId,o.MemberMobile, o.MemberName, content, o.RegisterTime);
                            }
                        }
                        else if (DateTime.Now > o.RegisterTime.Value.AddYears(6))//大于6
                        {
                            if (DateTime.Now.ToString("yyyy-MM-dd") == Convert.ToDateTime(string.Format("{0}-{1}-{2}", currentYear, currentMonth, noticeDay)).AddMonths(noticemonth).ToString("yyyy-MM-dd"))
                            {
                                //发送短信
                                var content = string.Format("{0}应于{1}月份年检", o.CarNumber, currentMonth);
                                SendSmsMessageBLL.SendAuditNotice(o.MemberId,o.MemberMobile, o.MemberName, content, o.RegisterTime);
                            }
                        }
                        else
                        {
                            //6年内
                            if (DateTime.Now.ToString("yyyy-MM-dd") == o.LastCheckTime.Value.AddYears(2).AddMonths(noticemonth).ToString("yyyy-MM") + "-" + noticeDay)
                            {
                                //发送短信
                                var content = string.Format("{0}应于{1}月份年检", o.CarNumber, currentMonth);
                                SendSmsMessageBLL.SendAuditNotice(o.MemberId,o.MemberMobile, o.MemberName, content, o.LastCheckTime);
                            }
                            if (DateTime.Now.ToString("yyyy-MM-dd") == o.LastCheckTime.Value.AddYears(4).AddMonths(noticemonth).ToString("yyyy-MM") + "-" + noticeDay)
                            {
                                //发送短信
                                var content = string.Format("{0}应于{1}月份年检", o.CarNumber, currentMonth);
                                SendSmsMessageBLL.SendAuditNotice(o.MemberId, o.MemberMobile, o.MemberName, content, o.LastCheckTime);
                            }
                            if (DateTime.Now.ToString("yyyy-MM-dd") == o.LastCheckTime.Value.AddYears(6).AddMonths(noticemonth).ToString("yyyy-MM") + "-" + noticeDay)
                            {
                                //发送短信
                                var content = string.Format("{0}应于{1}月份年检", o.CarNumber, currentMonth);
                                SendSmsMessageBLL.SendAuditNotice(o.MemberId, o.MemberMobile, o.MemberName, content, o.LastCheckTime);
                            }
                        }
                    }

                    //运营（教练 客运） 类型 载客
                    if (o.UseType == (int)QX360.Model.Enums.UseType.营运 && o.CarType == (int)QX360.Model.Enums.CarType.微_小型客车 || o.UseType == (int)QX360.Model.Enums.UseType.营运 && o.CarType == (int)QX360.Model.Enums.CarType.中_大型客车)
                    {
                        if (DateTime.Now > o.RegisterTime.Value.AddYears(5))//大于5
                        {

                            if (DateTime.Now.ToString("yyyy-MM-dd") == Convert.ToDateTime(string.Format("{0}-{1}-{2}", currentYear, currentMonth, noticeDay)).AddMonths(noticemonth).ToString("yyyy-MM-dd") || DateTime.Now.ToString("yyyy-MM-dd") == Convert.ToDateTime(string.Format("{0}-{1}-{2}", currentYear, currentMonth, noticeDay)).AddMonths(6).AddMonths(noticemonth).ToString("yyyy-MM-dd"))
                            {
                                //发送短信
                                var content = string.Format("{0}应于{1}月份年检", o.CarNumber, currentMonth);
                                SendSmsMessageBLL.SendAuditNotice(o.MemberId, o.MemberMobile, o.MemberName, content, o.RegisterTime);
                            }
                        }
                        else
                        {
                            if (DateTime.Now.ToString("yyyy-MM-dd") == Convert.ToDateTime(string.Format("{0}-{1}-{2}", currentYear, currentMonth, noticeDay)).AddMonths(noticemonth).ToString("yyyy-MM-dd"))
                            {
                                //发送短信
                                var content = string.Format("{0}应于{1}月份年检", o.CarNumber, currentMonth);
                                SendSmsMessageBLL.SendAuditNotice(o.MemberId, o.MemberMobile, o.MemberName, content, o.RegisterTime);
                            }
                        }
                    }
                    //非运营/营运 类型 载货
                    if ((o.UseType == (int)QX360.Model.Enums.UseType.非营运 && o.CarType == (int)QX360.Model.Enums.CarType.货车1) || (o.UseType == (int)QX360.Model.Enums.UseType.非营运 && o.CarType == (int)QX360.Model.Enums.CarType.货车2) || (o.UseType == (int)QX360.Model.Enums.UseType.非营运 && o.CarType == (int)QX360.Model.Enums.CarType.低速车) || (o.UseType == (int)QX360.Model.Enums.UseType.非营运 && o.CarType == (int)QX360.Model.Enums.CarType.中_大型客车))
                    {

                        if (DateTime.Now > o.RegisterTime.Value.AddYears(10))//大于10
                        {
                            if (DateTime.Now.ToString("yyyy-MM-dd") == Convert.ToDateTime(string.Format("{0}-{1}-{2}", currentYear, currentMonth, noticeDay)).AddMonths(noticemonth).ToString("yyyy-MM-dd") || DateTime.Now.ToString("yyyy-MM-dd") == Convert.ToDateTime(string.Format("{0}-{1}-{2}", currentYear, currentMonth, noticeDay)).AddMonths(6).AddMonths(noticemonth).ToString("yyyy-MM-dd"))
                            {
                                //发送短信
                                var content = string.Format("{0}应于{1}月份年检", o.CarNumber, currentMonth);
                                SendSmsMessageBLL.SendAuditNotice(o.MemberId, o.MemberMobile, o.MemberName, content, o.RegisterTime);
                            }
                        }
                        else
                        {
                            if (DateTime.Now.ToString("yyyy-MM-dd") == Convert.ToDateTime(string.Format("{0}-{1}-{2}", currentYear, currentMonth, noticeDay)).AddMonths(noticemonth).ToString("yyyy-MM-dd"))
                            {
                                //发送短信
                                var content = string.Format("{0}应于{1}月份年检", o.CarNumber, currentMonth);
                                SendSmsMessageBLL.SendAuditNotice(o.MemberId, o.MemberMobile, o.MemberName, content, o.RegisterTime);
                            }
                        }
                    }
                    //营运
                    if ((o.UseType == (int)QX360.Model.Enums.UseType.营运 && o.CarType == (int)QX360.Model.Enums.CarType.货车1) || (o.UseType == (int)QX360.Model.Enums.UseType.营运 && o.CarType == (int)QX360.Model.Enums.CarType.货车2) || (o.UseType == (int)QX360.Model.Enums.UseType.营运 && o.CarType == (int)QX360.Model.Enums.CarType.低速车))
                    {

                        if (DateTime.Now > o.RegisterTime.Value.AddYears(10))//大于10
                        {
                            if (DateTime.Now.ToString("yyyy-MM-dd") == Convert.ToDateTime(string.Format("{0}-{1}-{2}", currentYear, currentMonth, noticeDay)).AddMonths(noticemonth).ToString("yyyy-MM-dd") || DateTime.Now.ToString("yyyy-MM-dd") == Convert.ToDateTime(string.Format("{0}-{1}-{2}", currentYear, currentMonth, noticeDay)).AddMonths(6).AddMonths(noticemonth).ToString("yyyy-MM-dd"))
                            {
                                //发送短信
                                var content = string.Format("{0}应于{1}月份年检", o.CarNumber, currentMonth);
                                SendSmsMessageBLL.SendAuditNotice(o.MemberId, o.MemberMobile, o.MemberName, content, o.RegisterTime);
                            }
                        }
                        else
                        {
                            if (DateTime.Now.ToString("yyyy-MM-dd") == Convert.ToDateTime(string.Format("{0}-{1}-{2}", currentYear, currentMonth, noticeDay)).AddMonths(noticemonth).ToString("yyyy-MM-dd"))
                            {
                                //发送短信
                                var content = string.Format("{0}应于{1}月份年检", o.CarNumber, currentMonth);
                                SendSmsMessageBLL.SendAuditNotice(o.MemberId, o.MemberMobile, o.MemberName, content, o.RegisterTime);
                            }
                        }
                    }
                    //校车 类型 校车
                    if (o.UseType == (int)QX360.Model.Enums.UseType.校车 && o.CarType == (int)QX360.Model.Enums.CarType.校车)
                    {
                        if (DateTime.Now.ToString("yyyy-MM-dd") == Convert.ToDateTime(string.Format("{0}-{1}-{2}", currentYear, currentMonth, noticeDay)).AddMonths(-1).ToString("yyyy-MM-dd") || DateTime.Now.ToString("yyyy-MM-dd") == Convert.ToDateTime(string.Format("{0}-{1}-{2}", currentYear, currentMonth, noticeDay)).AddMonths(6).AddMonths(-1).ToString("yyyy-MM-dd"))
                        {
                            //发送短信
                            var content = string.Format("{0}应于{1}月份年检", o.CarNumber, currentMonth);
                            SendSmsMessageBLL.SendAuditNotice(o.MemberId, o.MemberMobile, o.MemberName, content, o.RegisterTime);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                logger.Error("发送年检通知失败:" + ex.Message);
            }
        }
    }
}
