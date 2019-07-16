using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficManagement.Core.Web.Controllers.Models
{
  public abstract class ControllerModelBase
  {

    protected int Convert(string input)
    {
      int result = 0;
      if (int.TryParse(input, out result))
        return result;
      return -1;
    }

  }
}
