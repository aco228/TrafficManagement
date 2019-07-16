using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TrafficManagement.Core.Managers;

namespace TrafficManagement.Core.Web.Controllers.Postback.Models
{
  public class DogbainPostbackControllerModel : PostbackControllerModelBase
  {
    public string txid { get; set; } = "";
    public string zone { get; set; } = "";
    public string ievent { get; set; } = "";
    public string email { get; set; } = "";
    public string username { get; set; } = "";
    public string password { get; set; } = "";
    
    public override void Init(HttpRequestBase Request)
    {
      this.ievent = Request["event"] != null ? Request["event"].ToString().ToLower() : "";
      if (ievent.Equals("initial"))
        ievent = "settle";

      this.Type = ActionModelEvent.Create;
      if (ievent.Equals("subscribed"))
        this.Type = ActionModelEvent.Subscribe;
      if (ievent.Equals("settle"))
        this.Type = ActionModelEvent.Charge;


    }
  }
}
