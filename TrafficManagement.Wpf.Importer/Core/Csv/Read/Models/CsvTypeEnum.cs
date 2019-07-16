using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficManagement.Wpf.Importer.Csv.Read.Types
{
    public enum CsvTypeEnum
    {
        Unknown = 0,
        TwoColumnsMsisdnName = 1,
        EightColumnsWithHeader = 2
    }
}
