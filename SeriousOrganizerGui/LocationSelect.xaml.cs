using SeriousOrganizerGui.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SeriousOrganizerGui
{
    /// <summary>
    /// Interaction logic for LabelSelect.xaml
    /// </summary>
    public partial class LocationSelect : Window
    {
        public LocationSelect()
        {
            InitializeComponent();

            LocationList.ItemsSource = DataClient.Location.Get();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Ok_Button_Click(object sender, RoutedEventArgs e)
        {
            //if (LocationList.SelectedItems.Count > 0)
            //{
            //    var selected = LocationList.SelectedItems.Cast<Dto.Location>().Select(x => x.Id);
            //    var entryIds = _entries.Select(ix => DataClient.Client.GetDir(ix.Index).Id);
            //    DataClient.Label.AddLabelsToEntry(entryIds, selected);

            this.Close();
            //}
        }

        private void BtnAddLocation_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtPath.Text)) return;

            DataClient.Location.Add(txtName.Text, txtPath.Text);
        }

        private void Remove_Button_Click(object sender, RoutedEventArgs e)
        {
            if (LocationList.SelectedItems.Count > 0)
            {
                var selected = LocationList.SelectedItems.Cast<Dto.Location>().Select(x => x.Id).ToList();

                var msgResponse = MessageBox.Show($"Are you sure you want to remove {selected.Count()} locations?", "Delete confirmation", MessageBoxButton.OKCancel);
                if (msgResponse == MessageBoxResult.Cancel)
                    return;

                foreach (var locationId in selected)
                {
                    DataClient.Location.Remove(locationId);
                }
            }
        }
    }
}
