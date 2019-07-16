using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficManagement.Core;

namespace TrafficManagement.Desktop.Code
{
  public class DatabaseLoaders
  {
    public List<CountryModel> Countries = new List<CountryModel>();
    public List<PrelanderTypeModel> PrelanderTypes = new List<PrelanderTypeModel>();
    public List<LanderModel> Landers = new List<LanderModel>();
    public List<PrelanderModel> Prelanders = new List<PrelanderModel>();
    public List<ServiceModel> Services = new List<ServiceModel>();

    public DatabaseLoaders()
    {
      CCSubmitDirect db = CCSubmitDirect.Instance;

      DirectContainer dc_countries = db.LoadContainer(
        @"SELECT c.countryid, c.name, c.code FROM livesports.tm_country_used AS u
        LEFT OUTER JOIN livesports.tm_country AS c ON u.countryid=c.countryid;");
      foreach (var c in dc_countries.Rows)
        Countries.Add(new CountryModel()
        {
          ID = c.GetInt("countryid").Value,
          Name = c.GetString("name")
        });
      
      DirectContainer dc_lander = db.LoadContainer(@"SELECT * FROM livesports.tm_lander;");
      foreach (var c in dc_lander.Rows)
        Landers.Add(new LanderModel()
        {
          ID = c.GetInt("landerid").Value,
          Name = c.GetString("name")
        });

      DirectContainer dc_prelanders = db.LoadContainer(@"SELECT * FROM livesports.tm_prelander;");
      foreach (var c in dc_prelanders.Rows)
        Prelanders.Add(new PrelanderModel()
        {
          ID = c.GetInt("prelanderid").Value,
          Name = c.GetString("name")
        });

      DirectContainer dc_prelandertype = db.LoadContainer(@"SELECT * FROM livesports.tm_prelandertype;");
      foreach (var c in dc_prelandertype.Rows)
        PrelanderTypes.Add(new PrelanderTypeModel()
        {
          ID = c.GetInt("prelandertypeid").Value,
          Name = c.GetString("name")
        });


      DirectContainer dc_services = db.LoadContainer(@"SELECT * FROM livesports.tm_service;");
      foreach (var c in dc_services.Rows)
        Services.Add(new ServiceModel()
        {
          ID = c.GetInt("serviceid").Value,
          Name = c.GetString("name")
        });
    }

  }
}
