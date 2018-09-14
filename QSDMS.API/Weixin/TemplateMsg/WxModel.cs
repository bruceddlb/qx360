using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSMS.API.Weixin.TemplateMsg
{
    public class WxMsgModel
    {
        /// <summary>
        /// 消息副标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 接收人openid
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 消息模板
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// 连接
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        /// 扩展内容
        /// </summary>
        public string Content { get; set; }
    }
}
