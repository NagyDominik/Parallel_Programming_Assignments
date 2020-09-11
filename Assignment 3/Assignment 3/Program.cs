using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment_3
{
    class Program
    {
        static void Main(string[] args)
        {
            //Ex1
            //Console.WriteLine("Sequential:");
            //MeasureTime(() => { Method10(); Method5(); });

            //Console.WriteLine("Parallel (Task.StartNew):");
            //MeasureTime(() => { ParallelTaskStartNew(); });

            //Console.WriteLine("Parallel (Parallel.Invoke):");
            //MeasureTime(() => { ParallelParallelInvoke(); });

            //Ex2
            Console.WriteLine("Exercise 2:");
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            Task ex2 = Task.Factory.StartNew(() => Exercise2(token));
            Console.ReadLine();
            cts.Cancel();
            Console.WriteLine("Tasks cancelled!");

            Console.ReadLine();
        }

        private static void MeasureTime(Action p)
        {
            Stopwatch sw = Stopwatch.StartNew();
            p.Invoke();
            sw.Stop();
            Console.WriteLine("Time = {0:F5} sec.", sw.ElapsedMilliseconds / 1000d);
            sw.Reset();
        }

        static void Method10()
        {
            Thread.Sleep(10_000);
        }

        static void Method5()
        {
            Thread.Sleep(5_000);
        }

        static void Method10Busy(CancellationToken token)
        {
            int time = 10_000;
            DateTime start = DateTime.Now;
            while (DateTime.Compare(DateTime.Now, start.AddMilliseconds(time)) >= 0)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Canceling task 1");
                    token.ThrowIfCancellationRequested();
                }
                Thread.Sleep(1000);
            }
        }

        static void Method5Busy(CancellationToken token)
        {
            int time = 5_000;
            DateTime start = DateTime.Now;
            while (DateTime.Compare(DateTime.Now, start.AddMilliseconds(time)) >= 0)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Canceling task 2");
                    token.ThrowIfCancellationRequested();
                }
                Thread.Sleep(1000);
            }
        }

        static void ParallelTaskStartNew()
        {
            TaskFactory tf = new TaskFactory();
            var task1 = tf.StartNew(() => { Method10(); });
            var task2 = tf.StartNew(() => { Method5(); });
            Task.WaitAll(new[] { task1, task2 });
        }

        static void ParallelParallelInvoke()
        {
            Parallel.Invoke(Method10, Method5);
        }

        static void Exercise2(CancellationToken token)
        {
            TaskFactory tf = new TaskFactory();
            var task1 = tf.StartNew(() => { Method10Busy(token); }, token);
            var task2 = tf.StartNew(() => { Method5Busy(token); }, token);
            try
            {
                Task.WaitAll(new[] { task1, task2 });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetType());
            }
            
        }
    }
}
