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

    public class FreeDateBLL : BaseBLL<IFreeDateService<FreeDateEntity, FreeDateEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static FreeDateBLL m_Instance = new FreeDateBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static FreeDateBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "FreeDateCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(FreeDateEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<FreeDateEntity> GetPageList(FreeDateEntity para, ref Pagination pagination)
        {
            List<FreeDateEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<FreeDateEntity> GetList(FreeDateEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(FreeDateEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(FreeDateEntity entity)
        {
            return InstanceDAL.Update(entity);
        }

        public bool Delete(string keyValue)
        {
            var freetimelist = FreeTimeBLL.Instance.GetList(new FreeTimeEntity() { FreeDateId = keyValue });
            foreach (var item in freetimelist)
            {
                FreeTimeBLL.Instance.Delete(item.FreeTimeId);
            }
            return InstanceDAL.Delete(keyValue);
        }
        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public FreeDateEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }

        public int ClearData()
        {
            return InstanceDAL.ClearData();
        }

        public void DeleteByObjectId(string objectid)
        {
            var list = this.GetList(new FreeDateEntity() { ObjectId = objectid });
            if (list != null)
            {
                foreach (var item in list)
                {
                    this.Delete(item.FreeDateId);
                }
            }
        }
    }
}
