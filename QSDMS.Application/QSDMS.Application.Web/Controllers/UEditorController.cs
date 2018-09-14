using QSDMS.Application.Web.App_Code.UEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace QSDMS.Application.Web.Controllers
{
    public class UEditorController : Controller
    {
        // GET: UEditor
        public ContentResult Handle()
        {
            UploadConfig config = null;
            IUEditorHandle handle = null;
            string action = Request["action"];
            switch (action)
            {
                case "config":
                    handle = new ConfigHandler();
                    break;
                case "uploadimage":
                    config = new UploadConfig()
                    {
                        AllowExtensions = UEConfig.GetStringList("imageAllowFiles"),
                        PathFormat = UEConfig.GetString("imagePathFormat"),
                        SizeLimit = UEConfig.GetInt("imageMaxSize"),
                        UploadFieldName = UEConfig.GetString("imageFieldName")
                    };
                    handle = new UploadHandle(config);
                    break;
                case "uploadvideo":
                    config = new UploadConfig()
                    {
                        AllowExtensions = UEConfig.GetStringList("videoAllowFiles"),
                        PathFormat = UEConfig.GetString("videoPathFormat"),
                        SizeLimit = UEConfig.GetInt("videoMaxSize"),
                        UploadFieldName = UEConfig.GetString("videoFieldName")
                    };
                    handle = new UploadHandle(config);
                    break;
                default:
                    handle = new NotSupportedHandler();
                    break;
            }

            var result = handle.Process();
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            return Content(jsonString);
        }

        [HttpPost]
        // GET: UEditor
        public ContentResult Handle(string action)
        {
            UploadConfig config = null;
            IUEditorHandle handle = null;
            action = Request["action"];
            switch (action)
            {
                case "config":
                    handle = new ConfigHandler();
                    break;
                case "uploadimage":
                    config = new UploadConfig()
                    {
                        AllowExtensions = UEConfig.GetStringList("imageAllowFiles"),
                        PathFormat = UEConfig.GetString("imagePathFormat"),
                        SizeLimit = UEConfig.GetInt("imageMaxSize"),
                        UploadFieldName = UEConfig.GetString("imageFieldName")
                    };
                    handle = new UploadHandle(config);
                    break;
                case "uploadtemplateimage":
                    
                default:
                    handle = new NotSupportedHandler();
                    break;
            }

            var result = handle.Process();
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            return Content(jsonString);
        }





        //  
        // GET: /Upload/  
        [HttpGet]
        public ActionResult Upload()
        {
            string url = Request.QueryString["url"];
            if (url == null)
            {
                url = "";
            }
            ViewData["url"] = url;
            return View();
        }

        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase filename)
        {
            //具体的保存代码  
            return View();
        }

    }
}
