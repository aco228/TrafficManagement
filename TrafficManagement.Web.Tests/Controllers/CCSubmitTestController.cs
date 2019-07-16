using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TrafficManagement.Core.Web.Controllers;
using TrafficManagement.Core.Web.Controllers.Code.Filters;
using TrafficManagement.Core.Web.Controllers.Models;
using TrafficManagement.Web.Tests.Models;

namespace TrafficManagement.Web.Tests.Controllers
{
  //[RequireHttps]
  [AllowCrossSite]
  public class CCSubmitTestController : Controller
  {
    // GET: CCSubmitTest
    public ActionResult Index() => Content("Ok");

    public ActionResult CompleteFlow()
    {

      var baseUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));
      if (baseUrl.Contains("https://"))
        baseUrl = baseUrl.Replace("https://", "");

      var hostUrl = baseUrl + "/CCSubmitTest/ToLander/?";
      var refererUrl = baseUrl + "/CCSubmitTest/CompleteFlow/?lp=l7&aid=&dbg=testFlow&s=test&c=22&lxid=-1";

      var prelanderParams = new ControllerRedirectModel()
      {
        //Email = "test@api.com",
        host = hostUrl,
        //Msisdn = "000 000 000",
        type = "lander",
        prelander = "lander",
        referrer = refererUrl,
        // 'request' - query params from referer
        request = "lp=l7&dbg=true&s=test&c=22&lxid=-1"
      };

      //var controller = DependencyResolver.Current.GetService<CCSubmitApiController>();
      //controller.ControllerContext = new ControllerContext(this.Request.RequestContext, controller);

      //return RedirectToAction("Index", "CCSubmitApi");
      return RedirectToAction("Redirection", "CCSubmitApi", prelanderParams); 
    }

    [HttpGet]
    public ActionResult ToLander(ControllerRedirectModel redirectModel)
    {
      //return Redirect("http:google.com");

      var requestParams = "";
      var type = "type=" + redirectModel.type;
      var prelander = "&prelander=" + redirectModel.prelander;


      requestParams += type;
      requestParams += prelander;

      var landerUrl = "http://dusan.landing/sports/s10-giveaway/?";
      return Redirect(landerUrl + requestParams);
    }

    [HttpGet]
    [Route("CCSubmitTest/create_user")]
    public ActionResult CreateUser(ControllerCreateUserModel createUserModel)
    {
      return RedirectToAction("CreateUser", "CCSubmitApi", createUserModel);
    }

    [HttpGet]
    [Route("CCSubmitTest/subscribe")]
    public ActionResult Subscribe(ControllerSubscribeModel subscribeModel)
    {
      return RedirectToAction("Subscribe", "CCSubmitApi", subscribeModel);
    }

    [HttpGet]
    [Route("CCSubmitTest/end_flow")]
    public ActionResult EndFLow(ControllerEndFlowModel endFlowModel)
    {

      return RedirectToAction("EndFlow", "CCSubmitApi", endFlowModel);
    }

  }

}