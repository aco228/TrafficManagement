using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficManagement.Core.Managers
{
  public class LeadManager
  {
    private static object LOCK_OBJ = new object();
    private static Dictionary<string, int> country_map = null;

    public static int? GetLeadID(string msisdn, string email, CCSubmitDirect db = null)
    {
      if (string.IsNullOrEmpty(msisdn) && string.IsNullOrEmpty(email))
        return null;

      if (db == null)
        db = CCSubmitDirect.Instance;

      if (!string.IsNullOrEmpty(msisdn) && !string.IsNullOrEmpty(email))
      {
        string query = string.Format(" (msisdn='{0}' AND email='{1}') OR (msisdn=null AND email='{1}') OR (msisdn='{0}' AND email=null)", msisdn, email);
        int? id = db.LoadInt(string.Format("SELECT leadid FROM [].tm_lead WHERE {0}", query));
        if (id.HasValue)
          return id;
      }


      if (!string.IsNullOrEmpty(msisdn))
      {
        string query = string.Format(" msisdn='{0}'", msisdn);
        int? id = db.LoadInt(string.Format("SELECT leadid FROM [].tm_lead WHERE {0}", query));
        if (id.HasValue)
          return id;
      }

      if (!string.IsNullOrEmpty(email))
      {
        string query = string.Format(" email='{0}'", email);
        int? id = db.LoadInt(string.Format("SELECT leadid FROM [].tm_lead WHERE {0}", query));
        if (id.HasValue)
          return id;
      }

      return null;
    }

    public static int? GetCountryID(string country, CCSubmitDirect db = null)
    {
      if (string.IsNullOrEmpty(country))
        return null;

      if (db == null) db = CCSubmitDirect.Instance;
      lock (LOCK_OBJ)
      {
        if (country_map == null)
        {
          country_map = new Dictionary<string, int>();
          DirectContainer dc = db.LoadContainer(@"SELECT c.countryid, c.name, c.code FROM livesports.tm_country_used AS u
                                                  LEFT OUTER JOIN livesports.tm_country AS c ON u.countryid=c.countryid");
          foreach (var r in dc.Rows)
          {
            if (!country_map.ContainsKey(r.GetString("name").ToLower()))
              country_map.Add(r.GetString("name").ToLower(), r.GetInt("countryid").Value);
            if (!country_map.ContainsKey(r.GetString("code").ToLower()))
              country_map.Add(r.GetString("code").ToLower(), r.GetInt("countryid").Value);
          }
        }
      }

      if (country_map.ContainsKey(country.ToLower()))
        return country_map[country.ToLower()];

      lock (LOCK_OBJ)
      {
        DirectContainer dcc = db.LoadContainer(string.Format(@"SELECT countryid, name, code FROM livesports.tm_country WHERE name LIKE '%{0}%' OR code='{1}'", country.ToLower(), country.ToLower()));
        if (!dcc.HasValue)
          return null;

        if (db.LoadInt("SELECT COUNT(*) FROM [].tm_country_used WHERE countryid={0}", dcc.GetInt("countryid").Value).Value == 0)
          db.Execute("INSERT INTO [].tm_country_used (countryid)", dcc.GetInt("countryid").Value);


        if (!country_map.ContainsKey(dcc.GetString("name").ToLower()))
          country_map.Add(dcc.GetString("name").ToLower(), dcc.GetInt("countryid").Value);
        if (!country_map.ContainsKey(dcc.GetString("code").ToLower()))
          country_map.Add(dcc.GetString("code").ToLower(), dcc.GetInt("countryid").Value);

        return dcc.GetInt("countryid").Value;
      }


    }

    public static string CreateLeadConstructQuery(string msisdn, string email, string first_name, string last_name, string country, string address, string city, string zip, string device, string carrier, string device_mf, string device_os, int actions_count, CCSubmitDirect db = null)
    {
      if (db == null) db = CCSubmitDirect.Instance;
      return db.Construct("INSERT INTO [].tm_lead (msisdn, email, first_name, last_name, country, countryid, address, city, zip, device, operator, device_mf, device_os, actions_count, updated, created)",
        msisdn, email, first_name, last_name, country, GetCountryID(country), address, city, zip, device, carrier, device_mf, device_os, actions_count, DirectTime.Now, DirectTime.Now);
    }

    public static int? CreateLead(string msisdn, string email, string first_name, string last_name, string country, string address, string city, string zip, string device, string carrier, string device_mf, string device_os, int actions_count, CCSubmitDirect db = null)
    {
      if (db == null) db = CCSubmitDirect.Instance;
      return db.Execute(CreateLeadConstructQuery(msisdn, email, first_name, last_name, country, address, city, zip, device, carrier, device_mf, device_os, actions_count, db));
    }

    public static LeadEntry ManageLeadFromAction(ActionModel model)
    {
      CCSubmitDirect db = model.Database;
      if (db == null)
        db = CCSubmitDirect.Instance;

      int? leadid = db.LoadInt("SELECT leadID FROM [].tm_lead_action WHERE actionid=" + model.ActionID);
      LeadEntry leadEntry = null;

      if (leadid.HasValue)
      {
        // this action has valid lead, so we update values

        leadEntry = new LeadEntry(leadid.Value, db);
        leadEntry.CheckValue("first_name", model.FirstName);
        leadEntry.CheckValue("last_name", model.LastName);
        leadEntry.CheckValue("msisdn", model.Msisdn);
        leadEntry.CheckValue("email", model.Email);
        leadEntry.CheckValue("address", model.Address);
        leadEntry.CheckValue("zip", model.Zip);
        leadEntry.CheckValue("country", model.Country);
        leadEntry.CheckValue("city", model.City);
      }
      else
      {
        // this action does not have valid lead, so we need to create/find new one
        // we try to find it by email, msisdn
        leadid = LeadManager.GetLeadID(model.Msisdn, model.Email, db);
        if (leadid.HasValue)
        {
          // if we find it, we try to update values

          leadEntry = new LeadEntry(leadid.Value, db);
          leadEntry.CheckValue("first_name", model.FirstName);
          leadEntry.CheckValue("last_name", model.LastName);
          leadEntry.CheckValue("address", model.Address);
          leadEntry.CheckValue("email", model.Email);
          leadEntry.CheckValue("zip", model.Zip);
          leadEntry.CheckValue("country", model.Country);
          leadEntry.CheckValue("city", model.City);
          leadEntry.CheckValue("msisdn", model.Msisdn);
          leadEntry.UpdateSql += "actions_count=actions_count+1,";
        }
        else
        {
          // if not, we create new one
          leadid = CreateLead(model.Msisdn, model.Email, model.FirstName, model.LastName, model.Country, model.Address, model.City, model.Zip, model.device_model, model.carrier, model.device_mf, model.device_os, 1);
          leadEntry = new LeadEntry(leadid.Value, db);
        }

        db.Transactional.Execute("UPDATE [].tm_lead_action SET leadid={0} WHERE actionid={1}", leadid.Value, model.ActionID);
      }

      leadEntry.FinishUpdateSql();
      return leadEntry;
    }


  }
}
