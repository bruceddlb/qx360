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
    /// 系统设置管理
    /// </summary>
    public class SettingsService : BaseSqlDataService, ISettingsService<SettingsEntity, SettingsEntity, Pagination>
    {
        public int QueryCount(SettingsEntity para)
        {
            throw new NotImplementedException();
        }

        public List<SettingsEntity> GetPageList(SettingsEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Settings");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_Setting.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_Setting, SettingsEntity>(pageList.ToList());
        }

        public List<SettingsEntity> GetList(SettingsEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_Settings");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_Setting.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_Setting, SettingsEntity>(list.ToList());
        }

        public SettingsEntity GetEntity(string keyValue)
        {
            var model = tbl_Setting.SingleOrDefault("where SettingId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_Setting, SettingsEntity>(model, null);
        }

        public bool Add(SettingsEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<SettingsEntity, tbl_Setting>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(SettingsEntity entity)
        {

            var model = tbl_Setting.SingleOrDefault("where SettingId=@0", entity.SettingId);
            model = EntityConvertTools.CopyToModel<SettingsEntity, tbl_Setting>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_Setting.Delete("where SettingId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(SettingsEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();
            if (para == null)
            {
                return sbWhere.ToString();
            }
            if (para.Name != null)
            {
                sbWhere.AppendFormat(" and Name='{0}'", para.Name);
            }          
            return sbWhere.ToString();
        }
    }
}
