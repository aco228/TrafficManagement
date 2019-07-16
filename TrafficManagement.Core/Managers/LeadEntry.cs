using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficManagement.Core.Managers
{
  public class LeadEntry
  {
    private int _id;
    private CCSubmitDirect _database = null;
    public DirectContainer Container { get; protected set; }
    public string UpdateSql = "";

    public LeadEntry(int leadID, CCSubmitDirect db = null)
    {
      this._id = leadID;
      if (db == null)
        this._database = CCSubmitDirect.Instance;
      else
        this._database = db;

      this.Container = db.LoadContainer("SELECT * FROM [].tm_lead WHERE leadid=" + this._id);
    }


    public void CheckValue(string db_key, string value)
    {
      if (!string.IsNullOrEmpty(value) && !this.Container.GetString(db_key).Equals(value))
      {
        this._database.Transactional.Execute("INSERT INTO [].tm_lead_history (leadid, name, old_value, new_value)", this._id, db_key, this.Container.GetString(db_key), value);
        this.UpdateSql += db_key + "='" + value + "',";
        //this._database.Execute("UPDATE [].tm_lead SET "+db_key+"={0} WHERE leadid={1}", value, this._id);
      }
    }

    public void FinishUpdateSql()
    {
      if(!string.IsNullOrEmpty(this.UpdateSql))
        this._database.Transactional.Execute(this._database.Construct("UPDATE [].tm_lead SET " + this.UpdateSql + " updated=CURRENT_TIMESTAMP WHERE leadid=" + _id + ";"));
    }

  }
}
