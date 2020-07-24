using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;
using Unity.Collections;

public class MeshBuilder
{
    public static MeshData CreateMesh (ChunkData chunk, int index, MeshData mesh)
    {
        for (int x = 0; x < World.ChunkSize; x++)
        {
            for (int y = 0; y < World.ChunkSize; y++)
            {
                for (int z = 0; z < World.ChunkSize; z++)
                {
                    MakeCube(new Vector3Int(x, (World.ChunkSize * index) + y, z), ref chunk, ref mesh);
                }
            }
        }

        return mesh;
    }

    public static void MakeCube (Vector3Int positionIndex, ref ChunkData chunk, ref MeshData mesh)
    {
        if (chunk.blockDatas[positionIndex.x, positionIndex.y, positionIndex.z].idName == "voxel:air")
        {
            return;
        }

        for (int i = 0; i < 6; i++)
        {
            if (!chunk.IsSolidNeighbourBlock((Direction)i, positionIndex.x, positionIndex.y, positionIndex.z))
            {
                CreateFace((Direction)i, 0.5f, positionIndex, chunk.blockDatas[positionIndex.x, positionIndex.y, positionIndex.z].idName, ref mesh);
            }
        }
    }

    private static void CreateFace (Direction direction, float faceScale, Vector3 facePos, string blockID, ref MeshData mesh)
    {

        mesh.vertices.AddRange(CubeTablet.FaceVertices(direction, faceScale, facePos));
        int count = mesh.vertices.Count;
        mesh.triangles.Add(count - 4);
        mesh.triangles.Add(count - 4 + 1);
        mesh.triangles.Add(count - 4 + 2);
        mesh.triangles.Add(count - 4);
        mesh.triangles.Add(count - 4 + 2);
        mesh.triangles.Add(count - 4 + 3);
        BlockData block;
        BlockDatabase.blockDatas.TryGetValue(blockID, out block);

        AddTexture(block.textureID[(int)direction], ref mesh);

    }

    private static void AddTexture (int textureID, ref MeshData mesh)
    {
        float y = textureID / CubeTablet.TextureAtlasSizeInBlocks;
        float x = textureID - (y * CubeTablet.TextureAtlasSizeInBlocks);

        x *= CubeTablet.NormalizeBlockTextureSize;
        y *= CubeTablet.NormalizeBlockTextureSize;

        y = 1f - y - CubeTablet.NormalizeBlockTextureSize;

        mesh.uvs.Add(new Vector2(x + CubeTablet.NormalizeBlockTextureSize, y + CubeTablet.NormalizeBlockTextureSize));
        mesh.uvs.Add(new Vector2(x, y + CubeTablet.NormalizeBlockTextureSize));
        mesh.uvs.Add(new Vector2(x, y));
        mesh.uvs.Add(new Vector2(x + CubeTablet.NormalizeBlockTextureSize, y));
    }

    /*public MeshData CreateMesh (ChunkData chunk, int index)
    {
        lock (this)
        {
            vertices = new List<Vector3>();
            triangles = new List<int>();
            uvs = new List<Vector2>();
            for (int x = 0; x < World.ChunkSize; x++)
            {
                for (int y = 0; y < World.ChunkSize; y++)
                {
                    for (int z = 0; z < World.ChunkSize; z++)
                    {
                        MakeCube(new Vector3Int(x, (World.ChunkSize * index) + y, z), chunk);
                    }
                }
            }

            return new MeshData(vertices, triangles, uvs);
        }
    }

    public void MakeCube (Vector3Int positionIndex, ChunkData chunk)
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
    }*/
}
