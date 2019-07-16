using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrafficManagement.Web.Tests.Models
{
  public class CreateUserModel
  {
    public string aid { get; set; } = "";
    public string ga_cid { get; set; } = "";
    public string email { get; set; } = "";
    public string password { get; set; } = "";
    public string country { get; set; } = "";
    public string lxid { get; set; } = "";
    public string pp { get; set; } = "";
    public string affid { get; set; } = "";
    public string pubid { get; set; } = "";

    public string referrer { get; set; } = "";
    public string host { get; set; } = "";
  }
}