
using QSDMS.Cache.Factory;
using QSDMS.Data.IService;
using QSDMS.Model;
using QSDMS.Util;
using QSDMS.Util.WebControl;
using QSDMS.Util.Extension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using QSDMS.Util.Excel;
using iFramework.Business;
using iFramework.Framework.Security;

namespace QSDMS.Business
{
    /// <summary>   
    /// 描 述：用户管理
    /// </summary>
    public class UserBLL : BaseBLL<IUserService>
    {
        /// <summary>
        /// 访问实例
        /// </summary>
        public static UserBLL m_Instance = new UserBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static UserBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public UserBLL() { }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "userCache";

        #region 获取数据

        /// <summary>
        /// 用户列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserEntity> GetList()
        {
            return InstanceDAL.GetList();
        }

        /// <summary>
        /// 获取部门下面用户
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserEntity> GetDepartmentUserList()
        {
            return InstanceDAL.GetDepartmentUserList();
        }
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<UserEntity> GetPageList(Pagination pagination, UserEntity para)
        {
            return InstanceDAL.GetPageList(pagination, para);
        }

        /// <summary>
        /// 用户实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public UserEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 账户不能重复
        /// </summary>
        /// <param name="account">账户值</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistAccount(string account, string keyValue = "")
        {
            return InstanceDAL.ExistAccount(account, keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            try
            {
                InstanceDAL.RemoveForm(keyValue);
                CacheFactory.Cache().RemoveCache(cacheKey);
                UpdateIMUserList(keyValue, false, null);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 保存用户表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="userEntity">用户实体</param>
        /// <returns></returns>
        public string SaveForm(string keyValue, UserEntity userEntity)
        {
            try
            {
                keyValue = InstanceDAL.SaveForm(keyValue, userEntity);
                CacheFactory.Cache().RemoveCache(cacheKey);
                //UpdateIMUserList(keyValue, true, userEntity);
                return keyValue;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 修改用户登录密码
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="Password">新密码（MD5 小写）</param>
        public void RevisePassword(string keyValue, string password)
        {
            try
            {
                InstanceDAL.RevisePassword(keyValue, password);
                CacheFactory.Cache().RemoveCache(cacheKey);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 修改用户状态
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="State">状态：1-启动；0-禁用</param>
        public void UpdateState(string keyValue, int State)
        {
            try
            {
                InstanceDAL.UpdateState(keyValue, State);
                CacheFactory.Cache().RemoveCache(cacheKey);
                if (State == 0)
                {
                    UpdateIMUserList(keyValue, false, null);
                }
                else
                {
                    UserEntity entity = InstanceDAL.GetEntity(keyValue);
                    UpdateIMUserList(keyValue, true, entity);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public UserEntity CheckLogin(string username, string password)
        {
            UserEntity userEntity = InstanceDAL.CheckLogin(username);
            if (userEntity != null)
            {
                if (userEntity.EnabledMark == 1)
                {
                    string dbPassword = Md5Helper.MD5(DESEncrypt.Encrypt(password.ToLower(), userEntity.Secretkey).ToLower(), 32).ToLower();
                    if (dbPassword == userEntity.Password)
                    {
                        DateTime LastVisit = DateTime.Now;
                        int LogOnCount = (userEntity.LogOnCount).ToInt() + 1;
                        if (userEntity.LastVisit != null)
                        {
                            userEntity.PreviousVisit = userEntity.LastVisit.ToDate();
                        }
                        userEntity.LastVisit = LastVisit;
                        userEntity.LogOnCount = LogOnCount;
                        userEntity.UserOnLine = 1;
                        InstanceDAL.UpdateEntity(userEntity);
                        return userEntity;
                    }
                    else
                    {
                        throw new Exception("密码和账户名不匹配");
                    }
                }
                else
                {
                    throw new Exception("账户名被系统锁定,请联系管理员");
                }
            }
            else
            {
                throw new Exception("账户不存在，请重新输入");
            }
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="userEntity">实体对象</param>
        public void UpdateEntity(UserEntity userEntity)
        {
            InstanceDAL.UpdateEntity(userEntity);
        }

        /// <summary>
        /// 更新实时通信用户列表
        /// </summary>
        private void UpdateIMUserList(string keyValue, bool isAdd, UserEntity userEntity)
        {
            try
            {
                //IMUserModel entity = new IMUserModel();
                //OrganizeBLL bll = new OrganizeBLL();
                //DepartmentBLL dbll = new DepartmentBLL();
                //entity.UserId = keyValue;
                //if (userEntity != null)
                //{
                //    entity.RealName = userEntity.RealName;
                //    entity.DepartmentId = dbll.GetEntity(userEntity.DepartmentId).FullName;
                //    entity.Gender = (int)userEntity.Gender;
                //    entity.HeadIcon = userEntity.HeadIcon;
                //    entity.OrganizeId = bll.GetEntity(userEntity.OrganizeId).FullName; ;
                //}
                //SendHubs.callMethod("upDateUserList", entity, isAdd);
            }
            catch
            {

            }
        }
        #endregion

        #region 处理数据
        /// <summary>
        /// 导出用户列表
        /// </summary>
        /// <returns></returns>
        public void GetExportList()
        {

        }
        #endregion
    }
}
