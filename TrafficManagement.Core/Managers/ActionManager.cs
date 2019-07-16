using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficManagement.Core.Web;

namespace TrafficManagement.Core.Managers
{

  public class ActionManager
  {

    public static int? GetActionID(string clickid, CCSubmitDirect db = null)
    {
      if (db == null)
        db = CCSubmitDirect.Instance;
      return db.LoadInt("SELECT actionid FROM [].tm_lead_action WHERE clickid={0}", clickid);
    }

    public static int? ExecuteAction(ActionModel model)
    {
      try
      {
        return ExecuteActionImplementation(model);
      }
      catch(Exception e)
      {
        return null;
      }
    }

    public static int? ExecuteActionImplementation(ActionModel model)
    {
      if (model.ActionID == -1)
      {
        int? actionID = Create(model);
        if (!actionID.HasValue)
          return null;

        model.ActionID = actionID.Value;
      }

      DirectContainer dc = model.Database.LoadContainer("SELECT * FROM [].tm_lead_action WHERE actionid=" + model.ActionID);
      LeadEntry leadEntry = LeadManager.ManageLeadFromAction(model);

      if ((model.Event == ActionModelEvent.Redirection || model.Event == ActionModelEvent.InputEmail) && !dc.GetBoolean("input_redirect"))
        ActionManager.Update(model, "input_redirect", "1", dc);
      if (model.Event == ActionModelEvent.Create)
        ActionManager.Update(model, "input_redirect", "1", dc);
      if ((model.Event == ActionModelEvent.InputEmail && !dc.GetBoolean("input_email")) || (dc.GetBoolean("input_email") == false && !string.IsNullOrEmpty(leadEntry.Container.GetString("email"))))
        ActionManager.Update(model, "input_email", "1", dc);
      if ((model.Event == ActionModelEvent.InputContact && !dc.GetBoolean("input_contact")) || (dc.GetBoolean("input_contact") == false && !string.IsNullOrEmpty(leadEntry.Container.GetString("first_name")) && !string.IsNullOrEmpty(leadEntry.Container.GetString("last_name"))))
        ActionManager.Update(model, "input_contact", "1", dc);
      if (model.Event == ActionModelEvent.Subscribe)
        ActionManager.Update(model, "has_subscription", "1", dc);
      if (model.Event == ActionModelEvent.Chargeback)
        ActionManager.Update(model, "has_chargeback", "1", dc);
      if (model.Event == ActionModelEvent.Refund)
        ActionManager.Update(model, "has_refund", "1", dc);
      if (model.Event == ActionModelEvent.Charge)
        ActionManager.Update(model, "times_charged", "++", dc);
      if (model.Event == ActionModelEvent.Upsell)
        ActionManager.Update(model, "times_upsell", "++", dc);
      if (model.Event == ActionModelEvent.EndFlow)
      {
        ActionManager.Update(model, "provider_redirection", model.ProviderRedirection, dc);
        ActionManager.Update(model, "has_redirectedToProvider", "1", dc);
      }
      
      if (model.Service != ActionService.Default && (!dc.GetInt("serviceid").HasValue || (int)model.Service != dc.GetInt("serviceid").Value))
        ActionManager.Update(model, "serviceid", ((int)model.Service).ToString(), dc);
      if (!string.IsNullOrEmpty(model.ClickID) && !dc.GetString("clickid").Equals(model.ClickID))
        ActionManager.Update(model, "clickid", model.ClickID, dc);
      if (model.AffID.HasValue && (!dc.GetInt("affid").HasValue || (dc.GetInt("affid").HasValue && model.AffID.Value != dc.GetInt("affid").Value)))
        ActionManager.Update(model, "affid", model.AffID.ToString(), dc);
      if (!string.IsNullOrEmpty(model.PubID) && !dc.GetString("pubid").Equals(model.PubID))
        ActionManager.Update(model, "pubid", model.PubID, dc, true);
      if (!string.IsNullOrEmpty(model.LanderUrl) && (string.IsNullOrEmpty(dc.GetString("lander_url")) || !dc.GetString("lander_url").Equals(model.LanderUrl)))
        ActionManager.Update(model, "lander_url", model.LanderUrl, dc, true);

      int? landerID = CCSubmitCacheManager.GetLanderID(model.LanderName, model.Database);
      if (landerID.HasValue && dc.GetInt("landerid").HasValue && dc.GetInt("landerid").Value != landerID.Value)
        ActionManager.Update(model, "landerid", landerID.Value.ToString(), dc);
      
      model.SQLUpdateQuery = "UPDATE [].tm_lead_action SET " + model.SQLUpdateQuery + " updated=CURRENT_TIMESTAMP WHERE actionid=" + model.ActionID + ";";
      model.Database.Transactional.Execute(model.SQLUpdateQuery);
      model.Database.Transactional.Run();
      
      return model.ActionID;
    }
    
    
    public static int? Create(ActionModel input)
    {
      CCSubmitDirect db = input.Database;

      // check prelander type
      int? prelandertypeid = CCSubmitCacheManager.GetPrelanderTypeID(input.PrelanderType);
      if(!prelandertypeid.HasValue)
        return null;

      // check prelander
      int? prelanderid = CCSubmitCacheManager.GetPrelanderID(input.prelander, prelandertypeid.Value);
      if (!prelanderid.HasValue)
        return null;

      // get lead id
      int? leadid = null;
      if (!string.IsNullOrEmpty(input.Msisdn) || !string.IsNullOrEmpty(input.Email))
      {
        leadid = LeadManager.GetLeadID(input.Msisdn, input.Email, db);
        if (!leadid.HasValue)
          leadid = LeadManager.CreateLead(input.Msisdn, input.Email, input.FirstName, input.LastName, input.Country, input.Address, input.City, input.Zip, input.device_model, input.carrier, input.device_mf, input.device_os, 1, db);
        else
          db.Transactional.Execute("UPDATE [].tm_lead SET actions_count=actions_count+1 WHERE leadid={0};", leadid.Value);
      }

      int? actionid = db.Execute("INSERT INTO [].tm_lead_action (leadid, prelandertypeid, prelanderid, host, request, referrer, ip, updated)",
        leadid, prelandertypeid.Value, prelanderid.Value, input.host, input.request, input.referrer, input.ipAddress, DirectTime.Now);

      return actionid;
    }

    public static void Update2(int actionid, string key, string value, DirectContainer cache = null, CCSubmitDirect db = null)
    {
      if (db == null)
        db = CCSubmitDirect.Instance;
      if(cache == null)
        cache = db.LoadContainer("SELECT * FROM [].tm_lead_action WHERE actionid=" + actionid);

      string new_value = (value.Equals("++") ? (cache.GetInt(key).Value + 1).ToString() : value);
      db.Execute("INSERT INTO [].tm_action_history (actionid, name, old_value, new_value)", actionid, key, cache.GetString(key), new_value);
      
      if(value.Equals("++"))
        db.Execute("UPDATE [].tm_lead_action SET "+key+"="+key+"+1 WHERE actionid={1}", value, actionid);
      else
        db.Execute("UPDATE [].tm_lead_action SET " + key + "={0} WHERE actionid={1}", value, actionid);
    }


    public static void Update(ActionModel model, string key, string value, DirectContainer cache = null, bool isString = false)
    {
      if (model.Database == null)
        model.Database = CCSubmitDirect.Instance;
      if (cache == null)
        cache = model.Database.LoadContainer("SELECT * FROM [].tm_lead_action WHERE actionid=" + model.ActionID);

      if (!cache.HasValue)
        return;

      string new_value = string.Empty;
      if (value.Equals("++") && !cache.GetInt(key).HasValue)
        new_value = "1";
      else
        new_value = (value.Equals("++") ? (cache.GetInt(key).Value + 1).ToString() : value);

      model.Database.Transactional.Execute("INSERT INTO [].tm_action_history (actionid, name, old_value, new_value)", model.ActionID, key, cache.GetString(key), new_value);

      if (value.Equals("++"))
        model.SQLUpdateQuery += key + "=" + key + "+1,";
      else
      {
        if(isString)
          model.SQLUpdateQuery += model.Database.Construct(key + "='{0}',", value);
        else
          model.SQLUpdateQuery += model.Database.Construct(key + "={0},", value);
      }
        
    }



  }
}
