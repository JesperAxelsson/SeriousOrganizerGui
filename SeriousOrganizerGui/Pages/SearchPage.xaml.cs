using SeriousOrganizerGui.Data;
using SeriousOrganizerGui.Data.Providers;
using SeriousOrganizerGui.Dto;
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
    public enum TriState : byte
    {
        Neutral = 0,
        Selected = 1,
        UnSelected = 2,
    }

    public class TriStateToggle : INotifyPropertyChanged
    {


        private Dto.Label _inner;
        private TriState _state;

        public event PropertyChangedEventHandler PropertyChanged;

        public TriStateToggle(Dto.Label inner)
        {
            _state = TriState.Neutral;
            _inner = inner;
        }

        public static TriStateToggle Create(Dto.Label inner) => new TriStateToggle(inner);

        public TriState State
        {
            get => _state;
            set
            {
                _state = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("State"));
            }
        }

        public int Id { get => _inner.Id; }
        public string Name { get => _inner.Name; }
    }

    /// <summary>
    /// Interaction logic for SearchPage.xaml
    /// </summary>
    public partial class SearchPage : UserControl
    {

        private ItemProviderTurbo<DirEntry> _turbo;

        private DirEntryProvider _dirEntryProvider;
        private Lens<Dto.Label, TriStateToggle> _labelLens;


        public SearchPage()
        {
            InitializeComponent();

            _dirEntryProvider = new DirEntryProvider(DataClient.Client);
            _turbo = new ItemProviderTurbo<DirEntry>(_dirEntryProvider);
            DataClient.Client.SendTextSearchChanged(""); // Reset search text
            _turbo.Update();

            dir_list.ItemsSource = _turbo;
            _labelLens = new Lens<Dto.Label, TriStateToggle>(DataClient.Label.Get(), TriStateToggle.Create);
            label_list.ItemsSource = _labelLens.GetSink;
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
                file_list.SelectedItem = null;
            }
        }

        private void file_list_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var lv = sender as ListView;
            if (lv.SelectedIndex < 0) return;

            var path = ((FileEntry)lv.SelectedItem).Path;

            if (File.Exists(path) || Directory.Exists(path))
            {
                Process.Start(new ProcessStartInfo(path) { CreateNoWindow = true, UseShellExecute = true });
            }
            else
            {
                MessageBox.Show("Failed to find file or folder.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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


        private void add_label_Button_Click(object sender, RoutedEventArgs e)
        {
            var select = new AddLabelDialog();
            select.ShowInTaskbar = false;
            select.Owner = Window.GetWindow(this);
            select.ShowDialog();
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

        private void Label_list_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            if (item != null)
            {
                var toggler = item.Content as TriStateToggle;

                switch (toggler.State)
                {
                    case TriState.Neutral:
                        toggler.State = TriState.Selected;
                        break;
                    case TriState.Selected:
                    case TriState.UnSelected:
                        toggler.State = TriState.Neutral;
                        break;
                }

                DataClient.Label.FilterLabel(toggler.Id, (byte)toggler.State);
                UpdateSearchList();
            }
        }

        private void Label_list_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            if (item != null)
            {
                var toggler = item.Content as TriStateToggle;

                switch (toggler.State)
                {
                    case TriState.Neutral:
                        toggler.State = TriState.UnSelected;
                        break;
                    case TriState.Selected:
                    case TriState.UnSelected:
                        toggler.State = TriState.Neutral;
                        break;
                }

                DataClient.Label.FilterLabel(toggler.Id, (byte)toggler.State);
                UpdateSearchList();
            }
        }

        private void remove_label_Button_Click(object sender, RoutedEventArgs e)
        {
            var lbl = label_list.SelectedItem as TriStateToggle;

            if (lbl != null && MessageBox.Show(Window.GetWindow(this), "Are you sure you want to remove label: " + lbl.Name, "Delete", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                DataClient.Label.Remove(lbl.Id);
                UpdateSearchList();
            }
        }


        private void Delete_Files_OnClick(object sender, RoutedEventArgs e)
        {
            var fileEntries = FindSelectedItems<FileEntry>(file_list);
            if (fileEntries.Count() == 0)
            {
                MessageBox.Show($"No files selected", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBoxResult messageResult;

            if (fileEntries.Count() == 1)
            {
                var fileEntry = fileEntries.First();
                messageResult = MessageBox.Show($"Are you sure you want to remove {fileEntry.Path}?", $"Remove file {fileEntry.Name}", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }
            else
            {
                messageResult = MessageBox.Show($"Are you sure you want to remove {fileEntries.Count()} files?", $"Remove files", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }

            if (messageResult != MessageBoxResult.OK)
                return;

            // Remove files
            foreach (var entry in fileEntries)
            {
                try
                {
                    File.Delete(entry.Path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete file {entry.Path} \n Error: {ex.ToString()}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Delete_Entries_OnClick(object sender, RoutedEventArgs e)
        {
            var dirIndexes = FindSelectedItems<Indexed>(dir_list);

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

        private static IEnumerable<T> FindSelectedItems<T>(ListView listView)
            where T : class
        {
            if (listView.SelectedItems.Count > 0)
            {
                return listView.SelectedItems.Cast<T>();
            }

            return Enumerable.Empty<T>();
        }
    }
}
