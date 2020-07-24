using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

public class MultiThreadPool
{
    private ConcurrentQueue<ThreadInfo> threadInfoQueue = new ConcurrentQueue<ThreadInfo>();

    private JobThread[] threads;
    private bool jobs_started;

   

    public void RequestData (Func<object> generateData, Action<object> callback, byte threadID)
    {
        int temp_count = threads[0].CheckJobCount;
        int temp = 0;
        for (int i = 0; i < threads.Length; i++)
        {
            if (temp_count > threads[i].CheckJobCount)
            {
                temp_count = threads[i].CheckJobCount;
                temp = i;
            }
        }
        threads[temp].AddJob(delegate
        { DataThread(generateData, callback);});
    }


    private void DataThread (Func<object> generateData, Action<object> callback)
    {
        object data = generateData ();
        lock (threadInfoQueue)
        {
            threadInfoQueue.Enqueue(new ThreadInfo(callback, data));
        }
    }

    public void Start ()
    {
        if (jobs_started)
        {
            return;
        }
        jobs_started = true;

        for (int i = 0; i < threads.Length; i++)
        {
            threads[i] = new JobThread();
            threads[i].Start();
        }
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public MultiThreadPool (int threadCount)
    {
        //
        jobs_started = false;
        threads = new JobThread[threadCount];
    }

    public void Update ()
    {
        if (threadInfoQueue.Count > 0)
        {
            for (int i = 0; i < threadInfoQueue.Count; i++)
            {
                ThreadInfo threadInfo;
                threadInfoQueue.TryDequeue(out threadInfo);
                threadInfo.callback(threadInfo.parameter);
            }
        }
        for (int i = 0; i < threads.Length; i++)
        {
            if (threads[i].CheckJobCount > 0)
            {
                threads[i].Commit();
            }
        }
    }

}

struct ThreadInfo
{
    public readonly Action<object> callback;
    public readonly object parameter;

    public ThreadInfo (Action<object> callback, object parameter)
    {
        this.callback = callback;
        this.parameter = parameter;
    }
}