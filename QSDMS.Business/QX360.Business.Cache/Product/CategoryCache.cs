using QX360.Model;
using QSDMS.Cache.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Business.Cache
{
    public class CategoryCache
    {
        //private CategoryBLL category = new CategoryBLL();
        ///// <summary>
        ///// 分类列表
        ///// </summary>
        ///// <returns></returns>
        //public List<CategoryEntity> GetList(CategoryEntity para)
        //{
        //    var cacheList = CacheFactory.Cache().GetCache<List<CategoryEntity>>(category.cacheKey);
        //    if (cacheList == null)
        //    {
        //        var data = category.GetList(para);
        //        CacheFactory.Cache().WriteCache(data, category.cacheKey);
        //        return data;
        //    }
        //    else
        //    {
        //        return cacheList;
        //    }
        //}
    }
}
