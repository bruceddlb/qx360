using System;
using System.Web;



namespace QSDMS.Application.Web.App_Code.UEditor
{

    /// <summary>
    /// Config 的摘要说明
    /// </summary>
    public class ConfigHandler : IUEditorHandle
    {
        public ConfigHandler() : base() { }

        public Object Process()
        {
            return (UEConfig.Items);
        }
    }

}