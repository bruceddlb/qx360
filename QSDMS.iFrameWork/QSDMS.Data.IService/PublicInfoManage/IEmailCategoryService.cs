
using QSDMS.Model;
using System.Collections.Generic;

namespace QSDMS.Data.IService
{
    /// <summary>   
    /// 描 述：邮件分类
    /// </summary>
    public interface IEmailCategoryService
    {
        #region 获取数据
        /// <summary>
        /// 分类列表
        /// </summary>
        /// <param name="UserId">用户Id</param>
        /// <returns></returns>
        IEnumerable<EmailCategoryEntity> GetList(string UserId);
        /// <summary>
        /// 分类实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        EmailCategoryEntity GetEntity(string keyValue);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        /// <summary>
        /// 保存分类表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="emailCategoryEntity">分类实体</param>
        /// <returns></returns>
        void SaveForm(string keyValue, EmailCategoryEntity emailCategoryEntity);
        #endregion
    }
}
