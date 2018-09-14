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
    /// 意见管理
    /// </summary>
    public class BannerService : BaseSqlDataService, IBannerService<BannerEntity, BannerEntity, Pagination>
    {
        public int QueryCount(BannerEntity para)
        {
            throw new NotImplementedException();
        }

        public List<BannerEntity> GetPageList(BannerEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Banner");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_Banner.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_Banner, BannerEntity>(pageList.ToList());
        }

        public List<BannerEntity> GetList(BannerEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Banner");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_Banner.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_Banner, BannerEntity>(list.ToList());
        }

        public BannerEntity GetEntity(string keyValue)
        {
            var model = tbl_Banner.SingleOrDefault("where BannerId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_Banner, BannerEntity>(model, null);
        }

        public bool Add(BannerEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<BannerEntity, tbl_Banner>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(BannerEntity entity)
        {

            var model = tbl_Banner.SingleOrDefault("where BannerId=@0", entity.BannerId);
            model = EntityConvertTools.CopyToModel<BannerEntity, tbl_Banner>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_Banner.Delete("where BannerId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(BannerEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.Name != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',Name)>0)", para.Name);
            }

            return sbWhere.ToString();
        }
    }
}
