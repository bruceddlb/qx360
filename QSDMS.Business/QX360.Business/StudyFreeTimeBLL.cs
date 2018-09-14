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

    public class StudyFreeTimeBLL : BaseBLL<IStudyFreeTimeService<StudyFreeTimeEntity, StudyFreeTimeEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static StudyFreeTimeBLL m_Instance = new StudyFreeTimeBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static StudyFreeTimeBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "StudyFreeTimeCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(StudyFreeTimeEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<StudyFreeTimeEntity> GetPageList(StudyFreeTimeEntity para, ref Pagination pagination)
        {
            List<StudyFreeTimeEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<StudyFreeTimeEntity> GetList(StudyFreeTimeEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(StudyFreeTimeEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(StudyFreeTimeEntity entity)
        {
            return InstanceDAL.Update(entity);
        }

        public bool Delete(string keyValue)
        {
            return InstanceDAL.Delete(keyValue);
        }
        public void DeleteByObjectId(string objectid)
        {
            var list = this.GetList(new StudyFreeTimeEntity() { StudyFreeDateId = objectid });
            if (list != null)
            {
                foreach (var item in list)
                {
                    this.Delete(item.StudyFreeTimeId);
                }
            }
        }
        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public StudyFreeTimeEntity GetEntity(string keyValue)
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
        public void AddInitFreeTime(List<MonthWorkDayEntity> monthworklist, List<WorkTimeTableEntity> worktimelist, string objectid)
        {
            foreach (var monthitem in monthworklist)
            {
                //判断日期是否存在
                var list = StudyFreeDateBLL.Instance.GetList(new StudyFreeDateEntity()
                {
                    ObjectId = objectid,
                    StartTime = monthitem.WorkDay.ToString(),
                    EndTime = monthitem.WorkDay.ToString()
                });
                if (list != null && list.Count() > 0)
                {
                    //已存在的不再增加
                    //如果时间段有新增则更新
                    foreach (var workitem in worktimelist)
                    {
                        var studyworklist = StudyFreeTimeBLL.Instance.GetList(new StudyFreeTimeEntity()
                        {
                            WorkTimeTableId = workitem.WorkTimeTableId,
                            StudyFreeDateId = list.FirstOrDefault().StudyFreeDateId
                        });
                        if (studyworklist != null && studyworklist.Count() > 0)
                        {
                            //存在
                        }
                        else
                        {
                            StudyFreeTimeEntity freetime = new StudyFreeTimeEntity();
                            freetime.StudyFreeTimeId = Util.NewUpperGuid();
                            freetime.StudyFreeDateId = list.FirstOrDefault().StudyFreeDateId;
                            freetime.TimeSection = workitem.StartTime + "-" + workitem.EndTime;
                            freetime.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.空闲;
                            freetime.SortNum = workitem.SortNum;
                            freetime.TimeType = workitem.TimeType;
                            freetime.WorkTimeTableId = workitem.WorkTimeTableId;
                            StudyFreeTimeBLL.Instance.Add(freetime);
                        }
                    }
                }
                else
                {
                    StudyFreeDateEntity freedate = new StudyFreeDateEntity();
                    freedate.StudyFreeDateId = Util.NewUpperGuid();
                    freedate.ObjectId = objectid;
                    freedate.FreeDate = monthitem.WorkDay;
                    freedate.Week = monthitem.Week;
                    freedate.WeekName = monthitem.WeekName;
                    freedate.DateType = monthitem.DateType;
                    freedate.WorkdayItemId = monthitem.MonthWorkDayId;
                    StudyFreeDateBLL.Instance.Add(freedate);
                    //if (monthitem.DateType == (int)QX360.Model.Enums.DateType.工作日)
                    //{
                    foreach (var workitem in worktimelist)
                    {
                        StudyFreeTimeEntity freetime = new StudyFreeTimeEntity();
                        freetime.StudyFreeTimeId = Util.NewUpperGuid();
                        freetime.StudyFreeDateId = freedate.StudyFreeDateId;
                        freetime.TimeSection = workitem.StartTime + "-" + workitem.EndTime;
                        freetime.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.空闲;
                        freetime.SortNum = workitem.SortNum;
                        freetime.TimeType = workitem.TimeType;
                        freetime.WorkTimeTableId = workitem.WorkTimeTableId;
                        StudyFreeTimeBLL.Instance.Add(freetime);
                    }
                    //}
                }
            }
        }
    }
}
