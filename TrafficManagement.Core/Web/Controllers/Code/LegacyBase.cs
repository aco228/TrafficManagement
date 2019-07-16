using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TrafficManagement.Core.Web.Controllers.Code
{
  public abstract class LegacyBase
  {
    protected CCSubmitDirect Database = null;
    protected HttpRequestBase Request = null;

    public LegacyBase(CCSubmitDirect db, HttpRequestBase request)
    {
      this.Database = db;
      this.Request = request;
    }

    public abstract void Run();

  }
}
