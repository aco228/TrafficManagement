using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficManagement.Core;
using TrafficManagement.Core.Managers;
using TrafficManagement.Core.Undercover;

namespace TrafficManagement.Console.Tests
{
  class Program
  {
    static void Main(string[] args)
    {
      CCSubmitDirect db = CCSubmitDirect.Instance;
      DirectContainer dc = db.LoadContainer("SELECT leadid, country, countryid FROM [].tm_lead order by leadid desc;");
      
      int count = dc.RowsCount;
      foreach(var row in dc.Rows)
      {
        string country = row.GetString("country");
        if (string.IsNullOrEmpty(country) || row.GetInt("countryid").HasValue)
          continue;

        int? id = LeadManager.GetCountryID(country, db);
        if(id.HasValue)
          db.Transactional.Execute("UPDATE [].tm_lead SET countryid={0} WHERE leadid={1}", id, row.GetInt("leadid").Value);

        if (db.Transactional.Count >= 500)
          db.Transactional.Run();
      }

      int a = 0;
    }
  }
}
