using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TrafficManagement.Wpf.Importer.Csv.Read.Mapper;
using TrafficManagement.Wpf.Importer.Core.Csv.Read.Models;

namespace TrafficManagement.Wpf.Importer.Csv.Readers
{
  public static class CsvReaderCustom
  {

    public static DynamicCsvInfo<T> Read<T>(string pathToCsv, bool isHeadless)
    {

      int columnCount = 0;

      using (var reader = new StreamReader(pathToCsv))
      using (var csv = new CsvReader(reader))
      {
        try
        {

          var allRecordsCount = 0;

          if (isHeadless)
          {
            csv.Configuration.HasHeaderRecord = false;
          }
          else
          {
            // ignore header case
            //csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();
            csv.Configuration.RegisterClassMap<DynamicCsvMap>();
          }
          csv.Configuration.HeaderValidated = null;
          csv.Configuration.MissingFieldFound = null;


          var records = csv.GetRecords<T>().ToList();
          allRecordsCount = records.Count();

          if(records.Count > 0)
          {
            // doesnt work 
            columnCount = csv.Context.ColumnCount;
          }

          return new DynamicCsvInfo<T>
          {
             Records = records,
             IsHeadless = isHeadless,
             AllRecordsCount = allRecordsCount,
             CsvColumnCount = columnCount
          };
        }
        catch (Exception ex)
        {
          var emptyList = new List<T>();

          return new DynamicCsvInfo<T>
          {
            Records = new List<T>(),
            IsHeadless = IsHeadless(ex),
            AllRecordsCount = 0,
            CsvColumnCount = columnCount
          };
        }

      }
    }

    private static bool IsHeadless(Exception ex)
    {
      if (ex.Message.Contains("No members are mapped"))
      {
        return true;
      }
      else
        return false;
    }

    public static List<string> PropertyNamesWithValues(List<DynamicCsv> records)
    {

      var numberOfAllPropertiesInRecord = records[0].GetType().GetProperties().Count();
      var namesOfPropertiesWithValue = new List<string>();

      foreach (var record in records)
      {

        foreach (PropertyInfo prop in record.GetType().GetProperties())
        {
          var hasValue = prop.GetValue(record, null) != null && prop.GetValue(record, null).ToString() != "";
          if (hasValue)
          {
            if (!namesOfPropertiesWithValue.Contains(prop.Name))
            {
              namesOfPropertiesWithValue.Add(prop.Name);
            }
          }

          if (numberOfAllPropertiesInRecord == namesOfPropertiesWithValue.Count())
            return namesOfPropertiesWithValue;
        }
      }

      return namesOfPropertiesWithValue;
    }

  }

}
