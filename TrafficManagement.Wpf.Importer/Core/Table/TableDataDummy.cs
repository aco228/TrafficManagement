using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficManagement.Wpf.Importer.Core.Table
{
  public class TableDataDummy
  {
    public int A { get; set; }
    public int B { get; set; }
    public int C { get; set; }

    private ObservableCollection<TableDataDummy> TableDataCollection;

    public ObservableCollection<TableDataDummy> TableDataFetch()
    {
      TableDataCollection = new ObservableCollection<TableDataDummy>();
      TableDataCollection.Add(new TableDataDummy() { A = 6, B = 7, C = 5 });
      TableDataCollection.Add(new TableDataDummy() { A = 5, B = 8, C = 4 });
      TableDataCollection.Add(new TableDataDummy() { A = 4, B = 3, C = 0 });
      return TableDataCollection;
    }

  }
}
