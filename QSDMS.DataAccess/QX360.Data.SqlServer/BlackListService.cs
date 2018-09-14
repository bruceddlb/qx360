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
    public class BlackListService : BaseSqlDataService, IBlackListService<BlackListEntity, BlackListEntity, Pagination>
    {
        public int QueryCount(BlackListEntity para)
        {
            throw new NotImplementedException();
        }

        public List<BlackListEntity> GetPageList(BlackListEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_BlackList");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_BlackList.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_BlackList, BlackListEntity>(pageList.ToList());
        }

        public List<BlackListEntity> GetList(BlackListEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_BlackList");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_BlackList.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_BlackList, BlackListEntity>(list.ToList());
        }

        public BlackListEntity GetEntity(string keyValue)
        {
            var model = tbl_BlackList.SingleOrDefault("where BlackListId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_BlackList, BlackListEntity>(model, null);
        }

        public bool Add(BlackListEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<BlackListEntity, tbl_BlackList>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(BlackListEntity entity)
        {

            var model = tbl_BlackList.SingleOrDefault("where BlackListId=@0", entity.BlackListId);
            model = EntityConvertTools.CopyToModel<BlackListEntity, tbl_BlackList>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_BlackList.Delete("where BlackListId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(BlackListEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.ObjectId != null)
            {
                sbWhere.AppendFormat(" and ObjectId='{0}'", para.ObjectId);
            }
            if (para.ObjectName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',ObjectName)>0", para.ObjectName);
            }
            if (para.SchoolId != null)
            {
                sbWhere.AppendFormat(" and SchoolId='{0}'", para.SchoolId);
            }

            return sbWhere.ToString();
        }
    }
}
