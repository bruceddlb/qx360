using QSDMS.Util;
using QSDMS.Util.WebControl;
using QX360.Data.IServices;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Business
{

    public class TrainingFreeTimeBLL : BaseBLL<ITrainingFreeTimeService<TrainingFreeTimeEntity, TrainingFreeTimeEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TrainingFreeTimeBLL m_Instance = new TrainingFreeTimeBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TrainingFreeTimeBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "TrainingFreeTimeCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(TrainingFreeTimeEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<TrainingFreeTimeEntity> GetPageList(TrainingFreeTimeEntity para, ref Pagination pagination)
        {
            List<TrainingFreeTimeEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<TrainingFreeTimeEntity> GetList(TrainingFreeTimeEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(TrainingFreeTimeEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(TrainingFreeTimeEntity entity)
        {
            return InstanceDAL.Update(entity);
        }

        public bool Delete(string keyValue)
        {
            return InstanceDAL.Delete(keyValue);
        }
        public void DeleteByObjectId(string objectid)
        {
            var list = this.GetList(new TrainingFreeTimeEntity() { TrainingFreeDateId = objectid });
            if (list != null)
            {
                foreach (var item in list)
                {
                    this.Delete(item.TrainingFreeTimeId);
                }
            }
        }

        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public TrainingFreeTimeEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
        public int ClearData()
        {
            return InstanceDAL.ClearData();
        }

        /// <summary>
        /// 如果是选择了驾校对应插入学车时间信息
        /// </summary>
        /// <param name="monthworklist"></param>
        /// <param name="worktimelist"></param>
        public void AddInitFreeTime(List<MonthWorkDayEntity> monthworklist, List<TrainingTimeTableEntity> worktimelist, string objectid)
        {
            //DateTime firsttime = DateTime.Now;// Time.CalculateFirstDateOfWeek(DateTime.Now);
            //DateTime endTime = DateTime.Now.AddDays(6);// Time.CalculateLastDateOfWeek(DateTime.Now);
            //List<TrainingFreeDateEntity> monthworklist = new List<TrainingFreeDateEntity>();
            //while (true)
            //{
            //    var dateid = Util.NewUpperGuid();
            //    if (DateTime.Now.DayOfWeek == firsttime.DayOfWeek)
            //    {
            //        monthworklist.Add(new TrainingFreeDateEntity() { TrainingFreeDateId = dateid, FreeDate = firsttime, IsCurrentDay = true, Week = Convert.ToInt32(firsttime.DayOfWeek) });
            //    }
            //    else
            //    {
            //        monthworklist.Add(new TrainingFreeDateEntity() { TrainingFreeDateId = dateid, FreeDate = firsttime, IsCurrentDay = false, Week = Convert.ToInt32(firsttime.DayOfWeek) });
            //    }
            //    firsttime = firsttime.AddDays(1);
            //    if (firsttime > endTime)
            //    {
            //        break;
            //    }
            //}

            foreach (var monthitem in monthworklist)
            {

                //判断日期是否存在
                var list = TrainingFreeDateBLL.Instance.GetList(new TrainingFreeDateEntity()
                {
                    ObjectId = objectid,
                    StartTime = monthitem.WorkDay.ToString(),
                    EndTime = monthitem.WorkDay.ToString()
                });
                if (list != null && list.Count() > 0)
                {
                    //已存在的不再增加
                    foreach (var workitem in worktimelist)
                    {
                        var trainingworklist = TrainingFreeTimeBLL.Instance.GetList(new TrainingFreeTimeEntity()
                        {
                            TrainingTimeTableId = workitem.TrainingTimeTableId,
                            TrainingFreeDateId = list.FirstOrDefault().TrainingFreeDateId
                        });
                        if (trainingworklist != null && trainingworklist.Count() > 0)
                        {
                            //存在
                        }
                        else
                        {

                            TrainingFreeTimeEntity freetime = new TrainingFreeTimeEntity();
                            freetime.TrainingFreeTimeId = Util.NewUpperGuid();
                            freetime.TrainingFreeDateId = list.FirstOrDefault().TrainingFreeDateId;
                            freetime.TimeSection = workitem.StartTime + "-" + workitem.EndTime;
                            freetime.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.空闲;
                            freetime.SortNum = workitem.SortNum;
                            freetime.TrainingTimeTableId = workitem.TrainingTimeTableId;
                            TrainingFreeTimeBLL.Instance.Add(freetime);
                        }
                    }
                }
                else
                {
                    TrainingFreeDateEntity freedate = new TrainingFreeDateEntity();
                    freedate.TrainingFreeDateId = Util.NewUpperGuid();
                    freedate.ObjectId = objectid;
                    freedate.FreeDate = monthitem.WorkDay;
                    freedate.Week = monthitem.Week;
                    freedate.DateType = monthitem.DateType;
                    freedate.WorkdayItemId = monthitem.MonthWorkDayId;
                    TrainingFreeDateBLL.Instance.Add(freedate);
                    foreach (var workitem in worktimelist)
                    {
                        TrainingFreeTimeEntity freetime = new TrainingFreeTimeEntity();
                        freetime.TrainingFreeTimeId = Util.NewUpperGuid();
                        freetime.TrainingFreeDateId = freedate.TrainingFreeDateId;
                        freetime.TimeSection = workitem.StartTime + "-" + workitem.EndTime;
                        freetime.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.空闲;
                        freetime.SortNum = workitem.SortNum;
                        freetime.TrainingTimeTableId = workitem.TrainingTimeTableId;
                        TrainingFreeTimeBLL.Instance.Add(freetime);
                    }
                }
            }
        }
    }
}
