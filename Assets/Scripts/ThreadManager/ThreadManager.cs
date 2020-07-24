using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Collections.Concurrent;
public class ThreadManager : MonoBehaviour
{
    private static ThreadManager threadManager;

    MultiThreadPool[] multiThreadPools;

    public int GetMaxThreadCount
    {
        get { return Mathf.Max(Environment.ProcessorCount - 1, 1); }
    }

    private void Awake ()
    {
        threadManager = FindObjectOfType<ThreadManager>();
        multiThreadPools = new MultiThreadPool[2];

        //Generation pool
        multiThreadPools[0] = new MultiThreadPool(GetMaxThreadCount);

        //Render pool
        multiThreadPools[1] = new MultiThreadPool(4);
        
        StartThreads ();
    }

    public static void RequestData (Func<object> generateData, Action<object> callback, ThreadType threadType)
    {
        switch (threadType)
        {
            case ThreadType.Generation:
                threadManager.multiThreadPools[0].RequestData(generateData, callback, 0);
                break;
            case ThreadType.Render:
                threadManager.multiThreadPools[1].RequestData(generateData, callback, 0);
                break;

            default:
                threadManager.multiThreadPools[0].RequestData(generateData, callback, 0);
                break;
        }
        
    }
    private void StartThreads ()
    {
        for (int i = 0; i < multiThreadPools.Length; i++)
        {
            multiThreadPools[i].Start();
        }
    }

    private void Update ()
    {
        for (int i = 0; i < multiThreadPools.Length; i++)
        {
            multiThreadPools[i].Update();
        }
    }
}

public enum ThreadType : byte
{
    Generation,
    Render
}
