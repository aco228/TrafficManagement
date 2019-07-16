using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficManagement.Wpf.Importer.Core.Csv.Read.Models
{
  public class DynamicCsvInfo<T>
  {
    public List<T> Records { get; set; }
    public bool IsHeadless { get; set; }
    public int AllRecordsCount { get; set; }
    public int CsvColumnCount { get; set; }
  }
}
