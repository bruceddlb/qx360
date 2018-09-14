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
    /// 消息推送设置
    /// </summary>
    public class MessageSetUpService : BaseSqlDataService, IMessageSetUpService<MessageSetUpEntity, MessageSetUpEntity, Pagination>
    {
        public int QueryCount(MessageSetUpEntity para)
        {
            throw new NotImplementedException();
        }

        public List<MessageSetUpEntity> GetPageList(MessageSetUpEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_MessageSetUp");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_MessageSetUp.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_MessageSetUp, MessageSetUpEntity>(pageList.ToList());
        }

        public List<MessageSetUpEntity> GetList(MessageSetUpEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_MessageSetUp");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_MessageSetUp.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_MessageSetUp, MessageSetUpEntity>(list.ToList());
        }

        public MessageSetUpEntity GetEntity(string keyValue)
        {
            var model = tbl_MessageSetUp.SingleOrDefault("where MessageSetUpId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_MessageSetUp, MessageSetUpEntity>(model, null);
        }

        public bool Add(MessageSetUpEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<MessageSetUpEntity, tbl_MessageSetUp>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(MessageSetUpEntity entity)
        {

            var model = tbl_MessageSetUp.SingleOrDefault("where MessageSetUpId=@0", entity.MessageSetUpId);
            model = EntityConvertTools.CopyToModel<MessageSetUpEntity, tbl_MessageSetUp>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_MessageSetUp.Delete("where MessageSetUpId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(MessageSetUpEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }

            if (para.AccountId != null)
            {
                sbWhere.AppendFormat(" and AccountId='{0}'", para.AccountId);
            }
            if (para.AlterType != null)
            {
                sbWhere.AppendFormat(" and AlterType='{0}'", para.AlterType);
            }

            return sbWhere.ToString();
        }
    }
}
