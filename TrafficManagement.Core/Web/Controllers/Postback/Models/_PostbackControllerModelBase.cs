using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TrafficManagement.Core.Managers;

namespace TrafficManagement.Core.Web.Controllers.Postback.Models
{  
  public abstract class PostbackControllerModelBase
  {
    public ActionModelEvent Type { get; protected set; } = ActionModelEvent.Default;
    public abstract void Init(HttpRequestBase Request);

  }
}
