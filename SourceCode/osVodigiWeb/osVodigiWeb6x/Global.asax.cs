using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using osVodigiWeb6x.Controllers;
using osVodigiWeb6x.Exceptions;

namespace osVodigiWeb6x
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        /* 
        public void Application_Error(Object sender, EventArgs e)
        {
            
            Exception exception = Server.GetLastError();
            Server.ClearError();

            var routeData = new RouteData();
            routeData.Values.Add("controller", "ApplicationError");
            routeData.Values.Add("action", "Error");
            routeData.Values.Add("exception", exception);

            if (exception.GetType() == typeof(HttpException))
            {
                routeData.Values.Add("statusCode", ((HttpException)exception).GetHttpCode());
            }
            else if (exception.GetType() == typeof(NotAuthcException))
            {
                routeData.Values.Add("statusCode", 401);
                // 401 Unauthorized - Authentication is required
            }
            else if (exception.GetType() == typeof(NotAuthzException))
            {
                routeData.Values.Add("statusCode", 403);
                // 403 Forbidden - Permission is required
            }
            else
            {
                routeData.Values.Add("statusCode", 500);
            }

            Console.WriteLine("EXCEPTION handled by Application_Error: {0}", exception.ToString());

            Response.TrySkipIisCustomErrors = true;
            IController controller = new ApplicationErrorController();
            controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
            Response.End();
            
        }*/
    }
}