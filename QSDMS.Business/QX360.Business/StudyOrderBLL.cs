using QSDMS.Util.WebControl;
using QX360.Data.IServices;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Business
{

    public class StudyOrderBLL : BaseBLL<IStudyOrderService<StudyOrderEntity, StudyOrderEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static StudyOrderBLL m_Instance = new StudyOrderBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static StudyOrderBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "StudyOrderCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(StudyOrderEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<StudyOrderEntity> GetPageList(StudyOrderEntity para, ref Pagination pagination)
        {
            List<StudyOrderEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<StudyOrderEntity> GetList(StudyOrderEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(StudyOrderEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(StudyOrderEntity entity)
        {
            return InstanceDAL.Update(entity);
        }

        public bool Delete(string keyValue)
        {
            var detail = StudyOrderDetailBLL.Instance.GetList(new StudyOrderDetailEntity() { StudyOrderId = keyValue });
            detail.ForEach((o) =>
            {
                StudyFreeTimeEntity freetime = new StudyFreeTimeEntity();
                freetime.StudyFreeTimeId = o.StudyFreeTimeId;
                freetime.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.空闲;
                StudyFreeTimeBLL.Instance.Update(freetime);
                StudyOrderDetailBLL.Instance.Delete(o.StudyOrderDetailId);
            });
            return InstanceDAL.Delete(keyValue);
        }
        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public StudyOrderEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }

        public string GetOrderNo()
        {
            return InstanceDAL.GetOrderNo();
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="id"></param>
        public void Cancel(string id)
        {
            var entity = StudyOrderBLL.Instance.GetEntity(id);
            if (entity != null)
            {
                entity.StudyOrderId = id;
                entity.Status = (int)QX360.Model.Enums.StudySubscribeStatus.取消;
                this.Update(entity);
                //修改预约时间状态
                var detail = StudyOrderDetailBLL.Instance.GetList(new StudyOrderDetailEntity() { StudyOrderId = id });
                detail.ForEach((o) =>
                {
                    StudyFreeTimeEntity freetime = new StudyFreeTimeEntity();
                    freetime.StudyFreeTimeId = o.StudyFreeTimeId;
                    freetime.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.空闲;
                    StudyFreeTimeBLL.Instance.Update(freetime);
                });
            }

        }
    }
}
