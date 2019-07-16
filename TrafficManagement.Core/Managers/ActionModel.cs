using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficManagement.Core.Web.Controllers.Models;

namespace TrafficManagement.Core.Managers
{
  public enum ActionModelEvent { Default, Redirection, Create, InputEmail, InputContact, Subscribe, Chargeback, Refund, Charge, Upsell, EndFlow }
  public enum ActionService
  {
    Default = 0,
    Dogbain1 = 1,
    Dogbain2 = 2,
    Invoreak = 3,
    CCCash = 4,
    Cloverdale = 5
  }

  public class ActionModel
  {
    public CCSubmitDirect Database { get; set; } = null;
    public ActionService Service { get; set; } = ActionService.Default;
    public ActionModelEvent Event { get; set; } = ActionModelEvent.Default;

    public int ActionID { get; set; } = -1;
    public int? AffID { get; set; } = null;
    public string PubID { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Msisdn { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ClickID { get; set; } = string.Empty;
    public string LanderName { get; set; } = string.Empty;
    public string LanderUrl { get; set; } = string.Empty;
    public string ProviderRedirection { get; set; } = string.Empty;

    // prelander properties
    public string prelander { get; set; } = "";
    public string PrelanderType { get; set; } = "";
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
    
    public string SQLUpdateQuery = string.Empty;


    public static ActionService Convert(string input) => (ActionService) Enum.Parse(typeof(ActionService), input);

    public static ActionModel ConvertFromRedirectInput(ControllerRedirectModel inputModel)
    {
      return new ActionModel()
      {
        Msisdn = inputModel.msisdn,
        Email  =inputModel.email,
        prelander = inputModel.prelander,
        PrelanderType = inputModel.type,
        device_mf = inputModel.device_mf,
        device_model = inputModel.device_model,
        device_os = inputModel.device_os,
        carrier = inputModel.carrier,
        isp = inputModel.isp,
        countryCode = inputModel.countryCode,
        ipAddress = inputModel.ipAddress,
        host = inputModel.host,
        request = inputModel.request,
        referrer = inputModel.referrer
      };
    }

  }
}
