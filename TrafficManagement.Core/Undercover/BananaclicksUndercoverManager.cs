using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficManagement.Core.Undercover
{
  public class BananaclicksUndercoverManager : BananaclicksApiBase
  {

    // this method is obsolete because all results are one hour late
    public double? GetAffiliateProfit_Old(string affiliateID)
    {
      string url = "/v1/service/rest/statistics";
      DateTime nextDay = DateTime.Now.AddDays(1);
      string paramss = "?resultsPerPage=100000&page=1&sort=aff_sub&dateFrom=[currentDay]&dateTo=[nextDay]&affiliates={[affiliateID]}&dimensions={affiliate,conversiontype,connectiontype,mobileoperator,aff_sub,aff_sub1,aff_sub2,aff_sub3,aff_sub4,aff_sub5,adv_sub,adv_sub1,adv_sub2,adv_sub3,adv_sub4,adv_sub5,country,devicebrand,devicemodel,deviceos,time_hour,creative,trafficsupplier}&measures={clicks,conversions,conversionrates,payout,profit,revenue}"
        .Replace("[affiliateID]", affiliateID)
        .Replace("[currentDay]", string.Format("{0}-{1}-{2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day))
        .Replace("[nextDay]", string.Format("{0}-{1}-{2}", nextDay.Year, nextDay.Month, nextDay.Day));
      string response = this.Get(url, paramss);
      if (string.IsNullOrEmpty(response))
        return null;

      dynamic json = JsonConvert.DeserializeObject(response);
      double result = 0.0;
      foreach (var row in json)
        try
        {
          result += double.Parse(row.payout.ToString());
        }
        catch (Exception e) { }

      return result;
    }

    public double? GetAffiliateProfit(string affiliateID, DateTime? date = null)
    {
      if (date == null)
        date = DateTime.Today;

      string data = Get("/v1/service/rest/reports/rawConversions", "?date=[date]&affiliates={[aff]}&offers={}&additionalColumns={payout}&conversionType={Active,Test}"
        .Replace("[date]", string.Format("{0}-{1}-{2}", date.Value.Year, date.Value.Month, date.Value.Day))
        .Replace("[aff]", affiliateID));

      if (string.IsNullOrEmpty(data))
        return null;

      string[] lines = data.Split('\n');
      if (lines.Length < 1)
        return null;

      double result = 0.0;
      for (int i = 1; i < lines.Length; i++)
      {
        string[] split = lines[i].Split(',');
        double payout;
        if (double.TryParse(split[split.Length - 1].Replace("\"", string.Empty), out payout))
          result += payout;
      }

      return result;
    }

    public int? GetAffiliateCurrentTransactions(string affiliateID)
    {
      CCSubmitDirect ccdb = CCSubmitDirect.Instance;
      DateTime startDate = DateTime.Today.AddHours(-2);
      string startDateQuery = string.Format("{0}-{1}-{2} {3}:00:00", startDate.Year, startDate.Month, startDate.Day, startDate.Hour);
      DateTime endDate = startDate.AddDays(1);
      string endDateQuery = string.Format("{0}-{1}-{2} {3}:00:00", endDate.Year, endDate.Month, endDate.Day, endDate.Hour);

      string query = string.Format("SELECT COUNT(*) FROM livesports.cc_client WHERE affid={0} AND created>=\"{1}\" AND created<=\"{2}\" AND (has_subscription=1 OR (has_subscription=0 AND times_charged=1))  ORDER BY clientid;",
        affiliateID, startDateQuery, endDateQuery);

      int? result = ccdb.LoadInt(query);
      Console.WriteLine(affiliateID + " - " + result.Value);
      return result;
    }

    public double? GetAffiliateCurrentPrice(string affiliateID)
    {
      double? affPrice = this.GetAffiliateProfit_Old(affiliateID);
      if (!affPrice.HasValue)
        return null;

      int? currentTransactions = this.GetAffiliateCurrentTransactions(affiliateID);
      if (!currentTransactions.HasValue)
        return null;

      return affPrice.Value / (currentTransactions.Value * 1.0);
    }


    public string GetAffiliateByID(string id)
    {
      return this.Get("/v1/service/rest/affiliate/" + id);
    }

  }
}
