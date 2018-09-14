
using iFramework.Business;
using QSDMS.Data.IService;
using QSDMS.Model;
using QSDMS.Util.WebControl;
using System;
using System.Collections.Generic;

namespace QSDMS.Business
{
    /// <summary>   
    /// 描 述：编号规则
    /// </summary>
    public class CodeRuleBLL : BaseBLL<ICodeRuleService>
    {
        /// <summary>
        /// 访问实例
        /// </summary>
        public static CodeRuleBLL m_Instance = new CodeRuleBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static CodeRuleBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public CodeRuleBLL() { }

        #region 获取数据
        /// <summary>
        /// 规则列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<CodeRuleEntity> GetPageList(Pagination pagination, string queryJson)
        {
            return InstanceDAL.GetPageList(pagination, queryJson);
        }
        /// <summary>
        /// 规则实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public CodeRuleEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 规则编号不能重复
        /// </summary>
        /// <param name="enCode">编号</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistEnCode(string enCode, string keyValue)
        {
            return InstanceDAL.ExistEnCode(enCode, keyValue);
        }
        /// <summary>
        /// 规则名称不能重复
        /// </summary>
        /// <param name="fullName">名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistFullName(string fullName, string keyValue)
        {
            return InstanceDAL.ExistFullName(fullName, keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除规则
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            try
            {
                InstanceDAL.RemoveForm(keyValue);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 保存规则表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="codeRuleEntity">规则实体</param>
        /// <returns></returns>
        public void SaveForm(string keyValue, CodeRuleEntity codeRuleEntity)
        {
            try
            {
                //调用单据编码示例               
                InstanceDAL.SaveForm(keyValue, codeRuleEntity);

            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region 单据编码处理
        /// <summary>
        /// 获得指定模块或者编号的单据号
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="moduleId">模块ID</param>
        /// <param name="enCode">模板编码</param>
        /// <returns>单据号</returns>
        public string GetBillCode(string userId, string moduleId, string enCode = "")
        {
            return InstanceDAL.GetBillCode(userId, moduleId, enCode);
        }
        /// <summary>
        /// 占用单据号
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="moduleId">模块ID</param>
        /// <param name="enCode">模板编码</param>
        /// <returns>true/false</returns>
        public bool UseRuleSeed(string userId, string moduleId, string enCode)
        {
            return InstanceDAL.UseRuleSeed(userId, moduleId, enCode);
        }
        /// <summary>
        /// 获得指定模块或者编号的单据号（直接使用）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="moduleId">模块ID</param>
        /// <param name="enCode">模板编码</param>
        /// <returns>单据号</returns>
        public string SetBillCode(string userId, string moduleId, string enCode)
        {
            return InstanceDAL.SetBillCode(userId, moduleId, enCode);
        }
        #endregion
    }
}
