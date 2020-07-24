using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DebugScreen : MonoBehaviour
{
    Text text;

    float frameRate;
    float timer;

    

    private void Start ()
    {
        text = GetComponent<Text>();
    }

    private void Update ()
    {
        string debugText = "Debug screen: \n";
        debugText += frameRate + " fps\n\n";
        debugText += World.GetChunkCoordFromV3(World.world.viewer.position) + "\n\n";
        debugText += ChunkPool.chunksPool.Count + "\n";
        debugText += World.chunkcount;

        text.text = debugText;

        if (timer > 1f)
        {
            frameRate = (int)(1f / Time.unscaledDeltaTime);
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

}
