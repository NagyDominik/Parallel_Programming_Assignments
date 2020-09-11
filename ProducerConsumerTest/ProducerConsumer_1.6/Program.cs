using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ProducerConsumer_1._5
{
    class Program
    {
        const int BUFFER_SIZE = 10;

        const int MIN_NUMBER = 1;
        const int MAX_NUMBER = 20;

        const int PRODUCER_DELAY = 1000;
        const int CONSUMER_DELAY = 2000;

        static void Main(string[] args)
        {
            BlockingCollection<int> buffer = new BlockingCollection<int>(BUFFER_SIZE);

            Thread producer = new Thread(() => Produce(buffer, MIN_NUMBER, MAX_NUMBER, PRODUCER_DELAY));
            Thread consumer = new Thread(() => Consume(buffer, CONSUMER_DELAY));

            producer.Start();
            consumer.Start();

            producer.Join(); // Wait until producer is done

            buffer.CompleteAdding();

            consumer.Join();

            Console.WriteLine("All done!");

            Console.ReadLine();
        }

        public static void Produce(BlockingCollection<int> buffer, int min, int max, int delayInMillis)
        {
            for (int i = min; i <= max; i++)
            {
                buffer.Add(i);
                Console.WriteLine($"Producer added: {i}");

                Thread.Sleep(delayInMillis);
            }
            Console.WriteLine("Producer done!");
        }

        public static void Consume(BlockingCollection<int> buffer, int delayInMillis)
        {
            while (!buffer.IsCompleted)
            {
                Console.WriteLine($"Consumer fetched: {buffer.Take()}");
                Thread.Sleep(delayInMillis);
            }

            Console.WriteLine("Consumer done!");
        }
    }
}
