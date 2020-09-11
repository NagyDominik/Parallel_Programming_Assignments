using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ProducerConsumer_1._5
{
    public class ThreadSafeQueue<T>
    {
        private readonly T[] buffer;
        private readonly int maxSize;
        private int first;
        private int last;

        private readonly SemaphoreSlim bufferFull;
        private readonly SemaphoreSlim bufferEmpty;

        public T EXIT { get; private set; }
        public int Count { get; private set; }

        public ThreadSafeQueue(int size, T exit)
        {
            buffer = new T[maxSize = size];
            Count = first = last = 0;
            EXIT = exit;

            bufferFull = new SemaphoreSlim(maxSize, maxSize);
            bufferEmpty = new SemaphoreSlim(0, maxSize);
        }

        public void Add(T item)
        {
            bufferFull.Wait();
            lock (this)
            {
                if (Count == maxSize)
                {
                    throw new InvalidOperationException("Buffer is full!");
                }
                buffer[first++] = item;
                first %= maxSize; // When "first" reaches the end of the buffer automatically resets to 0
                Count++;
            }
            bufferEmpty.Release();
        }

        public T Fetch()
        {
            bufferEmpty.Wait();
            T item = default(T);
            lock (this)
            {
                if (Count == 0)
                {
                    throw new InvalidOperationException("Buffer is empty!");
                }
                item = buffer[last++];
                last %= maxSize; // When "last" reaches the end of the buffer automatically resets to 0
                Count--;
            }
            bufferFull.Release();
            return item;
        }
    }
}
