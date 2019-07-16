using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficManagement.Core.Web.Controllers.Models
{
  public class ControllerEndFlowModel : ControllerModelBase
  {
    public string aid { get; set; } = "";
    public string provider_redirection { get; set; } = "";

    public int LeadActionID => Convert(this.aid);
  }
}
