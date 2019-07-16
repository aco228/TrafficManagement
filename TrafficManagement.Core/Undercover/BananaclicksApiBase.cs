using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TrafficManagement.Core.Undercover
{
  public class BananaclicksApiBase
  {
    private readonly string URL = "https://affiliate.bananaclicks.com";
    private readonly string API_KEY = "149f5cf5c5594eeb8fd1658230df04f7";
    private readonly string SECREC_KEY = "27c056f565a54d308514cdad036819c4";

    public BananaclicksApiBase()
    {

    }

    private string ComputeSha256Hash(string rawData)
    {
      HMACSHA256 hmac = new HMACSHA256(Encoding.ASCII.GetBytes(SECREC_KEY));
      string calc_sig = Convert.ToBase64String(hmac.ComputeHash(Encoding.ASCII.GetBytes(rawData)));
      return calc_sig;
    }

    public string Get(string method, string paramss = "")
    {
      string finalUrl = URL + method + paramss;
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(finalUrl);
      string token = string.Format("ApiKey {0}:{1}", API_KEY, ComputeSha256Hash(method));
      request.Headers["Authorization"] = token;
      //request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
      try
      {
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
          return reader.ReadToEnd();
      }
      catch (Exception e)
      {
        return string.Empty;
      }
    }

  }
}
