using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficManagement.Core.Managers;

namespace TrafficManagement.Core.Web.Controllers.Models
{
  

  public class ControllerCreateUserModel : ControllerModelBase
  {
    public string aid { get; set; } = "";
    public string ga_cid { get; set; } = "";
    public string email { get; set; } = "";
    public string password { get; set; } = "";
    public string country { get; set; } = "";
    public string lxid { get; set; } = "";
    public string pp { get; set; } = "";
    public int? affid { get; set; }
    public string pubid { get; set; } = "";
    public string landerName { get; set; } = "";

    public string prelander { get; set; } = "";
    public string type { get; set; } = "";
    public string referrer { get; set; } = "";
    public string host { get; set; } = "";

    public int LeadActionID => Convert(this.aid);
    public ActionService Service => ActionModel.Convert(this.pp);
  }
  
}
