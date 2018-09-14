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

    public class TrainingOrderBLL : BaseBLL<ITrainingOrderService<TrainingOrderEntity, TrainingOrderEntity, Pagination>>
    {

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TrainingOrderBLL m_Instance = new TrainingOrderBLL();

        /// <summary>
        /// 访问实例
        /// </summary>
        public static TrainingOrderBLL Instance
        {
            get { return m_Instance; }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string cacheKey = "ITrainingOrderCache";


        /// <summary>
        /// 构造方法
        /// </summary>

        public int QueryCount(TrainingOrderEntity para)
        {
            return InstanceDAL.QueryCount(para);
        }

        public List<TrainingOrderEntity> GetPageList(TrainingOrderEntity para, ref Pagination pagination)
        {
            List<TrainingOrderEntity> list = InstanceDAL.GetPageList(para, ref pagination);

            return list;
        }

        public List<TrainingOrderEntity> GetList(TrainingOrderEntity para)
        {
            return InstanceDAL.GetList(para);
        }

        public bool Add(TrainingOrderEntity entity)
        {
            return InstanceDAL.Add(entity);
        }

        public bool Update(TrainingOrderEntity entity)
        {
            return InstanceDAL.Update(entity);
        }

        public bool Delete(string keyValue)
        {
            var detail = TrainingOrderDetailBLL.Instance.GetList(new TrainingOrderDetailEntity() { TrainingOrderId = keyValue });
            detail.ForEach((o) =>
            {
                //修改系统预约时间状态
                TrainingFreeTimeEntity freetime = new TrainingFreeTimeEntity();
                freetime.TrainingFreeTimeId = o.TrainingFreeTimeId;
                freetime.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.空闲;
                TrainingFreeTimeBLL.Instance.Update(freetime);
                //删除加班时间明细
                var customtime = TrainingCustomFreeTimeBLL.Instance.GetEntity(o.TrainingFreeTimeId);
                if (customtime != null)
                {
                    TrainingCustomFreeTimeBLL.Instance.Delete(customtime.TrainingCustomFreeTimeId);
                }
                TrainingOrderDetailBLL.Instance.Delete(o.TrainingOrderDetailId);
            });
            return InstanceDAL.Delete(keyValue);
        }
        /// <summary>
        /// 实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public TrainingOrderEntity GetEntity(string keyValue)
        {
            return InstanceDAL.GetEntity(keyValue);
        }
        /// <summary>
        /// 获取业务单号
        /// </summary>
        /// <returns></returns>
        public string GetOrderNo()
        {
            return InstanceDAL.GetOrderNo();
        }

        public void Cancel(string id)
        {
            var entity = TrainingOrderBLL.Instance.GetEntity(id);
            if (entity != null)
            {
                entity.TrainingOrderId = id;
                entity.Status = (int)QX360.Model.Enums.TrainingStatus.已取消;
                this.Update(entity);
                //修改预约时间状态
                var detail = TrainingOrderDetailBLL.Instance.GetList(new TrainingOrderDetailEntity() { TrainingOrderId = id });
                detail.ForEach((o) =>
                {
                    TrainingFreeTimeEntity freetime = new TrainingFreeTimeEntity();
                    freetime.TrainingFreeTimeId = o.TrainingFreeTimeId;
                    freetime.FreeStatus = (int)QX360.Model.Enums.FreeTimeStatus.空闲;
                    TrainingFreeTimeBLL.Instance.Update(freetime);
                });
            }
        }
    }
}
