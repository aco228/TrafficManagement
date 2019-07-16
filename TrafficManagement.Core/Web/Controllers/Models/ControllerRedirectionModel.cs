using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficManagement.Core.Managers;

namespace TrafficManagement.Core.Web.Controllers.Models
{
  public class ControllerRedirectionModel : ControllerModelBase
  {
    public string aid { get; set; } = string.Empty;
    public string lxid { get; set; } = "";
    public string pp { get; set; } = "";
    public string landerName { get; set; } = string.Empty;
    public int? affid { get; set; }
    public string pubid { get; set; } = "";
    public string url { get; set; } = string.Empty;

    public string prelander { get; set; } = "";
    public string type { get; set; } = "";
    public string referrer { get; set; } = "";
    public string host { get; set; } = "";

    public int LeadActionID => Convert(this.aid);
    public ActionService Service => ActionModel.Convert(this.pp);
  }
}
