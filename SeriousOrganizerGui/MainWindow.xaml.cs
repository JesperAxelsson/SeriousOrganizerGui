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



    public class ItemProviderTurbo : IList<DirEntry>, IEnumerable<DirEntry>, INotifyCollectionChanged
    {
        //private List<DirEntry> _store;
        private readonly Client _client;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        //public void NotifyPropertyChanged(string propName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        //}

        public ItemProviderTurbo(Client client)
        {
            //_store = new List<DirEntry>();
            _client = client;
        }

        public DirEntry this[int index] { get => _client.GetDir(index); set => throw new NotImplementedException(); }

        public int Count => _client.GetDirCount();

        public bool IsReadOnly => false;

        private IEnumerable<DirEntry> DirEntries()
        {
            var count = Count;
            for (var i = 0; i < count; i++)
            {
                Console.WriteLine("Loop: " + i);
                yield return this[i];
            }
        }

        public void Update()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public IEnumerator<DirEntry> GetEnumerator()
        {
            return DirEntries().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region " Skip these "

        public bool Contains(DirEntry item)
        {
            throw new NotImplementedException();
        }

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

        #endregion
    }


    public class ItemProvider<T> : IList<T>
    {
        private List<T> _store;

        private ItemProvider()
        {
            _store = new List<T>();
        }

        public T this[int index] { get => _store[index]; set => throw new NotImplementedException(); }

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(T item)
        {
            throw new NotImplementedException("Add() Not used!");
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
