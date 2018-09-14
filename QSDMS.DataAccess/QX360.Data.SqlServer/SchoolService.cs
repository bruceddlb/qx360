using iFramework.Framework.Log;
using QSDMS.Model;
using QSDMS.Util;
using QSDMS.Util.WebControl;
using QSMS.API.BaiduMap;
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
    /// 驾校管理
    /// </summary>
    public class SchoolService : BaseSqlDataService, ISchoolService<SchoolEntity, SchoolEntity, Pagination>
    {
        private Log _logger;
        /// <summary>
        /// 日志操作
        /// </summary>
        public Log Logger
        {
            get { return _logger ?? (_logger = LogFactory.GetLogger(this.GetType().ToString())); }
        }

        public int QueryCount(SchoolEntity para)
        {
            throw new NotImplementedException();
        }

        public List<SchoolEntity> GetPageList(SchoolEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_School");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }

            var currentpage = tbl_School.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_School, SchoolEntity>(pageList.ToList());
        }

        public List<SchoolEntity> GetList(SchoolEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_School");
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
            var list = tbl_School.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_School, SchoolEntity>(list.ToList());
        }

        public SchoolEntity GetEntity(string keyValue)
        {
            var model = tbl_School.SingleOrDefault("where SchoolId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_School, SchoolEntity>(model, null);
        }

        public bool Add(SchoolEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<SchoolEntity, tbl_School>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(SchoolEntity entity)
        {

            var model = tbl_School.SingleOrDefault("where SchoolId=@0", entity.SchoolId);
            model = EntityConvertTools.CopyToModel<SchoolEntity, tbl_School>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_School.Delete("where SchoolId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(SchoolEntity para)
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

            if (para.Name != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',Name)>0)", para.Name);
            }
            if (para.ConectTel != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',ConectTel)>0)", para.ConectTel);
            }
            if (para.AddressInfo != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',AddressInfo)>0)", para.AddressInfo);
            }
            if (para.Status != null)
            {
                sbWhere.AppendFormat(" and Status='{0}'", para.Status);
            }
            if (para.SchoolId != null)
            {
                sbWhere.AppendFormat(" and SchoolId='{0}'", para.SchoolId);
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
            if (para.IsTraining != null)
            {
                sbWhere.AppendFormat(" and IsTraining='{0}'", para.IsTraining);
            }
            if (para.MasterAccount != null)
            {
                sbWhere.AppendFormat(" and MasterAccount='{0}'", para.MasterAccount);
            }
            if (para.CreateId != null)
            {
                sbWhere.AppendFormat(" or CreateId='{0}'", para.CreateId);
            }
            if (para.SimpleSpelling != null)
            {
                sbWhere.AppendFormat(" and SimpleSpelling='{0}'", para.SimpleSpelling);
            }

            //价格条件
            if (para.TrainingPriceRange != null)
            {
                switch (para.TrainingPriceRange)
                {
                    case Model.Enums.PriceRange.三千以内:
                        sbWhere.AppendFormat(" and TrainingPrice<='{0}'", "3000");
                        break;
                    case Model.Enums.PriceRange.三千到四千:
                        sbWhere.AppendFormat(" and TrainingPrice >'3000' and TrainingPrice<='4000'");
                        break;
                    case Model.Enums.PriceRange.四千到五千:
                        sbWhere.AppendFormat(" and TrainingPrice >'4000' and TrainingPrice<='5000'");
                        break;
                    case Model.Enums.PriceRange.五千到六千:
                        sbWhere.AppendFormat(" and TrainingPrice >'5000' and TrainingPrice<='6000'");
                        break;
                    case Model.Enums.PriceRange.六千以上:
                        sbWhere.AppendFormat(" and TrainingPrice >'6000'");
                        break;

                }
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

        /// <summary>
        /// 登陆验证
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public SchoolEntity CheckLogin(string username, string pwd)
        {
            var sql = new StringBuilder();
            sql.AppendFormat(@"select * from tbl_School where MasterAccount='{0}' or ConectTel='{0}'", username);
            var list = tbl_School.Query(sql.ToString());
            if (list != null)
            {
                var account = list.FirstOrDefault();
                if (account != null && account.Pwd == pwd)
                {
                    var model = EntityConvertTools.CopyToModel<tbl_School, SchoolEntity>(account, null);
                    return model;
                }
            }
            return null;
        }

    }
}
