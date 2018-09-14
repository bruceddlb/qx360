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

    public class SettingsBLL : BaseBLL<ISettingsService<SettingsEntity, SettingsEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static SettingsBLL m_Instance = new SettingsBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static SettingsBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "SettingsCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(SettingsEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<SettingsEntity> GetPageList(SettingsEntity para, ref Pagination pagination)
        {
            List<SettingsEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<SettingsEntity> GetList(SettingsEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(SettingsEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(SettingsEntity entity)
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
        public SettingsEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetValue(string name)
        {
            var list = this.GetList(new SettingsEntity() { Name = name });
            if (list != null && list.Count > 0)
            {
                return list.FirstOrDefault().Value;
            }
            return "";
        }

        /// <summary>
        /// 获取备注
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetRemark(string name)
        {
            var list = this.GetList(new SettingsEntity() { Name = name });
            if (list != null && list.Count > 0)
            {
                return list.FirstOrDefault().Remark;
            }
            return "";
        }
    }
}
