using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrafficManagement.Desktop.Code;

namespace TrafficManagement.Desktop
{
  public partial class FormMain : Form
  {

    public FormMain()
    {
      InitializeComponent();
    }

    public DatabaseLoaders loaders = null;

    private void FormMain_Load(object sender, EventArgs e)
    {
      loaders = new DatabaseLoaders();

      PopulateCombos();
    }

    private void PopulateCombos()
    {
      foreach (var country in loaders.Countries)
        cb_country.Items.Add(country.Name);
      foreach (var item in loaders.Landers)
        cb_lander.Items.Add(item.Name);
      foreach (var item in loaders.Prelanders)
        cb_prelander.Items.Add(item.Name);
      foreach (var item in loaders.PrelanderTypes)
        cb_prelanderType.Items.Add(item.Name);
      foreach (var item in loaders.Services)
        cb_service.Items.Add(item.Name);
    }

  }
}
