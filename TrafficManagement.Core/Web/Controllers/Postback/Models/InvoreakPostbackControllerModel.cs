using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TrafficManagement.Core.Web.Controllers.Postback.Models
{
  public class InvoreakPostbackControllerModel : PostbackControllerModelBase
  {
    public string orderid { get; set; } = "";
    public string result { get; set; } = "";
    public string message { get; set; } = "";
    
    public override void Init(HttpRequestBase Request)
    {
      this.Type = this.message.Equals("transaction successful") ? Managers.ActionModelEvent.Charge : Managers.ActionModelEvent.Subscribe;
    }
  }
}
