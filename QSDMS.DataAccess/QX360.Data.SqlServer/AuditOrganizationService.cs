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
    /// 年检机构
    /// </summary>
    public class AuditOrganizationService : BaseSqlDataService, IAuditOrganizationService<AuditOrganizationEntity, AuditOrganizationEntity, Pagination>
    {
        public int QueryCount(AuditOrganizationEntity para)
        {
            throw new NotImplementedException();
        }

        public List<AuditOrganizationEntity> GetPageList(AuditOrganizationEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_AuditOrganization");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_AuditOrganization.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_AuditOrganization, AuditOrganizationEntity>(pageList.ToList());
        }

        public List<AuditOrganizationEntity> GetList(AuditOrganizationEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_AuditOrganization");
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
            var list = tbl_AuditOrganization.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_AuditOrganization, AuditOrganizationEntity>(list.ToList());
        }

        public AuditOrganizationEntity GetEntity(string keyValue)
        {
            var model = tbl_AuditOrganization.SingleOrDefault("where OrganizationId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_AuditOrganization, AuditOrganizationEntity>(model, null);
        }

        public bool Add(AuditOrganizationEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<AuditOrganizationEntity, tbl_AuditOrganization>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(AuditOrganizationEntity entity)
        {

            var model = tbl_AuditOrganization.SingleOrDefault("where OrganizationId=@0", entity.OrganizationId);
            model = EntityConvertTools.CopyToModel<AuditOrganizationEntity, tbl_AuditOrganization>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_AuditOrganization.Delete("where OrganizationId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(AuditOrganizationEntity para)
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
                    sbWhere.AppendFormat(" and OrganizationId in({0})", str);
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
          
            if (para.IsTake != null)
            {
                sbWhere.AppendFormat(" and IsTake='{0}'", para.IsTake);
            }
            if (para.Status != null)
            {
                sbWhere.AppendFormat(" and Status='{0}'", para.Status);
            }
            if (para.OrganizationId != null)
            {
                sbWhere.AppendFormat(" and OrganizationId='{0}'", para.OrganizationId);
            }

            return sbWhere.ToString();
        }
    }
}
