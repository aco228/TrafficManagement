using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TrafficManagement.Core.Web.Controllers.Models;

namespace TrafficManagement.Core.Web.Controllers.Code
{
  public class LegacyCreateUser : LegacyBase
  {
    private ControllerCreateUserModel _input = null;

    public LegacyCreateUser(CCSubmitDirect db, HttpRequestBase request, ControllerCreateUserModel input) : base(db,request)
    {
      this._input = input;
    }

    public override void Run()
    {      
      if(this.Database.LoadInt("SELECT COUNT(*) as 'br' FROM livesports.cc_client where clickid={0};", this._input.lxid).Value == 0)
      {
        this.Database.Transactional.Execute("INSERT INTO [].cc_client (clickid, affid, payment_provider, pubid, email, password, status, country, referrer, host, updated, created)",
          this._input.lxid,
          this._input.affid,
          this._input.pp,
          this._input.pubid,
          this._input.email,
          this._input.password,
          "created",
          this._input.country,
          this._input.referrer, 
          this._input.host,
          DirectTime.Now, DirectTime.Now);
      }
    }
  }
}
