﻿using QSDMS.Util.WebControl;
using QX360.Data.IServices;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Business
{

    public class AuditOrderBLL : BaseBLL<IAuditOrderService<AuditOrderEntity, AuditOrderEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static AuditOrderBLL m_Instance = new AuditOrderBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static AuditOrderBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "AuditOrderCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(AuditOrderEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<AuditOrderEntity> GetPageList(AuditOrderEntity para, ref Pagination pagination)
        {
            List<AuditOrderEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<AuditOrderEntity> GetList(AuditOrderEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(AuditOrderEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(AuditOrderEntity entity)
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
        public AuditOrderEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }

        public string GetOrderNo()
        {
            return InstanceDAL.GetOrderNo();
        }
    }
}
