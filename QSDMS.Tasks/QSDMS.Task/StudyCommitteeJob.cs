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
    /// 自动评价服务
    /// 
    /// </summary>
    public class StudyCommitteeJob : IJob
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(StudyCommitteeJob));
        int day = 0;
        public void Execute(IJobExecutionContext context)
        {
            var exprie = SettingsBLL.Instance.GetValue("sys_commite_time");

            if (exprie != "")
            {
                day = int.Parse(exprie);
            }
            //处理当前的数据
            logger.Info("开始执行自动评价:start....");
            //学车订单
            logger.Info("学车订单:start....");
            StudyCommittee();
            logger.Info("学车订单:end....");

            logger.Info("开始执行自动评价:end....");
            logger.Info("开始执行自动评价..时间:" + DateTime.Now);
        }
        /// <summary>
        /// 学车
        /// </summary>
        public void StudyCommittee()
        {
            try
            {
                var studylist = StudyOrderBLL.Instance.GetList(new StudyOrderEntity()
                {
                    Status = (int)QX360.Model.Enums.StudySubscribeStatus.预约完成
                });
                if (studylist.Count > 0)
                {
                    studylist.ForEach((o) =>
                    {
                        //
                        if (o.ServiceDate != null && o.ServiceTime != null)
                        {
                            DateTime servicetime = Convert.ToDateTime(Convert.ToDateTime(o.ServiceDate).ToString("yyyy-MM-dd").ToString());
                            if (DateTime.Now > servicetime.AddDays(day))
                            {
                                StudyCommitteeEntity entity = new StudyCommitteeEntity();
                                entity.StudyCommitteeId = Util.Util.NewUpperGuid();
                                entity.TeacherId = o.TeacherId;
                                entity.TeacherName = o.TeacherName;
                                entity.MemberId = o.MemberId;
                                entity.MemberName = o.MemberName;
                                entity.MemberMobile = o.MemberMobile;
                                entity.CommitTime = DateTime.Now;
                                entity.CommitLev = 5;
                                entity.CommitContent = "默认好评";
                                StudyCommitteeBLL.Instance.Add(entity);

                                o.StudyOrderId = o.StudyOrderId;
                                o.Status = (int)QX360.Model.Enums.StudySubscribeStatus.学员评价;
                                StudyOrderBLL.Instance.Update(o);


                            }

                        }
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error("自动评价学车提示失败:" + ex.Message);
            }
        }

    }
}
