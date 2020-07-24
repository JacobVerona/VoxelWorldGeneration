using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Collections.Concurrent;

public class JobThread : IDisposable
{
    private ConcurrentQueue<Action> jobs;

    private readonly Thread thread;
    private readonly AutoResetEvent job_event;

    private bool job_stop;
    private bool commit_wait;
    public int CheckJobCount
    {
        get { return jobs.Count; }
    }

    public JobThread ()
    {
        job_stop = false;
        jobs = new ConcurrentQueue<Action>();
        job_event = new AutoResetEvent(false);

        thread = new Thread(ThreadFunction)
        {
            IsBackground = true
        };
    }

    ~JobThread ()
    {
        Stop();
    }

    public void Start ()
    {
        thread.Start();
    }

    public void AddJob (Action action)
    {
        jobs.Enqueue(action);
    }

    public void Commit ()
    {
        if (jobs.Count <= 0 && !commit_wait)
        {
            return;
        }

        commit_wait = false;
        job_event.Set();
    }

    private void Stop ()
    {
        job_stop = true;
        job_event.Set();
    }

    private void ThreadFunction ()
    {
        while (!job_stop)
        {
            for (int i = 0; i < jobs.Count; i++)
            {
                jobs.TryDequeue(out Action job);
                job?.Invoke();
            }

            lock (this)
            {
                commit_wait = true;
            }
            job_event.WaitOne();
        }
    }

    public void Dispose ()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose (bool disposing)
    {
        Stop();

        if (disposing)
        {
            job_event.Close();
        }
    }
}

/*public class JobThread : IDisposable
{
    private ConcurrentQueue<ThreadWork> jobs;

    private readonly Thread thread;
    private readonly AutoResetEvent job_event;

    private bool job_stop;
    private bool commit_wait;
    public int CheckJobCount
    {
        get { return jobs.Count; }
    }
    
    public JobThread ()
    {
        job_stop = false;
        jobs = new ConcurrentQueue<ThreadWork>();
        job_event = new AutoResetEvent(false);

        thread = new Thread(ThreadFunction)
        {
            IsBackground = true
        };
    }

    ~JobThread ()
    {
        Stop();
    }

    public void Start()
    {
        thread.Start();
    }

    public void AddJob (Action action, byte priority)
    {
        jobs.Enqueue(new ThreadWork(action, priority));
    }

    public void Commit ()
    {
        if (jobs.Count <= 0 && !commit_wait)
        {
            return;
        }
        commit_wait = false;
        job_event.Set();
    }

    private void Stop ()
    {
        job_stop = true;
        job_event.Set();
    }

    private void ThreadFunction ()
    {
        while (!job_stop)
        {


            for (int i = 0; i < jobs.Count; i++)
            {
                jobs.TryDequeue(out ThreadWork job);
                job.Invoke();
            }
            

            //lock (this)
            {
                commit_wait = true;
            }

            job_event.WaitOne();
        }
    }

    public void Dispose ()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    private void Dispose (bool disposing)
    {
        Stop();

        if (disposing)
        {
            job_event.Close();
        }
    }
}*/
