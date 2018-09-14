using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QSDMS.Application.Web.Resources.Ajax
{
    /// <summary>
    /// AjaxImportState 的摘要说明
    /// </summary>
    public class AjaxImportState : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request["action"] as string;
            string cacheKey = context.Request["cacheid"];

            if (action != null && action.Equals("queryProcessing"))
            {
                string state = string.Empty;
                if (HttpRuntime.Cache[cacheKey + "-state"] == null)
                {
                    state = "processing" + "|" + (HttpRuntime.Cache[cacheKey + "-row"] as string);
                }
                else
                {
                    state = (HttpRuntime.Cache[cacheKey + "-state"] as string) + "|" + (HttpRuntime.Cache[cacheKey + "-row"] as string);
                }
                context.Response.Write(state);
                context.Response.End();
                return;
            }
            else if (action != null && action.Equals("removeCache"))
            {
                HttpRuntime.Cache.Remove(cacheKey + "-state");
                HttpRuntime.Cache.Remove(cacheKey + "-row");
                context.Response.Write("");
                return;
            }
            else
            {
                context.Response.Write("error");
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