
using System.Collections.Generic;
using System.Linq;
using System;
using QSDMS.Data.IService;
using QSDMS.Model;
using QSDMS.Util;

namespace QSDMS.Data.Service.SqlServer
{
    /// <summary>   
    /// 描 述：权限配置管理（角色、岗位、职位、用户组、用户）
    /// </summary>
    public class PermissionService : IPermissionService
    {
        #region 获取数据
        /// <summary>
        /// 获取成员列表
        /// </summary>
        /// <param name="objectId">对象Id</param>
        /// <returns></returns>
        public IEnumerable<UserRelationEntity> GetMemberList(string objectId)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_UserRelation where 1=1 ");
            if (!string.IsNullOrEmpty(objectId))
            {
                sql.Append(" and ObjectId=@0", objectId);
            }
            sql.Append(" order by CreateDate desc");
            var list = Base_UserRelation.Query(sql);
            return EntityConvertTools.CopyToList<Base_UserRelation, UserRelationEntity>(list.ToList());
        }
        /// <summary>
        /// 获取对象列表
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public IEnumerable<UserRelationEntity> GetObjectList(string userId)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_UserRelation where 1=1 ");
            if (!string.IsNullOrEmpty(userId))
            {
                sql.Append(" and UserId=@0", userId);
            }
            sql.Append(" order by CreateDate desc");
            var list = Base_UserRelation.Query(sql);
            return EntityConvertTools.CopyToList<Base_UserRelation, UserRelationEntity>(list.ToList());

        }
        /// <summary>
        /// 获取功能列表
        /// </summary>
        /// <param name="objectId">对象Id</param>
        /// <returns></returns>
        public IEnumerable<AuthorizeEntity> GetModuleList(string objectId)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_Authorize where 1=1 and ItemType=1");
            if (!string.IsNullOrEmpty(objectId))
            {
                sql.Append(" and ObjectId=@0", objectId);
            }
            var list = Base_Authorize.Fetch(sql);
            return EntityConvertTools.CopyToList<Base_Authorize, AuthorizeEntity>(list.ToList());
        }
        /// <summary>
        /// 获取按钮列表
        /// </summary>
        /// <param name="objectId">对象Id</param>
        /// <returns></returns>
        public IEnumerable<AuthorizeEntity> GetModuleButtonList(string objectId)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_Authorize where 1=1 and ItemType=2");
            if (!string.IsNullOrEmpty(objectId))
            {
                sql.Append(" and ObjectId=@0", objectId);
            }
            var list = Base_Authorize.Query(sql);
            return EntityConvertTools.CopyToList<Base_Authorize, AuthorizeEntity>(list.ToList());
        }
        /// <summary>
        /// 获取视图列表
        /// </summary>
        /// <param name="objectId">对象Id</param>
        /// <returns></returns>
        public IEnumerable<AuthorizeEntity> GetModuleColumnList(string objectId)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_Authorize where 1=1 and ItemType=3");
            if (!string.IsNullOrEmpty(objectId))
            {
                sql.Append(" and ObjectId=@0", objectId);
            }
            var list = Base_Authorize.Query(sql);
            return EntityConvertTools.CopyToList<Base_Authorize, AuthorizeEntity>(list.ToList());
        }

        /// <summary>
        /// 获取数据权限列表
        /// </summary>
        /// <param name="objectId">对象Id</param>
        /// <returns></returns>
        public IEnumerable<AuthorizeDataEntity> GetAuthorizeDataList(string objectId)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select * from Base_AuthorizeData where 1=1");
            if (!string.IsNullOrEmpty(objectId))
            {
                sql.Append(" and ObjectId=@0", objectId);
            }
            sql.Append(" order by SortCode");
            var list = Base_AuthorizeDatum.Query(sql);
            return EntityConvertTools.CopyToList<Base_AuthorizeDatum, AuthorizeDataEntity>(list.ToList());

        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 添加成员
        /// </summary>
        /// <param name="authorizeType">权限分类</param>
        /// <param name="objectId">对象Id</param>
        /// <param name="userIds">成员Id</param>
        public void SaveMember(AuthorizeTypeEnum authorizeType, string objectId, string[] userIds)
        {

            try
            {
                using (var tran = QSDMS_SQLDB.GetInstance().GetTransaction())
                {
                    Base_UserRelation.Delete("where ObjectId=@0 and IsDefault=0", objectId);
                    int SortCode = 1;
                    foreach (string item in userIds)
                    {
                        UserRelationEntity userRelationEntity = new UserRelationEntity();
                        userRelationEntity.Create();
                        userRelationEntity.Category = (int)authorizeType;
                        userRelationEntity.ObjectId = objectId;
                        userRelationEntity.UserId = item;
                        userRelationEntity.SortCode = SortCode++;
                        Base_UserRelation userrelation = EntityConvertTools.CopyToModel<UserRelationEntity, Base_UserRelation>(userRelationEntity,null);
                        userrelation.Insert();
                    }
                    tran.Complete();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 添加授权
        /// </summary>
        /// <param name="authorizeType">权限分类</param>
        /// <param name="objectId">对象Id</param>
        /// <param name="moduleIds">功能Id</param>
        /// <param name="moduleButtonIds">按钮Id</param>
        /// <param name="moduleColumnIds">视图Id</param>
        /// <param name="authorizeDataList">数据权限</param>
        public void SaveAuthorize(AuthorizeTypeEnum authorizeType, string objectId, string[] moduleIds, string[] moduleButtonIds, string[] moduleColumnIds, IEnumerable<AuthorizeDataEntity> authorizeDataList)
        {
            
            try
            {
                using (var tran = QSDMS_SQLDB.GetInstance().GetTransaction())
                {
                    //删除授权
                    Base_Authorize.Delete("where ObjectId=@0", objectId);

                    #region 功能
                    int SortCode = 1;
                    foreach (string item in moduleIds)
                    {
                        AuthorizeEntity authorizeEntity = new AuthorizeEntity();
                        authorizeEntity.Create();
                        authorizeEntity.Category = (int)authorizeType;
                        authorizeEntity.ObjectId = objectId;
                        authorizeEntity.ItemType = (int)Enums.FormElementTypeEnum.菜单;
                        authorizeEntity.ItemId = item;
                        authorizeEntity.SortCode = SortCode++;
                        Base_Authorize authorize = EntityConvertTools.CopyToModel<AuthorizeEntity, Base_Authorize>(authorizeEntity,null);
                        authorize.Insert();
                    }
                    #endregion

                    #region 按钮
                    SortCode = 1;
                    foreach (string item in moduleButtonIds)
                    {
                        AuthorizeEntity authorizeEntity = new AuthorizeEntity();
                        authorizeEntity.Create();
                        authorizeEntity.Category = (int)authorizeType;
                        authorizeEntity.ObjectId = objectId;
                        authorizeEntity.ItemType = (int)Enums.FormElementTypeEnum.按钮;
                        authorizeEntity.ItemId = item;
                        authorizeEntity.SortCode = SortCode++;
                        Base_Authorize authorize = EntityConvertTools.CopyToModel<AuthorizeEntity, Base_Authorize>(authorizeEntity,null);
                        authorize.Insert();
                    }
                    #endregion

                    #region 视图
                    SortCode = 1;
                    foreach (string item in moduleColumnIds)
                    {
                        AuthorizeEntity authorizeEntity = new AuthorizeEntity();
                        authorizeEntity.Create();
                        authorizeEntity.Category = (int)authorizeType;
                        authorizeEntity.ObjectId = objectId;
                        authorizeEntity.ItemType = (int)Enums.FormElementTypeEnum.视图;
                        authorizeEntity.ItemId = item;
                        authorizeEntity.SortCode = SortCode++;
                        Base_Authorize authorize = EntityConvertTools.CopyToModel<AuthorizeEntity, Base_Authorize>(authorizeEntity,null);
                        authorize.Insert();
                    }
                    #endregion

                    #region 数据权限
                    SortCode = 1;
                    Base_AuthorizeDatum.Delete("where objectId=@0", objectId);
                    int index = 0;
                    foreach (AuthorizeDataEntity authorizeDataEntity in authorizeDataList)
                    {
                        authorizeDataEntity.Create();
                        authorizeDataEntity.Category = (int)authorizeType;
                        authorizeDataEntity.ObjectId = objectId;
                        authorizeDataEntity.SortCode = SortCode++;
                        Base_AuthorizeDatum authorizedata = EntityConvertTools.CopyToModel<AuthorizeDataEntity, Base_AuthorizeDatum>(authorizeDataEntity,null);
                        authorizedata.Insert();
                        index++;
                    }
                    #endregion
                    tran.Complete();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}
