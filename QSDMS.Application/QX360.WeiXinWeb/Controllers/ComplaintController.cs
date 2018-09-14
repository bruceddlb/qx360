using iFramework.Framework;
using Newtonsoft.Json;
using QSDMS.Util;
using QX360.Business;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QX360.WeiXinWeb.Controllers
{
    public class ComplaintController : BaseController
    {
        [HttpPost]
        public JsonResult Send(string json)
        {
            var result = new ReturnMessage(false) { Message = "提交失败!" };
            try
            {
                var entity = JsonConvert.DeserializeObject<AdviseEntity>(json);
                if (entity == null)
                {
                    return Json(result);
                }

                entity.AdviseId = Util.NewUpperGuid();
                entity.ConnectName = LoginUser.NickName;
                entity.CreateId = LoginUser.UserId;
                entity.CreateTime = DateTime.Now;
                if (AdviseBLL.Instance.Add(entity))
                {

                    if (entity.AttachmentPicList != null)
                    {
                        int index = 0;
                        AttachmentPicBLL.Instance.DeleteByObjectId(entity.AdviseId);
                        foreach (var picitem in entity.AttachmentPicList)
                        {
                            if (picitem != null)
                            {
                                AttachmentPicEntity pic = new AttachmentPicEntity();
                                pic.PicId = Util.NewUpperGuid();
                                pic.PicName = picitem.PicName;
                                pic.SortNum = index;
                                pic.ObjectId = entity.AdviseId;
                                pic.Type = (int)QX360.Model.Enums.AttachmentPicType.投诉建议;
                                AttachmentPicBLL.Instance.Add(pic);
                            }
                            index++;
                        }
                    }
                }

                result.IsSuccess = true;
                result.Message = "提交成功";
            }
            catch (Exception ex)
            {
                ex.Data["Method"] = "ComplaintController>>Send";
                new ExceptionHelper().LogException(ex);
            }
            return Json(result);
        }

    }
}
