using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TrafficManagement.Core.Undercover
{
  public class DollarConversion
  {
    private static double? _currentValue = null;
    private static DateTime? _lastConversion = null;

    public static double Convert(double dollar)
    {
      if (!_currentValue.HasValue || !_lastConversion.HasValue || (DateTime.Now - _lastConversion.Value).TotalDays > 1)
      {
        double? result = Request();
        if (result.HasValue)
        {
          _currentValue = result.Value;
          _lastConversion = DateTime.Now;
        }
        else
          _currentValue = 0.8877840909; // value in case there is problem with api
      }

      return dollar * _currentValue.Value;
    }


    private static double? Request()
    {
      string finalUrl = "https://api.exchangeratesapi.io/latest?base=USD";
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(finalUrl);
      try
      {
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
        {
          string htmlResponse = reader.ReadToEnd();
          dynamic json = JsonConvert.DeserializeObject(htmlResponse);
          return json["rates"]["EUR"];
        }
      }
      catch (Exception e)
      {
        return null;
      }
    }


  }
}
