using MessagePack;
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

    public abstract class Indexed  //: IEqualityComparer<Indexed>, IEqualityComparer
    {

        [IgnoreMember]

        public int Index { get; set; }

        //public bool Equals(Indexed x, Indexed y)
        //{
        //    return x.Index == y.Index;
        //}

        //public new bool Equals(object x, object y)
        //{
        //    return (Indexed)x.Equals((Indexed)y);
        //}

        //public int GetHashCode(Indexed obj)
        //{
        //    return obj.Index * 2147483647;
        //}

        public override int GetHashCode()
        {
            return Index * 2147483647;
        }

        public override bool Equals(Object? obj)
        {
            if (obj == null || !(obj is Indexed))
                return false;
            else
                return Index == ((Indexed)obj).Index;
        }
    }

    public class ItemIndex : Indexed
    {
    }

    public class ItemProviderTurbo<T> : IList<T>, IList, INotifyCollectionChanged
        where T : Indexed
    {
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        private IItemProvider<T> _provider;
        private LRUCache<int, T> _lruCache = new LRUCache<int, T>(200);

        public ItemProviderTurbo(IItemProvider<T> provider)
        {
            _provider = provider;
        }

        public IItemProvider<T> Provider { get => _provider; }

        public void ChangeProvider(IItemProvider<T> provider)
        {
            _provider = provider;
            Update();
        }

        public void Update()
        {
            Console.WriteLine("Update");
            _lruCache.Clear();
            Count = _provider.GetCount();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public T this[int index] { get => _lruCache.Get(index, k => _provider.GetItem(k)); set => throw new NotImplementedException(); }
        object? IList.this[int index] { get => this[index]; set => throw new NotImplementedException(); }

        public int IndexOf(object? value)
        {
            var val = (value as Indexed);
            if (val == null) return -1;
            return val.Index;
        }

        public int Count { get; private set; }

        public bool IsReadOnly => false;
        public bool IsFixedSize => true;
        public object SyncRoot => new object();
        public bool IsSynchronized => false;

        public bool Contains(object? value)
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

        public int Add(object? value)
        {
            throw new NotImplementedException();
        }


        public void Insert(int index, object? value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object? value)
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
