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
  public class InvoreakPostbackController : PostbackControllerBase
  {

    public ActionResult Index(InvoreakPostbackControllerModel input)
    {
      this.CreateLog();
      input.Init(this.Request);

      if(input.result.Equals("1"))
      {
        this.Log("Invoreak returned result that is not 1");
        return this.Content("");
      };
      
      int? actionid = ActionManager.GetActionID(input.orderid, this.Database);
      if (actionid.HasValue)
      {
        ActionManager.ExecuteAction(new ActionModel()
        {
          Database = this.Database,
          Service = ActionService.Invoreak,
          Event = input.Type,
          ActionID = actionid.Value,
          ClickID = input.orderid
        });
      }

      if (input.Type == ActionModelEvent.Charge)
        this.SystemPostback(input.orderid);

      return this.Content("");
    }

  }
}
