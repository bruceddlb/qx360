
using QSMS.API.Weixin.TemplateMsg;
using QX360.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QX360.Business
{
    /// <summary>
    /// 消息通知处理类
    /// </summary>
    public class SendWxMessage
    {
        /// <summary>
        /// 预约成功消息提醒
        /// </summary>
        /// <param name="toUser">openid</param>
        /// <param name="username">name</param>
        /// <param name="servcicetime">time</param>
        public static void SendSuccessNotice(string toUser, string username, string servcicetime, string content, string orderno)
        {
            try
            {
                var templateList = WxTemplateBLL.Instance.GetList(new WxTemplateEntity() { Call_index = "subsuccessnotice" });
                if (templateList != null && templateList.Count > 0)
                {
                    var template = templateList.FirstOrDefault();
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("first", "尊敬的 " + username + " 先生/女士");
                    data.Add("keyword1", content);
                    data.Add("keyword2", servcicetime);
                    data.Add("keyword3", orderno);
                    data.Add("remark", template.Remark);

                    WxMsgModel model = new WxMsgModel();
                    model.TemplateId = template.TemplateId;
                    model.OpenId = toUser;
                    model.LinkUrl = "";
                    SendTemplateMsg.SendMsg(model, data);
                }
            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// 取消消息模板
        /// </summary>
        /// <param name="toUser"></param>
        /// <param name="username"></param>
        /// <param name="servcicetime"></param>
        /// <param name="content"></param>
        /// <param name="orderno"></param>
        public static void SendCancelNotice(string toUser, string username, string servcicetime, string content, string orderno)
        {
            try
            {
                var templateList = WxTemplateBLL.Instance.GetList(new WxTemplateEntity() { Call_index = "cancelnotice" });
                if (templateList != null && templateList.Count > 0)
                {
                    var template = templateList.FirstOrDefault();
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("first", "尊敬的 " + username + " 先生/女士");
                    data.Add("keyword1", content);
                    data.Add("keyword2", servcicetime);
                    data.Add("keyword3", orderno);
                    data.Add("remark", template.Remark);

                    WxMsgModel model = new WxMsgModel();
                    model.TemplateId = template.TemplateId;
                    model.OpenId = toUser;
                    model.LinkUrl = "";
                    SendTemplateMsg.SendMsg(model, data);
                }
            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// 订单完成提醒
        /// </summary>
        /// <param name="toUser"></param>
        /// <param name="username"></param>
        /// <param name="servcicetime"></param>
        /// <param name="content"></param>
        /// <param name="orderno"></param>
        public static void SendFinishNotice(string toUser, string username, string servcicetime, string content, string orderno)
        {
            try
            {
                var templateList = WxTemplateBLL.Instance.GetList(new WxTemplateEntity() { Call_index = "finishnotice" });
                if (templateList != null && templateList.Count > 0)
                {
                    var template = templateList.FirstOrDefault();
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("first", "尊敬的 " + username + " 先生/女士");
                    data.Add("keyword1", content);
                    data.Add("keyword2", servcicetime);
                    data.Add("keyword3", orderno);
                    data.Add("remark", template.Remark);

                    WxMsgModel model = new WxMsgModel();
                    model.TemplateId = template.TemplateId;
                    model.OpenId = toUser;
                    model.LinkUrl = "";
                    SendTemplateMsg.SendMsg(model, data);
                }
            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// 内容改变提醒
        /// </summary>
        /// <param name="toUser"></param>
        /// <param name="username"></param>
        /// <param name="servcicetime"></param>
        /// <param name="content"></param>
        /// <param name="orderno"></param>
        public static void SendChangeNotice(string toUser, string username, string servcicetime, string content, string orderno)
        {
            try
            {
                var templateList = WxTemplateBLL.Instance.GetList(new WxTemplateEntity() { Call_index = "changenotice" });
                if (templateList != null && templateList.Count > 0)
                {
                    var template = templateList.FirstOrDefault();
                    Dictionary<string, string> data = new Dictionary<string, string>();
                    data.Add("first", "尊敬的 " + username + " 先生/女士");
                    data.Add("keyword1", content);
                    data.Add("keyword2", servcicetime);
                    data.Add("keyword3", orderno);
                    data.Add("remark", template.Remark);

                    WxMsgModel model = new WxMsgModel();
                    model.TemplateId = template.TemplateId;
                    model.OpenId = toUser;
                    model.LinkUrl = "";
                    SendTemplateMsg.SendMsg(model, data);
                }
            }
            catch (Exception ex)
            {

            }

        }

    }
}
