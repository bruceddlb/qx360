using QSDMS.Model;
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
    /// 店铺管理
    /// </summary>
    public class ShopService : BaseSqlDataService, IShopService<ShopEntity, ShopEntity, Pagination>
    {
        public int QueryCount(ShopEntity para)
        {
            throw new NotImplementedException();
        }

        public List<ShopEntity> GetPageList(ShopEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Shop");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_Shop.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_Shop, ShopEntity>(pageList.ToList());
        }

        public List<ShopEntity> GetList(ShopEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Shop");
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
            var list = tbl_Shop.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_Shop, ShopEntity>(list.ToList());
        }

        public ShopEntity GetEntity(string keyValue)
        {
            var model = tbl_Shop.SingleOrDefault("where ShopId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_Shop, ShopEntity>(model, null);
        }

        public bool Add(ShopEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<ShopEntity, tbl_Shop>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(ShopEntity entity)
        {

            var model = tbl_Shop.SingleOrDefault("where ShopId=@0", entity.ShopId);
            model = EntityConvertTools.CopyToModel<ShopEntity, tbl_Shop>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_Shop.Delete("where ShopId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(ShopEntity para)
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

            if (para.Name != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',Name)>0)", para.Name);
            }
            if (para.ConectTel != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',ConectTel)>0)", para.ConectTel);
            }
           
            if (para.ProvinceId != null)
            {
                sbWhere.AppendFormat(" and ProvinceId='{0}'", para.ProvinceId);
            }
            if (para.CityId != null)
            {
                sbWhere.AppendFormat(" and CityId='{0}'", para.CityId);
            }
            if (para.CountyId != null)
            {
                sbWhere.AppendFormat(" and CountyId='{0}'", para.CountyId);
            }
            if (para.Status != null)
            {
                sbWhere.AppendFormat(" and Status='{0}'", para.Status);
            }
            //距离条件
            if (para.DistanceRange != null && para.Lng != null && para.Lat != null)
            {
                int haolong = 0;//距离
                switch (para.DistanceRange)
                {
                    case Model.Enums.DistanceRange.一千米内:
                        haolong = 1;
                        sbWhere.AppendFormat(" and lat >'{0}'", para.Lat - haolong);
                        sbWhere.AppendFormat("and lat <'{0}' and lng > '{1}' and lng < '{2}'", para.Lat + haolong, para.Lng - haolong, para.Lng + haolong);
                        sbWhere.Append("and sqrt( ");
                        sbWhere.Append("(");
                        sbWhere.AppendFormat("(({0}-Lng)/{1}*PI()*12656*cos((({2}+Lat)/{1})*PI()/180)/180) ", para.Lng, haolong, para.Lat);
                        sbWhere.AppendFormat("* ");
                        sbWhere.AppendFormat("(({0}-Lng)*PI()*12656*cos((({2}+Lat)/{1})*PI()/180)/180)) ", para.Lng, haolong, para.Lat);
                        sbWhere.AppendFormat("+ ");
                        sbWhere.AppendFormat("( ");
                        sbWhere.AppendFormat("(({0}-Lat)*PI()*12656/180)*(({0}-Lat)*PI()*12656/180)) /{1} ", para.Lat, haolong);
                        sbWhere.AppendFormat(")<='{0}'", haolong);

                        break;
                    case Model.Enums.DistanceRange.一千米至两千米:
                        haolong = 3;
                        sbWhere.AppendFormat(" and lat >'{0}'", para.Lat - haolong);
                        sbWhere.AppendFormat("and lat <'{0}' and lng > '{1}' and lng < '{2}'", para.Lat + haolong, para.Lng - haolong, para.Lng + haolong);
                        sbWhere.Append("and sqrt( ");
                        sbWhere.Append("(");
                        sbWhere.AppendFormat("(({0}-Lng)/{1}*PI()*12656*cos((({2}+Lat)/{1})*PI()/180)/180) ", para.Lng, haolong, para.Lat);
                        sbWhere.AppendFormat("* ");
                        sbWhere.AppendFormat("(({0}-Lng)*PI()*12656*cos((({2}+Lat)/{1})*PI()/180)/180)) ", para.Lng, haolong, para.Lat);
                        sbWhere.AppendFormat("+ ");
                        sbWhere.AppendFormat("( ");
                        sbWhere.AppendFormat("(({0}-Lat)*PI()*12656/180)*(({0}-Lat)*PI()*12656/180)) /{1} ", para.Lat, haolong);
                        sbWhere.AppendFormat(")<='{0}'", haolong);
                        break;
                    case Model.Enums.DistanceRange.二千米至五千米:
                        haolong = 5;
                        sbWhere.AppendFormat(" and lat >'{0}'", para.Lat - haolong);
                        sbWhere.AppendFormat("and lat <'{0}' and lng > '{1}' and lng < '{2}'", para.Lat + haolong, para.Lng - haolong, para.Lng + haolong);
                        sbWhere.Append("and sqrt( ");
                        sbWhere.Append("(");
                        sbWhere.AppendFormat("(({0}-Lng)/{1}*PI()*12656*cos((({2}+Lat)/{1})*PI()/180)/180) ", para.Lng, haolong, para.Lat);
                        sbWhere.AppendFormat("* ");
                        sbWhere.AppendFormat("(({0}-Lng)*PI()*12656*cos((({2}+Lat)/{1})*PI()/180)/180)) ", para.Lng, haolong, para.Lat);
                        sbWhere.AppendFormat("+ ");
                        sbWhere.AppendFormat("( ");
                        sbWhere.AppendFormat("(({0}-Lat)*PI()*12656/180)*(({0}-Lat)*PI()*12656/180)) /{1} ", para.Lat, haolong);
                        sbWhere.AppendFormat(")<='{0}'", haolong);
                        break;
                    case Model.Enums.DistanceRange.五千米以上:
                        haolong = 5;
                        sbWhere.AppendFormat(" and lat >'{0}'", para.Lat - haolong);
                        sbWhere.AppendFormat("and lat <'{0}' and lng > '{1}' and lng < '{2}'", para.Lat + haolong, para.Lng - haolong, para.Lng + haolong);
                        sbWhere.Append("and sqrt( ");
                        sbWhere.Append("(");
                        sbWhere.AppendFormat("(({0}-Lng)/{1}*PI()*12656*cos((({2}+Lat)/{1})*PI()/180)/180) ", para.Lng, haolong, para.Lat);
                        sbWhere.AppendFormat("* ");
                        sbWhere.AppendFormat("(({0}-Lng)*PI()*12656*cos((({2}+Lat)/{1})*PI()/180)/180)) ", para.Lng, haolong, para.Lat);
                        sbWhere.AppendFormat("+ ");
                        sbWhere.AppendFormat("( ");
                        sbWhere.AppendFormat("(({0}-Lat)*PI()*12656/180)*(({0}-Lat)*PI()*12656/180)) /{1} ", para.Lat, haolong);
                        sbWhere.AppendFormat(")>'{0}'", haolong);
                        break;

                }
            }
            return sbWhere.ToString();
        }
    }
}
