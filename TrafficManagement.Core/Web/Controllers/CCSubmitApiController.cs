using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TrafficManagement.Core.Managers;
using TrafficManagement.Core.Web.Controllers.Code;
using TrafficManagement.Core.Web.Controllers.Code.Filters;
using TrafficManagement.Core.Web.Controllers.Models;

namespace TrafficManagement.Core.Web.Controllers
{
  [AllowCrossSite]
  public class CCSubmitApiController : Controller
  {
    public ActionResult Index() => this.Content("ok");
    private ActionResult Result(bool status, string message, object data) => this.Json(new { status = status, message = message, data = data }, JsonRequestBehavior.AllowGet);
    private ActionResult Error(string text) => this.Json(ControllerJsonResponse.Error(text), JsonRequestBehavior.AllowGet);
    private ActionResult Success(string text) => this.Json(ControllerJsonResponse.Success(text), JsonRequestBehavior.AllowGet);
    private ActionResult Success(string text, int aid) => this.Json(ControllerJsonResponse.Success(text, aid), JsonRequestBehavior.AllowGet);



    // SUMMARY: Initial redirection from prelander
    public ActionResult Redirection(ControllerRedirectModel input)
    {
      if (string.IsNullOrEmpty(input.host) || string.IsNullOrEmpty(input.request))
        return this.Content("error");
      string redirect = "https://" + input.host + input.request;
      CCSubmitDirect db = CCSubmitDirect.Instance;
            
      int? aid = ActionManager.ExecuteAction(new ActionModel()
      {
        Database = db,
        ActionID = -1,
        Event = ActionModelEvent.Default,

        Msisdn = input.msisdn,
        Email = input.email,
        prelander = input.prelander,
        PrelanderType = input.type,
        device_mf = input.device_mf,
        device_model = input.device_model,
        device_os = input.device_os,
        carrier = input.carrier,
        isp = input.isp,
        countryCode = input.countryCode,
        ipAddress = input.ipAddress,
        host = input.host,
        request = input.request,
        referrer = input.referrer
      });

      if(!aid.HasValue)
        return this.Result(false, "error", null);
        
        
      db.Transactional.Run();
      return this.Redirect(redirect + "&aid=" + aid.Value);
    }

    [Route("redirection")]
    public ActionResult RedirectionToLander(ControllerRedirectionModel input)
    {
      CCSubmitDirect db = CCSubmitDirect.Instance;
      int? actionID = ActionManager.ExecuteAction(new ActionModel()
      {
        Database = db,
        ActionID = input.LeadActionID,
        AffID = input.affid,
        PubID = input.pubid,

        PrelanderType = input.type,
        prelander = input.prelander,
        LanderName = input.landerName,
        Service = input.Service,
        Event = ActionModelEvent.InputEmail,
        ClickID = input.lxid,
        ipAddress = Request.UserHostAddress,
      });

      db.Transactional.Run();
      if (!actionID.HasValue)
        return this.Error("aid error");

      return this.Success("success", actionID.Value);
    }
   
    // Create user [from landing page]
    [Route("create_user")]
    public ActionResult CreateUser(ControllerCreateUserModel input)
    {
      CCSubmitDirect db = CCSubmitDirect.Instance;

      if(string.IsNullOrEmpty(input.email))
        return this.Error("no_email");

      if(CCSubmitCacheManager.CheckIfEmailBlacklist(input.email))
        return this.Error("email_blacklist");

      // Legacy update
      (new LegacyCreateUser(db, this.Request, input)).Run();
      int? actionID = ActionManager.ExecuteAction(new ActionModel()
      {
        Database = db,
        ActionID = input.LeadActionID,
        AffID = input.affid,
        PubID = input.pubid,

        PrelanderType = input.type,
        prelander = input.prelander,
        LanderName = input.landerName,
        LanderUrl = Request.UrlReferrer != null ? Request.UrlReferrer.AbsolutePath : string.Empty,
        Service = input.Service, 
        Event = ActionModelEvent.InputEmail,
        ClickID = input.lxid,
        ipAddress = Request.UserHostAddress,

        Email = input.email
      });

      db.Transactional.Run();
      if (!actionID.HasValue)
        return this.Error("aid error");

      return this.Success("success", actionID.Value);
    }

    // SUMMARY: Subscribe [ from landing page ]
    [Route("subscribe")]
    public ActionResult Subscribe(ControllerSubscribeModel input)
    {
      CCSubmitDirect db = CCSubmitDirect.Instance;
      (new LegacySubscribe(db, this.Request, input)).Run();

      if(input.LeadActionID == -1)
        return this.Error("aid error");

      ActionManager.ExecuteAction(new ActionModel()
      {
        Database = db,
        Service = ActionModel.Convert(input.pp),
        Event = ActionModelEvent.InputContact,
        ActionID = input.LeadActionID,

        FirstName = input.firstname,
        LastName = input.lastname,
        Msisdn = input.msisdn,
        Country = input.country,
        Zip = input.zip
      });

      db.Transactional.Run();
      return this.Success("success", input.LeadActionID);
    }


    // SUMMARY: Subscribe [ from landing page ]
    [Route("end_flow")]
    public ActionResult EndFlow(ControllerEndFlowModel input)
    {
      CCSubmitDirect db = CCSubmitDirect.Instance;

      if(input.LeadActionID == -1)
        return this.Error("aid error");

      ActionManager.ExecuteAction(new ActionModel()
      {
        Database = db,
        Event = ActionModelEvent.EndFlow,
        ActionID = input.LeadActionID,
        ProviderRedirection = input.provider_redirection
      });
      
      db.Transactional.Run();
      return this.Success("success", input.LeadActionID);
    }



  }
}
