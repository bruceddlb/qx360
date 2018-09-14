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
    /// 系统自动修改订单状态为完成状态
    /// 修改用户未取消 和预约时间过期的订单
    /// 每天晚上 23:00处理
    /// </summary>
    public class UpdateFinishStatusJob : IJob
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(UpdateFinishStatusJob));
        public void Execute(IJobExecutionContext context)
        {
            //处理当前的数据
            logger.Info("开始执行修改订单状态操作:start....");
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
            logger.Info("修改预约单状态作业完成..时间:" + DateTime.Now);
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
                            //DateTime servicetime = Convert.ToDateTime(Convert.ToDateTime(o.ServiceDate).ToString("yyyy-MM-dd").ToString() + " " + o.ServiceTime.Split(',')[0].ToString().Split('-')[0]);
                            DateTime servicetime = Convert.ToDateTime(Convert.ToDateTime(o.ServiceDate).ToString("yyyy-MM-dd").ToString());
                            if (servicetime < DateTime.Now)
                            {
                                o.StudyOrderId = o.StudyOrderId;
                                o.Status = (int)QX360.Model.Enums.StudySubscribeStatus.预约完成;
                                StudyOrderBLL.Instance.Update(o);
                                //发送消息
                                string _servicetime = string.Format("{0} {1}", DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd"), o.ServiceTime.TrimEnd(','));
                                SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.学车预约提示, QX360.Model.Enums.NoticeType.完成通知, o.MemberId,o.MemberName, _servicetime, "预约学车已完成,请前往人车APP对本次服务进行评价", o.StudyOrderNo);

                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error("处理学车订单失败:" + ex.Message);
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
                    Status = (int)QX360.Model.Enums.PaySatus.待支付
                });
                if (trainglist.Count > 0)
                {
                    trainglist.ForEach((o) =>
                    {
                        //当前时间大于预约时间修改状态为完成状态
                        if (o.ServiceDate != null && o.ServiceTime != null)
                        {
                            DateTime servicetime = Convert.ToDateTime(Convert.ToDateTime(o.ServiceDate).ToString("yyyy-MM-dd").ToString());
                            if (servicetime < DateTime.Now)
                            {
                                o.TrainingOrderId = o.TrainingOrderId;
                                o.Status = (int)QX360.Model.Enums.PaySatus.已取消;
                                TrainingOrderBLL.Instance.Update(o);
                                //发送消息
                                string _servicetime = string.Format("{0} {1}", DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd"), o.ServiceTime.TrimEnd(','));
                                if (o.UserType == (int)QX360.Model.Enums.TrainingUserType.学员)
                                {
                                    SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.实训预约提示, QX360.Model.Enums.NoticeType.取消提醒, o.MemberId, o.MemberName, _servicetime, "实训预约未支付，系统已自动取消,请重新预约", o.TrainingOrderNo);
                                }
                                else if (o.UserType == (int)QX360.Model.Enums.TrainingUserType.教练)
                                {
                                    SendSysMessageBLL.SendMessageTeacher(QX360.Model.Enums.NoticeType.预约提醒, o.MemberId, _servicetime, "实训预约未支付，系统已自动取消,请重新预约", o.TrainingOrderNo);
                                }
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error("处理实训取消订单失败:" + ex.Message);
            }

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
                            if (servicetime < DateTime.Now)
                            {
                                o.TrainingOrderId = o.TrainingOrderId;
                                o.Status = (int)QX360.Model.Enums.PaySatus.已完成;
                                TrainingOrderBLL.Instance.Update(o);
                                //发送消息
                                string _servicetime = string.Format("{0} {1}", DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd"), o.ServiceTime.TrimEnd(','));
                                if (o.UserType == (int)QX360.Model.Enums.TrainingUserType.学员)
                                {
                                    SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.实训预约提示, QX360.Model.Enums.NoticeType.完成通知, o.MemberId, o.MemberName, _servicetime, "实训预约已完成，祝您生活愉快", o.TrainingOrderNo);
                                }
                                else if (o.UserType == (int)QX360.Model.Enums.TrainingUserType.教练)
                                {
                                    SendSysMessageBLL.SendMessageTeacher(QX360.Model.Enums.NoticeType.预约提醒, o.MemberId, _servicetime, "实训预约已完成，祝您生活愉快", o.TrainingOrderNo);
                                }
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {

                logger.Error("处理实训完成订单失败:" + ex.ToString() + ",StackTrace:" + ex.StackTrace);
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
                    Status = (int)QX360.Model.Enums.PaySatus.待支付
                });
                if (withdrivinglist.Count > 0)
                {
                    withdrivinglist.ForEach((o) =>
                    {
                        //当前时间大于预约时间修改状态为完成状态
                        if (o.ServiceDate != null && o.ServiceTime != null)
                        {
                            DateTime servicetime = Convert.ToDateTime(Convert.ToDateTime(o.ServiceDate).ToString("yyyy-MM-dd").ToString());
                            if (servicetime < DateTime.Now)
                            {
                                o.DrivingOrderId = o.DrivingOrderId;
                                o.Status = (int)QX360.Model.Enums.PaySatus.已取消;
                                WithDrivingOrderBLL.Instance.Update(o);
                                //发送消息
                                string _servicetime = string.Format("{0} {1}", DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd"), o.ServiceTime);
                                SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.陪驾预约提示, QX360.Model.Enums.NoticeType.取消提醒, o.MemberId, o.MemberName, _servicetime, "陪驾预约未支付，系统已自动取消,请重新预约", o.DrivingOrderNo);

                            }

                        }
                    });
                }
            }
            catch (Exception ex)
            {

                logger.Error("处理陪驾取消订单失败:" + ex.Message);
            }
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
                            if (servicetime < DateTime.Now)
                            {
                                o.DrivingOrderId = o.DrivingOrderId;
                                o.Status = (int)QX360.Model.Enums.PaySatus.已完成;
                                WithDrivingOrderBLL.Instance.Update(o);
                                //发送消息
                                string _servicetime = string.Format("{0} {1}", DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd"), o.ServiceTime);
                                SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.陪驾预约提示, QX360.Model.Enums.NoticeType.完成通知, o.MemberId, o.MemberName, _servicetime, "陪驾预约已完成，祝您生活愉快", o.DrivingOrderNo);

                            }

                        }
                    });
                }
            }
            catch (Exception ex)
            {

                logger.Error("处理陪驾完成订单失败:" + ex.Message);
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
                    Status = (int)QX360.Model.Enums.PaySatus.待支付
                });
                if (auditlist.Count > 0)
                {
                    auditlist.ForEach((o) =>
                    {
                        //当前时间大于预约时间修改状态为完成状态
                        if (o.ServiceDate != null && o.ServiceTime != null)
                        {
                            DateTime servicetime = Convert.ToDateTime(Convert.ToDateTime(o.ServiceDate).ToString("yyyy-MM-dd").ToString());
                            if (servicetime < DateTime.Now)
                            {
                                o.AuditOrderId = o.AuditOrderId;
                                o.Status = (int)QX360.Model.Enums.PaySatus.已取消;
                                AuditOrderBLL.Instance.Update(o);
                                //发送消息
                                string _servicetime = string.Format("{0} {1}", DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd"), o.ServiceTime);
                                SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.年检预约提示, QX360.Model.Enums.NoticeType.取消提醒, o.MemberId, o.MemberName, _servicetime, "年检预约未支付，系统已自动取消,请重新预约", o.AuditOrderNo);

                            }

                        }
                    });
                }
            }
            catch (Exception ex)
            {

                logger.Error("处理年检取消订单失败:" + ex.Message);
            }

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
                            if (servicetime < DateTime.Now)
                            {
                                o.AuditOrderId = o.AuditOrderId;
                                o.Status = (int)QX360.Model.Enums.PaySatus.已完成;
                                AuditOrderBLL.Instance.Update(o);
                                //发送消息
                                string _servicetime = string.Format("{0} {1}", DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd"), o.ServiceTime);
                                SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.年检预约提示, QX360.Model.Enums.NoticeType.完成通知, o.MemberId, o.MemberName, _servicetime, "年检预约已完成，请前往人车APP对本次服务进行评价", o.AuditOrderNo);

                            }

                        }
                    });
                }
            }
            catch (Exception ex)
            {

                logger.Error("处理年检完成订单失败:" + ex.Message);
            }

            try
            {
                var auditlist = GroupAuditOrderBLL.Instance.GetList(new GroupAuditOrderEntity()
                {
                    Status = (int)QX360.Model.Enums.PaySatus.待支付
                });
                if (auditlist.Count > 0)
                {
                    auditlist.ForEach((o) =>
                    {
                        //当前时间大于预约时间修改状态为完成状态
                        if (o.ServiceDate != null && o.ServiceTime != null)
                        {
                            DateTime servicetime = Convert.ToDateTime(Convert.ToDateTime(o.ServiceDate).ToString("yyyy-MM-dd").ToString());
                            if (servicetime < DateTime.Now)
                            {
                                o.GroupAuditOrderId = o.GroupAuditOrderId;
                                o.Status = (int)QX360.Model.Enums.PaySatus.已取消;
                                GroupAuditOrderBLL.Instance.Update(o);
                                //发送消息
                                string _servicetime = string.Format("{0} {1}", DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd"), o.ServiceTime);
                                SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.年检预约提示, QX360.Model.Enums.NoticeType.取消提醒, o.MemberId, o.MemberName, _servicetime, "年检预约未支付，系统已自动取消,请重新预约", o.GroupAuditOrderNo);

                            }

                        }
                    });
                }
            }
            catch (Exception ex)
            {

                logger.Error("处理集团年检取消订单失败:" + ex.Message);
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
                            if (servicetime < DateTime.Now)
                            {
                                o.GroupAuditOrderId = o.GroupAuditOrderId;
                                o.Status = (int)QX360.Model.Enums.PaySatus.已完成;
                                GroupAuditOrderBLL.Instance.Update(o);
                                //发送消息
                                string _servicetime = string.Format("{0} {1}", DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd"), o.ServiceTime);
                                SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.年检预约提示, QX360.Model.Enums.NoticeType.完成通知, o.MemberId, o.MemberName, _servicetime, "年检预约已完成，请前往人车APP对本次服务进行评价", o.GroupAuditOrderNo);

                            }

                        }
                    });
                }
            }
            catch (Exception ex)
            {

                logger.Error("处理集团年检完成订单失败:" + ex.Message);
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
                    Status = (int)QX360.Model.Enums.PaySatus.待支付
                });
                if (list.Count > 0)
                {
                    list.ForEach((o) =>
                    {
                        //当前时间大于预约时间修改状态为完成状态
                        if (o.ServiceDate != null && o.ServiceTime != null)
                        {

                            DateTime servicetime = Convert.ToDateTime(Convert.ToDateTime(o.ServiceDate).ToString("yyyy-MM-dd").ToString());
                            if (servicetime < DateTime.Now)
                            {
                                o.TakeAuditOrderId = o.TakeAuditOrderId;
                                o.Status = (int)QX360.Model.Enums.PaySatus.已取消;
                                TakeAuditOrderBLL.Instance.Update(o);
                                //发送消息
                                string _servicetime = string.Format("{0} {1}", DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd"), o.ServiceTime);
                                SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.代审预约提示, QX360.Model.Enums.NoticeType.取消提醒, o.MemberId, o.MemberName, _servicetime, "代审预约未支付，系统已自动取消,请重新预约", o.TakeAuditOrderNo);

                            }

                        }
                    });
                }
            }
            catch (Exception ex)
            {

                logger.Error("处理代审取消订单失败:" + ex.Message);
            }
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
                            if (servicetime < DateTime.Now)
                            {
                                o.TakeAuditOrderId = o.TakeAuditOrderId;
                                o.Status = (int)QX360.Model.Enums.PaySatus.已完成;
                                TakeAuditOrderBLL.Instance.Update(o);
                                //发送消息
                                string _servicetime = string.Format("{0} {1}", DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd"), o.ServiceTime);
                                SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.代审预约提示, QX360.Model.Enums.NoticeType.完成通知, o.MemberId, o.MemberName, _servicetime, "代审预约已完成，请前往人车APP对本次服务进行评价", o.TakeAuditOrderNo);

                            }

                        }
                    });
                }
            }
            catch (Exception ex)
            {

                logger.Error("处理代审完成订单失败:" + ex.Message);
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
                            if (servicetime < DateTime.Now)
                            {
                                o.SeeCarOrderId = o.SeeCarOrderId;
                                o.Status = (int)QX360.Model.Enums.SubscribeStatus.已完成;
                                SeeCarOrderBLL.Instance.Update(o);
                                //发送消息
                                string _servicetime = string.Format("{0} {1}", DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd"), o.ServiceTime);
                                SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.看车预约提示, QX360.Model.Enums.NoticeType.完成通知, o.MemberId, o.MemberName, _servicetime, "看车预约已完成，祝您生活愉快", o.SeeCarOrderNo);

                            }

                        }
                    });
                }
            }
            catch (Exception ex)
            {

                logger.Error("处理看车完成订单失败:" + ex.Message);
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
                            if (servicetime < DateTime.Now)
                            {
                                o.InsuranceOrderId = o.InsuranceOrderId;
                                o.Status = (int)QX360.Model.Enums.SubscribeStatus.已完成;
                                InsuranceOrderBLL.Instance.Update(o);
                                //发送消息
                                string _servicetime = string.Format("{0} {1}", DateTime.Parse(o.ServiceDate.ToString()).ToString("yyyy-MM-dd"), o.ServiceTime);
                                SendSysMessageBLL.SendMessage(QX360.Model.Enums.MessageAlterType.保险预约提示, QX360.Model.Enums.NoticeType.完成通知, o.MemberId, o.MemberName, _servicetime, "保险预约已完成，祝您生活愉快", o.InsuranceOrderNo);

                            }

                        }
                    });
                }
            }
            catch (Exception ex)
            {

                logger.Error("处理保险完成订单失败:" + ex.Message);
            }
        }
    }
}
