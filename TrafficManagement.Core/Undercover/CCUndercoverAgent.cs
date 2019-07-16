using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TrafficManagement.Core.Undercover
{
  public class CCUndercoverAgent
  {
    public static UndercoverResult Init(string clickID)
    {
      CCUndercoverAgent.Log(clickID, "CC_NEW:: Starting for : " + clickID);
      UndercoverResult result = new UndercoverResult();
      CCSubmitDirect db = CCSubmitDirect.Instance;
      DirectContainer dc = db.LoadContainer(string.Format("SELECT affid, pubid, referrer FROM livesports.cc_client WHERE clickid='{0}' ORDER BY clientid DESC LIMIT 1;", clickID));
      if (!dc.HasValue)
      {
        CCUndercoverAgent.Log(clickID, "CC_NEW:: There is no entries for clickID: " + clickID);
        return result;
      }

      string affID = !string.IsNullOrEmpty(dc.GetString("affid")) ? dc.GetString("affid") : GetParamByName(dc.GetString("referrer"), "affiliate_id");
      string pubID = !string.IsNullOrEmpty(dc.GetString("pubid")) ? dc.GetString("pubid") : GetParamByName(dc.GetString("referrer"), "utm_campaign");

      if (string.IsNullOrEmpty(affID))
      {
        CCUndercoverAgent.Log(clickID, "CC_NEW:: There is no AffID for clickID: " + clickID);
        return result;
      }

      DirectContainer _directContainer;
      if (!string.IsNullOrEmpty(pubID))
        _directContainer = db.LoadContainer("SELECT * FROM [].cc_undercover WHERE (aff_id={0} AND pub_id={1}) OR (aff_id={0} AND pub_id IS NULL) ORDER BY pub_id DESC LIMIT 1;", int.Parse(affID), pubID);
      else
        _directContainer = db.LoadContainer("SELECT * FROM [].cc_undercover WHERE (aff_id={0} AND pub_id IS NULL) ORDER BY pub_id DESC LIMIT 1;", int.Parse(affID));


      if (_directContainer.HasValue && _directContainer.GetDouble("tcost").HasValue)
        try
        {
          return GetByVariableTCost(_directContainer, affID);
        }
        catch (Exception e)
        {
          Log(clickID, "[FATAL WITH tcost] " + e.ToString());
        }

      Log(clickID, "CCUndercover will go to old way, for clickID = " + clickID + ", affID=" + affID);
      return result;
    }

    private static UndercoverResult GetByVariableTCost(DirectContainer container, string affID)
    {
      UndercoverResult result = null;
      var banana = new BananaclicksUndercoverManager();
      double tcost = container.GetDouble("tcost").Value;

      //
      // Affiliate price from bananaclicks

      double? affPrice = banana.GetAffiliateProfit(affID);
      if (!affPrice.HasValue)
      {
        Log(string.Empty, "[FATAL]:: THERE IS NO affPrice for affiliate=" + affID);
        return null;
      }
      affPrice = DollarConversion.Convert(affPrice.Value);

      //
      // Current transactions from database

      int? currentTransactions = banana.GetAffiliateCurrentTransactions(affID);
      if (!currentTransactions.HasValue)
      {
        Log(string.Empty, "[FATAL]:: THERE IS NO currentTransactions for affiliate=" + affID);
        return null;
      }
      else
        currentTransactions += 1;


      //
      // Current price [bananaclicks price] / [conversions from database]

      double current_price = 0.0;
      if (affPrice.Value == 0.0 || affPrice.Value > 0 && currentTransactions.Value == 1)
        current_price = 0.0;
      else
        current_price = affPrice.Value / (currentTransactions.Value * 1.0);

      string logString = string.Format("affID={0}, tcost={1}, affiliate_profit={2}, current_price={3}, current_transactions={4} ", affID, tcost, Math.Round(affPrice.Value, 2), Math.Round(current_price, 2), currentTransactions.Value);

      //
      // Logic for undercover

      if (current_price == 0 || current_price <= tcost)
      {
        logString += " (REPORT)";
        result = UndercoverResult.SendToBananaclicks();
      }
      else
      {
        logString += " THIS WILL NOT BE REPORTED";
        result = new UndercoverResult() { DontSendConversionToBananaclicks = true };
      }

      Log(string.Empty, logString);
      return result;
    }

    private static string GetParamByName(string input, string parameterName)
    {
      Match match = new Regex(string.Format(@"(\?{0}=([A-Za-z0-9]+))|(\&{0}=([A-Za-z0-9]+))", parameterName)).Match(input);
      string result = string.Empty;
      if (!string.IsNullOrEmpty(match.Groups[2].Value.ToString()))
        result = match.Groups[2].Value.ToString();
      else if (string.IsNullOrEmpty(result) && !string.IsNullOrEmpty(match.Groups[4].Value.ToString()))
        result = match.Groups[4].Value.ToString();
      return result;
    }
    public static void Log(string clickid, string text)
    {
      PaywallDirect.Instance.Execute(string.Format("INSERT INTO MobilePaywall.core.UndercoverLog (UndercoverOfferID, Text) VALUES ({0}, '{1}')",
        "NULL", "VARIABLE:: " + text));
    }
  }
}
