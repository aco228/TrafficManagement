using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficManagement.Core.Web
{
  public class CCSubmitCacheManager
  {

    public static Dictionary<string, int> Prelanders = new Dictionary<string, int>();
    public static Dictionary<string, int> PrelanderTypes = new Dictionary<string, int>();
    public static Dictionary<string, int> Landers = new Dictionary<string, int>();
    public static Dictionary<string, int> EmailBlacklist = null;

    #region # Get ID's from dictonaries #

    public static int? GetPrelanderID(string prelander, int prelanderType, CCSubmitDirect db = null)
    {
      if (db == null)
        db = CCSubmitDirect.Instance;

      if (string.IsNullOrEmpty(prelander))
        return null;

      int? prelanderid = null;
      if (!CCSubmitCacheManager.Prelanders.ContainsKey(prelander))
      {
        prelanderid = db.LoadInt("SELECT prelanderid FROM [].tm_prelander WHERE name={0}", prelander);
        if (!prelanderid.HasValue)
          prelanderid = db.Execute("INSERT INTO [].tm_prelander (name, prelandertypeid)", prelander, prelanderType);
        if (!prelanderid.HasValue)
          return null;
        CCSubmitCacheManager.Prelanders.Add(prelander, prelanderid.Value);
      }
      else
        prelanderid = CCSubmitCacheManager.Prelanders[prelander];

      return prelanderid;
    }

    public static int? GetPrelanderTypeID(string type, CCSubmitDirect db = null)
    {
      if (db == null)
        db = CCSubmitDirect.Instance;

      if (string.IsNullOrEmpty(type))
        return null;

      int? prelandertypeid = null;
      if (!CCSubmitCacheManager.PrelanderTypes.ContainsKey(type))
      {
        prelandertypeid = db.LoadInt("SELECT prelandertypeid FROM [].tm_prelandertype WHERE name={0}", type);
        if (!prelandertypeid.HasValue)
          prelandertypeid = db.Execute("INSERT INTO [].tm_prelandertype (name)", type);

        if (!prelandertypeid.HasValue)
          return null;

        CCSubmitCacheManager.PrelanderTypes.Add(type, prelandertypeid.Value);
      }
      else
        prelandertypeid = CCSubmitCacheManager.PrelanderTypes[type];
      return prelandertypeid;
    }

    public static int? GetLanderID(string name, CCSubmitDirect db = null)
    {
      if (db == null)
        db = CCSubmitDirect.Instance;

      if (string.IsNullOrEmpty(name))
        return null;

      int? id = null;
      if (!CCSubmitCacheManager.Landers.ContainsKey(name))
      {
        id = db.LoadInt("SELECT landerid FROM [].tm_lander WHERE name={0}", name);
        if (!id.HasValue)
          id = db.Execute("INSERT INTO [].tm_lander (name)", name);

        if (!id.HasValue)
          return null;

        CCSubmitCacheManager.Landers.Add(name, id.Value);
      }
      else
        id = CCSubmitCacheManager.Landers[name];

      return id;
    }

    #endregion

    public static bool CheckIfEmailBlacklist(string email, CCSubmitDirect db = null)
    {
      if (db == null)
        db = CCSubmitDirect.Instance;

      if (string.IsNullOrEmpty(email))
        return false;

      if(EmailBlacklist == null)
      {
        DirectContainer dc = db.LoadContainer("SELECT email FROM livesports.ls_email_blacklist;");
        if (dc == null || !dc.HasValue)
          return false;

        EmailBlacklist = new Dictionary<string, int>();
        foreach(var row in dc.Rows)
          if (!EmailBlacklist.ContainsKey(row.GetString("email")))
            EmailBlacklist.Add(row.GetString("email"), 1);
      }

      return EmailBlacklist.ContainsKey(email);
    }

  }
}
