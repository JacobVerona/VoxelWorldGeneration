using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;

public struct MeshData
{
    public List<Vector3> vertices;
    public List<int> triangles;
    public List<Vector2> uvs;

    public MeshData (List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        this.vertices = vertices;
        this.triangles = triangles;
        this.uvs = uvs;
    }

    public MeshData (ChunkData chunk, int index)
    {
        this.vertices = new List<Vector3>();
        this.triangles = new List<int>();
        this.uvs = new List<Vector2>();
        //lock (this)
        {
            for (int x = 0; x < World.ChunkSize; x++)
            {
                for (int y = 0; y < World.ChunkSize; y++)
                {
                    for (int z = 0; z < World.ChunkSize; z++)
                    {
                        MakeCube(new Vector3Int(x, (World.ChunkSize * index) + y, z), ref chunk);
                    }
                }
            }
        }
    }

    private void MakeCube (Vector3Int positionIndex, ref ChunkData chunk)
    {
        if (chunk.blockDatas[positionIndex.x, positionIndex.y, positionIndex.z].idName == "voxel:air")
        {
            return;
        }

        for (int i = 0; i < 6; i++)
        {
            if (!chunk.IsSolidNeighbourBlock((Direction)i, positionIndex.x, positionIndex.y, positionIndex.z))
            {
                CreateFace((Direction)i, 0.5f, positionIndex, chunk.blockDatas[positionIndex.x, positionIndex.y, positionIndex.z].idName);
            }
        }
    }

    private void CreateFace (Direction direction, float faceScale, Vector3 facePos, string blockID)
    {
        //lock (this)
        {
            vertices.AddRange(CubeTablet.FaceVertices(direction, faceScale, facePos));
            int count = vertices.Count;
            triangles.Add(count - 4);
            triangles.Add(count - 4 + 1);
            triangles.Add(count - 4 + 2);
            triangles.Add(count - 4);
            triangles.Add(count - 4 + 2);
            triangles.Add(count - 4 + 3);
            BlockData block;
            BlockDatabase.blockDatas.TryGetValue(blockID, out block);

            AddTexture(block.textureID[(int)direction]);
        }
    }

    private void AddTexture (int textureID)
    {
        float y = textureID / CubeTablet.TextureAtlasSizeInBlocks;
        float x = textureID - (y * CubeTablet.TextureAtlasSizeInBlocks);

        x *= CubeTablet.NormalizeBlockTextureSize;
        y *= CubeTablet.NormalizeBlockTextureSize;

        y = 1f - y - CubeTablet.NormalizeBlockTextureSize;

        uvs.Add(new Vector2(x + CubeTablet.NormalizeBlockTextureSize, y + CubeTablet.NormalizeBlockTextureSize));
        uvs.Add(new Vector2(x, y + CubeTablet.NormalizeBlockTextureSize));
        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x + CubeTablet.NormalizeBlockTextureSize, y));
    }
}
