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
    /// 考场管理员
    /// </summary>
    public class ExamPlaceMasterService : BaseSqlDataService, IExamPlaceMasterService<ExamPlaceMasterEntity, ExamPlaceMasterEntity, Pagination>
    {
        public int QueryCount(ExamPlaceMasterEntity para)
        {
            throw new NotImplementedException();
        }

        public List<ExamPlaceMasterEntity> GetPageList(ExamPlaceMasterEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_ExamPlaceMaster");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_ExamPlaceMaster.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_ExamPlaceMaster, ExamPlaceMasterEntity>(pageList.ToList());
        }

        public List<ExamPlaceMasterEntity> GetList(ExamPlaceMasterEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_ExamPlaceMaster");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_ExamPlaceMaster.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_ExamPlaceMaster, ExamPlaceMasterEntity>(list.ToList());
        }

        public ExamPlaceMasterEntity GetEntity(string keyValue)
        {
            var model = tbl_ExamPlaceMaster.SingleOrDefault("where ExamPlaceMasterId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_ExamPlaceMaster, ExamPlaceMasterEntity>(model, null);
        }

        public bool Add(ExamPlaceMasterEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<ExamPlaceMasterEntity, tbl_ExamPlaceMaster>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(ExamPlaceMasterEntity entity)
        {

            var model = tbl_ExamPlaceMaster.SingleOrDefault("where ExamPlaceMasterId=@0", entity.ExamPlaceMasterId);
            model = EntityConvertTools.CopyToModel<ExamPlaceMasterEntity, tbl_ExamPlaceMaster>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_ExamPlaceMaster.Delete("where ExamPlaceMasterId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(ExamPlaceMasterEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.MasterAccount != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',MasterAccount)>0)", para.MasterAccount);
            }
            if (para.MasterName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',MasterName)>0)", para.MasterName);
            }
            if (para.CreateId != null)
            {
                sbWhere.AppendFormat(" and CreateId='{0}'", para.CreateId);
            }
            //if (para.ExamPlaceIds != null)
            //{
            //    var str = "";
            //    string[] exemplaceids = para.ExamPlaceIds.Split(',');
            //    foreach (var item in exemplaceids)
            //    {
            //        if (item != "")
            //        {
            //            str += string.Format("'{0}',", item);
            //        }
            //    }
            //    str = str.Substring(0, str.Length - 1);
            //    sbWhere.AppendFormat(" and SchoolId in({0})", str);
            //}

            return sbWhere.ToString();
        }
        /// <summary>
        /// 登陆验证
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public ExamPlaceMasterEntity CheckLogin(string username, string pwd)
        {
            var sql = new StringBuilder();
            sql.AppendFormat(@"select * from tbl_ExamPlaceMaster where MasterAccount='{0}' or MasterName='{0}'", username);
            var list = tbl_ExamPlaceMaster.Query(sql.ToString());
            if (list != null)
            {
                var account = list.FirstOrDefault();
                if (account != null && account.MasterPwd == pwd)
                {
                    var model = EntityConvertTools.CopyToModel<tbl_ExamPlaceMaster, ExamPlaceMasterEntity>(account, null);
                    return model;
                }
            }
            return null;
        }
    }
}
