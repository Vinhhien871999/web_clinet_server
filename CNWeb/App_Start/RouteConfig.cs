using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CNWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "contact",
                url: "lien-he",
                defaults: new { controller = "Home", action = "Contact", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "logout",
               url: "dang-xuat",
               defaults: new { controller = "Home", action = "Logout", id = UrlParameter.Optional }
           );
            routes.MapRoute(
                name: "about",
                url: "ve-chung-toi",
                defaults: new { controller = "Home", action = "About", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "blog",
                url: "blog",
                defaults: new { controller = "Home", action = "Blog", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "xemchitiet",
                url: "san-pham/{tensp}-{id}",
                defaults: new { controller = "ProductDetail", action = "Single", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "register",
                url: "dang-ky",
                defaults: new { controller = "Home", action = "Register", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "login",
               url: "dang-nhap",
               defaults: new { controller = "Home", action = "Login", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "user",
               url: "thong-tin-ca-nhan",
               defaults: new { controller = "User", action = "Index", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "listaddress",
               url: "so-dia-chi",
               defaults: new { controller = "User", action = "ListAddress", id = UrlParameter.Optional }
           );

            routes.MapRoute(
               name: "infocard",
               url: "thong-tin-the",
               defaults: new { controller = "User", action = "InfoCard", id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "infopayment",
               url: "thong-tin-don-hang",
               defaults: new { controller = "User", action = "DonHang", id = UrlParameter.Optional }
           );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


            
        }
    }
}



