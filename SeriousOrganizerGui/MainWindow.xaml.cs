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

namespace SeriousOrganizerGui
{

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
