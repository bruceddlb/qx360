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

    public class MonthWorkDayBLL : BaseBLL<IMonthWorkDayService<MonthWorkDayEntity, MonthWorkDayEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static MonthWorkDayBLL m_Instance = new MonthWorkDayBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static MonthWorkDayBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "MonthWorkDayCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(MonthWorkDayEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<MonthWorkDayEntity> GetPageList(MonthWorkDayEntity para, ref Pagination pagination)
        {
            List<MonthWorkDayEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<MonthWorkDayEntity> GetList(MonthWorkDayEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(MonthWorkDayEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(MonthWorkDayEntity entity)
        {
            return InstanceDAL.Update(entity);
        }

        public bool Delete(string keyValue)
        {
            return InstanceDAL.Delete(keyValue);
        }
        public void DeleteByObjectId(string objectid)
        {
            var list = this.GetList(new MonthWorkDayEntity() { ObjectId = objectid });
            if (list != null)
            {
                foreach (var item in list)
                {
                    this.Delete(item.MonthWorkDayId);
                }
            }
        }
        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public MonthWorkDayEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }

        /// <summary>
        /// 修改时间同步业务数据
        /// </summary>
        /// <param name="objecttype"></param>
        /// <param name="workDay"></param>
        public void SynBusData(int? objectType, int? dateType, DateTime? workDay, string workdayItemId)
        {
            switch (objectType)
            {
                case 1://驾校
                    var stufrredatelist = StudyFreeDateBLL.Instance.GetList(new StudyFreeDateEntity()
                    {
                        StartTime = Convert.ToDateTime(workDay.ToString()).ToString("yyyy-MM-dd"),
                        EndTime = Convert.ToDateTime(workDay.ToString()).ToString("yyyy-MM-dd"),
                        WorkdayItemId = workdayItemId
                    });
                    stufrredatelist.ForEach((o) =>
                    {
                        o.DateType = dateType;
                        StudyFreeDateBLL.Instance.Update(o);
                    });
                    break;
                case 2://年审
                    var auditdatelist = AuditFreeDateBLL.Instance.GetList(new AuditFreeDateEntity()
                    {
                        StartTime = Convert.ToDateTime(workDay.ToString()).ToString("yyyy-MM-dd"),
                        EndTime = Convert.ToDateTime(workDay.ToString()).ToString("yyyy-MM-dd"),
                        WorkdayItemId = workdayItemId
                    });
                    auditdatelist.ForEach((o) =>
                    {
                        o.DateType = dateType;
                        AuditFreeDateBLL.Instance.Update(o);
                    });
                    break;
                case 3://保险
                    //var insurancedatelist = AuditFreeDateBLL.Instance.GetList(new AuditFreeDateEntity()
                    //{
                    //    StartTime = Convert.ToDateTime(workDay.ToString()).ToString("yyyy-MM-dd"),
                    //    EndTime = Convert.ToDateTime(workDay.ToString()).ToString("yyyy-MM-dd"),
                    //});
                    //auditdatelist.ForEach((o) =>
                    //{
                    //    o.DateType = dateType;
                    //    AuditFreeDateBLL.Instance.Update(o);
                    //});
                    break;
                case 4://考场
                    var examplacedatelist = TrainingFreeDateBLL.Instance.GetList(new TrainingFreeDateEntity()
                    {
                        StartTime = Convert.ToDateTime(workDay.ToString()).ToString("yyyy-MM-dd"),
                        EndTime = Convert.ToDateTime(workDay.ToString()).ToString("yyyy-MM-dd"),
                        WorkdayItemId = workdayItemId
                    });
                    examplacedatelist.ForEach((o) =>
                   {
                       o.DateType = dateType;
                       TrainingFreeDateBLL.Instance.Update(o);
                   });
                    break;
                default:
                    break;
            }
        }
    }
}
