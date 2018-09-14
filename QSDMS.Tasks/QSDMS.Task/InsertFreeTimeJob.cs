using Common.Logging;
using QSDMS.Business.Cache;
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
    /// 自动处理预约空闲时间
    /// 每天凌晨 1:10处理
    /// </summary>
    public class InsertFreeTimeJob : IJob
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(InsertFreeTimeJob));
        public void Execute(IJobExecutionContext context)
        {
            //处理当前的数据
            logger.Info("开始执行处理空闲时间操作:start....");

            //驾校教练空闲时间
            logger.Info("驾校教练空闲时间:start....");
            //删除数据
            //StudyFreeDateBLL.Instance.DeleteByTime(DateTime.Now.AddDays(-15).ToString(), DateTime.Now.AddDays(-7).ToString());
            //WithDrivingFreeDateBLL.Instance.DeleteByTime(DateTime.Now.AddDays(-15).ToString(), DateTime.Now.AddDays(-7).ToString());
            //TrainingFreeDateBLL.Instance.DeleteByTime(DateTime.Now.AddDays(-15).ToString(), DateTime.Now.AddDays(-7).ToString());
            //驾校教练空闲时间
            var teacherList = TeacherBLL.Instance.GetList(new TeacherEntity() { Status = (int)QX360.Model.Enums.UseStatus.启用 });
            foreach (var o in teacherList)
            {
                //未归属驾校
                if (o.SchoolId == null || o.SchoolId == "-1")
                {
                    continue;
                }
                DateTime firsttime = DateTime.Now; //Util.Time.CalculateFirstDateOfWeek(DateTime.Now);
                DateTime endTime = DateTime.Now.AddDays(6); //Util.Time.CalculateLastDateOfWeek(DateTime.Now);
                var monthworklist = MonthWorkDayBLL.Instance.GetList(new MonthWorkDayEntity()
                {
                    ObjectId = o.SchoolId,
                    StartTime = firsttime.ToString(),
                    EndTime = endTime.ToString()

                }).OrderBy(p => p.WorkDay).ToList();
                if (monthworklist.Count() == 0)
                {
                    logger.Error("驾校：[" + o.SchoolName + "]未设置当月工作日时间");
                    continue;
                }
                var worktimelist = WorkTimeTableBLL.Instance.GetList(new WorkTimeTableEntity()
                {
                    SchoolId = o.SchoolId,

                }).OrderBy(p => p.SortNum).ToList();
                if (worktimelist.Count() == 0)
                {
                    logger.Error("驾校：[" + o.SchoolName + "]未设置该驾校学车时段");
                    continue;
                }
                //如果教练对应有驾校则初始化配置时间信息
                StudyFreeTimeBLL.Instance.AddInitFreeTime(monthworklist, worktimelist, o.TeacherId);

            }
            logger.Info("驾校教练空闲时间:end....");

            //陪驾空闲时间设置
            logger.Info("陪驾空闲时间:start....");
            var withdrivingteacherList = TeacherBLL.Instance.GetList(new TeacherEntity() { Status = (int)QX360.Model.Enums.UseStatus.启用, IsWithDriving = 1 });
            foreach (var o in withdrivingteacherList)
            {
                WithDrivingFreeTimeBLL.Instance.AddInitFreeTime(o.TeacherId);
            }

            logger.Info("陪驾空闲时间:end....");

            //实训车空闲时间
            logger.Info("实训车空闲时间:start....");
            var traingcarList = TrainingCarBLL.Instance.GetList(new TrainingCarEntity() { Status = (int)QX360.Model.Enums.UseStatus.启用 });
            foreach (var o in traingcarList)
            {
                DateTime firsttime = DateTime.Now; //Util.Time.CalculateFirstDateOfWeek(DateTime.Now);
                DateTime endTime = DateTime.Now.AddDays(6); //Util.Time.CalculateLastDateOfWeek(DateTime.Now);
                var monthworklist = MonthWorkDayBLL.Instance.GetList(new MonthWorkDayEntity()
                {
                    ObjectId = o.SchoolId,
                    StartTime = firsttime.ToString(),
                    EndTime = endTime.ToString()

                }).OrderBy(p => p.WorkDay).ToList();
                if (monthworklist.Count() == 0)
                {
                    logger.Error("考场：[" + o.SchoolName + "]未设置当月工作日时间");
                    continue;
                }
                var worktimelist = TrainingTimeTableBLL.Instance.GetList(new TrainingTimeTableEntity()
                {
                    SchoolId = o.SchoolId

                }).OrderBy(p => p.SortNum).ToList();
                if (worktimelist.Count() == 0)
                {
                    logger.Error("驾校：[" + o.SchoolName + "]未设置该驾校实训时段");
                    continue;
                }
                TrainingFreeTimeBLL.Instance.AddInitFreeTime(monthworklist, worktimelist, o.TrainingCarId);
            }

            logger.Info("实训车空闲时间:end....");

            logger.Info("机构工作时间:start....");
            var auditList = AuditOrganizationBLL.Instance.GetList(new AuditOrganizationEntity() { Status = (int)QX360.Model.Enums.UseStatus.启用 });
            foreach (var o in auditList)
            {
                DateTime firsttime = DateTime.Now; //Util.Time.CalculateFirstDateOfWeek(DateTime.Now);
                DateTime endTime = DateTime.Now.AddDays(6); //Util.Time.CalculateLastDateOfWeek(DateTime.Now);
                var monthworklist = MonthWorkDayBLL.Instance.GetList(new MonthWorkDayEntity()
                {
                    ObjectId = o.OrganizationId,
                    StartTime = firsttime.ToString(),
                    EndTime = endTime.ToString()

                }).OrderBy(p => p.WorkDay).ToList();
                if (monthworklist.Count() == 0)
                {
                    logger.Error("年检机构：[" + o.Name + "]未设置当月工作日时间");
                    continue;
                }

                AuditFreeTimeBLL.Instance.AddInitFreeTime(monthworklist, o.OrganizationId);
            }
            logger.Info("机构工作时间:end....");

            logger.Info("执行处理空闲时间操作end....时间：" + DateTime.Now);
        }
    }
}
