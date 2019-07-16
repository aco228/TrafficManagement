using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TrafficManagement.Core.Managers;

namespace TrafficManagement.Core.Web.Controllers.Postback.Models
{
  public class CCCashPostbackControllerModel : PostbackControllerModelBase
  {
    public string firstname { get; set; } = "";
    public string lastname { get; set; } = "";
    public string address { get; set; } = "";
    public string email { get; set; } = "";
    public string zip { get; set; } = "";
    public string country { get; set; } = "";
    public string city { get; set; } = "";
    public string clickid { get; set; } = "";
    public string subtracking { get; set; } = "";
    public string _event { get; set; } = "";

    public int aid { get; set; } = -1;
    public int affid { get; set; } = -1;
    public string offer { get; set; } = "";
    public string pubid { get; set; } = "";
    public string msisdn { get; set; } = "";
    
    public override void Init(HttpRequestBase Request)
    {
      this._event = Request["event"] != null ? Request["event"].ToString().ToLower() : string.Empty;
      this.Type = this._event.Equals("join") ? ActionModelEvent.Charge : ActionModelEvent.Create;

      // load data from subtracking
      if (!string.IsNullOrEmpty(this.subtracking))
      {
        string[] subtracking_info = this.subtracking.Split('_');
        if (subtracking_info.Length == 0)
          return;

        if (subtracking_info.Length > 0)
          this.offer = subtracking_info[0];

        int affid_num = -1;
        if (subtracking_info.Length > 1 && int.TryParse(subtracking_info[1], out affid_num))
          this.affid = affid_num;

        if (subtracking_info.Length > 2)
          this.pubid = subtracking_info[2];

        if (subtracking_info.Length > 3)
          this.msisdn = subtracking_info[3];

        int aid_num = -1;
        if (subtracking_info.Length > 4 && int.TryParse(subtracking_info[4], out aid_num))
          this.aid = aid_num;

      }
    }
  }
}
