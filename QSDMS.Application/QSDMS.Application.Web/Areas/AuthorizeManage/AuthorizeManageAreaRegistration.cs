using System.Web.Mvc;

namespace QSDMS.Application.Web.Areas.AuthorizeManage
{
    public class AuthorizeManageAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "AuthorizeManage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
              this.AreaName + "_Default",
              this.AreaName + "/{controller}/{action}/{id}",
              new { area = this.AreaName, controller = "Home", action = "Index", id = UrlParameter.Optional },
              new string[] { "QSDMS.Application.Web.Areas." + this.AreaName + ".Controllers" }
            );
        }
    }
}
