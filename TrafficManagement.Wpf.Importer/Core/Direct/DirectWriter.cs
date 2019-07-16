using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficManagement.Core;

namespace TrafficManagement.Wpf.Importer.Direct
{
    public static class DirectWriter
    {
        public static (bool isSuccess,int numOfRows) InsertCsvData(List<string[]> csvArrayList)
        {
            CCSubmitDirect db = CCSubmitDirect.Instance;

            string query = "";

            query += db.Construct("INSERT INTO [].tm_lead (msisdn, email, first_name, last_name, country, address, city, zip, device, operator)",
                                csvArrayList);

            var numOfRows =  db.Execute(query);

            return (numOfRows > 0 ? true : false, numOfRows != null ? (int)numOfRows : 0);
        }
    }
}
