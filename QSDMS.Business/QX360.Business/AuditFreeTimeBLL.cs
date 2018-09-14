using QSDMS.Business.Cache;
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

    public class AuditFreeTimeBLL : BaseBLL<IAuditFreeTimeService<AuditFreeTimeEntity, AuditFreeTimeEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static AuditFreeTimeBLL m_Instance = new AuditFreeTimeBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static AuditFreeTimeBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "AuditFreeTimeCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(AuditFreeTimeEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<AuditFreeTimeEntity> GetPageList(AuditFreeTimeEntity para, ref Pagination pagination)
        {
            List<AuditFreeTimeEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<AuditFreeTimeEntity> GetList(AuditFreeTimeEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(AuditFreeTimeEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(AuditFreeTimeEntity entity)
        {
            return InstanceDAL.Update(entity);
        }

        public bool Delete(string keyValue)
        {
            return InstanceDAL.Delete(keyValue);
        }
        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public AuditFreeTimeEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
        public void DeleteByObjectId(string objectid)
        {
            var list = this.GetList(new AuditFreeTimeEntity() { AuditFreeDateId = objectid });
            if (list != null)
            {
                foreach (var item in list)
                {
                    this.Delete(item.AuditFreeTimeId);
                }
            }
        }

        /// <summary>
        /// 如果是选择了驾校对应插入学车时间信息
        /// </summary>
        /// <param name="monthworklist"></param>
        /// <param name="worktimelist"></param>
        public void AddInitFreeTime(List<MonthWorkDayEntity> monthworklist, string objectid)
        {
            //工作时间段
            string[] names = System.Enum.GetNames(typeof(QX360.Model.Enums.TimeSpaceType));
            int[] values = (int[])System.Enum.GetValues(typeof(QX360.Model.Enums.TimeSpaceType));
            List<AuditFreeTimeEntity> worktimelist = new List<AuditFreeTimeEntity>();
            for (int i = 0; i < names.Length; i++)
            {
                worktimelist.Add(new AuditFreeTimeEntity() { WorkTimeTableId = values[i].ToString(), TimeSection = names[i], SortNum = values[i] });
            }
            worktimelist = worktimelist.OrderBy(p => p.SortNum).ToList();
            ///日期
            foreach (var monthitem in monthworklist)
            {
                //判断日期是否存在
                var list = AuditFreeDateBLL.Instance.GetList(new AuditFreeDateEntity()
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
                        var auditworklist = AuditFreeTimeBLL.Instance.GetList(new AuditFreeTimeEntity()
                        {
                            WorkTimeTableId = workitem.WorkTimeTableId,
                            AuditFreeDateId = list.FirstOrDefault().AuditFreeDateId
                        });
                        if (auditworklist != null && auditworklist.Count() > 0)
                        {
                            //存在
                        }
                        else
                        {
                            AuditFreeTimeEntity freetime = new AuditFreeTimeEntity();
                            freetime.AuditFreeTimeId = Util.NewUpperGuid();
                            freetime.AuditFreeDateId = list.FirstOrDefault().AuditFreeDateId;
                            freetime.TimeSection = workitem.StartTime + "-" + workitem.EndTime;
                            freetime.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.空闲;
                            freetime.SortNum = workitem.SortNum;
                            freetime.WorkTimeTableId = workitem.WorkTimeTableId;
                            AuditFreeTimeBLL.Instance.Add(freetime);
                        }
                    }
                }
                else
                {
                    AuditFreeDateEntity freedate = new AuditFreeDateEntity();
                    freedate.AuditFreeDateId = Util.NewUpperGuid();
                    freedate.ObjectId = objectid;
                    freedate.FreeDate = monthitem.WorkDay;
                    freedate.Week = monthitem.Week;
                    freedate.WeekName = monthitem.WeekName;
                    freedate.DateType = monthitem.DateType;
                    freedate.WorkdayItemId = monthitem.MonthWorkDayId;
                    AuditFreeDateBLL.Instance.Add(freedate);
                    foreach (var workitem in worktimelist)
                    {
                        AuditFreeTimeEntity freetime = new AuditFreeTimeEntity();
                        freetime.AuditFreeTimeId = Util.NewUpperGuid();
                        freetime.AuditFreeDateId = freedate.AuditFreeDateId;
                        freetime.TimeSection = workitem.TimeSection;
                        freetime.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.空闲;
                        freetime.SortNum = workitem.SortNum;
                        freetime.WorkTimeTableId = workitem.WorkTimeTableId;
                        AuditFreeTimeBLL.Instance.Add(freetime);
                    }
                }
            }
        }
    }
}
