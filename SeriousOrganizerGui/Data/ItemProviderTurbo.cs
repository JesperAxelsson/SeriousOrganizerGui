using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriousOrganizerGui.Data
{
    public interface IItemProvider<T>
    {
        int GetCount();
        T GetItem(int index);
    }

    public interface Indexed
    {
        int Index { get; set; }
    }

    public class ItemIndex : Indexed
    {
        public int Index { get; set; }
    }

    public class ItemProviderTurbo<T> : IList<T>, IList, INotifyCollectionChanged
        where T : Indexed
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private readonly IItemProvider<T> _provider;
        private LRUCache<int, T> _lruCache = new LRUCache<int, T>(200);

        public ItemProviderTurbo(IItemProvider<T> provider)
        {
            _provider = provider;
        }

        public void Update()
        {
            Console.WriteLine("Update");
            _lruCache.Clear();
            Count = _provider.GetCount();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public T this[int index] { get => _lruCache.Get(index, k => _provider.GetItem(k)); set => throw new NotImplementedException(); }
        object IList.this[int index] { get => this[index]; set => throw new NotImplementedException(); }

        public int IndexOf(object value)
        {
            return (value as Indexed).Index;
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

        public bool Contains(T item)
        {
            return false;
        }

        #region " Not implemented "



        private class Enumarator
        {
            private int _count = 0;

            public Enumarator(int count)
            {
                _count = count;
            }

            public IEnumerable GetIt()
            {
                for (var i = 0; i < _count; i++)
                {
                    yield return new ItemIndex { Index = i };
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Enumerable.Empty<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            var ena = new Enumarator(Count);
            return ena.GetIt().GetEnumerator();
            //return GetEnumerator();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public void Add(T item)
        {
            throw new NotImplementedException("Add() Not used!");
        }

        public void Clear()
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
