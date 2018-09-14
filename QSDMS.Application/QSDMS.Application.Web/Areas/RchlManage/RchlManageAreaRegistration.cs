using System.Web.Mvc;

namespace QSDMS.Application.Web.Areas.QX360Manage
{
    public class QX360ManageAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "QX360Manage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "QX360Manage_default",
                "QX360Manage/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
