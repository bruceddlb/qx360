using QSDMS.Model;
using QSDMS.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSDMS.Data.IService
{
    public interface IUserRoleService
    {
        #region 获取数据
        /// <summary>
        /// 用户角色列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<UserRoleEntity> GetList();

        /// <summary>
        /// 获取用户角色
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        IEnumerable<UserRoleEntity> GetRoleList(string userid);

        /// <summary>
        /// 用户角色列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        IEnumerable<UserRoleEntity> GetPageList(Pagination pagination, string queryJson);


        /// <summary>
        /// 用户角色实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        UserRoleEntity GetEntity(string keyValue);

        #endregion

        #region 提交数据
        /// <summary>
        /// 删除用户角色
        /// </summary>
        /// <param name="keyValue">主键</param>
        void RemoveForm(string keyValue);
        /// <summary>
        /// 保存用户角色表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="userRoleEntity"></param>
        /// <returns></returns>
        void SaveForm(string keyValue, UserRoleEntity userRoleEntity);

        #endregion
    }
}
