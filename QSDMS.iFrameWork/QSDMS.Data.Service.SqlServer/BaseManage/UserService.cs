
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Linq;
using System;
using QSDMS.Model;
using QSDMS.Data.IService;
using QSDMS.Util.Extension;
using QSDMS.Util;
using QSDMS.Util.WebControl;
using iFramework.Framework.Security;
using iFramework.Framework;

namespace QSDMS.Data.Service.SqlServer
{
    /// <summary>   
    /// 描 述：用户管理
    /// </summary>
    public class UserService : IUserService
    {

        #region 获取数据

        /// <summary>
        /// 用户列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserEntity> GetList()
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_User where 1=1 and UserId<>'System' and DeleteMark=0");
            sql.Append(" order by CreateDate desc");
            var list = Base_User.Query(sql);
            return EntityConvertTools.CopyToList<Base_User, UserEntity>(list.ToList());
        }

        /// <summary>
        /// 获取部门下面用户列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserEntity> GetDepartmentUserList()
        {
            var sql = PetaPoco.Sql.Builder.Append(@"SELECT  u.* FROM    Base_User u
                                    LEFT JOIN Base_Department d ON d.DepartmentId = u.DepartmentId
                            WHERE   1=1
     AND u.UserId <> 'System' AND u.EnabledMark = 1 AND u.DeleteMark=0");
            var list = Base_User.Query(sql);
            return EntityConvertTools.CopyToList<Base_User, UserEntity>(list.ToList());
        }

        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public IEnumerable<UserEntity> GetPageList(Pagination pagination, UserEntity para)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_User where 1=1 and UserId<>'System' and DeleteMark=0");


            //公司主键
            if (para.OrganizeId != null)
            {
                sql.Append(" and OrganizeId=@0", para.OrganizeId);
            }
            //部门主键
            if (para.DepartmentId != null)
            {
                sql.Append(" and DepartmentId=@0", para.DepartmentId);

            }
            if (para.DepartmentId != null)
            {
                sql.Append(" and DepartmentId=@0", para.DepartmentId);

            }
            if (para.Account != null)
            {
                sql.Append("and (charindex(@0,Account)>0)", para.Account);

            }
            if (para.RealName != null)
            {
                sql.Append("and (charindex(@0,RealName)>0)", para.RealName);

            }
            if (para.Mobile != null)
            {
                sql.Append("and (charindex(@0,Mobile)>0)", para.Mobile);

            }
            if (para.CreateUserId != null)
            {
                sql.Append("and (CreateUserId=@0 or UserId=@0)", para.CreateUserId);
            }

            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.OrderBy(new object[] { pagination.sidx + " " + pagination.sord });
            }
            var currentpage = Base_User.Page(pagination.page, pagination.rows, sql);
            //数据对象
            var pageList = currentpage.Items;
            //分页对象

            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<Base_User, UserEntity>(pageList.ToList());
        }

        /// <summary>
        /// 用户实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public UserEntity GetEntity(string keyValue)
        {
            var user = Base_User.SingleOrDefault("where UserId=@0", keyValue);
            return EntityConvertTools.CopyToModel<Base_User, UserEntity>(user, null);
        }
        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns></returns>
        public UserEntity CheckLogin(string username)
        {
            var user = Base_User.SingleOrDefault("where (Account=@0 or Mobile=@0 or Email=@0)", username);
            return EntityConvertTools.CopyToModel<Base_User, UserEntity>(user, null);
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 账户不能重复
        /// </summary>
        /// <param name="account">账户值</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public bool ExistAccount(string account, string keyValue)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_User where 1=1");
            if (!string.IsNullOrEmpty(account))
            {
                sql.Append(" and Account=@0", account);
            }
            if (!string.IsNullOrEmpty(keyValue))
            {
                sql.Append(" and UserId!=@0", keyValue);
            }
            return Base_User.Query(sql).Count() == 0 ? true : false;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="keyValue">主键</param>
        public void RemoveForm(string keyValue)
        {
            using (var tran = QSDMS_SQLDB.GetInstance().GetTransaction())
            {
                Base_User.Delete("where UserId=@0", keyValue);
                Base_UserRelation.Delete("where UserId=@0", keyValue);
                Base_UserRole.Delete("where UserId=@0", keyValue);
                Base_UserAuthorize.Delete("where UserId=@0", keyValue);
                tran.Complete();
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
                using (var tran = QSDMS_SQLDB.GetInstance().GetTransaction())
                {
                    #region 基本信息
                    if (!string.IsNullOrEmpty(keyValue))
                    {
                        userEntity.Modify(keyValue);
                        userEntity.Password = null;
                        Base_User model = Base_User.SingleOrDefault("where UserId=@0", keyValue);
                        model = EntityConvertTools.CopyToModel<UserEntity, Base_User>(userEntity, model);
                        model.UserId = keyValue;
                        model.Update();
                    }
                    else
                    {
                        userEntity.Create();
                        keyValue = userEntity.UserId;
                        userEntity.Secretkey = Md5Helper.MD5(CommonHelper.CreateNo(), 16).ToLower();
                        userEntity.Password = Md5Helper.MD5(DESEncrypt.Encrypt(userEntity.Password, userEntity.Secretkey).ToLower(), 32).ToLower();
                        userEntity.EnabledMark = 1;
                        userEntity.DeleteMark = 0;
                        Base_User model = EntityConvertTools.CopyToModel<UserEntity, Base_User>(userEntity, null);
                        model.Insert();

                    }
                    #endregion

                    #region 默认添加 角色、岗位、职位
                    Base_UserRelation.Delete("where UserId=@0 and IsDefault=1", userEntity.UserId);
                    List<UserRelationEntity> userRelationEntitys = new List<UserRelationEntity>();
                    //角色 这里多个角色逻辑处理
                    //if (!string.IsNullOrEmpty(userEntity.RoleId))
                    //{
                    //    userRelationEntitys.Add(new UserRelationEntity
                    //    {
                    //        Category = (int)QSDMS.Model.Enums.UserCategoryEnum.角色,
                    //        UserRelationId = Guid.NewGuid().ToString(),
                    //        UserId = userEntity.UserId,
                    //        ObjectId = userEntity.RoleId,
                    //        CreateDate = DateTime.Now,
                    //        CreateUserId = OperatorProvider.Provider.Current().UserId,
                    //        CreateUserName = OperatorProvider.Provider.Current().UserName,
                    //        IsDefault = 1,
                    //    });
                    //}
                    //一个用户多个角色
                    if (!string.IsNullOrEmpty(userEntity.RoleId))
                    {

                        Base_UserRole.Delete("where UserId=@0", userEntity.UserId);
                        string[] roles = userEntity.RoleId.Split(',');
                        for (int i = 0; i < roles.Length; i++)
                        {
                            //用户角色表
                            string roleid = roles[i];
                            var userrole = new UserRoleEntity();
                            userrole.UserRoleId = Util.Util.NewUpperGuid();
                            userrole.UserId = userEntity.UserId;
                            userrole.RoleId = roleid.Split('|')[0];
                            userrole.RoleName = roleid.Split('|')[1];
                            Base_UserRole model = EntityConvertTools.CopyToModel<UserRoleEntity, Base_UserRole>(userrole, null);
                            model.Insert();

                            //用户关系表                            
                            userRelationEntitys.Add(new UserRelationEntity
                            {
                                Category = (int)QSDMS.Model.Enums.UserCategoryEnum.角色,
                                UserRelationId = Guid.NewGuid().ToString(),
                                UserId = userEntity.UserId,
                                ObjectId = userrole.RoleId,
                                CreateDate = DateTime.Now,
                                CreateUserId = OperatorProvider.Provider.Current().UserId,
                                CreateUserName = OperatorProvider.Provider.Current().UserName,
                                IsDefault = 1,
                            });
                        }
                    }
                    //岗位
                    if (!string.IsNullOrEmpty(userEntity.DutyId))
                    {
                        userRelationEntitys.Add(new UserRelationEntity
                        {
                            Category = (int)QSDMS.Model.Enums.UserCategoryEnum.岗位,
                            UserRelationId = Guid.NewGuid().ToString(),
                            UserId = userEntity.UserId,
                            ObjectId = userEntity.DutyId,
                            CreateDate = DateTime.Now,
                            CreateUserId = OperatorProvider.Provider.Current().UserId,
                            CreateUserName = OperatorProvider.Provider.Current().UserName,
                            IsDefault = 1,
                        });
                    }
                    //职位
                    if (!string.IsNullOrEmpty(userEntity.PostId))
                    {
                        userRelationEntitys.Add(new UserRelationEntity
                        {
                            Category = (int)QSDMS.Model.Enums.UserCategoryEnum.职位,
                            UserRelationId = Guid.NewGuid().ToString(),
                            UserId = userEntity.UserId,
                            ObjectId = userEntity.PostId,
                            CreateDate = DateTime.Now,
                            CreateUserId = OperatorProvider.Provider.Current().UserId,
                            CreateUserName = OperatorProvider.Provider.Current().UserName,
                            IsDefault = 1,
                        });
                    }
                    //插入用户关系表
                    foreach (UserRelationEntity userRelationItem in userRelationEntitys)
                    {
                        Base_UserRelation model = EntityConvertTools.CopyToModel<UserRelationEntity, Base_UserRelation>(userRelationItem, null);
                        model.Insert();
                    }
                    #endregion

                    Base_UserAuthorize.Delete("where UserId=@0", userEntity.UserId);
                    //插入用户对应数据权限
                    if (!string.IsNullOrEmpty(userEntity.AuthorizeDataId))
                    {
                        string[] uthorizeDatas = userEntity.AuthorizeDataId.Split(',');
                        for (int i = 0; i < uthorizeDatas.Length; i++)
                        {
                            string objectid = uthorizeDatas[i];
                            var userAuthorize = new UserAuthorizeEntity();
                            userAuthorize.UserAuthorizeId = Util.Util.NewUpperGuid();
                            userAuthorize.UserId = userEntity.UserId;
                            userAuthorize.ObjectId = objectid.Split('|')[0];
                            userAuthorize.ObjectName = objectid.Split('|')[1];
                            Base_UserAuthorize model = EntityConvertTools.CopyToModel<UserAuthorizeEntity, Base_UserAuthorize>(userAuthorize, null);
                            model.Insert();
                        }
                    }

                    tran.Complete();
                }
                return keyValue;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 修改用户登录密码
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="Password">新密码（MD5 小写）</param>
        public void RevisePassword(string keyValue, string Password)
        {
            UserEntity userEntity = new UserEntity();
            userEntity.UserId = keyValue;
            userEntity.Secretkey = Md5Helper.MD5(CommonHelper.CreateNo(), 16).ToLower();
            userEntity.Password = Md5Helper.MD5(DESEncrypt.Encrypt(Password, userEntity.Secretkey).ToLower(), 32).ToLower();
            Base_User user = Base_User.SingleOrDefault("where UserId=@0", keyValue);
            user = EntityConvertTools.CopyToModel<UserEntity, Base_User>(userEntity, user);
            user.UserId = keyValue;
            user.Update();
        }
        /// <summary>
        /// 修改用户状态
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="State">状态：1-启动；0-禁用</param>
        public void UpdateState(string keyValue, int State)
        {
            UserEntity userEntity = new UserEntity();
            userEntity.Modify(keyValue);
            userEntity.EnabledMark = State;
            Base_User user = Base_User.SingleOrDefault("where UserId=@0", keyValue);
            user = EntityConvertTools.CopyToModel<UserEntity, Base_User>(userEntity, user);
            user.UserId = keyValue;
            user.Update();

        }
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="userEntity">实体对象</param>
        public void UpdateEntity(UserEntity userEntity)
        {
            Base_User moudle = Base_User.SingleOrDefault("where UserId=@0", userEntity.UserId);
            moudle = EntityConvertTools.CopyToModel<UserEntity, Base_User>(userEntity, moudle);
            moudle.Update();
        }
        #endregion
    }
}
