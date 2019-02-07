using SeriousOrganizerGui.Dto;
using System;
using System.Collections;
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

using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using SeriousOrganizerGui.Data;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Markup;

namespace SeriousOrganizerGui
{
    public abstract class BaseConverter : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class SizeToStringConverter : BaseConverter, IValueConverter
    {
        const UInt64 KB = 1000;
        const UInt64 MB = KB * KB;
        const UInt64 GB = KB * KB * KB;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return null;
            var size = (UInt64)value;

            if (size > GB) return (size / GB) + " GB";
            if (size > MB) return (size / MB) + " MB";
            if (size > KB) return (size / KB) + " KB";

            return value + " B";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class NameToBrushConverter : BaseConverter, IValueConverter
    {
        static readonly string[] MovieExtentions = { ".mp4", ".wmv", ".m4v", ".avi", ".mkv" };

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string input = (value as string).TrimEnd();

            if (MovieExtentions.Any(me => input.EndsWith(me, StringComparison.Ordinal)))
                return Brushes.Blue;

            return DependencyProperty.UnsetValue;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Client _client = new Client();
        private ItemProviderTurbo<DirEntry> _turbo;

        private DirEntryProvider _dirEntryProvider;

        public MainWindow()
        {
            InitializeComponent();

            _client.Connect();
            _dirEntryProvider = new DirEntryProvider(_client);
            _turbo = new ItemProviderTurbo<DirEntry>(_dirEntryProvider);
            _client.SendTextSearchChanged(""); // Reset search text
            _turbo.Update();

            dir_list.ItemsSource = _turbo;


            //var bin = new Binding("");
            //bin.Mode = 
        }

        private void UpdateSearchList()
        {
            _turbo.Update();
            Console.WriteLine($"Update Search List");
        }

        private void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = ((TextBox)sender).Text;
            var new_length = _client.SendTextSearchChanged(text);
            UpdateSearchList();
        }

        private void dir_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lv = sender as ListView;
            if (lv.SelectedIndex < 0) return;

            Console.WriteLine("Selected row: " + lv.SelectedIndex);
            var provider = new FileEntryProvider(_client, lv.SelectedIndex);
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
                    _client.SendSortOrder((SortColumn)Enum.Parse(typeof(SortColumn), header), direction);
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

            _client.SendLabelAdd(name);

        }

        private void BtnGetLabels_Click(object sender, RoutedEventArgs e)
        {
            var lbls = _client.SendLabelsGet();
            Console.WriteLine(lbls);
        }
    }

    public class DirEntryProvider : IItemProvider<DirEntry>
    {
        private readonly Client _client;

        public DirEntryProvider(Client client)
        {
            _client = client;
        }

        public int GetCount()
        {
            return _client.GetDirCount();
        }

        public DirEntry GetItem(int index)
        {
            return _client.GetDir(index);
        }
    }

    public class FileEntryProvider : IItemProvider<FileEntry>
    {
        private readonly Client _client;
        private readonly int _dirIndex;

        public FileEntryProvider(Client client, int dirIndex)
        {
            _client = client;
            _dirIndex = dirIndex;
        }

        public int GetCount()
        {
            return _client.GetFileCount(_dirIndex);
        }

        public FileEntry GetItem(int index)
        {
            return _client.GetFile(_dirIndex, index);
        }
    }
}
