using log4net;
using Quartz;
using QX360.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSDMS.Task
{
    public class ClearDataJob : IJob
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CarAuditNoticeJob));
        public void Execute(IJobExecutionContext context)
        {
            //处理当前的数据
            logger.Info("删除前三个月的实训预约时间:start....");
            DeleteTraningFreeDate();
            logger.Info("删除前三个月的实训预约时间:end....");
            logger.Info("删除前三个月的实训预约时间..时间:" + DateTime.Now);

            logger.Info("删除前三个月的学车预约时间:start....");
            DeleteStudyFreeDate();
            logger.Info("删除前三个月的学车预约时间:end....");
            logger.Info("删除前三个月的学车预约时间..时间:" + DateTime.Now);
        }

        /// <summary>
        /// 删除前三个月的实训预约时间
        /// </summary>
        public void DeleteTraningFreeDate()
        {
            try
            {
                TrainingFreeDateBLL.Instance.ClearData();
            }
            catch (Exception ex)
            {
                logger.Error("删除前三个月的实训预约时间失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 删除前三个月的学车预约时间
        /// </summary>
        public void DeleteStudyFreeDate()
        {
            try
            {
                StudyFreeDateBLL.Instance.ClearData();
            }
            catch (Exception ex)
            {
                logger.Error("删除前三个月的学车预约时间失败:" + ex.Message);
            }
        }
    }
}
