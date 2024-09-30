namespace sw.util
{
    using sw.util;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.InteropServices;
    using System.Threading;

    [Serializable]
    public class KeyedPriorityQueue<K, V, P> where V: class
    {
        private EventHandler<KeyedPriorityQueueHeadChangedEventArgs<V>> firstElementChanged;
        private List<HeapNode<K, V, P>> heap;
        private HeapNode<K, V, P> placeHolder;
        private Comparer<P> priorityComparer;
        Dictionary<K, int> map;
        private int size;

        public event EventHandler<KeyedPriorityQueueHeadChangedEventArgs<V>> FirstElementChanged
        {
            add
            {
                EventHandler<KeyedPriorityQueueHeadChangedEventArgs<V>> handler2;
                EventHandler<KeyedPriorityQueueHeadChangedEventArgs<V>> firstElementChanged = this.firstElementChanged;
                do
                {
                    handler2 = firstElementChanged;
                    EventHandler<KeyedPriorityQueueHeadChangedEventArgs<V>> handler3 = (EventHandler<KeyedPriorityQueueHeadChangedEventArgs<V>>) Delegate.Combine(handler2, value);
                    firstElementChanged = Interlocked.CompareExchange<EventHandler<KeyedPriorityQueueHeadChangedEventArgs<V>>>(ref this.firstElementChanged, handler3, handler2);
                }
                while (firstElementChanged != handler2);
            }
            remove
            {
                EventHandler<KeyedPriorityQueueHeadChangedEventArgs<V>> handler2;
                EventHandler<KeyedPriorityQueueHeadChangedEventArgs<V>> firstElementChanged = this.firstElementChanged;
                do
                {
                    handler2 = firstElementChanged;
                    EventHandler<KeyedPriorityQueueHeadChangedEventArgs<V>> handler3 = (EventHandler<KeyedPriorityQueueHeadChangedEventArgs<V>>) Delegate.Remove(handler2, value);
                    firstElementChanged = Interlocked.CompareExchange<EventHandler<KeyedPriorityQueueHeadChangedEventArgs<V>>>(ref this.firstElementChanged, handler3, handler2);
                }
                while (firstElementChanged != handler2);
            }
        }

        public KeyedPriorityQueue()
        {
            this.heap = new List<HeapNode<K, V, P>>();
            this.priorityComparer = Comparer<P>.Default;
            this.placeHolder = new HeapNode<K, V, P>();
            this.heap.Add(new HeapNode<K, V, P>());
            this.map = new Dictionary<K, int>();
        }

        public void Clear()
        {
            this.heap.Clear();
            this.heap.Add(new HeapNode<K, V, P>());
            map.Clear();
            this.size = 0;
        }

        public V Dequeue()
        {
            V local4;
            V local = (this.size < 1) ? (local4 = default(V)) : this.DequeueImpl();
            V newHead = (this.size < 1) ? (local4 = default(V)) : this.heap[1].Value;
            this.RaiseHeadChangedEvent(default(V), newHead);
            return local;
        }

        private V DequeueImpl()
        {
            V local = this.heap[1].Value;
            map.Remove(this.heap[1].Key);
            this.heap[1] = this.heap[this.size];
            
            this.heap[this.size--] = this.placeHolder;
            if(this.size>0)
                map[heap[1].Key] = 1;
            this.Heapify(1);
            
            return local;
        }

        public void Enqueue(K key, V value, P priority)
        {
            if(map.ContainsKey(key))
            {
                Remove(key);
            }
            V local = (this.size > 0) ? this.heap[1].Value : default(V);
            int num = ++this.size;
            int num2 = num / 2;
            if (num == this.heap.Count)
            {
                this.heap.Add(this.placeHolder);
            }
            while ((num > 1) && this.IsHigher(priority, this.heap[num2].Priority))
            {
                this.heap[num] = this.heap[num2];
                map[this.heap[num].Key] = num;
                num = num2;
                num2 = num / 2;
            }
            this.heap[num] = new HeapNode<K, V, P>(key, value, priority);
            map[key] = num;
            V newHead = this.heap[1].Value;
            if (!newHead.Equals(local))
            {
                this.RaiseHeadChangedEvent(local, newHead);
            }
        }

        public V FindByPriority(P priority, Predicate<V> match)
        {
            if (this.size >= 1)
            {
                return this.Search(priority, 1, match);
            }
            return default(V);
        }

        private void Heapify(int i)
        {
            int num = 2 * i;
            int num2 = num + 1;
            int j = i;
            if ((num <= this.size) && this.IsHigher(this.heap[num].Priority, this.heap[i].Priority))
            {
                j = num;
            }
            if ((num2 <= this.size) && this.IsHigher(this.heap[num2].Priority, this.heap[j].Priority))
            {
                j = num2;
            }
            if (j != i)
            {
                this.Swap(i, j);
                this.Heapify(j);
            }
        }

        protected virtual bool IsHigher(P p1, P p2)
        {
            return (this.priorityComparer.Compare(p1, p2) < 1);
        }

        public V Peek()
        {
            if (this.size >= 1)
            {
                return this.heap[1].Value;
            }
            return default(V);
        }

        private void RaiseHeadChangedEvent(V oldHead, V newHead)
        {
            if (oldHead != newHead)
            {
                EventHandler<KeyedPriorityQueueHeadChangedEventArgs<V>> firstElementChanged = this.firstElementChanged;
                if (firstElementChanged != null)
                {
                    firstElementChanged(this, new KeyedPriorityQueueHeadChangedEventArgs<V>(oldHead, newHead));
                }
            }
        }

        public V Remove(K key)
        {
           
            if (this.size >= 1)
            {
                int i;
                V oldHead = this.heap[1].Value;
                if (map.TryGetValue(key, out i))
                {
                    V local2 = this.heap[i].Value;
                    this.Swap(i, this.size);
                    this.heap[this.size--] = this.placeHolder;
                    this.Heapify(i);
                    V local3 = this.heap[1].Value;
                    if (!oldHead.Equals(local3))
                    {
                        this.RaiseHeadChangedEvent(oldHead, local3);
                    }
                    map.Remove(key);
                    return local2;
                }
                
               
            }
            //LoggerHelper.Debug("remove not found:" + key );
            return default(V);
        }

        private V Search(P priority, int i, Predicate<V> match)
        {
            V local = default(V);
            if (this.IsHigher(this.heap[i].Priority, priority))
            {
                if (match(this.heap[i].Value))
                {
                    local = this.heap[i].Value;
                }
                int num = 2 * i;
                int num2 = num + 1;
                if ((local == null) && (num <= this.size))
                {
                    local = this.Search(priority, num, match);
                }
                if ((local == null) && (num2 <= this.size))
                {
                    local = this.Search(priority, num2, match);
                }
            }
            return local;
        }

        private void Swap(int i, int j)
        {
            HeapNode<K, V, P> node = this.heap[i];
            this.heap[i] = this.heap[j];
            this.heap[j] = node;
            map[heap[i].Key] = i;
            map[heap[j].Key] =j;
        }

        public int Count
        {
            get
            {
                return this.size;
            }
        }

        public ReadOnlyCollection<K> Keys
        {
            get
            {
                List<K> list = new List<K>();
                for (int i = 1; i <= this.size; i++)
                {
                    list.Add(this.heap[i].Key);
                }
                return new ReadOnlyCollection<K>(list);
            }
        }

        public ReadOnlyCollection<V> Values
        {
            get
            {
                List<V> list = new List<V>();
                for (int i = 1; i <= this.size; i++)
                {
                    list.Add(this.heap[i].Value);
                }
                return new ReadOnlyCollection<V>(list);
            }
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        private struct HeapNode<KK, VV, PP>
        {
            public KK Key;
            public VV Value;
            public PP Priority;
            public HeapNode(KK key, VV value, PP priority)
            {
                this.Key = key;
                this.Value = value;
                this.Priority = priority;
            }
        }
    }
}

