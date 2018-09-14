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
    /// 会员信息
    /// </summary>
    public class MemberService : BaseSqlDataService, IMemberService<MemberEntity, MemberEntity, Pagination>
    {
        public int QueryCount(MemberEntity para)
        {
            throw new NotImplementedException();
        }

        public List<MemberEntity> GetPageList(MemberEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Member");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_Member.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_Member, MemberEntity>(pageList.ToList());
        }

        public List<MemberEntity> GetList(MemberEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Member");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_Member.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_Member, MemberEntity>(list.ToList());
        }

        public MemberEntity GetEntity(string keyValue)
        {
            var model = tbl_Member.SingleOrDefault("where MemberId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_Member, MemberEntity>(model, null);
        }

        public bool Add(MemberEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<MemberEntity, tbl_Member>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(MemberEntity entity)
        {

            var model = tbl_Member.SingleOrDefault("where MemberId=@0", entity.MemberId);
            model = EntityConvertTools.CopyToModel<MemberEntity, tbl_Member>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_Member.Delete("where MemberId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(MemberEntity para)
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
                    sbWhere.AppendFormat(" and SchoolId in({0})", str);
                }
            }
            if (para == null)
            {
                return sbWhere.ToString();
            }

            if (para.MemberName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',MemberName)>0)", para.MemberName);
            }
            if (para.Mobile != null)
            {
                sbWhere.AppendFormat(" and Mobile='{0}'", para.Mobile);
            }
            if (para.CarNumber != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',CarNumber)>0)", para.CarNumber);
            }
            if (para.SchoolName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',SchoolName)>0)", para.SchoolName);
            }
            if (para.LevId != null)
            {
                sbWhere.AppendFormat(" and LevId='{0}'", para.LevId);
            }
           
            //指定驾校
            if (para.SchoolId != null)
            {
                sbWhere.AppendFormat(" and SchoolId='{0}'", para.SchoolId);
            }
            //指定教练
            if (para.TeacherId != null)
            {
                sbWhere.AppendFormat(" and TeacherId='{0}'", para.TeacherId);
            }
            if (para.SimpleSpelling != null)
            {
                sbWhere.AppendFormat(" and SimpleSpelling='{0}'", para.SimpleSpelling);
            }
            return sbWhere.ToString();
        }

        /// <summary>
        /// 登陆验证
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public MemberEntity CheckLogin(string username,string pwd)
        {
            var sql = new StringBuilder();
            sql.AppendFormat(@"select * from tbl_Member where MemberName='{0}' or Mobile='{0}'", username);
            var list = tbl_Member.Query(sql.ToString());
            if (list != null)
            {
                var account = list.FirstOrDefault();
                if (account != null && account.Pwd == pwd)
                {
                    var model = EntityConvertTools.CopyToModel<tbl_Member, MemberEntity>(account, null);
                    return model;
                }
            }
            return null;
        }
        public MemberEntity GetEntityForOpenId(string openid)
        {
            var model = tbl_Member.SingleOrDefault("where OpenId=@0", openid);
            if (model != null)
            {
                return EntityConvertTools.CopyToModel<tbl_Member, MemberEntity>(model, null);
            }
            else
            {
                return null;
            }
        }
    }
}
