using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficManagement.Core;

namespace TrafficManagement.Wpf.Importer.Direct
{
    public class DirectReader
    {
        public static List<string> QueryMsisdnsFromTm_Lead(string msisdnParams)
        {

            var tableTypeString = "[].tm_lead";

            var listMsisdn = new List<string>();

            CCSubmitDirect db = CCSubmitDirect.Instance;
            DirectContainer dc = db.LoadContainer("SELECT msisdn FROM " + tableTypeString + " " +
                                                  "WHERE msisdn in (" +  msisdnParams + ")");
            foreach (var row in dc.Rows)
                listMsisdn.Add(row.GetString("msisdn"));

            return listMsisdn;
        }
    }
}
