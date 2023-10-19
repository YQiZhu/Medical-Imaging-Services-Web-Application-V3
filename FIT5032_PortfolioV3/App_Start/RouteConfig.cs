using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FIT5032_PortfolioV3
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "SelecClinicStaffDate",
                url: "Appointments/SelecClinicStaffDate",
                defaults: new { controller = "Appointments", action = "SelecClinicStaffDate" }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "SendEmail",
                url: "MedImages/SendEmail/{id}",
                defaults: new { controller = "MedImages", action = "SendEmail", id = UrlParameter.Optional }
            );
            
        }
    }
}
