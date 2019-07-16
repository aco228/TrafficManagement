using Direct.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TrafficManagement.Core.Managers;
using TrafficManagement.Core.Web.Controllers.Postback.Models;

namespace TrafficManagement.Core.Web.Controllers.Postback
{
  public class CCCashPostbackController : PostbackControllerBase
  {

    [Route("cccashcallback")]
    public ActionResult Index(CCCashPostbackControllerModel input)
    {
      this.CreateLog();
      input.Init(this.Request);
      LegacyUpdate(input);


      if (input.aid != -1)
        ActionManager.ExecuteAction(new ActionModel()
        {
          Database = this.Database,
          Service = ActionService.CCCash,
          Event = input.Type,

          ActionID = input.aid,
          FirstName = input.firstname,
          LastName = input.lastname,
          ClickID = input.clickid,
          Email = input.email,
          Msisdn = input.msisdn,
          Address = input.address,
          Zip = input.zip,
          City = input.city,
          Country = input.country
        });

      if (input.Type == Managers.ActionModelEvent.Charge)
        this.SystemPostback(input.clickid);

      return this.Content("");
    }

    private void LegacyUpdate(CCCashPostbackControllerModel input)
    {
      int? ccid = this.Database.LoadInt($"SELECT clientid FROM livesports.cc_client WHERE clickid='{input.clickid}'");
      int PAYMENT_PROVIDER_ID = 4;
      if (!ccid.HasValue)
        this.Database.Execute("INSERT INTO livesports.cc_client (payment_provider, country, host, email, firstname, lastname, address, zip, city, clickid, affid, msisdn, pubid, updated, created)",
          PAYMENT_PROVIDER_ID, input.country, input.offer, input.email, input.firstname, input.lastname, input.address, input.zip, input.city, input.clickid, input.affid, input.msisdn, input.pubid, DirectTime.Now, DirectTime.Now);
      else
        this.Database.Execute("UPDATE [].cc_client SET msisdn={0}, affid={1}, pubid={2} WHERE clickid={3}", input.msisdn, input.affid, input.pubid, input.clickid);
    }

  }
}
