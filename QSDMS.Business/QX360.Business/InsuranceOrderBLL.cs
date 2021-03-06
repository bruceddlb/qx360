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

    public class InsuranceOrderBLL : BaseBLL<IInsuranceOrderService<InsuranceOrderEntity, InsuranceOrderEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static InsuranceOrderBLL m_Instance = new InsuranceOrderBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static InsuranceOrderBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "InsuranceOrderCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(InsuranceOrderEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<InsuranceOrderEntity> GetPageList(InsuranceOrderEntity para, ref Pagination pagination)
        {
            List<InsuranceOrderEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<InsuranceOrderEntity> GetList(InsuranceOrderEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(InsuranceOrderEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(InsuranceOrderEntity entity)
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
        public InsuranceOrderEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }

        public string GetOrderNo() {
            return InstanceDAL.GetOrderNo();
        }
    }
}
