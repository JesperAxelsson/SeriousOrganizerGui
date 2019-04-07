using SeriousOrganizerGui.Data;
using SeriousOrganizerGui.Data.Providers;
using SeriousOrganizerGui.Dto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

            DataClient.Client.Connect();
            _dirEntryProvider = new DirEntryProvider(DataClient.Client);
            _turbo = new ItemProviderTurbo<DirEntry>(_dirEntryProvider);
            DataClient.Client.SendTextSearchChanged(""); // Reset search text
            _turbo.Update();

            dir_list.ItemsSource = _turbo;
            label_list.ItemsSource = DataClient.Label.Get();
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

            Console.WriteLine("Selected row: " + lv.SelectedIndex);
            var provider = new FileEntryProvider(DataClient.Client, lv.SelectedIndex);
            var foo = new ItemProviderTurbo<FileEntry>(provider);
            foo.Update();
            file_list.ItemsSource = foo;
        }

        private void file_list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var lv = sender as ListView;
            if (lv.SelectedIndex < 0) return;

            var path = ((FileEntry)lv.SelectedItem).Path;

            Process.Start(new ProcessStartInfo(path) { CreateNoWindow = true });
        }

        private void MoreInfo(object sender, RoutedEventArgs e)
        {
            var lv = sender as ListView;
            if (lv.SelectedIndex < 0) return;

            var path = ((FileEntry)lv.SelectedItem).Path;

            Process.Start(new ProcessStartInfo(path) { CreateNoWindow = true });
        }

        private void menuItem_CopyUsername_Click(object sender, RoutedEventArgs e)
        {
            var items1 = file_list.SelectedItems;
            var items = file_list.SelectedItems.Cast<FileEntry>().Select(o => o.Path).ToList();
            var count = items.LongCount();
            //if (items.Count == 0)
            //{
            //    MessageBox.Show("No items selected");
            //    return;
            //}

            //if (MessageBox.Show($"Are you sure you want to delete {items.Count} items?", "Delete confirmation") == MessageBoxResult.Cancel)
            //    return; // Skip deleting

            //foreach (var path in items.)
            //    File.Delete()

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


        private void BtnAddLabel_Click(object sender, RoutedEventArgs e)
        {
            var name = txt_newlabel.Text;
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            DataClient.Label.Add(name);
        }

        private void BtnOpenLabelSelect(object sender, RoutedEventArgs e)
        {
            List<Indexed> entriesSelected = dir_list.SelectedItems.Cast<Indexed>().ToList();

            var select = new LabelSelect();
            select.ShowInTaskbar = false;
            select.Owner = (Window)((dynamic)this.Parent).Parent;
            select.ShowDialog();
        }
    }
}
