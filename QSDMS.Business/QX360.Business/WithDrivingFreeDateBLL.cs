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

    public class WithDrivingFreeDateBLL : BaseBLL<IWithDrivingFreeDateService<WithDrivingFreeDateEntity, WithDrivingFreeDateEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static WithDrivingFreeDateBLL m_Instance = new WithDrivingFreeDateBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static WithDrivingFreeDateBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "WithDrivingFreeDateCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(WithDrivingFreeDateEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<WithDrivingFreeDateEntity> GetPageList(WithDrivingFreeDateEntity para, ref Pagination pagination)
        {
            List<WithDrivingFreeDateEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<WithDrivingFreeDateEntity> GetList(WithDrivingFreeDateEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(WithDrivingFreeDateEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(WithDrivingFreeDateEntity entity)
        {
            return InstanceDAL.Update(entity);
        }

        public bool Delete(string keyValue)
        {
            WithDrivingFreeTimeBLL.Instance.DeleteByObjectId(keyValue);
            return InstanceDAL.Delete(keyValue);
        }
        public void DeleteByObjectId(string objectid)
        {
            var list = this.GetList(new WithDrivingFreeDateEntity() { ObjectId = objectid });
            if (list != null)
            {
                foreach (var item in list)
                {
                    this.Delete(item.WithDrivingFreeDateId);
                }
            }
        }

        /// <summary>
        /// 根据时间删除操作
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public void DeleteByTime(string startTime, string endTime)
        {
            var list = this.GetList(new WithDrivingFreeDateEntity() { StartTime = startTime, EndTime = endTime });
            if (list != null)
            {
                foreach (var freedateitem in list)
                {
                    var freetimelist = WithDrivingFreeTimeBLL.Instance.GetList(new WithDrivingFreeTimeEntity() { WithDrivingFreeDateId = freedateitem.WithDrivingFreeDateId });
                    foreach (var freetimeitem in freetimelist)
                    {
                        WithDrivingFreeTimeBLL.Instance.Delete(freetimeitem.WithDrivingFreeTimeId);
                    }
                    this.Delete(freedateitem.WithDrivingFreeDateId);
                }
            }
        }

        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public WithDrivingFreeDateEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
    }
}
