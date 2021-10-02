/*
using System;
using System.Collections;
using System.Collections.Generic;

class Indexed_Queue<T> : IEnumerable<T>
{
    T[] backing_array;
    int first_item, last_item = -1;

    public Indexed_Queue()
    {
        backing_array = new T[2];
    }

    /// <param name="Initial_Capacity">the max capacity of the array</param>
    /// <param name="AutoResize">set the queue to resize when it reaches capacity</param>
    public Indexed_Queue(int Initial_Capacity, bool AutoResize = true)
    {
        this.AutoResize = AutoResize;
        if (Initial_Capacity <= 0)
            throw new Exception("Queue cannot be initialized with a capacity of 0 or less items");

        backing_array = new T[Initial_Capacity];
    }

    /// <summary>
    /// current capacity of the queue
    /// </summary>
    public int Capacity => backing_array.Length;

    /// <summary>
    /// set true to automatically resize the backing array when the queue reaches it's capacity
    /// </summary>
    public bool AutoResize = true;

    public T this[int index]
    {
        get
        {
            if (!TryPeek(index, out var value))
                throw new Exception("index is out of bounds of the queued items");
            return value;
        }
    }

    /// <summary>
    /// amount of items in the queue
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// tries to add an item into the queue.
    /// </summary>
    /// <param name="item">the item to enqueue</param>
    /// <returns> false if no more items can be added</returns>
    public bool TryEnqueue(T item)
    {
        if (Count == backing_array.Length)
        {
            if (!AutoResize)
                return false;

            T[] new_array = new T[backing_array.Length * 2];

            for (int i = first_item; i < backing_array.Length; ++i)
            {
                new_array[i - first_item] = backing_array[i];
            }
            var pos = Count - first_item;
            for (int i = 0; i < last_item + 1; ++i)
            {
                new_array[pos + i] = backing_array[i];
            }

            first_item = 0;
            last_item = Count - 1;
            backing_array = new_array;
        }

        Count++;
        last_item++;
        if (last_item == backing_array.Length)
            last_item = 0;
        backing_array[last_item] = item;
        return true;
    }

    /// <summary>
    /// If queue is full, this will forcibly dequeue first item in the queue to make room
    /// </summary>
    /// <param name="item">item to enqueue</param>
    /// <param name="dequeued">item dequeued from queue</param>
    /// <returns>true if item was dequeued</returns>
    public bool ForceEnqueue(T item, out T dequeued)
    {
        if (!TryEnqueue(item))
        {
            TryDequeue(out dequeued);
            TryEnqueue(item);
            return true;
        }
        dequeued = default;
        return false;
    }

    /// <summary>
    /// removes from and retrieves the first item in the queue.
    /// </summary>
    /// <param name="item">the item that was dequeued</param>
    /// <returns>false if there are no more items in the queue</returns>
    public bool TryDequeue(out T item)
    {
        if (Count == 0)
        {
            item = default;
            return false;
        }
        item = backing_array[first_item];
        backing_array[first_item] = default;

        first_item++;
        if (first_item == backing_array.Length)
            first_item = 0;
        Count--;
        return true;
    }

    /// <summary>
    /// tries to peek into the queue.
    /// returns false if there are no items at the specified depth
    /// </summary>
    /// <param name="depth">how many places to peek into the queue</param>
    /// <param name="item">the item at that position</param>
    /// <returns></returns>
    public bool TryPeek(int depth, out T item)
    {
        if (depth >= Count || depth < 0)
        {
            item = default;
            return false;
        }
        var pos = first_item + depth;
        if (pos >= backing_array.Length)
            pos -= backing_array.Length;
        item = backing_array[pos];
        return true;
    }

    public void Clear()
    {
        Array.Clear(backing_array, 0, backing_array.Length);
        first_item = 0;
        last_item = 0;
        Count = 0;
    }

    class QueueEnumerator : IEnumerator<T>
    {
        public Indexed_Queue<T> queue;

        T current;
        public T Current => current;

        object IEnumerator.Current => current;

        int index = -1;

        public void Dispose()
        {
        }

        public bool MoveNext()
        => queue.TryPeek(++index, out current);


        public void Reset()
        {

        }
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => new QueueEnumerator { queue = this };
    IEnumerator IEnumerable.GetEnumerator() => new QueueEnumerator { queue = this };
}

class PriorityQueue<T> : IEnumerable<(float priority, T item)>
{
    public PriorityQueue()
    {
        queue = new (float, T)[16];
    }

    public PriorityQueue(int initial_capacity)
    {
        queue = new (float, T)[initial_capacity];
    }

    public PriorityQueue(params (float priority, T item)[] args)
    {
        queue = new (float, T)[args.Length > 16 ? args.Length : 16];
        foreach(var item in args)
        {
            Enqueue(item.priority, item.item);
        }
    }

    (float priority, T item)[] queue;
    int count = 0;

    public void Enqueue(float priority, T item)
    {
        if (count == queue.Length)
            System.Array.Resize(ref queue, count * 2);

        int index = count;
        while (index >= 1)
        {
            if (queue[index - 1].priority > priority)
            {
                queue[index] = (priority, item);
                count++;
                return;
            }
            queue[index] = queue[index - 1];
            index--;

        }
        queue[0] = (priority, item);
        count++;
    }

    /// <summary>
    /// dequeues items with the lowest priority first
    /// </summary>
    public bool TryDequeue(out T item)
    {
        if (count == 0)
        {
            item = default;
            return false;
        }
        count--;
        item = queue[count].item;
        queue[count] = default;
        return true;
    }

    public void Clear()
    {
        for (int i = 0; i < count; ++i)
            queue[i] = default;
        count = 0;
    }

    IEnumerator<(float priority, T item)> IEnumerable<(float priority, T item)>.GetEnumerator()
    => new Priority_Enumerator { queue = this, index = count };

    IEnumerator IEnumerable.GetEnumerator()
    => new Priority_Enumerator { queue = this, index = count };

    class Priority_Enumerator : IEnumerator<(float, T)>
    {
        public PriorityQueue<T> queue;
        public int index;

        public object Current => queue.queue[index];

        (float, T) IEnumerator<(float, T)>.Current => queue.queue[index];

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            index--;
            return index >= 0;
        }

        public void Reset()
        {

        }
    }
}*/