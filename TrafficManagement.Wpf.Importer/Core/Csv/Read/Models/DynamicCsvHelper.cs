using System.Collections.Generic;

namespace TrafficManagement.Wpf.Importer.Core.Csv.Read.Models
{
  public static class DynamicCsvHelper
  {

    public static List<string[]> DynamicCsvListToParamsList(List<DynamicCsv> records)
    {
      var dynamicCsvParamsList = new List<string[]>();

      foreach (var record in records)
      {
        dynamicCsvParamsList.Add(DynamicCsvToParamsList(record));
      }

      return dynamicCsvParamsList;
    }

    public static string[] DynamicCsvToParamsList(DynamicCsv record)
    {
      return new string[] {record.Msisdn,
                                     record.Email,
                                     record.FirstName,
                                     record.LastName,
                                     record.Country,
                                     record.Address,
                                     record.City,
                                     record.Zip,
                                     record.Device,
                                     record.Operator };
    }
  }
}
