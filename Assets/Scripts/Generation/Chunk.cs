using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Chunk
{
    Mesh mesh;

    byte indexID;
    public GameObject chunk;

    MeshFilter meshFilter;
    public bool toUpdate;

    public Chunk (Vector3 position, Material material, byte ID)
    {
        indexID = ID;
        this.mesh = new Mesh();
        chunk = ChunkPool.SpawnGameObject();
        chunk.SetActive(false);
        chunk.name = string.Concat("Chunk: ", position.ToString());
        chunk.transform.position = new Vector3(position.x, 0, position.z);
        chunk.GetComponent<MeshRenderer>().material = material;
        meshFilter = chunk.GetComponent<MeshFilter>();
        toUpdate = true;
    }

    ~Chunk ()
    {
        ChunkPool.RemoveGameObject(chunk);
        GC.SuppressFinalize(this);
    }


    public void Draw ()
    {
        meshFilter.sharedMesh = mesh;
        toUpdate = false;
    }


    public void Request (ChunkData chunkData)
    {
        ThreadManager.RequestData(() => { return new MeshData(chunkData, indexID); }, RecreateMesh, ThreadType.Render);
    }

    public void RecreateMesh (object meshData_)
    {
        MeshData meshData = (MeshData)meshData_;
        mesh = new Mesh
        {
            vertices = meshData.vertices.ToArray(),
            triangles = meshData.triangles.ToArray(),
            uv = meshData.uvs.ToArray()
        };
        mesh.RecalculateNormals();

        toUpdate = true;
    }
 }
