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

        private ItemProviderTurbo<DirEntry> _turbo;

        private DirEntryProvider _dirEntryProvider;



        public SearchPage()
        {
            InitializeComponent();

            _dirEntryProvider = new DirEntryProvider(DataClient.Client);
            _turbo = new ItemProviderTurbo<DirEntry>(_dirEntryProvider);
            DataClient.Client.SendTextSearchChanged(""); // Reset search text
            _turbo.Update();

            dir_list.ItemsSource = _turbo;

        }



        private void UpdateSearchList()
        {
            _turbo.Update();
            Console.WriteLine($"Update Search List");
        }

        private void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = ((TextBox)sender).Text;
            var new_length = DataClient.Client.SendTextSearchChanged(text);
            UpdateSearchList();
        }

        private void dir_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lv = sender as ListView;
            if (lv.SelectedIndex < 0) return;

            var currentProvider = file_list.ItemsSource as ItemProviderTurbo<FileEntry>;
            Console.WriteLine("Selected row: " + lv.SelectedIndex);

            if (currentProvider is null)
            {
                var provider = new FileEntryProvider(DataClient.Client, lv.SelectedIndex);
                var foo = new ItemProviderTurbo<FileEntry>(provider);
                foo.Update();
                file_list.ItemsSource = foo;
            }
            else
            {
                (currentProvider.Provider as FileEntryProvider).SetDirIndex(lv.SelectedIndex);
                currentProvider.Update();
                file_list.Reset();
            }
        }

     


        GridViewColumnHeader _lastHeaderClicked = null;
        SortOrder _lastDirection = SortOrder.Asc;
        void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            SortOrder direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = SortOrder.Asc;
                    }
                    else
                    {
                        if (_lastDirection == SortOrder.Asc)
                        {
                            direction = SortOrder.Desc;
                        }
                        else
                        {
                            direction = SortOrder.Asc;
                        }
                    }

                    string header = headerClicked.Column.Header as string;
                    DataClient.Client.SendSortOrder((SortColumn)Enum.Parse(typeof(SortColumn), header), direction);
                    _turbo.Update();

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
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
            List<Indexed> entriesSelected = dir_list.SelectedItems.Cast<Indexed>().Distinct().ToList();

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

        private void Delete_Entries_OnClick(object sender, RoutedEventArgs e)
        {
            var dirIndexes = dir_list.FindSelectedItems<Indexed>();

            var dirEntries = new List<DirEntry>();
            foreach (var ix in dirIndexes)
            {
                dirEntries.Add(_dirEntryProvider.GetItem(ix.Index));
            }


            if (dirEntries.Count() == 0)
            {
                MessageBox.Show($"No entries selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBoxResult messageResult;

            if (dirEntries.Count() == 1)
            {
                var dirEntry = dirEntries.First();
                messageResult = MessageBox.Show($"Are you sure you want to remove {dirEntry.Path}?", $"Remove entry {dirEntry.Name}", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }
            else
            {
                messageResult = MessageBox.Show($"Are you sure you want to remove {dirEntries.Count()} entries?", $"Remove entries", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }

            if (messageResult != MessageBoxResult.OK)
                return;

            // Remove the entries
            foreach (var entry in dirEntries)
            {
                try
                {
                    if (File.Exists(entry.Path))
                        File.Delete(entry.Path);

                    if (Directory.Exists(entry.Path))
                        Directory.Delete(entry.Path, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete file {entry.Path} \n Error: {ex.ToString()}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        //private static T FindClickedItem<T>(object sender)
        //    where T : class
        //{
        //    var mi = sender as MenuItem;
        //    if (mi == null)
        //    {
        //        return null;
        //    }

        //    var cm = mi.CommandParameter as ContextMenu;
        //    if (cm == null)
        //    {
        //        return null;
        //    }

        //    return (cm.PlacementTarget as ListViewItem)?.Content as T;
        //}
        
        private void LabelPanel_StateChanged(object sender, EventArgs e)
        {
            UpdateSearchList();
        }
    }
}
