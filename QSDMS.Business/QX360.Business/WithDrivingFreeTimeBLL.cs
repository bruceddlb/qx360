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

    public class WithDrivingFreeTimeBLL : BaseBLL<IWithDrivingFreeTimeService<WithDrivingFreeTimeEntity, WithDrivingFreeTimeEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static WithDrivingFreeTimeBLL m_Instance = new WithDrivingFreeTimeBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static WithDrivingFreeTimeBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "WithDrivingFreeTimeCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(WithDrivingFreeTimeEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<WithDrivingFreeTimeEntity> GetPageList(WithDrivingFreeTimeEntity para, ref Pagination pagination)
        {
            List<WithDrivingFreeTimeEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<WithDrivingFreeTimeEntity> GetList(WithDrivingFreeTimeEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(WithDrivingFreeTimeEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(WithDrivingFreeTimeEntity entity)
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
        public WithDrivingFreeTimeEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
        public void DeleteByObjectId(string objectid)
        {
            var list = this.GetList(new WithDrivingFreeTimeEntity() { WithDrivingFreeDateId = objectid });
            if (list != null)
            {
                foreach (var item in list)
                {
                    this.Delete(item.WithDrivingFreeTimeId);
                }
            }
        }

        /// <summary>
        /// 如果是选择了驾校对应插入学车时间信息
        /// </summary>
        /// <param name="monthworklist"></param>
        /// <param name="worktimelist"></param>
        public void AddInitFreeTime(string objectid)
        {
            DateTime firsttime = DateTime.Now;// Time.CalculateFirstDateOfWeek(DateTime.Now);
            DateTime endTime = DateTime.Now.AddDays(6); //Time.CalculateLastDateOfWeek(DateTime.Now);
            List<WithDrivingFreeDateEntity> monthworklist = new List<WithDrivingFreeDateEntity>();
            while (true)
            {
                var dateid = Util.NewUpperGuid();
                if (DateTime.Now.DayOfWeek == firsttime.DayOfWeek)
                {
                    monthworklist.Add(new WithDrivingFreeDateEntity() { WithDrivingFreeDateId = dateid, FreeDate = firsttime, IsCurrentDay = true, Week = Convert.ToInt32(firsttime.DayOfWeek) });
                }
                else
                {
                    monthworklist.Add(new WithDrivingFreeDateEntity() { WithDrivingFreeDateId = dateid, FreeDate = firsttime, IsCurrentDay = false, Week = Convert.ToInt32(firsttime.DayOfWeek) });
                }
                firsttime = firsttime.AddDays(1);
                if (firsttime > endTime)
                {
                    break;
                }
            }
            //时间
            //string[] names = System.Enum.GetNames(typeof(QX360.Model.Enums.WithDrivingTimeSpaceType));
            //int[] values = (int[])System.Enum.GetValues(typeof(QX360.Model.Enums.WithDrivingTimeSpaceType));
            //List<WithDrivingFreeTimeEntity> worktimelist = new List<WithDrivingFreeTimeEntity>();
            //for (int i = 0; i < names.Length; i++)
            //{
            //    worktimelist.Add(new WithDrivingFreeTimeEntity() { WorkTimeTableId = values[i].ToString(), TimeSection = names[i] });
            //}
            List<WithDrivingFreeTimeEntity> worktimelist = new List<WithDrivingFreeTimeEntity>();
            DataItemCache dataItemCache = new DataItemCache();
            var dataItemList = dataItemCache.GetDataItemList("pjsd");
            foreach (var dataitem in dataItemList)
            {
                worktimelist.Add(new WithDrivingFreeTimeEntity() { WorkTimeTableId = dataitem.ItemDetailId.ToString(), TimeSection = dataitem.ItemName, Remark = dataitem.Description, SortNum = dataitem.SortCode });
            }
            worktimelist = worktimelist.OrderBy(p => p.SortNum).ToList();
            foreach (var monthitem in monthworklist)
            {               
                //判断日期是否存在
                var list = WithDrivingFreeDateBLL.Instance.GetList(new WithDrivingFreeDateEntity()
                {
                    ObjectId = objectid,
                    StartTime = monthitem.FreeDate.ToString(),
                    EndTime = monthitem.FreeDate.ToString()
                });
                if (list != null && list.Count() > 0)
                {
                    //已存在的不再增加
                }
                else
                {
                    WithDrivingFreeDateEntity freedate = new WithDrivingFreeDateEntity();
                    freedate.WithDrivingFreeDateId = Util.NewUpperGuid();
                    freedate.ObjectId = objectid;
                    freedate.FreeDate = monthitem.FreeDate;
                    freedate.Week = monthitem.Week;
                    WithDrivingFreeDateBLL.Instance.Add(freedate);
                    foreach (var workitem in worktimelist)
                    {
                        WithDrivingFreeTimeEntity freetime = new WithDrivingFreeTimeEntity();
                        freetime.WithDrivingFreeTimeId = Util.NewUpperGuid();
                        freetime.WithDrivingFreeDateId = freedate.WithDrivingFreeDateId;
                        freetime.TimeSection = workitem.TimeSection;
                        freetime.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.空闲;
                        freetime.Remark = workitem.Remark;
                        freetime.WorkTimeTableId = workitem.WorkTimeTableId;
                        freetime.SortNum = workitem.SortNum;
                        this.Add(freetime);
                    }
                }
            }
        }
    }
}
