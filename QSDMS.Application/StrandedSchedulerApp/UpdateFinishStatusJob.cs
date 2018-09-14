using Common.Logging;
using Quartz;
using QX360.Business;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrandedSchedulerApp
{
    /// <summary>
    /// 系统自动修改订单状态为完成状态
    /// 修改用户未取消 和预约时间过期的订单
    /// </summary>
    public class UpdateFinishStatusJob : IJob
    {
        //private static readonly ILog logger = LogManager.GetLogger(typeof(UpdateFinishStatusJob));
        int noticemonth = -2;//提前3个月
        string noticeDay = SettingsBLL.Instance.GetValue("audit_notice_time") == "" ? "1" : SettingsBLL.Instance.GetValue("audit_notice_time");

        public void Execute(IJobExecutionContext context)
        {
            var txt = string.Format("车牌号：{0},上次年检时间：{1}", "aaa", "2018-11-11");
            txt = "车牌号：1111【上次年检时间：111111啊实打实】";
             //SendSmsMessageBLL.SendAuditNotice("18602707864", "asdasd", "aa",DateTime.Now);
            //SendSmsMessageBLL.SendWCFRegisterSms("18602707864", "110000");
            //AuditNotice();
            var content = string.Format("{0}应于{1}月份年检", "ew1000", 1);
            SendSmsMessageBLL.SendAuditNotice("","18602707864", "", content, null);
        }
      

    }
}
