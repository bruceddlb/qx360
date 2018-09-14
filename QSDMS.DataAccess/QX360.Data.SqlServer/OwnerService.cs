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
    public class OwnerService : BaseSqlDataService, IOwnerService<OwnerEntity, OwnerEntity, Pagination>
    {
        public int QueryCount(OwnerEntity para)
        {
            throw new NotImplementedException();
        }

        public List<OwnerEntity> GetPageList(OwnerEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Owner");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_Owner.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_Owner, OwnerEntity>(pageList.ToList());
        }

        public List<OwnerEntity> GetList(OwnerEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Owner");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_Owner.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_Owner, OwnerEntity>(list.ToList());
        }

        public OwnerEntity GetEntity(string keyValue)
        {
            var model = tbl_Owner.SingleOrDefault("where OwnerId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_Owner, OwnerEntity>(model, null);
        }

        public bool Add(OwnerEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<OwnerEntity, tbl_Owner>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(OwnerEntity entity)
        {

            var model = tbl_Owner.SingleOrDefault("where OwnerId=@0", entity.OwnerId);
            model = EntityConvertTools.CopyToModel<OwnerEntity, tbl_Owner>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_Owner.Delete("where OwnerId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(OwnerEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.MemberId != null)
            {
                sbWhere.AppendFormat(" and MemberId='{0}'", para.MemberId);
            }
            if (para.MemberMobile != null)
            {
                sbWhere.AppendFormat(" and MemberMobile='{0}'", para.MemberMobile);
            }
            if (para.MemberName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',MemberName)>0)", para.MemberName);
            }
            if (para.MemberMobile != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',MemberMobile)>0)", para.MemberMobile);
            }
            if (para.UseType != null)
            {
                sbWhere.AppendFormat(" and UseType='{0}'", para.UseType);
            }
            if (para.CarFrameNum != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',CarFrameNum)>0)", para.CarFrameNum);
            }
            if (para.CarNumber != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',CarNumber)>0)", para.CarNumber);
            }
            if (para.CarType != null)
            {
                sbWhere.AppendFormat(" and CarType='{0}'", para.CarType);
            }
            return sbWhere.ToString();
        }
    }
}
