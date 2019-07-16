using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficManagement.Core.Web.Controllers.Models
{
  public class ControllerJsonResponse
  {
    public bool status = false;
    public string message = "";
    public int? aid = null;

    public static ControllerJsonResponse Error(string message) => new ControllerJsonResponse() { status = false, message = message };
    public static ControllerJsonResponse Success(string message) => new ControllerJsonResponse() { status = true, message = message };
    public static ControllerJsonResponse Success(string message, int aid) => new ControllerJsonResponse() { status = true, message = message, aid = aid };

  }
}
