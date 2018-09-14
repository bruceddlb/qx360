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
    /// 相关业务图管理
    /// </summary>
    public class AttachmentPicService : BaseSqlDataService, IAttachmentPicService<AttachmentPicEntity, AttachmentPicEntity, Pagination>
    {
        public int QueryCount(AttachmentPicEntity para)
        {
            throw new NotImplementedException();
        }

        public List<AttachmentPicEntity> GetPageList(AttachmentPicEntity para, ref Pagination pagination)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_AttachmentPic");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            if (!string.IsNullOrWhiteSpace(pagination.sidx))
            {
                sql.AppendFormat(" order by {0} {1}", pagination.sidx, pagination.sord);
            }
            var currentpage = tbl_AttachmentPic.Page(pagination.page, pagination.rows, sql.ToString());
            //数据对象
            var pageList = currentpage.Items;
            //分页对象
            pagination.records = Converter.ParseInt32(currentpage.TotalItems);
            return EntityConvertTools.CopyToList<tbl_AttachmentPic, AttachmentPicEntity>(pageList.ToList());
        }

        public List<AttachmentPicEntity> GetList(AttachmentPicEntity para)
        {
            var sql = new StringBuilder();
            sql.Append(@"select * from tbl_AttachmentPic");
            string where = ConverPara(para);
            if (!string.IsNullOrEmpty(where))
            {
                sql.AppendFormat(" where 1=1 {0}", where);
            }
            var list = tbl_AttachmentPic.Query(sql.ToString());
            return EntityConvertTools.CopyToList<tbl_AttachmentPic, AttachmentPicEntity>(list.ToList());
        }

        public AttachmentPicEntity GetEntity(string keyValue)
        {
            var model = tbl_AttachmentPic.SingleOrDefault("where PicId=@0", keyValue);
            return EntityConvertTools.CopyToModel<tbl_AttachmentPic, AttachmentPicEntity>(model, null);
        }

        public bool Add(AttachmentPicEntity entity)
        {
            var model = EntityConvertTools.CopyToModel<AttachmentPicEntity, tbl_AttachmentPic>(entity, null);
            model.Insert();
            return true;
        }

        public bool Update(AttachmentPicEntity entity)
        {

            var model = tbl_AttachmentPic.SingleOrDefault("where PicId=@0", entity.PicId);
            model = EntityConvertTools.CopyToModel<AttachmentPicEntity, tbl_AttachmentPic>(entity, model);
            int count = model.Update();
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public bool Delete(string keyValue)
        {
            int count = tbl_AttachmentPic.Delete("where PicId=@0", keyValue);
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        public string ConverPara(AttachmentPicEntity para)
        {
            StringBuilder sbWhere = new StringBuilder();

            if (para == null)
            {
                return sbWhere.ToString();
            }

            if (para.ObjectId != null)
            {
                sbWhere.AppendFormat(" and ObjectId='{0}'", para.ObjectId);
            }
            return sbWhere.ToString();
        }
    }
}
