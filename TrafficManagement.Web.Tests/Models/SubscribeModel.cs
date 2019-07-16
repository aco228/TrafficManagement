using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrafficManagement.Web.Tests.Models
{
  public class SubscribeModel
  {
    public string aid { get; set; } = "";
    public string pp { get; set; } = "";
    public string firstname { get; set; } = "";
    public string lastname { get; set; } = "";
    public string msisdn { get; set; } = "";
    public string country { get; set; } = "";
    public string zip { get; set; } = "";
    public string cc_number { get; set; } = "";
    public string cc_expiry_month { get; set; } = "";
    public string cc_expiry_year { get; set; } = "";
    public string ccv { get; set; } = "";
    public string time { get; set; } = "";
    public string gacid { get; set; } = "";
    public string lxid { get; set; } = "";
    public string url { get; set; } = "";
  }
}