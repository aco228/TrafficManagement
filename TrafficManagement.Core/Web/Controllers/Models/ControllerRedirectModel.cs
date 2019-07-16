using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficManagement.Core.Web.Controllers.Models
{
  public class ControllerRedirectModel : ControllerModelBase
  {
    public string msisdn { get; set; } = "";
    public string email { get; set; } = "";
    public string prelander { get; set; } = "";
    public string type { get; set; } = "";
    public string device_mf { get; set; } = "";
    public string device_model { get; set; } = "";
    public string device_os { get; set; } = "";
    public string carrier { get; set; } = "";
    public string isp { get; set; } = "";
    public string countryCode { get; set; } = "";
    public string ipAddress { get; set; } = "";
    public string host { get; set; } = "";
    public string request { get; set; } = "";
    public string referrer { get; set; } = "";
  }
}
