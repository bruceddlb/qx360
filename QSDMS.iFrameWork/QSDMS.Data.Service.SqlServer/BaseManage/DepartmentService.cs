
using QSDMS.Data.IService;
using QSDMS.Model;
using QSDMS.Util;
using QSDMS.Util.Extension;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace QSDMS.Data.Service.SqlServer
{
    /// <summary> 
    /// 描 述：部门管理
    /// </summary>
    public class DepartmentService : IDepartmentService
    {
        #region 获取数据
        /// <summary>
        /// 部门列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DepartmentEntity> GetList()
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_Department where 1=1 ");

            sql.Append(" order by CreateDate desc");
            var list = Base_Department.Query(sql);
            return EntityConvertTools.CopyToList<Base_Department, DepartmentEntity>(list.ToList());
        }
        /// <summary>
        /// 部门实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public DepartmentEntity GetEntity(string keyValue)
        {
            var model = Base_Department.SingleOrDefault("where DepartmentId=@0", keyValue);
            return EntityConvertTools.CopyToModel<Base_Department, DepartmentEntity>(model, null);
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 部门编号不能重复
        /// </summary>
        /// <param name="enCode">编号</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistEnCode(string enCode, string keyValue)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_Department where 1=1 ");
            if (!string.IsNullOrEmpty(enCode))
            {
                sql.Append(" and EnCode=@0", enCode);
            }
            if (!string.IsNullOrEmpty(keyValue))
            {
                sql.Append(" and DepartmentId!=@0", keyValue);
            }
            return Base_Department.Query(sql).Count() == 0 ? true : false;
        }
        /// <summary>
        /// 部门名称不能重复
        /// </summary>
        /// <param name="fullName">名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistFullName(string fullName, string keyValue)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_Department where 1=1 ");
            if (!string.IsNullOrEmpty(fullName))
            {
                sql.Append(" and FullName=@0", fullName);
            }
            if (!string.IsNullOrEmpty(keyValue))
            {
                sql.Append(" and DepartmentId!=@0", keyValue);
            }
            return Base_Department.Query(sql).Count() == 0 ? true : false;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            int count = QSDMS_SQLDB.GetInstance().Fetch<Base_Department>("select * from Base_Department").FindAll(t => t.ParentId == keyValue).Count();
            if (count > 0)
            {
                throw new Exception("当前所选数据有子节点数据！");
            }
            Base_Department.Delete("where DepartmentId=@0", keyValue);
        }
        /// <summary>
        /// 保存部门表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="departmentEntity">机构实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, DepartmentEntity departmentEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                departmentEntity.Modify(keyValue);
                Base_Department depart = Base_Department.SingleOrDefault("where DepartmentId=@0", keyValue);
                depart = EntityConvertTools.CopyToModel<DepartmentEntity, Base_Department>(departmentEntity, depart);
                depart.DepartmentId = keyValue;
                depart.Update();
            }
            else
            {
                departmentEntity.Create();
                Base_Department depart = EntityConvertTools.CopyToModel<DepartmentEntity, Base_Department>(departmentEntity, null);
                depart.Insert();

            }
        }
        #endregion
    }
}
