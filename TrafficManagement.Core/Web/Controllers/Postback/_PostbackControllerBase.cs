using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TrafficManagement.Core.Undercover;

namespace TrafficManagement.Core.Web.Controllers.Postback
{

  public class PostbackControllerBase : Controller
  {
    private CCSubmitDirect _database = null;
    protected CCSubmitDirect Database
    {
      get
      {
        if (this._database != null)
          return this._database;
        this._database = CCSubmitDirect.Instance;
        return this._database;
      }
    }

    #region # log #

    private int? _logID = -1;
    private MobilePaywallLogDirect db = null;

    protected void CreateLog()
    {
      db = MobilePaywallLogDirect.Instance;
      this._logID = this.db.Execute("INSERT INTO MobilePaywallLog.log.CCSubmitLog (Url) VALUES ('" + HttpContext.Request.RawUrl.ToString() + "');");
    }

    protected void Log(string text)
    {
      if (!this._logID.HasValue)
        return;
      this.db.Execute("INSERT INTO MobilePaywallLog.log.CCSubmitLogData (CCSubmitLogID, Text) VALUES (" + this._logID.Value + ", '" + text + "')");
    }

    #endregion

    // SUMMARY: Send postback informations to every other instance (legacy, bananaclicks, undercover)
    // Return if conversion is stolen
    protected bool SystemPostback(string clickid, string email = "", string username = "", string password = "")
    {
      bool _isStolen = false;
      this.Legacy_Subscribe(clickid, email, username, password);

      UndercoverResult undercover = CCUndercoverAgent.Init(clickid);
      if (!undercover.DontSendConversionToBananaclicks)
      {
        this.Log("This transaction will not be stolen and will be sent to banana ::clickid=" + clickid);
        this.SendPostbackToBananaclicks(clickid);
      }
      else
      {
        _isStolen = true;
        this.Log("THIS TRANSACTION WILL BE STOLEN ::clickid=" + clickid);
      }

      this.Legacy_Events(clickid, "settle", HttpContext.Request.RawUrl.ToString(), false, _isStolen);
      return _isStolen;
    }

    #region # LEGACY #

    // SUMMARY: Legacy method for inserting data into ls_client
    protected void Legacy_Subscribe(string clickid, string email = "", string username = "", string password = "")
    {
      if (!string.IsNullOrEmpty(email))
        email = string.Format("email='{0}',", email);
      if (!string.IsNullOrEmpty(username))
        username = string.Format("username='{0}',", username);
      if (!string.IsNullOrEmpty(password))
        password = string.Format("password='{0}',", password);

      this.Database.Execute($"UPDATE livesports.cc_client SET {email} {username} {password} clickid='{clickid}' WHERE clickid='{clickid}'");
    }
    
    protected void Legacy_Events(string clickid, string eventName, string callback_url, bool isUpsell, bool isStolen)
    {
      int? clientid = this.Database.LoadInt("SELECT clientid FROM livesports.cc_client WHERE clickid={0};", clickid);
      if (!clientid.HasValue)
        return;

      string additionalQuery = "";
      if (!string.IsNullOrEmpty(eventName))
        additionalQuery += string.Format(" status='{0}', ", eventName);
      if (eventName.Equals("subscribed"))
        additionalQuery += " has_subscription=1,";
      if (eventName.Equals("settle"))
        additionalQuery += " times_charged=times_charged+1,";
      if (eventName.Equals("chargeback"))
        additionalQuery += " has_chargeback=1,";
      if (eventName.Equals("credit"))
        additionalQuery += " has_refund=1,";

      if (isUpsell)
        additionalQuery += " times_upsell=times_upsell+1, ";
      if (isStolen)
        additionalQuery += " is_stolen=1, ";

      additionalQuery += "updated=CURRENT_TIMESTAMP";

      this.Database.Execute($"UPDATE livesports.cc_client SET {additionalQuery} WHERE clientid={clientid.Value}");
      this.Database.Execute($"INSERT INTO livesports.cc_event (clientid, name, url) VALUES ({clientid.Value}, '{eventName}', '{System.Net.WebUtility.UrlEncode(callback_url)}');");
    }

    public ActionResult Baza()
    {
      DirectContainer dc = MobilePaywallLogDirect.Instance.LoadContainer("SELECT TOP 1000 * FROM MobilePaywallLog.log.CCSubmitLog WHERE CCSubmitLogID>=11 ORDER BY CCSubmitLogID DESC;");
      string result = "";

      if (dc == null || !dc.HasValue)
        return this.Content("Database returned NULL.");

      foreach (var row in dc.Rows)
      {
        result += string.Format("<span style=\"color:green;display: inline-block; width:55px;\">{0}:</span>", row.GetString("CCSubmitLogID"));
        result += string.Format("<span style=\"color:blue;display: inline-block; width:170px;\">{0}:</span>", row.GetString("Created"));
        result += string.Format("<span style=\"\">{0}:</span>", row.GetString("Url"));
        result += string.Format("<br>");
      }

      return this.Content(result);
    }

    #endregion

    // SEND POSTBACK TO BANANA
    private void SendPostbackToBananaclicks(string transaction_id)
    {
      if (string.IsNullOrEmpty(transaction_id))
      {
        this.Log("Transaction IS NULL so callback to banana will not be sent");
        return;
      }

      string postbackLink = "http://conversions.bananaclicks.com/?transaction_id=" + transaction_id;
      this.Log("BANANA: " + postbackLink);
      HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(postbackLink);
      webRequest.AllowAutoRedirect = false;
      HttpWebResponse response;
      HttpStatusCode statusCode;

      try
      {
        response = (HttpWebResponse)webRequest.GetResponse();
      }
      catch (WebException we)
      {
        this.Log("Banana postback returned expection: " + we.Message);
        return;
      }
    }
  }
}
