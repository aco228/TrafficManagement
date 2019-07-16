using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace TrafficManagement.Core.Web.Controllers.Code.Filters
{
  public class AllowCrossSiteAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
      filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Headers", "*");
      filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Credentials", "true");
      filterContext.RequestContext.HttpContext.Response.AddHeader("Vary", "Origin");

      base.OnActionExecuting(filterContext);
    }
  }
}
