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
    public class AdviseService : BaseSqlDataService, IAdviseService<AdviseEntity, AdviseEntity, Pagination>
    {
        public int QueryCount(AdviseEntity para)
        {
            throw new NotImplementedException();
        }

        public List<AdviseEntity> GetPageList(AdviseEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Advise");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_Advise.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_Advise, AdviseEntity>(pageList.ToList());
        }

        public List<AdviseEntity> GetList(AdviseEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Advise");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_Advise.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_Advise, AdviseEntity>(list.ToList());
        }

        public AdviseEntity GetEntity(string keyValue)
        {
            var model = tbl_Advise.SingleOrDefault("where AdviseId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_Advise, AdviseEntity>(model, null);
        }

        public bool Add(AdviseEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<AdviseEntity, tbl_Advise>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(AdviseEntity entity)
        {

            var model = tbl_Advise.SingleOrDefault("where AdviseId=@0", entity.AdviseId);
            model = EntityConvertTools.CopyToModel<AdviseEntity, tbl_Advise>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_Advise.Delete("where AdviseId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(AdviseEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.ConnectName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',ConnectName)>0)", para.ConnectName);
            }
            if (para.ConnectTel != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',ConnectTel)>0)", para.ConnectTel);
            }
            if (para.AdviseContent != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',AdviseContent)>0)", para.AdviseContent);
            }

            return sbWhere.ToString();
        }
    }
}
