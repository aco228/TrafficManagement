using Direct.Core.DatabaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficManagement.Core
{
  public class MobilePaywallLogDirect : DirectDatabaseMsSql
  {
    public MobilePaywallLogDirect()
      : base("MobilePaywallLog", "log")
    {
      this.SetConnectionString("Data Source=192.168.11.104;Initial Catalog=MobilePaywallLog;uid=logMobilePaywall;pwd=Fv36dwC22x6GpYu;");
    }

    public static MobilePaywallLogDirect Instance
    {
      get
      {
        return new MobilePaywallLogDirect();
      }
    }

  }
}
