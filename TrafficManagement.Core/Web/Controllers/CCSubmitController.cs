using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TrafficManagement.Core.Managers;
using TrafficManagement.Core.Undercover;

namespace TrafficManagement.Core.Web.Controllers
{
  public class CCSubmitController : Controller
  {

    #region # log #

    private int? _logID = -1;
    private MobilePaywallLogDirect db = null;

    private void CreateLog()
    {
      db = MobilePaywallLogDirect.Instance;
      this._logID = this.db.Execute("INSERT INTO MobilePaywallLog.log.CCSubmitLog (Url) VALUES ('" + HttpContext.Request.RawUrl.ToString() + "');");
    }

    private void Log(string text)
    {
      if (!this._logID.HasValue)
        return;
      this.db.Execute("INSERT INTO MobilePaywallLog.log.CCSubmitLogData (CCSubmitLogID, Text) VALUES (" + this._logID.Value + ", '" + text + "')");
    }

    #endregion

    //[Route("test")]
    public ActionResult Index() => this.Content("ok");

    // CALLBACK -> David
    //[Route("callback")]
    public ActionResult callback()
    {
      this.CreateLog();

      string txid = Request["txid"] != null ? Request["txid"].ToString().ToLower() : "";
      string iEvent = Request["event"] != null ? Request["event"].ToString().ToLower() : "";
      string iZone = Request["zone"] != null ? Request["zone"].ToString().ToLower() : "";
      bool _isStolen = false;

      /*
      if (Request["success"] != null && Request["success"].ToString().Equals("1"))
        iEvent = "subscribed";
      */

      if (iEvent.Equals("initial"))
        iEvent = "settle";

      #region # NEW TRACKING DATABASE #

      /*
        NEW TRACKING 
      */

      CCSubmitDirect db = CCSubmitDirect.Instance;
      int? actionid = ActionManager.GetActionID(txid, db);
      ActionModelEvent action_event = ActionModelEvent.Create;
      if (iEvent.Equals("subscribed"))
        action_event = ActionModelEvent.Subscribe;
      if (iEvent.Equals("settle"))
        action_event = ActionModelEvent.Charge;

      if (actionid.HasValue)
      {
        ActionManager.ExecuteAction(new ActionModel()
        {
          Database = db,
          Event = action_event,
          ActionID = actionid.Value,
          ClickID = txid
        });
      }

      #endregion

      if (iEvent.Equals("subscribed") || iEvent.Equals("settle"))
      {
        string email = Request["email"] != null ? Request["email"].ToString() : string.Empty;
        string username = Request["username"] != null ? Request["username"].ToString() : string.Empty;
        string password = Request["password"] != null ? Request["password"].ToString() : string.Empty;
        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
          this.SendPostbackToSports_Subscribe(txid, username, password, email);

        /*
        string original_referrer = CCSubmitDirect.Instance.LoadString(string.Format("SELECT referrer FROM livesports.cc_client WHERE clickid='{0}' ORDER BY clientid DESC LIMIT 1;", txid));
        this.Log("Original referrer for this click is: " + original_referrer);
        Code.Undercover.UndercoverResult undercover = Callback.Code.Undercover.UndercoverAgent.ContinueWithReport_CC(original_referrer);
        */

        UndercoverResult undercover = CCUndercoverAgent.Init(txid);
        if (!undercover.DontSendConversionToBananaclicks)
        {
          this.Log("This transaction will not be stolen and will be sent to banana ::clickid=" + txid);
          this.SendPostbackToBananaclicks(txid);
        }
        else
        {
          _isStolen = true;
          this.Log("THIS TRANSACTION WILL BE STOLEN ::clickid=" + txid);
        }
      }

      if (iEvent.Equals("settle") || iEvent.Equals("initial") || iEvent.Equals("rebill") || iEvent.Equals("subscribed") || iEvent.Equals("chargeback"))
        this.SendPostbackToSports_Callback(txid, iEvent);

      bool is_upsell = iZone.ToLower().Contains("upsell");
      this.SendPostbackToSpots_CallbackInformation(txid, iEvent, HttpContext.Request.RawUrl.ToString(), is_upsell, _isStolen);
      return new HttpStatusCodeResult(200);
    }

    // CALLBACK -> Invoreak
    [Route("inveroak")]
    public ActionResult Inveroak()
    {
      this.CreateLog();
      string orderid = Request["orderid"] != null ? Request["orderid"].ToString() : string.Empty;
      string result = Request["result"] != null ? Request["result"].ToString() : string.Empty;
      string message = Request["message"] != null ? Request["message"].ToString() : string.Empty;
      this.Log(string.Format("Invoreak sent us: orderID={0}, result={1}, message={2}", orderid, result, message));

      if (!result.Equals("1"))
      {
        this.Log("Invoreak returned result that is not 1");
        return this.Content("");
      }

      /*
        NEW TRACKING 
      */

      //CCSubmitDirect db = CCSubmitDirect.Instance;
      //int? actionid = ActionManager.GetActionID(orderid, db);
      //ActionModelEvent action_event = ActionModelEvent.Create;
      //if (message.Equals("transaction successful"))
      //  action_event = ActionModelEvent.Charge;

      //if(actionid.HasValue)
      //{
      //  ActionManager.ExecuteAction(new ActionModel()
      //  {
      //    Database = db,
      //    Service = ActionService.Invoreak,
      //    Event = action_event,
      //    ActionID = actionid.Value,
      //    ClickID = orderid
      //  });
      //}

      if (!message.Equals("transaction successful"))
      {
        this.Log("Invoreak returned status: " + message + ", which is not succ trasanction");
        return this.Content("");
      }

      bool _isStolen = false;
      this.SendPostbackToSports_Subscribe(orderid, "", "", "");

      UndercoverResult undercover = CCUndercoverAgent.Init(orderid);
      if (!undercover.DontSendConversionToBananaclicks)
      {
        this.Log("This transaction will not be stolen and will be sent to banana ::clickid=" + orderid);
        this.SendPostbackToBananaclicks(orderid);
      }
      else
      {
        _isStolen = true;
        this.Log("THIS TRANSACTION WILL BE STOLEN ::clickid=" + orderid);
      }

      this.SendPostbackToSpots_CallbackInformation(orderid, "settle", HttpContext.Request.RawUrl.ToString(), false, _isStolen);
      return this.Content("");
    }

    // CALLBACK -> CCCash
    //[Route("cccashcallback")]
    public ActionResult cccashcallback()
    {
      this.CreateLog();
      string firstname = Request["firstname"] != null ? Request["firstname"].ToString() : string.Empty;
      string lastname = Request["lastname"] != null ? Request["lastname"].ToString() : string.Empty;
      string address = Request["address"] != null ? Request["address"].ToString() : string.Empty;
      string email = Request["email"] != null ? Request["email"].ToString() : string.Empty;
      string zip = Request["zip"] != null ? Request["zip"].ToString() : string.Empty;
      string country = Request["country"] != null ? Request["country"].ToString() : string.Empty;
      string city = Request["city"] != null ? Request["city"].ToString() : string.Empty;
      string clickid = Request["clickid"] != null ? Request["clickid"].ToString() : string.Empty;
      string ievent = Request["event"] != null ? Request["event"].ToString().ToLower() : string.Empty;

      string offer = "";
      int affid = -1;
      int aid = -1;
      string pubid = "";
      string msisdn = "";
      

      string subtracking = Request["subtracking"] != null ? Request["subtracking"].ToString().ToLower() : string.Empty;
      if (!string.IsNullOrEmpty(subtracking))
      {
        string[] subtracking_info = subtracking.Split('_');
        if (subtracking_info.Length == 3)
        {
          offer = subtracking_info[0];
          int.TryParse(subtracking_info[1], out affid);
          pubid = subtracking_info[2];
        }
        if (subtracking_info.Length == 4)
        {
          offer = subtracking_info[0];
          int.TryParse(subtracking_info[1], out affid);
          pubid = subtracking_info[2];
          msisdn = subtracking_info[3];
        }
        if(subtracking_info.Length == 5)
        {
          offer = subtracking_info[0];
          int.TryParse(subtracking_info[1], out affid);
          pubid = subtracking_info[2];
          msisdn = subtracking_info[3];
          if (!string.IsNullOrEmpty(subtracking_info[4]))
            int.TryParse(subtracking_info[4], out aid);
        }
      }


      this.Log($"CCCash: sent us: firstname:{firstname}, lastname:{lastname}, address:{address}, zip:{zip}, country:{country}, city:{city}, clickid:{clickid}");
      //CCSubmitDirect.Instance.Execute($"UPDATE livesports.cc_client SET country='{country}', firstname='{firstname}', lastname='{lastname}', address='{address}', zip='{zip}', city='{city}' WHERE clickid='{clickid}'");

      CCSubmitDirect db = CCSubmitDirect.Instance;


      #region # NEW TRACKING DATABSE #

      /*
          NEW TRACKING MANAGER
      */

      ActionModelEvent action_event = ActionModelEvent.Create;
      if (!string.IsNullOrEmpty(ievent) && ievent.Equals("join"))
        action_event = ActionModelEvent.Charge;

      if (aid != -1)
        ActionManager.ExecuteAction(new ActionModel()
        {
          Database = db,
          Service = ActionService.CCCash,
          Event = action_event,

          ActionID = aid,
          FirstName = firstname, LastName = lastname,
          ClickID = clickid,
          Email = email, Msisdn = msisdn,
          Address = address, Zip = zip, City = city, Country = country
        });

      #endregion

      int? ccid = db.LoadInt($"SELECT clientid FROM livesports.cc_client WHERE clickid='{clickid}'");
      int PAYMENT_PROVIDER_ID = 4;
      if (!ccid.HasValue)
        ccid = db.Execute("INSERT INTO livesports.cc_client (payment_provider, country, host, email, firstname, lastname, address, zip, city, clickid, affid, msisdn, pubid, updated, created)",
          PAYMENT_PROVIDER_ID, country, offer, email, firstname, lastname, address, zip, city, clickid, affid, msisdn, pubid, DirectTime.Now, DirectTime.Now);
      else
        db.Execute("UPDATE [].cc_client SET msisdn={0}, affid={1}, pubid={2} WHERE clickid={3}", msisdn, affid, pubid, clickid);

      if (!string.IsNullOrEmpty(ievent) && ievent.Equals("join"))
      {
        bool _isStolen = false;
        this.SendPostbackToSports_Subscribe(clickid, "", "", "");

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

        this.SendPostbackToSpots_CallbackInformation(clickid, "settle", HttpContext.Request.RawUrl.ToString(), false, _isStolen);
      }

      return this.Content("");
    }

    // SUMMARY: Returns view with data from database entries, all postbacks received from david app
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

    // SEND TRANSACTION INFORMATIONS
    private void SendPostbackToSports_Callback(string transaction_id, string type)
    {
      string postbackLink = "https://api-livesports.co/callback.php?lxid=" + transaction_id + "&type=" + type;
      this.Log("TRACKING: " + postbackLink);
      HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(postbackLink);
      webRequest.AllowAutoRedirect = false;
      HttpWebResponse response;
      HttpStatusCode statusCode;

      try
      {
        response = (HttpWebResponse)webRequest.GetResponse();
        this.Log("TRACKING RESPONSE: " + response.ToString());
      }
      catch (WebException we)
      {
        this.Log("FATAL:;SendPostbackToSports_Callback:: " + we.ToString());
        return;
      }
    }

    // SEND USERNAME AND PASSWORD
    private void SendPostbackToSports_Subscribe(string transaction_id, string username, string password, string email)
    {
      string postbackLink = "https://api-livesports.co/subscribe.php?set=" + string.Format("&lxid={0}&email={1}&username={2}&password={3}", transaction_id, email, username, password);
      this.Log("USER/PASS: " + postbackLink);
      HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(postbackLink);
      webRequest.AllowAutoRedirect = false;
      HttpWebResponse response;
      HttpStatusCode statusCode;

      try
      {
        response = (HttpWebResponse)webRequest.GetResponse();
        this.Log("USER/PASS RESPONSE: " + response.ToString());
      }
      catch (WebException we)
      {
        this.Log("FATAL:;SendPostbackToSports_Subscribe:: " + we.ToString());
        return;
      }
    }

    // Send event name and callback url to cc so that informations can be there stored
    private void SendPostbackToSpots_CallbackInformation(string clickid, string eventName, string callback_url, bool isUpsell, bool isStolen)
    {
      string postbackLink = "https://api-livesports.co/callback_for_postbacks.php?clickid=" + clickid +
        "&event_name=" + eventName +
        "&callback_url=" + System.Net.WebUtility.UrlEncode(callback_url) +
        "&upsell=" + (isUpsell ? 1 : 0) +
        "&isstolen=" + (isStolen ? 1 : 0);

      this.Log("TRACKING: " + postbackLink);
      HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(postbackLink);
      webRequest.AllowAutoRedirect = false;
      HttpWebResponse response;
      HttpStatusCode statusCode;

      try
      {
        response = (HttpWebResponse)webRequest.GetResponse();
        this.Log("TRACKING RESPONSE: " + response.ToString());
      }
      catch (WebException we)
      {
        this.Log("FATAL:;SendPostbackToSpots_CallbackInformation:: " + we.ToString());
        return;
      }
    }

  }
}
