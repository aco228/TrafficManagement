using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficManagement.Core.Undercover
{
  public class UndercoverResult
  {
    public bool DontSendConversionToBananaclicks = false;

    public static UndercoverResult SendToBananaclicks() => new UndercoverResult() { DontSendConversionToBananaclicks = false };
    public static UndercoverResult DontSendToBananaclicks() => new UndercoverResult() { DontSendConversionToBananaclicks = true };
  }
}
