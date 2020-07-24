using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ApplicationData : MonoBehaviour
{
    public BlockDatabase blockDatas;

    public void Awake ()
    {
        blockDatas.Init();
    }
}
