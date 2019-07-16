using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TrafficManagement.Wpf.Importer.Core.Helpers
{
  public static class ObjectHelper
  {
    public static List<string> ObjectPropertyNamesToList<T>(T obj)
    {

      var listOfPropNames = new List<string>();

      foreach (PropertyInfo prop in obj.GetType().GetProperties())
      {
        listOfPropNames.Add(prop.Name);
      }

      return listOfPropNames;
    }
  }
}
