using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriousOrganizerGui.Data
{
    public class BetterObservable<T> : IList<T>, IEnumerable<T>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private List<T> _store;

        private void PropChanged(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        private void ColChanged(NotifyCollectionChangedAction action) => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action));

        #region " Properties "
        public int Count => _store.Count;

        public bool IsReadOnly => ((IList<T>)_store).IsReadOnly;

        public T this[int index]
        {
            get => _store[index];
            set
            {
                _store[index] = value;
                ColChanged(NotifyCollectionChangedAction.Replace);
            }
        }
        #endregion

        #region " Constructors "

        public BetterObservable()
        {
            _store = new List<T>();
        }

        public BetterObservable(int capacity)
        {
            _store = new List<T>(capacity);
        }

        public BetterObservable(IEnumerable<T> collection)
        {
            _store = new List<T>(collection);
        }

        #endregion



        public void Replace(IEnumerable<T> collection)
        {
            _store.Clear();
            _store.AddRange(collection);
            PropChanged("Count");
            ColChanged(NotifyCollectionChangedAction.Reset);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            _store.AddRange(collection);
            PropChanged("Count");
            ColChanged(NotifyCollectionChangedAction.Reset);
        }

        public void Add(T item)
        {
            _store.Add(item);
            PropChanged("Count");
            ColChanged(NotifyCollectionChangedAction.Add);
        }

        public void Insert(int index, T item)
        {
            _store.Insert(index, item);
            PropChanged("Count");
            ColChanged(NotifyCollectionChangedAction.Add);
        }

        public void RemoveAt(int index)
        {
            _store.RemoveAt(index);
            PropChanged("Count");
            ColChanged(NotifyCollectionChangedAction.Remove);
        }

        public bool Remove(T item)
        {
            var removed = _store.Remove(item);
            PropChanged("Count");
            ColChanged(NotifyCollectionChangedAction.Remove);
            return removed;
        }

        public void Clear()
        {
            _store.Clear();
            PropChanged("Count");
            ColChanged(NotifyCollectionChangedAction.Reset);
        }


        public int IndexOf(T item) => _store.IndexOf(item);
        public bool Contains(T item) => _store.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => _store.CopyTo(array, arrayIndex);
        public IEnumerator<T> GetEnumerator() => _store.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _store.GetEnumerator();
    }
}
