using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TrafficManagement.Core.Managers;
using TrafficManagement.Core.Web.Controllers.Postback.Models;

namespace TrafficManagement.Core.Web.Controllers.Postback
{
  public class DogbainPostbackController : PostbackControllerBase
  {

    [Route("callback")]
    public ActionResult Index(DogbainPostbackControllerModel input)
    {
      this.CreateLog();
      input.Init(this.Request);

      int? aid = ActionManager.GetActionID(input.txid, this.Database);
      if(aid.HasValue)
        ActionManager.ExecuteAction(new ActionModel()
        {
          Database = this.Database,
          Event = input.Type,
          ActionID = aid.Value,
          ClickID = input.txid
        });

      bool isStolen = false;
      if(input.Type == ActionModelEvent.Subscribe || input.Type == ActionModelEvent.Charge)
        isStolen = this.SystemPostback(input.txid, input.email, input.username, input.password);

      if(input.zone.ToLower().Contains("upsell"))
        this.Legacy_Events(input.txid, input.ievent, HttpContext.Request.RawUrl.ToString(), true, isStolen);

      return this.Content("");
    }

  }
}
