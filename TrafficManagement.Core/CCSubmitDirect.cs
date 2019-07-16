using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficManagement.Core
{
  public class CCSubmitDirect : Direct.Core.DatabaseTypes.DirectDatabaseMysql
  {
    private static object LockObj = new object();
    private static CCSubmitDirect _instance = null;

    public static CCSubmitDirect Instance
    {
      get
      {
        lock (LockObj)
        {
          //if (_instance != null)
          //  return _instance;

          _instance = new CCSubmitDirect();
          return _instance;
        }
      }
    }

    public CCSubmitDirect()
      : base("livesports")
    {
      this.SetConnectionString("Server=46.166.160.58; database=livesports; UID=livesports; password=a48i72V\"B?8>79Z");
    }

  }
}
