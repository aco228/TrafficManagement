using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficManagement.Wpf.Importer.Csv
{
    public class CsvSetupParams
    {
        public string FilePath { get; set; }
        public int NumberOfColumns { get; set; }
        public bool HasHeader { get; set; }
        public bool TestOnly { get; set; }
    }
}
