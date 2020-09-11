using System;
using System.Threading;

namespace ProducerConsumer_1._3
{
    class Program
    {
        const int BUFFER_SIZE = 10;
        const int EXIT = -1;

        const int MIN_NUMBER = 1;
        const int MAX_NUMBER = 20;

        const int PRODUCER_DELAY = 1000;
        const int CONSUMER_DELAY = 2000;

        private static readonly SemaphoreSlim bufferFull = new SemaphoreSlim(BUFFER_SIZE, BUFFER_SIZE); // Decreases the first property, each time Produce is called, until it reaches 0
        private static readonly SemaphoreSlim bufferEmpty = new SemaphoreSlim(0, BUFFER_SIZE); // Increases the first property, each time Produce is called, until it reaches the second property

        static void Main(string[] args)
        {
            CircularBuffer<int> buffer = new CircularBuffer<int>(BUFFER_SIZE, EXIT);

            Thread producer = new Thread(() => Produce(buffer, MIN_NUMBER, MAX_NUMBER, PRODUCER_DELAY));
            Thread consumer = new Thread(() => Consume(buffer, CONSUMER_DELAY));

            producer.Start();
            consumer.Start();

            producer.Join(); // Wait until producer is done.

            bufferFull.Wait();
            buffer.Add(buffer.EXIT);
            bufferEmpty.Release();

            consumer.Join();

            Console.WriteLine("All done!");

            Console.ReadLine();
        }

        public static void Produce(CircularBuffer<int> buffer, int min, int max, int delayInMillis)
        {
            for (int i = min; i <= max; i++)
            {
                bufferFull.Wait();
                buffer.Add(i);
                bufferEmpty.Release();
                Console.WriteLine($"Producer added: {i}");
                
                Thread.Sleep(delayInMillis);
            }
            Console.WriteLine("Producer done!");
        }

        public  static void Consume(CircularBuffer<int> buffer, int delayInMillis)
        {
            bufferEmpty.Wait();
            int num = buffer.Fetch();
            bufferFull.Release();

            while (num != buffer.EXIT)
            {
                Console.WriteLine($"Consumer fetched: {num}");
                Thread.Sleep(delayInMillis);

                bufferEmpty.Wait();
                num = buffer.Fetch();
                bufferFull.Release();
            }

            bufferFull.Wait();
            buffer.Add(buffer.EXIT); // Putting back the exit vaule to the buffer for other possible consumers
            bufferEmpty.Release();

            Console.WriteLine("Consumer done!");
        }
    }
}
