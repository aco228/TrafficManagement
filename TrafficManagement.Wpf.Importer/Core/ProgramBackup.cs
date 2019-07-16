using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficManagement.Wpf.Importer.Csv;
using TrafficManagement.Wpf.Importer.Csv.Readers;
using TrafficManagement.Wpf.Importer.Csv.Write;

namespace TrafficManagement.Wpf.Importer.Core
{
  class ProgramBackup
  {
    /*
    static void Main(string[] args)
    {
      PromptUser();

    }
    */

    private static void PromptUser()
    {
      Console.WriteLine("Paste/Type File Path:");
      var filePath = Console.ReadLine();
      Console.WriteLine("Number of columns in CSV file :");
      var numberOfColumns = int.Parse(Console.ReadLine());
      Console.WriteLine("Does CSV file have header (column names) ?  Y/N :");
      var hasHeader = Console.ReadLine() == "Y" ? true : false;

      Console.WriteLine("Test if it works or copy to database ?  TEST/COPY :");
      var testOnly = Console.ReadLine() == "TEST" ? true : false;

      Console.WriteLine("Max records at once (max 1000) ?  TEST/COPY :");
      var inputedRecords = int.Parse(Console.ReadLine());
      var maxRecordsAtOnce = inputedRecords > 1000 ? inputedRecords : 1000;

      var csvParams = new CsvSetupParams()
      {
        FilePath = filePath,
        NumberOfColumns = numberOfColumns,
        HasHeader = hasHeader,
        TestOnly = testOnly
      };

      var csvTouple = CsvReaderCustom.ReadByType(csvParams);

      if (testOnly)
      {
        CsvWriterCustom.WriteToConsole(csvTouple.csvList);
      }
      else
      {
        CsvWriterCustom.WriteToDatabase(csvTouple.csvList, maxRecordsAtOnce);
      }
    }


  }
}
