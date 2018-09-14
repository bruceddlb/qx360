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
    /// 实训汽车管理
    /// </summary>
    public class TrainingCarService : BaseSqlDataService, ITrainingCarService<TrainingCarEntity, TrainingCarEntity, Pagination>
    {
        public int QueryCount(TrainingCarEntity para)
        {
            throw new NotImplementedException();
        }

        public List<TrainingCarEntity> GetPageList(TrainingCarEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_TrainingCar");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_TrainingCar.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_TrainingCar, TrainingCarEntity>(pageList.ToList());
        }

        public List<TrainingCarEntity> GetList(TrainingCarEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_TrainingCar");
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
            var list = tbl_TrainingCar.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_TrainingCar, TrainingCarEntity>(list.ToList());
        }

        public TrainingCarEntity GetEntity(string keyValue)
        {
            var model = tbl_TrainingCar.SingleOrDefault("where TrainingCarId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_TrainingCar, TrainingCarEntity>(model, null);
        }

        public bool Add(TrainingCarEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<TrainingCarEntity, tbl_TrainingCar>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(TrainingCarEntity entity)
        {

            var model = tbl_TrainingCar.SingleOrDefault("where TrainingCarId=@0", entity.TrainingCarId);
            model = EntityConvertTools.CopyToModel<TrainingCarEntity, tbl_TrainingCar>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_TrainingCar.Delete("where TrainingCarId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(TrainingCarEntity para)
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
            if (para.SchoolName != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',SchoolName)>0)", para.SchoolName);
            }
            if (para.CarNumber != null)
            {
                sbWhere.AppendFormat(" and (charindex('{0}',CarNumber)>0)", para.CarNumber);
            }
            if (para.Status != null)
            {
                sbWhere.AppendFormat(" and Status='{0}'", para.Status);
            }
           
            //指定驾校
            if (para.SchoolId != null)
            {
                sbWhere.AppendFormat(" and SchoolId='{0}'", para.SchoolId);
            }
            if (para.TrainingType != null)
            {
                sbWhere.AppendFormat(" and TrainingType='{0}'", para.TrainingType);
            }
            return sbWhere.ToString();
        }
    }
}
