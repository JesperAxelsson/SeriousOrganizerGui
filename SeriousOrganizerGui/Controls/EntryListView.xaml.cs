using SeriousOrganizerGui.Data;
using SeriousOrganizerGui.Dto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SeriousOrganizerGui.Extensions;
using System.Linq;
using System.IO;
using SeriousOrganizerGui.Data.Providers;
using System.Diagnostics;

namespace SeriousOrganizerGui.Controls
{

    public class SelectedIndexChangedEventArgs : EventArgs
    {
        public int Index { get; set; }

        public SelectedIndexChangedEventArgs(int index)
        {
            Index = index;
        }
    }

    /// <summary>
    /// Interaction logic for EntryListView.xaml
    /// </summary>
    public partial class EntryListView : UserControl
    {
        public event EventHandler? SelectedIndexChanged;

        private void SendSelectedIndexChanged(int newIndex)
        {
            SelectedIndexChanged?.Invoke(this, new SelectedIndexChangedEventArgs(newIndex));
        }

        private ItemProviderTurbo<DirEntry>? _turbo;

        private DirEntryProvider _dirEntryProvider;

        public EntryListView()
        {
            InitializeComponent();


            _dirEntryProvider = new DirEntryProvider(DataClient.Client);
            _turbo = new ItemProviderTurbo<DirEntry>(_dirEntryProvider);
            dir_list.ItemsSource = _turbo;
        }

        public void Refresh()
        {
            _turbo!.Update();
        }

        public IEnumerable<T> FindSelectedItems<T>()
            where T : class
        {
            return dir_list.FindSelectedItems<T>();
        }

        GridViewColumnHeader? _lastHeaderClicked = null;
        SortOrder _lastDirection = SortOrder.Asc;
        void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader? headerClicked = e.OriginalSource as GridViewColumnHeader;
            SortOrder direction;

            if (headerClicked == null) return;

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

                string? header = headerClicked.Column.Header as string;
                if (header == null) return;
                DataClient.Client.SendSortOrder((SortColumn)Enum.Parse(typeof(SortColumn), header), direction);
                _turbo!.Update();

                _lastHeaderClicked = headerClicked;
                _lastDirection = direction;
            }
        }

        private void dir_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lv = sender as ListView;
            if (lv?.SelectedIndex < 0) return;

            SendSelectedIndexChanged(lv!.SelectedIndex);
        }

        private void Delete_Entries_OnClick(object sender, RoutedEventArgs e)
        {
            var dirIndexes = dir_list.FindSelectedItems<Indexed>();
            var dirEntries = dirIndexes.Select(i => _dirEntryProvider.GetItem(i.Index)).ToList();

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
            Util.DeletePath(dirEntries.Select(de => de!.Path));
        }

        private void OpenInExplorer_OnClick(object sender, RoutedEventArgs e)
        {
            var clickedItem = Util.FindClickedItem<Indexed>(sender as MenuItem);
            if (clickedItem == null) return;

            var path = _dirEntryProvider.GetItem(clickedItem.Index).Path;
            if (path == null) return;

            Util.OpenPathInExplorer(path);
        }

        private void RenameEntry_OnClick(object sender, RoutedEventArgs e)
        {
            var clickedItem = Util.FindClickedItem<Indexed>(sender as MenuItem);
            if (clickedItem == null) return;

            var entry = _dirEntryProvider.GetItem(clickedItem.Index);
            if (entry == null) return;

            var renameDialog = new RenameFolderDialog(entry);
            renameDialog.ShowInTaskbar = false;
            renameDialog.Owner = Window.GetWindow(this);
            renameDialog.ShowDialog();
        }
    }
}
