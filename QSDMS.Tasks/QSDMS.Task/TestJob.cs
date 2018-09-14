using log4net;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSDMS.Task
{
    public class TestJob : IJob
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(TestJob));
        public void Execute(IJobExecutionContext context)
        {
            //晚点12点后调用该任务，同步当前前时间前一天老系统订单
            var date = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            //处理当前的数据
            logger.Info("开始执行操作:start....");
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(i);
            }
            logger.Info("开始执行操作:end....");
        }
    }
}
