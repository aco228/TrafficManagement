using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TrafficManagement.Wpf.Importer.Core.Csv.Read.Models;
using TrafficManagement.Wpf.Importer.Core.Helpers;
using TrafficManagement.Wpf.Importer.Csv.Readers;

namespace TrafficManagement.Wpf.Importer
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {

    private List<DynamicCsv> RecordsToInsert = null;
    private List<string> DynamicCsvObjectNames = null;
    private Dictionary<string, CsvNameByIndex> DynamicCsvComboNamesDictionary = new Dictionary<string, CsvNameByIndex>();

    public MainWindow()
    {
      InitializeComponent();

      this.Title = "Msisdn CSV importer";
      this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

      structureTitleLabel.Visibility = Visibility.Hidden;
    }

    private void Window_ContentRendered(object sender, EventArgs e)
    {
      //TestProgressBar();
      pbStatus.Visibility = Visibility.Hidden;
    }

    private async void TestProgressBar()
    {
      for (int i = 0; i < 100; i++)
      {
        await Task.Delay(100);
        pbStatus.Value++;
      }
    }

    private void BtnCsvChoose_Click(object sender, RoutedEventArgs e)
    {
      var openFileDlg = new OpenFileDialog();
      Nullable<bool> result = openFileDlg.ShowDialog();

      allRecordsCount.Content = "Total records count: " + 0;

      if (result == true)
      {

        filePathLabel.Text = openFileDlg.FileName;
        var dynamicCsvInfo = CsvReaderCustom.Read<DynamicCsv>(openFileDlg.FileName, false);

        allRecordsCount.Content = "Total records count: " + dynamicCsvInfo.AllRecordsCount;

        var sampleDataList = new List<DynamicCsv>();

        if (dynamicCsvInfo.IsHeadless)
        {
          dynamicCsvInfo = CsvReaderCustom.Read<DynamicCsv>(openFileDlg.FileName, true);
          allRecordsCount.Content = "Total records count: " + dynamicCsvInfo.AllRecordsCount;
          sampleDataList = dynamicCsvInfo.Records.Take(20).ToList();
        }
        else
        {
          sampleDataList = dynamicCsvInfo.Records.Take(20).ToList();
        }

        if (sampleDataList.Count == 0)
        {
          return;
        }

        RecordsToInsert = new List<DynamicCsv>();
        RecordsToInsert.AddRange(dynamicCsvInfo.Records);
        btnCsvImport.IsEnabled = true;

        //var dynamicTableColumnNames = typeof(DynamicCsv).GetProperties().Select(p => p.Name).ToArray();
        var dataListObservable = new ObservableCollection<DynamicCsv>(sampleDataList);

        this.csvDataGrid.ItemsSource = dataListObservable;

        CreateDynamicComboNames();
      }
    }

    private void AddColumnsToDynamicTable(string[] newColumnNames)
    {
      foreach (string name in newColumnNames)
      {
        csvDataGrid.Columns.Add(new DataGridTextColumn
        {
          // bind to a dictionary property
          Binding = new Binding("Custom[" + name + "]"),
          Header = name
        });
      }
    }

    private void BtnCsvImport_Click(object sender, RoutedEventArgs e)
    {

      pbStatus.Visibility = Visibility.Visible;

      // TODO INSERT DATA TO DB
      //foreach (var record in RecordsToInsert)
      //{

      //}

      var duplicateValuesEnumerable = DynamicCsvComboNamesDictionary.Values.Where( x => x.ColumnName != "UNSET")
                                                                           .GroupBy(x => x.ColumnName )
                                                                           .Where(x => x.Count() > 1);

      var duplicateValues = new StringBuilder();

      foreach (var item in duplicateValuesEnumerable)
      {
        duplicateValues.Append(item.Key + " ");
      }

      MessageBoxResult result = MessageBox.Show("Duplicate column names : " + duplicateValues);

    }

    private void ComboBoxCsvName_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var cbUnknown = (ComboBox)sender;
      DynamicCsvComboNamesDictionary[cbUnknown.Name].ColumnName = cbUnknown.SelectedValue.ToString();
      csvDataGrid.Columns[DynamicCsvComboNamesDictionary[cbUnknown.Name].Index].Header = cbUnknown.SelectedValue.ToString();
    }

    private void CreateDynamicComboNames()
    {

      var csvNamesComboList = new List<ComboBox>();

      var dynamicCsvObjectNames = ObjectHelper.ObjectPropertyNamesToList<DynamicCsv>(new DynamicCsv());
      dynamicCsvObjectNames.Add("UNSET");
      DynamicCsvObjectNames = dynamicCsvObjectNames;

      for (int index = 1; index < dynamicCsvObjectNames.Count(); index++)
      {
        var cb = new ComboBox();
        cb.HorizontalAlignment = HorizontalAlignment.Stretch;
        cb.ItemsSource = dynamicCsvObjectNames;
        cb.SelectedIndex = dynamicCsvObjectNames.Count() - 1;
        cb.Name = "cbColumnName" + index;
        cb.HorizontalAlignment = HorizontalAlignment.Left;

        cb.SelectionChanged += ComboBoxCsvName_SelectionChanged;

        //if (!DynamicCsvComboNamesDictionary.Keys.Contains(cb.Name))
        //  DynamicCsvComboNamesDictionary.Add(cb.Name, new CsvNameByIndex { Index = index - 1, ColumnName = cb.SelectedValue.ToString() });
        //else
        //  DynamicCsvComboNamesDictionary[cb.Name].ColumnName = cb.SelectedValue.ToString();

        csvNamesComboList.Add(cb);
        comboDockPanel.Children.Add(cb);
      }

      structureTitleLabel.Visibility = Visibility.Visible;

    }

  }
}
