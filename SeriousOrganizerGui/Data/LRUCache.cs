using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace SeriousOrganizerGui.Data
{
    public class LRUCache<K, V>
    {

        private class LRUCacheItem
        {
            public LRUCacheItem(K k, V v)
            {
                key = k;
                value = v;
            }
            public K key;
            public V value;
        }

        private int capacity;
        private Dictionary<K, LinkedListNode<LRUCacheItem>> _cacheMap = new Dictionary<K, LinkedListNode<LRUCacheItem>>();
        private LinkedList<LRUCacheItem> _lruList = new LinkedList<LRUCacheItem>();

        public LRUCache(int capacity)
        {
            this.capacity = capacity;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public V Get(K key, Func<K, V> create)
        {
            LinkedListNode<LRUCacheItem> node;
            if (!_cacheMap.TryGetValue(key, out node))
            {
                V val = create(key);
                Add(key, val);
                return val;
            }

            V value = node.Value.value;
            _lruList.Remove(node);
            _lruList.AddLast(node);
            return value;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public V Get(K key)
        {
            return Get(key, (_) => throw new KeyNotFoundException("LRUCache key: " + key));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Add(K key, V val)
        {
            if (_cacheMap.Count >= capacity)
            {
                RemoveFirst();
            }

            LRUCacheItem cacheItem = new LRUCacheItem(key, val);
            LinkedListNode<LRUCacheItem> node = new LinkedListNode<LRUCacheItem>(cacheItem);
            _lruList.AddLast(node);
            _cacheMap.Add(key, node);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Clear()
        {
            _cacheMap.Clear();
            _lruList.Clear();
        }

        private void RemoveFirst()
        {
            // Remove from LRUPriority
            LinkedListNode<LRUCacheItem> node = _lruList.First;
            _lruList.RemoveFirst();

            // Remove from cache
            _cacheMap.Remove(node.Value.key);
        }


    }

}