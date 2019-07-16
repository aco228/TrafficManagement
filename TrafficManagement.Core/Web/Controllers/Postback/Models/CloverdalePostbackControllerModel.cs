using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TrafficManagement.Core.Managers;

namespace TrafficManagement.Core.Web.Controllers.Postback.Models
{
  public class CloverdalePostbackControllerModel : PostbackControllerModelBase
  {
    public string firstname { get; set; } = "";
    public string lastname { get; set; } = "";
    public string address { get; set; } = "";
    public string email { get; set; } = "";
    public string zip { get; set; } = "";
    public string country { get; set; } = "";
    public string city { get; set; } = "";
    public string msisdn { get; set; } = "";
    public string clickid { get; set; } = "";
    public string pubid { get; set; } = "";
    public string _event { get; set; } = "";
    public string offer { get; set; } = "empty";


    public int aid { get; set; } = -1;

    public int _affid = -1;
    public string affid { get; set; } =  "";

    public override void Init(HttpRequestBase Request)
    {
      this._event = Request["event"] != null ? Request["event"].ToString().ToLower() : string.Empty;
      this.Type = this._event.Equals("settle") ? ActionModelEvent.Charge : ActionModelEvent.Create;
      int.TryParse(affid, out _affid);
    }
  }
}
