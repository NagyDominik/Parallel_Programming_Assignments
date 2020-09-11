using System;
using System.Collections.Generic;
using System.Text;

namespace ProducerConsumer_1._3
{
    public class CircularBuffer<T> //FIFO
    {
        private readonly T[] buffer;
        private readonly int maxSize;
        private int first;
        private int last;

        public T EXIT { get; private set; }
        public int Count { get; private set; }

        public CircularBuffer(int size, T exit)
        {
            buffer = new T[maxSize = size];
            Count = first = last = 0;
            EXIT = exit;
        }

        public void Add(T item)
        {
            if (Count == maxSize)
            {
                throw new InvalidOperationException("Buffer is full!");
            }
            buffer[first++] = item;
            first %= maxSize; // When "first" reaches the end of the buffer automatically resets to 0
            Count++;
        }

        public T Fetch()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("Buffer is empty!");
            }
            T item = buffer[last++];
            last %= maxSize; // When "last" reaches the end of the buffer automatically resets to 0
            Count--;
            return item;
        }
    }
}
