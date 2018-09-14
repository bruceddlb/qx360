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
    /// 教练管理
    /// </summary>
    public class TeacherService : BaseSqlDataService, ITeacherService<TeacherEntity, TeacherEntity, Pagination>
    {
        public int QueryCount(TeacherEntity para)
        {
            throw new NotImplementedException();
        }

        public List<TeacherEntity> GetPageList(TeacherEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Teacher");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_Teacher.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_Teacher, TeacherEntity>(pageList.ToList());
        }

        public List<TeacherEntity> GetList(TeacherEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Teacher");
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
            var list = tbl_Teacher.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_Teacher, TeacherEntity>(list.ToList());
        }

        public TeacherEntity GetEntity(string keyValue)
        {
            var model = tbl_Teacher.SingleOrDefault("where TeacherId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_Teacher, TeacherEntity>(model, null);
        }

        public bool Add(TeacherEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<TeacherEntity, tbl_Teacher>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(TeacherEntity entity)
        {

            var model = tbl_Teacher.SingleOrDefault("where TeacherId=@0", entity.TeacherId);
            model = EntityConvertTools.CopyToModel<TeacherEntity, tbl_Teacher>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_Teacher.Delete("where TeacherId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(TeacherEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();
            //当前用户对象
            var loginOperater = OperatorProvider.Provider.Current();
            if (loginOperater != null)
            {
                //如果是考场管理员教练取所有
                if (loginOperater.ObjectId.IndexOf(Config.GetValue("KCGLY_RoleId")) == -1)
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
                        sbWhere.AppendFormat(" and (SchoolId in({0})", str);
                        sbWhere.Append(" or SchoolId is null or SchoolId='-1')");//未归属的教练都可以查看

                    }
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
            if (para.Mobile != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',Mobile)>0)", para.Mobile);
            }

            if (para.SchoolName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',SchoolName)>0)", para.SchoolName);
            }
            if (para.CarNumber != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',CarNumber)>0)", para.CarNumber);
            }

            //指定驾校
            if (para.SchoolId != null)
            {
                sbWhere.AppendFormat(" and SchoolId='{0}'", para.SchoolId);
            }
            if (para.Status != null)
            {
                sbWhere.AppendFormat(" and Status='{0}'", para.Status);
            }
            if (para.IsWithDriving != null)
            {
                sbWhere.AppendFormat(" and IsWithDriving='{0}'", para.IsWithDriving);
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
                // sbWhere.AppendFormat(" and CountyId='{0}'", para.CountyId);
                sbWhere.AppendFormat(" and (charindex('{0}',ServicesAreaIds)>0)", para.CountyId);
            }
            if (para.IsTakeCar != null)
            {
                sbWhere.AppendFormat(" and IsTakeCar='{0}'", para.IsTakeCar);
            }
            if (para.MasterAccount != null)
            {
                sbWhere.AppendFormat(" and MasterAccount='{0}'", para.MasterAccount);
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
        public TeacherEntity CheckLogin(string username, string pwd)
        {
            var sql = new StringBuilder();
            sql.AppendFormat(@"select * from tbl_Teacher where MasterAccount='{0}' or Mobile='{0}'", username);
            var list = tbl_Teacher.Query(sql.ToString());
            if (list != null)
            {
                var account = list.FirstOrDefault();
                if (account != null && account.Pwd == pwd)
                {
                    var model = EntityConvertTools.CopyToModel<tbl_Teacher, TeacherEntity>(account, null);
                    return model;
                }
            }
            return null;
        }

        public TeacherEntity GetEntityByOpenId(string openid)
        {
            var model = tbl_Teacher.SingleOrDefault("where OpenId=@0", openid);
            if (model != null)
            {
                return EntityConvertTools.CopyToModel<tbl_Teacher, TeacherEntity>(model, null);
            }
            else
            {
                return null;
            }
        }
    }
}
