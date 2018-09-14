using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSMS.API.Weixin.TemplateMsg
{
    public class Config
    {
        //=======【基本信息设置】=====================================
        /* 微信公众号信息配置
        * APPID：绑定支付的APPID（必须配置）
        * APPSECRET：公众帐号secert
        */
        public static string APPID = ConfigurationManager.AppSettings["APPID"];// "wx5b82e2816301151f";      

        public static string APPSECRET = ConfigurationManager.AppSettings["APPSECRET"]; //"84f22b93d745a5658dfd48a5f7688979";

    }
}
