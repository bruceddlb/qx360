using iFramework.Framework;
using QSDMS.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace QSDMS.Application.Web.Resources.Ajax
{
    /// <summary>
    /// AjaxUploadFile 的摘要说明
    /// </summary>
    public class AjaxUploadFile : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            ReturnMessage result = new ReturnMessage(false) { Message = "上传视频失败!" };
            try
            {
                HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
                //没有文件上传，直接返回
                if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
                {
                    result.Message = "无效文件";
                    context.Response.Write(result.ToJson());
                }
                string FileEextension = Path.GetExtension(files[0].FileName);
                string virtualPath = string.Format("/File/Video/{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), FileEextension);
                string fullFileName =HttpContext.Current.Server.MapPath("~" + virtualPath);
                //创建文件夹，保存文件
                string path = Path.GetDirectoryName(fullFileName);
                Directory.CreateDirectory(path);
                files[0].SaveAs(fullFileName);

                result.ResultData["files"] = virtualPath;
                result.IsSuccess = true;
                result.Message = "上传成功";
                context.Response.Write(result.ToJson());
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                context.Response.Write(result.ToJson());
                new ExceptionHelper().LogException(ex);
            }

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}