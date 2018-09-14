

using Common.Logging;
using QSDMS.Util;
using QSMS.API.WCFAppSms;
using Quartz;
using Quartz.Impl;
using QX360.Business;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrandedSchedulerApp
{
    class Program
    {

        static void Main(string[] args)
        {
            LogManager.Adapter = new Common.Logging.Simple.TraceLoggerFactoryAdapter() { Level = LogLevel.All };
            try
            {
                //Dictionary<string,string> dic=new Dictionary<string,string>();
                //dic.Add("18602707864","asdasdasd,asdasdjasdjasdasd002020ajsdjasd阿什阿什啊是大的阿斯顿阿斯顿");
                //WCFAppSmsHelper.SendSms(dic);
                //UpdateFinishStatusJob aa = new UpdateFinishStatusJob();
                //aa.Execute(null);
                //ISchedulerFactory schedFact = new StdSchedulerFactory();
                //IScheduler scheduler = schedFact.GetScheduler();
                //scheduler.Start();       //开启调度器
                //IJobDetail job1 = JobBuilder.Create<StrandedJob>()
                //                     .WithIdentity("myJob", "group1")
                //                     .UsingJobData("jobSays", "Hello World!")
                //                     .Build();
                //ITrigger trigger1 = TriggerBuilder.Create()
                //                            .WithIdentity("mytrigger", "group1")
                //                            .StartNow()
                //                            .WithCronSchedule(Config.GetValue("JobExecTime"))    //时间表达式，5秒一次     /5 * * ? * *
                //                            .Build();
                //scheduler.ScheduleJob(job1, trigger1);
                string time = "2018-05-01";
                DataTable dt = new DataTable();
                //构建表头
                dt.Columns.Add("Time", typeof(string));
                var traningcarlist = TrainingCarBLL.Instance.GetList(new TrainingCarEntity()
                {
                    SchoolId = "E07B085877904C99B5546A02352ED196",
                    TrainingType = 1
                }).OrderBy((o) => o.SortNum).ToList();
                traningcarlist.ForEach((o) =>
                {
                    dt.Columns.Add("car_" + o.TrainingCarId, typeof(string));
                });
                //构建内容
                List<string> timelist = new List<string>();
                //查询预约时间 时间已一个车的设置时间
                var fistcar = traningcarlist.First();
                //查询预约的日期,根据日期查询对应的时间段，原则上一个车对应日期只有一条记录
                var freedatelist = TrainingFreeDateBLL.Instance.GetList(new TrainingFreeDateEntity() { ObjectId = fistcar.TrainingCarId, StartTime = time, EndTime = time });
                if (freedatelist != null)
                {

                    //查询时间段 
                    //系统时间段
                    var freetimelist = TrainingFreeTimeBLL.Instance.GetList(new TrainingFreeTimeEntity()
                    {
                        TrainingFreeDateId = freedatelist.First().TrainingFreeDateId
                    }).OrderBy((o) => o.SortNum);
                    //自定义的时间段
                    var custorfreetimelist = TrainingCustomFreeTimeBLL.Instance.GetList(new TrainingCustomFreeTimeEntity()
                    {
                        TrainingFreeDateId = freedatelist.First().TrainingFreeDateId
                    }).OrderBy((o) => o.SortNum);
                    freetimelist.Foreach((o) =>
                    {
                        timelist.Add(o.TimeSection);
                    });
                    custorfreetimelist.Foreach((o) =>
                    {
                        timelist.Add(o.TimeSection);
                    });
                }
                //已当前的时间为基准构建数据
                foreach (var item in timelist)
                {
                    DataRow dr = dt.NewRow();
                    dr["Time"] = item;
                    traningcarlist.ForEach((o) =>
                    {
                        string txt = "";
                        //查询对应车辆的预约信息
                        //根据时间段和日期id查询对应的时间段id，这里有系统的和自定义的
                        var _freedate = TrainingFreeDateBLL.Instance.GetList(new TrainingFreeDateEntity()
                        {
                            StartTime = time,
                            EndTime = time,
                            ObjectId = o.TrainingCarId
                        }).FirstOrDefault();
                        if (_freedate != null)
                        {
                            var _freetimeid = "";
                            //查询时间段
                            var _freetime = TrainingFreeTimeBLL.Instance.GetList(new TrainingFreeTimeEntity()
                            {
                                TrainingFreeDateId = _freedate.TrainingFreeDateId,
                                TimeSection = item
                            }).FirstOrDefault();
                            if (_freetime != null)
                            {
                                _freetimeid = _freetime.TrainingFreeTimeId;

                            }
                            var _cusfreetime = TrainingCustomFreeTimeBLL.Instance.GetList(new TrainingCustomFreeTimeEntity()
                            {
                                TrainingFreeDateId = _freedate.TrainingFreeDateId,
                                TimeSection = item
                            }).FirstOrDefault();
                            if (_cusfreetime != null)
                            {
                                _freetimeid = _cusfreetime.TrainingCustomFreeTimeId;

                            }
                            if (_freetimeid != "")
                            {
                                //查询订单明细表
                                var orderdetail = TrainingOrderDetailBLL.Instance.GetList(new TrainingOrderDetailEntity()
                                {
                                    TrainingFreeTimeId = _freetimeid
                                }).FirstOrDefault();
                                if (orderdetail != null)
                                {
                                    var order = TrainingOrderBLL.Instance.GetEntity(orderdetail.TrainingOrderId);
                                    if (order != null)
                                    {
                                        txt = string.Format("预约信息：{0},{1},订单号{2}", order.MemberName, order.MemberMobile, order.TrainingOrderNo);
                                    }
                                }
                            }

                        }
                        dr["car_" + o.TrainingCarId] = txt;
                    });
                    dt.Rows.Add(dr);
                }

                DataTable dt2 = dt.Clone();
                DataRow[] rows = GetTableRows(dt, 1, 2);
                dt2.Rows.Clear();
                foreach (DataRow row in rows)
                {
                    dt2.Rows.Add(row.ItemArray);
                }
            }
            catch (SchedulerException se)
            {
                Console.WriteLine(se);
            }

            Console.WriteLine("滞留件消息定时任务正在运行...");
            Console.ReadKey();
        }

        public static DataRow[] GetTableRows(DataTable dtAllEas, int PageIndex, int PageSize)
        {
            var rows = dtAllEas.Rows.Cast<DataRow>();
            var curRows = rows.Skip(PageIndex).Take(PageSize).ToArray();
            return curRows;
        }
    }
}
