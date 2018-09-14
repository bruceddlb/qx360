using System.Web.Mvc;

namespace QSDMS.Application.Web.Areas.AccountMange
{
    public class AccountMangeAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "AccountMange";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "AccountMange_default",
                "AccountMange/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
