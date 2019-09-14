using SeriousOrganizerGui.Controls;
using SeriousOrganizerGui.Data;
using SeriousOrganizerGui.Data.Providers;
using SeriousOrganizerGui.Dto;
using SeriousOrganizerGui.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SeriousOrganizerGui
{


    /// <summary>
    /// Interaction logic for SearchPage.xaml
    /// </summary>
    public partial class SearchPage : UserControl
    {


        public SearchPage()
        {
            InitializeComponent();

            DataClient.Client.SendTextSearchChanged(""); // Reset search text
            dir_list.Refresh();
        }

        private void UpdateSearchList()
        {
            dir_list.Refresh();
            Console.WriteLine($"Update Search List");
        }

        private void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = ((TextBox)sender).Text;
            var new_length = DataClient.Client.SendTextSearchChanged(text);
            UpdateSearchList();
        }

        private async void BtnReloadEntries(object sender, RoutedEventArgs e)
        {
            LoadPanel.Visibility = Visibility.Visible;
            dir_list.Visibility = Visibility.Collapsed;
            file_list.Visibility = Visibility.Collapsed;
            await Task.Run(() => DataClient.Client.SendReloadRequest())
                            .ContinueWith(task =>
                            {
                                LoadPanel.Visibility = Visibility.Collapsed;
                                dir_list.Visibility = Visibility.Visible;
                                file_list.Visibility = Visibility.Visible;

                                UpdateSearchList();

                            }, System.Threading.CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void BtnOpenLabelSelect(object sender, RoutedEventArgs e)
        {
            List<Indexed> entriesSelected = dir_list.FindSelectedItems<Indexed>().Distinct().ToList();

            var select = new LabelSelect(entriesSelected);
            select.ShowInTaskbar = false;
            select.Owner = Window.GetWindow(this);
            select.ShowDialog();
        }

        private void BtnOpenLocationSelect(object sender, RoutedEventArgs e)
        {
            var select = new LocationSelect();
            select.ShowInTaskbar = false;
            select.Owner = Window.GetWindow(this);
            select.ShowDialog();
        }

        private void LabelPanel_StateChanged(object sender, EventArgs e)
        {
            UpdateSearchList();
        }

        private void Dir_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            file_list.SetSelectedEntry(((SelectedIndexChangedEventArgs)e).Index);
        }
    }
}
