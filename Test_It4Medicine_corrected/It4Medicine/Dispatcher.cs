using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading;

namespace It4Medicine
{
    class Dispatcher : IDisposable
    {
        private int _numberOfThreads,_threadInfoStoreSize;
        private volatile bool _stopThread;

        private BlockingCollection<KeyValuePair<int, string>> ThreadInfoStore { get; set; }
        private BlockingCollection<Thread> ThreadStore { get; set; }

        private void InitStore()
        {
            if (ThreadInfoStore != null)
                ThreadInfoStore.Dispose();
            if (ThreadStore != null)
                ThreadStore.Dispose();

            ThreadInfoStore = new BlockingCollection<KeyValuePair<int, string>>();
            ThreadStore = new BlockingCollection<Thread>();
        }
        public void Start(int numberOfThreads, int threadInfoStoreSize)
        {
            _numberOfThreads = numberOfThreads;
            _threadInfoStoreSize = threadInfoStoreSize;

            InitStore();
            for (int i = 1; i <= _numberOfThreads; i++)
            {
                var thread = new Thread(ThreadProcess);
                thread.Start(i);
                ThreadStore.Add(thread);
            }
        }

        private void ThreadProcess(object threadIndex)
        {
            var rand = new Random();
            while (!_stopThread)
            {
                try
                {
                    var addedItem = new KeyValuePair<int, string>((int)threadIndex, Guid.NewGuid().ToString());
                    ThreadInfoStore.Add(addedItem);
                    Console.WriteLine("Thread {0} add {1}", addedItem.Key, addedItem.Value);

                    var firstItem = ThreadInfoStore.First();
                    if ((int)threadIndex == firstItem.Key || ThreadInfoStore.Count > _threadInfoStoreSize)
                    {
                        ThreadInfoStore.TryTake(out firstItem);
                        Console.WriteLine("Thread {0} delete {1}", firstItem.Key, firstItem.Value);
                    }
                
                    Thread.Sleep(rand.Next(300));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }

        public void Stop()
        {
            _stopThread = true;
            foreach (var thread in ThreadStore)
            {
                if (!thread.Join(500))
                {
                    thread.Abort();
                }
            }
        }

        public void ShowStatistic()
        {
            Console.WriteLine(new string('=', 20));
            
            foreach (var group in ThreadInfoStore.OrderBy(pair => pair.Key).GroupBy(pair => pair.Key))
                Console.WriteLine("Thread {0} - count = {1}", group.Key, group.Count());

            Console.WriteLine("Max container elements: " + ThreadInfoStore.Count());
        }

        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
        }
    }
}

