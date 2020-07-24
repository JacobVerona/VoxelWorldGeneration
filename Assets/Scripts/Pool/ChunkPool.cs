using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChunkPool
{
    public static int size = 200*World.Height*100;
    public static List<GameObject> chunksPool = new List<GameObject>(100*World.Height*100);

    public static GameObject chunkPool;

    public static GameObject SpawnGameObject ()
    {
        if (ChunkPool.chunkPool == null)
        {
            ChunkPool.chunkPool = GameObject.Find("ChunksPool");
            for (int i = 0; i < size; i++)
            {
                chunksPool.Add(new GameObject("Chunk: " + 0 + "/" + 0 + "/" + 0));
                chunksPool[i].AddComponent<MeshRenderer>();
                chunksPool[i].AddComponent<MeshFilter>();
                chunksPool[i].transform.SetParent(chunkPool.transform);
                chunksPool[i].SetActive(false);
            }
            Debug.Log("Created " + size + "objects");
        }
        if (chunksPool.Count == 0)
        {
            chunksPool.Add(new GameObject("Chunk: " + 0 + "/" + 0 + "/" + 0));
            chunksPool[chunksPool.Count - 1].AddComponent<MeshRenderer>();
            chunksPool[chunksPool.Count - 1].AddComponent<MeshFilter>();
            chunksPool[chunksPool.Count-1].transform.SetParent(chunkPool.transform);
            chunksPool[chunksPool.Count-1].SetActive(false);
        }
        GameObject obj = chunksPool[chunksPool.Count-1];
        chunksPool.RemoveAt(chunksPool.Count - 1);
        obj.SetActive(true);
        return obj;
    }

    public static void RemoveGameObject (GameObject obj)
    {
        chunksPool.Add(obj);
        obj.transform.SetParent(chunkPool.transform);
        obj.SetActive(false);
    }
}
