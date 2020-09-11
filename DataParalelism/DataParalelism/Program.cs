using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DataParalelism
{
    class Program
    {
        private static readonly int LENGTH = 100_000_000;

        static void Main(string[] args)
        {
            MessureTime(() => Ex2());
            MessureTime(() => Ex3());
            MessureTime(() => Ex4());

            Console.ReadLine();
        }


        private static void MessureTime(Action p)
        {
            Stopwatch sw = Stopwatch.StartNew();
            p.Invoke();
            sw.Stop();
            Console.WriteLine("Time = {0:F5} sec.", sw.ElapsedMilliseconds / 1000d);
            sw.Reset();
        }

        static void Ex2()
        {
            Console.WriteLine("Exercise 2:");
            int[] large_array = new int[LENGTH];
            ParallelGetRandomArray(large_array);

            Parallel.For(0, large_array.Length, (i) => {
                large_array[i] = (int)Math.Pow(large_array[i], 2);
            });
        }

        static void Ex3()
        {
            Console.WriteLine("Exercise 3:");
            int[] a = new int[LENGTH];
            int[] b = new int[LENGTH];
            int[] c = new int[LENGTH];

            Parallel.For(0, c.Length, (i) => {
                c[i] = a[i] + b[i];
            });

        }

        static void Ex4()
        {
            Console.WriteLine("Exercise 4:");
            int[] large_array = new int[LENGTH];
            ParallelGetRandomArray(large_array);

            Parallel.ForEach( Partitioner.Create(0, large_array.Length), (range) => {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    large_array[i] = (int)Math.Pow(large_array[i], 2);
                }
            });


            int[] a = new int[LENGTH];
            int[] b = new int[LENGTH];
            int[] c = new int[LENGTH];

            Parallel.ForEach(Partitioner.Create(0, c.Length), (range) => {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    c[i] = a[i] + b[i];
                }
            });
        }

        static int[] SequentialGetRandomArray(int[] a)
        {
            Random rnd = new Random();
            int[] ret = new int[LENGTH];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = rnd.Next();
            }

            return ret;
        }

        private static void ParallelGetRandomArray(int[] a)
        {
            Parallel.ForEach(
                Partitioner.Create(0, a.Length),
                new ParallelOptions(),
                () => { return new Random((int)DateTime.Now.Ticks); },
                (range, loopState, rnd) =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                        a[i] = rnd.Next(1, LENGTH);
                    return rnd;
                },
                // Finalizer section - nothing to do
                _ => { }
            );
        }

        static void Ex6()
        {

        }
    }
}
