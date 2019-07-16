using Direct.Core.DatabaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficManagement.Core
{
  public class PaywallDirect : DirectDatabaseMsSql
  {

    private static object LockObj = new object();
    private static PaywallDirect _instance = null;

    public PaywallDirect()
      : base("MobilePaywall", "core")
    {
      this.SetConnectionString("Data Source=192.168.11.104;Initial Catalog=MobilePaywall;uid=sa;pwd=m_q-6dGyRwcTf+b;");
    }

    public static PaywallDirect Instance
    {
      get
      {
        lock (LockObj)
        {
          if (_instance != null)
            return _instance;

          _instance = new PaywallDirect();
          return _instance;
        }
      }
    }

  }
}
