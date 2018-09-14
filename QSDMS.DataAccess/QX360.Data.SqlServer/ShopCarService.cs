﻿using QSDMS.Model;
using QSDMS.Util;
using QSDMS.Util.WebControl;
using QX360.Data.IServices;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace QX360.Data.SqlServer
{

    /// <summary>
    /// 店铺汽车管理
    /// </summary>
    public class ShopCarService : BaseSqlDataService, IShopCarService<ShopCarEntity, ShopCarEntity, Pagination>
    {
        public int QueryCount(ShopCarEntity para)
        {
            throw new NotImplementedException();
        }

        public List<ShopCarEntity> GetPageList(ShopCarEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_ShopCar");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_ShopCar.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_ShopCar, ShopCarEntity>(pageList.ToList());
        }

        public List<ShopCarEntity> GetList(ShopCarEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_ShopCar");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (para != null)
            {
                if (!string.IsNullOrWhiteSpace(para.sidx))
                {
                    sql.AppendFormat(" order by {0} {1}", para.sidx, para.sord);
                }
            }
            var list = tbl_ShopCar.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_ShopCar, ShopCarEntity>(list.ToList());
        }

        public ShopCarEntity GetEntity(string keyValue)
        {
            var model = tbl_ShopCar.SingleOrDefault("where ShopCarId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_ShopCar, ShopCarEntity>(model, null);
        }

        public bool Add(ShopCarEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<ShopCarEntity, tbl_ShopCar>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(ShopCarEntity entity)
        {

            var model = tbl_ShopCar.SingleOrDefault("where ShopCarId=@0", entity.ShopCarId);
            model = EntityConvertTools.CopyToModel<ShopCarEntity, tbl_ShopCar>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_ShopCar.Delete("where ShopCarId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(ShopCarEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();
            //当前用户对象
            var loginOperater = OperatorProvider.Provider.Current();
            if (loginOperater != null)
            {
                List<string> userDataAuthorize = loginOperater.UserDataAuthorize;
                if (userDataAuthorize != null && userDataAuthorize.Count > 0)
                {
                    var str = "";
                    foreach (var item in userDataAuthorize)
                    {
                        str += string.Format("'{0}',", item);
                    }
                    str = str.Substring(0, str.Length - 1);
                    sbWhere.AppendFormat(" and ShopId in({0})", str);
                }
            }
            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.ShopId != null)
            {
                sbWhere.AppendFormat(" and ShopId='{0}'", para.ShopId);
            }
            if (para.ShopName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',ShopName)>0)", para.ShopName);
            }
            if (para.Name != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',Name)>0)", para.Name);
            }
            if (para.SortDesc != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',SortDesc)>0)", para.SortDesc);
            }
            if (para.Status != null)
            {
                sbWhere.AppendFormat(" and Status='{0}'", para.Status);
            }
          
            return sbWhere.ToString();
        }
    }
}
