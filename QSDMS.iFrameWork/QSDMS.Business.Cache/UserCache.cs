
using QSDMS.Business;
using QSDMS.Cache.Factory;
using QSDMS.Model;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace QSDMS.Business.Cache
{
    /// <summary>  
    /// 描 述：用户信息缓存
    /// </summary>
    public class UserCache
    {
        private UserBLL busines = new UserBLL();

        /// <summary>
        /// 用户列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserEntity> GetList()
        {
            IEnumerable<UserEntity> data = new List<UserEntity>();
            var cacheList = CacheFactory.Cache().GetCache<IEnumerable<UserEntity>>(busines.cacheKey);
            if (cacheList == null)
            {
                data = busines.GetList();
                CacheFactory.Cache().WriteCache(data, busines.cacheKey);
            }
            else
            {
                data = cacheList;
            }
            return data;
        }

        /// <summary>
        /// 获取部门下面用户
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserEntity> GetDepartmentUserList() {
            IEnumerable<UserEntity> data = new List<UserEntity>();
            var cacheList = CacheFactory.Cache().GetCache<IEnumerable<UserEntity>>(busines.cacheKey);
            if (cacheList == null)
            {
                data = busines.GetDepartmentUserList();
                CacheFactory.Cache().WriteCache(data, busines.cacheKey);
            }
            else
            {
                data = cacheList;
            }
            return data;
        }
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="departmentId">部门Id</param>
        /// <returns></returns>
        public IEnumerable<UserEntity> GetList(string departmentId)
        {
            var data = this.GetList();
            if (!string.IsNullOrEmpty(departmentId))
            {
                data = data.Where(t => t.DepartmentId == departmentId);
            }
            return data;
        }
        //public Dictionary<string,appUserInfoModel> GetListToApp()
        //{
        //    Dictionary<string, appUserInfoModel> data = new Dictionary<string,appUserInfoModel>();
        //    var datalist = this.GetList();
        //    foreach (var item in datalist)
        //    {
        //        appUserInfoModel one = new appUserInfoModel {
        //            UserId = item.UserId,
        //            Account = item.Account,
        //            RealName = item.RealName,
        //            OrganizeId = item.OrganizeId,
        //            DepartmentId = item.DepartmentId
        //        };
        //        data.Add(item.UserId, one);
        //    }

        //    return data;
        //}
    }
}
