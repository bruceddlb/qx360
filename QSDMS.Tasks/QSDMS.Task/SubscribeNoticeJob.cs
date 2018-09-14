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
    /// 预约到期提醒 短信+微信
    /// 每天早上 7:00执行
    /// </summary>
    public class SubscribeNoticeJob : IJob
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SubscribeNoticeJob));
        int day = 0;
        public void Execute(IJobExecutionContext context)
        {
            var exprie = SettingsBLL.Instance.GetValue("notice_advanced_time");

            if (exprie != "")
            {
                day = int.Parse(exprie);
            }
            //处理当前的数据
            logger.Info("开始执行发送消息提醒:start....");
            //学车订单
            logger.Info("学车订单:start....");
            StudyOrder();
            logger.Info("学车订单:end....");
            //实训订单
            logger.Info("实训订单:start....");
            TraingOrder();
            logger.Info("实训订单:end....");
            //陪驾订单
            logger.Info("陪驾订单:start....");
            WithDrivingOrder();
            logger.Info("陪驾订单:end....");
            //年检订单
            logger.Info("年检订单:end....");
            AuditOrder();
            logger.Info("年检订单:end....");
            //代审订单
            logger.Info("代审订单:end....");
            TakeAuditOrder();
            logger.Info("代审订单:end....");
            //看车订单
            logger.Info("看车订单:end....");
            SeeCarOrder();
            logger.Info("看车订单:end....");
            //保险订单
            logger.Info("保险订单:end....");
            InsuranceOrder();
            logger.Info("保险订单:end....");
            logger.Info("开始执行操作:end....");
            logger.Info("执行发送消息提醒作业完成..时间:" + DateTime.Now);
        }

        /// <summary>
        /// 学车
        /// </summary>
        public void StudyOrder()
        {
            try
            {
                var studylist = StudyOrderBLL.Instance.GetList(new StudyOrderEntity()
                {
                    Status = (int)QX360.Model.Enums.StudySubscribeStatus.预约成功
                });
                if (studylist.Count > 0)
                {
                    studylist.ForEach((o) =>
                    {
                        //当前时间大于预约时间修改状态为完成状态
                        if (o.ServiceDate != null && o.ServiceTime != null)
                        {
                            DateTime servicetime = Convert.ToDateTime(Convert.ToDateTime(o.ServiceDate).ToString("yyyy-MM-dd").ToString());
                            //DateTime servicetime = Convert.ToDateTime(Convert.ToDateTime(o.ServiceDate).ToString("yyyy-MM-dd").ToString());

                            if (DateTime.Now > servicetime.AddDays(day))
                            {
                                //发送消息
                                string _servicetime = string.Format("{0} {1}", DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd"), o.ServiceTime.TrimEnd(','));
                                SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.学车预约提示, QX360.Model.Enums.NoticeType.预约提醒, o.MemberId, o.MemberName, _servicetime, "您的学车预约即将到期，请按时进行学车服务", o.StudyOrderNo);

                                //发送短信
                                SendSmsMessageBLL.SendSubricNotice(o.MemberId, o.MemberMobile, o.MemberName, _servicetime, "您的学车预约即将到期，请按时进行学车服务", o.StudyOrderNo);
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error("发送学车提示失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 实训
        /// </summary>
        public void TraingOrder()
        {
            try
            {
                var trainglist = TrainingOrderBLL.Instance.GetList(new TrainingOrderEntity()
                {
                    Status = (int)QX360.Model.Enums.PaySatus.已支付
                });
                if (trainglist.Count > 0)
                {
                    trainglist.ForEach((o) =>
                    {
                        //当前时间大于预约时间修改状态为完成状态
                        if (o.ServiceDate != null && o.ServiceTime != null)
                        {
                            DateTime servicetime = Convert.ToDateTime(Convert.ToDateTime(o.ServiceDate).ToString("yyyy-MM-dd").ToString());
                            if (DateTime.Now > servicetime.AddDays(day))
                            {
                                //发送消息
                                string _servicetime = string.Format("{0} {1}", DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd"), o.ServiceTime.TrimEnd(','));
                                SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.实训预约提示, QX360.Model.Enums.NoticeType.预约提醒, o.MemberId, o.MemberName, _servicetime, "您的实训预约即将到期，请按时进行实训服务", o.TrainingOrderNo);

                                //发送短信
                                SendSmsMessageBLL.SendSubricNotice(o.MemberId, o.MemberMobile, o.MemberName, _servicetime, "您的实训预约即将到期，请按时进行实训服务", o.TrainingOrderNo);
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error("发送实训提示失败:" + ex.Message);
            }

        }


        /// <summary>
        /// 陪驾
        /// </summary>
        public void WithDrivingOrder()
        {
            try
            {
                var withdrivinglist = WithDrivingOrderBLL.Instance.GetList(new WithDrivingOrderEntity()
                {
                    Status = (int)QX360.Model.Enums.PaySatus.已支付
                });
                if (withdrivinglist.Count > 0)
                {
                    withdrivinglist.ForEach((o) =>
                    {
                        //当前时间大于预约时间修改状态为完成状态
                        if (o.ServiceDate != null && o.ServiceTime != null)
                        {
                            DateTime servicetime = Convert.ToDateTime(Convert.ToDateTime(o.ServiceDate).ToString("yyyy-MM-dd").ToString());
                            if (DateTime.Now > servicetime.AddDays(day))
                            {
                                //发送消息
                                string _servicetime = string.Format("{0} {1}", DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd"), o.ServiceTime);
                                SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.陪驾预约提示, QX360.Model.Enums.NoticeType.预约提醒, o.MemberId, o.MemberName, _servicetime, "您的陪驾预约即将到期，请按时进行陪驾服务", o.DrivingOrderNo);

                                //发送短信
                                SendSmsMessageBLL.SendSubricNotice(o.MemberId, o.MemberMobile, o.MemberName, _servicetime, "您的陪驾预约即将到期，请按时进行陪驾服务", o.DrivingOrderNo);

                            }

                        }
                    });
                }
            }
            catch (Exception ex)
            {

                logger.Error("发送陪驾提示失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 年检
        /// </summary>
        public void AuditOrder()
        {
            try
            {
                var auditlist = AuditOrderBLL.Instance.GetList(new AuditOrderEntity()
                 {
                     Status = (int)QX360.Model.Enums.PaySatus.已支付
                 });
                if (auditlist.Count > 0)
                {
                    auditlist.ForEach((o) =>
                    {
                        //当前时间大于预约时间修改状态为完成状态
                        if (o.ServiceDate != null && o.ServiceTime != null)
                        {
                            DateTime servicetime = Convert.ToDateTime(Convert.ToDateTime(o.ServiceDate).ToString("yyyy-MM-dd").ToString());
                            if (DateTime.Now > servicetime.AddDays(day))
                            {
                                //发送消息
                                string _servicetime = string.Format("{0} {1}", DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd"), o.ServiceTime);
                                SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.年检预约提示, QX360.Model.Enums.NoticeType.预约提醒, o.MemberId, o.MemberName, _servicetime, "您的年检预约即将到期，请按时进行年检服务", o.AuditOrderNo);

                                //发送短信
                                SendSmsMessageBLL.SendSubricNotice(o.MemberId, o.Mobile, o.MemberName, _servicetime, "您的年检预约即将到期，请按时进行年检服务", o.AuditOrderNo);

                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {

                logger.Error("发送年检提示失败:" + ex.Message);
            }

            try
            {
                var auditlist = GroupAuditOrderBLL.Instance.GetList(new GroupAuditOrderEntity()
                 {
                     Status = (int)QX360.Model.Enums.PaySatus.已支付
                 });
                if (auditlist.Count > 0)
                {
                    auditlist.ForEach((o) =>
                    {
                        //当前时间大于预约时间修改状态为完成状态
                        if (o.ServiceDate != null && o.ServiceTime != null)
                        {
                            DateTime servicetime = Convert.ToDateTime(Convert.ToDateTime(o.ServiceDate).ToString("yyyy-MM-dd").ToString());
                            if (DateTime.Now > servicetime.AddDays(day))
                            {
                                //发送消息
                                string _servicetime = string.Format("{0} {1}", DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd"), o.ServiceTime);
                                SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.年检预约提示, QX360.Model.Enums.NoticeType.预约提醒, o.MemberId, o.MemberName, _servicetime, "您的年检预约即将到期，请按时进行年检服务", o.GroupAuditOrderNo);

                                //发送短信
                                SendSmsMessageBLL.SendSubricNotice(o.MemberId, o.Mobile, o.MemberName, _servicetime, "您的年检预约即将到期，请按时进行年检服务", o.GroupAuditOrderNo);

                            }

                        }
                    });
                }
            }
            catch (Exception ex)
            {

                logger.Error("发送集团年检提示失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 代审
        /// </summary>
        public void TakeAuditOrder()
        {
            try
            {
                var list = TakeAuditOrderBLL.Instance.GetList(new TakeAuditOrderEntity()
                 {
                     Status = (int)QX360.Model.Enums.PaySatus.已支付
                 });
                if (list.Count > 0)
                {
                    list.ForEach((o) =>
                    {
                        //当前时间大于预约时间修改状态为完成状态
                        if (o.ServiceDate != null && o.ServiceTime != null)
                        {
                            DateTime servicetime = Convert.ToDateTime(Convert.ToDateTime(o.ServiceDate).ToString("yyyy-MM-dd").ToString());
                            if (DateTime.Now > servicetime.AddDays(day))
                            {
                                //发送消息
                                string _servicetime = string.Format("{0} {1}", DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd"), o.ServiceTime);
                                SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.代审预约提示, QX360.Model.Enums.NoticeType.预约提醒, o.MemberId, o.MemberName, _servicetime, "您的代审预约即将到期，请按时进行代审服务", o.TakeAuditOrderNo);

                                //发送短信
                                SendSmsMessageBLL.SendSubricNotice(o.MemberId, o.Mobile, o.MemberName, _servicetime, "您的代审预约即将到期，请按时进行代审服务", o.TakeAuditOrderNo);

                            }

                        }
                    });
                }
            }
            catch (Exception ex)
            {

                logger.Error("发送代审提示失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 看车
        /// </summary>
        public void SeeCarOrder()
        {
            try
            {


                var list = SeeCarOrderBLL.Instance.GetList(new SeeCarOrderEntity()
                {
                    Status = (int)QX360.Model.Enums.SubscribeStatus.预约成功
                });
                if (list.Count > 0)
                {
                    list.ForEach((o) =>
                    {
                        //当前时间大于预约时间修改状态为完成状态
                        if (o.ServiceDate != null && o.ServiceTime != null)
                        {
                            DateTime servicetime = Convert.ToDateTime(Convert.ToDateTime(o.ServiceDate).ToString("yyyy-MM-dd").ToString());
                            if (DateTime.Now > servicetime.AddDays(day))
                            {
                                //发送消息
                                string _servicetime = string.Format("{0} {1}", DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd"), o.ServiceTime);
                                SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.看车预约提示, QX360.Model.Enums.NoticeType.预约提醒, o.MemberId, o.MemberName, _servicetime, "您的看车预约即将到期，请按时进行看车服务", o.SeeCarOrderNo);

                                //发送短信
                                SendSmsMessageBLL.SendSubricNotice(o.MemberId, o.MemberMobile, o.MemberName, _servicetime, "您的看车预约即将到期，请按时进行看车服务", o.SeeCarOrderNo);

                            }

                        }
                    });
                }
            }
            catch (Exception ex)
            {

                logger.Error("发送看车提示失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 保险
        /// </summary>
        public void InsuranceOrder()
        {
            try
            {
                var list = InsuranceOrderBLL.Instance.GetList(new InsuranceOrderEntity()
                {
                    Status = (int)QX360.Model.Enums.SubscribeStatus.预约成功
                });
                if (list.Count > 0)
                {
                    list.ForEach((o) =>
                    {
                        //当前时间大于预约时间修改状态为完成状态
                        if (o.ServiceDate != null && o.ServiceTime != null)
                        {
                            DateTime servicetime = Convert.ToDateTime(Convert.ToDateTime(o.ServiceDate).ToString("yyyy-MM-dd").ToString());
                            if (DateTime.Now > servicetime.AddDays(day))
                            {
                                //发送消息
                                string _servicetime = string.Format("{0} {1}", DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd"), o.ServiceTime);
                                SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.保险预约提示, QX360.Model.Enums.NoticeType.预约提醒, o.MemberId, o.MemberName, _servicetime, "您的保险预约即将到期，请按时进行保险服务", o.InsuranceOrderNo);

                                //发送短信
                                SendSmsMessageBLL.SendSubricNotice(o.MemberId, o.Mobile, o.MemberName, _servicetime, "您的保险预约即将到期，请按时进行保险服务", o.InsuranceOrderNo);

                            }

                        }
                    });
                }
            }
            catch (Exception ex)
            {

                logger.Error("发送保险提示失败:" + ex.Message);
            }
        }
    }
}
