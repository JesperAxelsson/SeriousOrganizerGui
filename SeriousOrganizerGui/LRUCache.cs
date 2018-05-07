using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace LRUCache
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
        private Dictionary<K, LinkedListNode<LRUCacheItem>> cacheMap = new Dictionary<K, LinkedListNode<LRUCacheItem>>();
        private LinkedList<LRUCacheItem> lruList = new LinkedList<LRUCacheItem>();

        public LRUCache(int capacity)
        {
            this.capacity = capacity;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public V Get(K key, Func<K, V> create)
        {
            LinkedListNode<LRUCacheItem> node;
            if (!cacheMap.TryGetValue(key, out node))
            {
                V val = create(key);
                Add(key, val);
                return val;
            }

            V value = node.Value.value;
            lruList.Remove(node);
            lruList.AddLast(node);
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
            if (cacheMap.Count >= capacity)
            {
                RemoveFirst();
            }

            LRUCacheItem cacheItem = new LRUCacheItem(key, val);
            LinkedListNode<LRUCacheItem> node = new LinkedListNode<LRUCacheItem>(cacheItem);
            lruList.AddLast(node);
            cacheMap.Add(key, node);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Clear()
        {
            cacheMap.Clear();
            lruList.Clear();
        }

        private void RemoveFirst()
        {
            // Remove from LRUPriority
            LinkedListNode<LRUCacheItem> node = lruList.First;
            lruList.RemoveFirst();

            // Remove from cache
            cacheMap.Remove(node.Value.key);
        }


    }

}