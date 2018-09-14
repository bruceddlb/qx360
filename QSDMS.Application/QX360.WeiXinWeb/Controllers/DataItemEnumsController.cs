using iFramework.Framework;
using QSDMS.Business.Cache;
using QSDMS.Util;
using QSDMS.Util.Attributes;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QX360.WeiXinWeb.Controllers
{
    public class DataItemEnumsController : Controller
    {
        //
        // GET: /DataItemEnums/

        /// <summary>
        /// 距离区间
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetDistanceRange()
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                //string[] names = System.Enum.GetNames(typeof(QX360.Model.Enums.DistanceRange));
                int[] values = (int[])System.Enum.GetValues(typeof(QX360.Model.Enums.DistanceRange));
                List<KeyValueEntity> list = new List<KeyValueEntity>();
                for (int i = 0; i < values.Length; i++)
                {
                    var discript = EnumAttribute.GetDescription((QX360.Model.Enums.DistanceRange)values[i]);
                    list.Add(new KeyValueEntity() { ItemId = values[i].ToString(), ItemName = discript });
                }
                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["List"] = list;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "DataItemEnumsController>>GetDistanceRange";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 价格区间
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetPriceRange()
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                //string[] names = System.Enum.GetNames(typeof(QX360.Model.Enums.DistanceRange));
                int[] values = (int[])System.Enum.GetValues(typeof(QX360.Model.Enums.PriceRange));
                List<KeyValueEntity> list = new List<KeyValueEntity>();
                for (int i = 0; i < values.Length; i++)
                {
                    var discript = EnumAttribute.GetDescription((QX360.Model.Enums.PriceRange)values[i]);
                    list.Add(new KeyValueEntity() { ItemId = values[i].ToString(), ItemName = discript });
                }
                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["List"] = list;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "DataItemEnumsController>>GetPriceRange";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 品牌
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetBrandRange(string EnCode)
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {

                DataItemCache dataItemCache = new DataItemCache();
                var data = dataItemCache.GetDataItemList(EnCode);
                List<KeyValueEntity> list = new List<KeyValueEntity>();
                if (data != null)
                {
                    data.Foreach((o) =>
                    {
                        list.Add(new KeyValueEntity() { ItemId = o.ItemDetailId.ToString(), ItemName = o.ItemName });
                    });
                }
                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["List"] = list;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "DataItemEnumsController>>GetBrandRange";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 价格区间
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetCarPriceRange()
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                //string[] names = System.Enum.GetNames(typeof(QX360.Model.Enums.DistanceRange));
                int[] values = (int[])System.Enum.GetValues(typeof(QX360.Model.Enums.CarPriceRange));
                List<KeyValueEntity> list = new List<KeyValueEntity>();
                for (int i = 0; i < values.Length; i++)
                {
                    var discript = EnumAttribute.GetDescription((QX360.Model.Enums.CarPriceRange)values[i]);
                    list.Add(new KeyValueEntity() { ItemId = values[i].ToString(), ItemName = discript });
                }
                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["List"] = list;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "DataItemEnumsController>>GetCarPriceRange";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 使用性质
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetUseType()
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                //string[] names = System.Enum.GetNames(typeof(QX360.Model.Enums.DistanceRange));
                int[] values = (int[])System.Enum.GetValues(typeof(QX360.Model.Enums.UseType));
                List<KeyValueEntity> list = new List<KeyValueEntity>();
                for (int i = 0; i < values.Length; i++)
                {
                    var discript = EnumAttribute.GetDescription((QX360.Model.Enums.UseType)values[i]);
                    list.Add(new KeyValueEntity() { ItemId = values[i].ToString(), ItemName = discript });
                }
                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["List"] = list;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "DataItemEnumsController>>GetUseType";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 车辆类型
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetCarType()
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {
                //string[] names = System.Enum.GetNames(typeof(QX360.Model.Enums.DistanceRange));
                int[] values = (int[])System.Enum.GetValues(typeof(QX360.Model.Enums.CarType));
                List<KeyValueEntity> list = new List<KeyValueEntity>();
                for (int i = 0; i < values.Length; i++)
                {
                    var discript = EnumAttribute.GetDescription((QX360.Model.Enums.CarType)values[i]);
                    list.Add(new KeyValueEntity() { ItemId = values[i].ToString(), ItemName = discript });
                }
                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["List"] = list;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "DataItemEnumsController>>GetCarType";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 车牌地域
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetDataItem(string EnCode)
        {
            var result = new ReturnMessage(false) { Message = "获取失败!" };
            try
            {

                DataItemCache dataItemCache = new DataItemCache();
                var data = dataItemCache.GetDataItemList(EnCode);
                List<KeyValueEntity> list = new List<KeyValueEntity>();
                if (data != null)
                {
                    data.Foreach((o) =>
                    {
                        list.Add(new KeyValueEntity() { ItemId = o.ItemDetailId.ToString(), ItemName = o.ItemName });
                    });
                }
                result.IsSuccess = true;
                result.Message = "获取成功!";
                result.ResultData["List"] = list;

            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "DataItemEnumsController>>GetCarArea";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
