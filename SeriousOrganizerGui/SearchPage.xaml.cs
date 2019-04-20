﻿using SeriousOrganizerGui.Data;
using SeriousOrganizerGui.Data.Providers;
using SeriousOrganizerGui.Dto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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


        public SearchPage()
        {
            InitializeComponent();

            _dirEntryProvider = new DirEntryProvider(DataClient.Client);
            _turbo = new ItemProviderTurbo<DirEntry>(_dirEntryProvider);
            DataClient.Client.SendTextSearchChanged(""); // Reset search text
            _turbo.Update();

            dir_list.ItemsSource = _turbo;
            label_list.ItemsSource = DataClient.Label.Get().Select(TriStateToggle.Create);
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
            List<Indexed> entriesSelected = dir_list.SelectedItems.Cast<Indexed>().Distinct().ToList();

            var indexes = new HashSet<int>();
            foreach (var ind in entriesSelected)
            {
                if (!indexes.Add(ind.Index))
                {
                    Debugger.Break();
                }
            }


            var select = new LabelSelect(entriesSelected);
            select.ShowInTaskbar = false;
            select.Owner = (Window)((dynamic)this.Parent).Parent;
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
    }
}
