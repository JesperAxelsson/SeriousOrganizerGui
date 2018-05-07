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

namespace SeriousOrganizerGui
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<DirEntry> _dirs = new ObservableCollection<DirEntry>();
        private Client _client = new Client();
        private ItemProviderTurbo _turbo;

        public MainWindow()
        {
            InitializeComponent();

            _client.Connect();
            _turbo = new ItemProviderTurbo(_client);
            _client.SendTextSearchChanged("");
            _turbo.Update();

            _client.SendTest("Ello from the other side!");


            //dir_list.ItemsSource = _dirs;
            dir_list.ItemsSource = _turbo;
        }

        private void UpdateSearchList()
        {
            var dirCount = _client.GetDirCount();


            var st = Stopwatch.StartNew();
            //_dirs.Clear();
            //for (var i = 0; i < dirCount; i++)
            //{
            //    _dirs.Add(_client.GetDir(i));
            //}

            _turbo.Update();

            st.Stop();
            var ela = st.ElapsedMilliseconds;
            Console.WriteLine($"Update in: {ela} ms");
        }

        private void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = ((TextBox)sender).Text;
            var new_length = _client.SendTextSearchChanged(text);
            UpdateSearchList();
        }
    }



    public class ItemProviderTurbo : IList<DirEntry>, IList, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private readonly Client _client;
        private LRUCache.LRUCache<int, DirEntry> _lruCache = new LRUCache.LRUCache<int, DirEntry>(200);

        public ItemProviderTurbo(Client client)
        {
            _client = client;
        }

        public DirEntry this[int index] { get { Console.WriteLine($"Get index: {index}"); return _lruCache.Get(index, k => _client.GetDir(k)); } set => throw new NotImplementedException(); }
        object IList.this[int index] { get => this[index]; set => throw new NotImplementedException(); }

        public int IndexOf(object value)
        {
            return -1;
        }


        public int Count { get; private set; }

        public bool IsReadOnly => false;
        public bool IsFixedSize => true;
        public object SyncRoot => new object();
        public bool IsSynchronized => false;


        public bool Contains(object value)
        {
            return false;
        }
        public bool Contains(DirEntry item)
        {
            return false;
        }

        public void Update()
        {
            Console.WriteLine("Update");
            _lruCache.Clear();
            Count = _client.GetDirCount();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public IEnumerator<DirEntry> GetEnumerator()
        {
            return Enumerable.Empty<DirEntry>().GetEnumerator();
            //return DirEntries().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region " Skip these "


        public void CopyTo(DirEntry[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public void Add(DirEntry item)
        {
            throw new NotImplementedException("Add() Not used!");
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(DirEntry item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, DirEntry item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(DirEntry item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public int Add(object value)
        {
            throw new NotImplementedException();
        }


        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
