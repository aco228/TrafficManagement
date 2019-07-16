using CsvHelper.Configuration.Attributes;

namespace TrafficManagement.Wpf.Importer.Core.Csv.Read.Models
{
  public class DynamicCsv
  {

    [Index(0)]
    public string Msisdn { get; set; }

    [Index(1)]
    public string FirstName { get; set; }
    [Index(2)]
    public string Country { get; set; }
    [Index(3)]
    public string Clicker { get; set; }
    [Index(4)]
    public string Status { get; set; }
    [Index(5)]
    public string Email { get; set; }
    [Index(6)]
    public string Device { get; set; }
    [Index(7)]
    public string Operator { get; set; }
    [Index(8)]
    public string LastName { get; set; }
    [Index(9)]
    public string City { get; set; }
    [Index(10)]
    public string Address { get; set; }
    [Index(11)]
    public string Zip { get; set; }
    [Index(12)]
    public string DeviceMf { get; set; }
    [Index(13)]
    public string DeviceOs { get; set; }

  }
}
