using Common.Logging;
using QSDMS.Business.Cache;
using QSDMS.Util;
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
    public class StrandedJob : IJob
    {
        private string name = string.Empty;
        public void Execute() {
            Console.WriteLine("当前时间: {0},下一次触发时间：{1}", DateTime.Now, "");
            
        }
        public void Execute(IJobExecutionContext context)
        {
           // Console.WriteLine("当前时间: {0},下一次触发时间：{1}", DateTime.Now, context.NextFireTimeUtc);

            var teacherList = TeacherBLL.Instance.GetList(new TeacherEntity() { Status = (int)QX360.Model.Enums.UseStatus.启用 });
            teacherList.ForEach((o) =>
            {
                //星期
                List<FreeDateEntity> freedateList = GetCurrentWeekList();
                freedateList.ForEach((freedate) =>
                {
                    freedate.FreeDateId = Util.NewUpperGuid();
                    freedate.ObjectId = o.TeacherId;
                    freedate.ObjectType = 2;
                    FreeDateBLL.Instance.Add(freedate);
                    //插入时间
                    List<KeyValueEntity> freetimeList = GetNormalTimeList();
                    for (int i = 0; i < freetimeList.Count; i++)
                    {
                        if ((i + 1) < freetimeList.Count)
                        {
                            FreeTimeEntity time = new FreeTimeEntity();
                            time.FreeTimeId = Util.NewUpperGuid();
                            time.FreeDateId = freedate.FreeDateId;
                            time.StartTime = freetimeList[i].ItemName.ToString();
                            time.EndTime = freetimeList[i + 1].ItemName.ToString();
                            time.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.空闲;
                            FreeTimeBLL.Instance.Add(time);
                        }
                    }
                });

            });
            System.Threading.Thread.Sleep(100);
        }

        /// <summary>
        /// 查询滞留件信息
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// 获取当前一周日期
        /// </summary>
        /// <returns></returns>
        public List<FreeDateEntity> GetCurrentWeekList()
        {
            DateTime firsttime = QSDMS.Util.Time.CalculateFirstDateOfWeek(DateTime.Now);
            DateTime endTime = QSDMS.Util.Time.CalculateLastDateOfWeek(DateTime.Now);
            List<FreeDateEntity> list = new List<FreeDateEntity>();
            while (true)
            {
                var dateid = QSDMS.Util.Util.NewUpperGuid();
                if (DateTime.Now.DayOfWeek == firsttime.DayOfWeek)
                {
                    list.Add(new FreeDateEntity() { FreeDateId = dateid, FreeDate = firsttime, IsCurrentDay = true, Week = Convert.ToInt32(firsttime.DayOfWeek), WeekName = QSDMS.Util.Time.GetChineseWeekDay(firsttime) });
                }
                else
                {
                    list.Add(new FreeDateEntity() { FreeDateId = dateid, FreeDate = firsttime, IsCurrentDay = false, Week = Convert.ToInt32(firsttime.DayOfWeek), WeekName = QSDMS.Util.Time.GetChineseWeekDay(firsttime) });
                }
                firsttime = firsttime.AddDays(1);
                if (firsttime > endTime)
                {
                    break;
                }
            }
            return list;
        }

        /// <summary>
        /// 获取时间段
        /// </summary>
        /// <returns></returns>
        public List<KeyValueEntity> GetNormalTimeList()
        {
            DataItemCache dataItemCache = new DataItemCache();
            var data = dataItemCache.GetDataItemList("sjd");
            List<KeyValueEntity> list = new List<KeyValueEntity>();
            if (data != null)
            {
                var itemdataList = data.OrderBy(i => i.SortCode).ThenBy(i => i.SortCode).ToList();
                itemdataList.ForEach((o) =>
                {
                    list.Add(new KeyValueEntity() { ItemId = o.ItemDetailId.ToString(), ItemName = o.ItemName });
                });
            }
            return list;
        }
    }
}
