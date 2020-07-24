using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class ChunkColumn 
{
    Chunk[] chunkObjects;

    public ChunkData chunkData;

    public Material material;

    public bool isGenerated;
    private bool toUpdateMesh;
    private bool isload = false;

    public bool IsLoad
    {
        get { return isload; }
        set 
        {
            if (value)
            {
                Load();
                World.LoadedChunks.Add(posit);
            }
            else
            {
                Delete();
                World.LoadedChunks.Remove(posit);
            }
            
            
            isload = value;
        }
    }

    public Vector2Int posit;
    public ChunkColumn (Vector2Int chunkCoord, Material material, World world)
    {
        posit = chunkCoord;
        this.material = material;
        chunkObjects = new Chunk[World.Height];
        for (int i = 0; i < World.Height; i++)
        {
            chunkObjects[i] = new Chunk(new Vector3(chunkCoord.x * World.ChunkSize, i * World.ChunkSize, chunkCoord.y * World.ChunkSize), material, (byte)i);
        }
        GenerateChunk(chunkCoord);
    }

    public bool CheckForViewDistance (Vector2Int startPos)
    {
        float dist = Vector3.Distance(Utils.V2IntToV2(ref posit), Utils.V2IntToV2(ref startPos));

        if (World.ViewDistance >= dist)
        {
            if (!isload)
            {
                IsLoad = true;
                return true;
            }
        }
        else
        {
            if (isload)
            {
                IsLoad = false;
            }
        }
        return false;
    }

    public void GenerateChunk (Vector2Int chunkCoord)
    {
        ThreadManager.RequestData(() => { return new ChunkData(chunkCoord); }, SetData, ThreadType.Generation);
    }

    public void SetData (object data)
    {
        chunkData = (ChunkData)data;
        isGenerated = true;
        toUpdateMesh = true;
        UpdateMesh();
    }

    public void UpdateMesh ()
    {
        if (isGenerated && toUpdateMesh)
        {
            for (int i = 0; i < chunkObjects.Length; i++)
            {
                chunkObjects[i].Request(chunkData);
            }
            toUpdateMesh = false;
        }
    }

    public void UpdateMeshChunk (int index)
    {
        chunkObjects[index].Request(chunkData);
    }

    public void Draw ()
    {
        if (isGenerated)
        {
            for (int i = 0; i < chunkObjects.Length; i++)
            {
                if (chunkObjects[i].toUpdate)
                {
                    chunkObjects[i].Draw();
                }
            }
        }
    }
    public void GizDraw ()
    {
        if (isGenerated)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(new Vector3(posit.x * World.ChunkSize, 0, posit.y * World.ChunkSize), Vector3.one * World.ChunkSize);
        }
    }

    public void Delete ()
    {
        for (int i = 0; i < chunkObjects.Length; i++)
        {
            chunkObjects[i].chunk.SetActive(false);
        }
    }

    public void Load ()
    {
        for (int i = 0; i < chunkObjects.Length; i++)
        {
            chunkObjects[i].chunk.SetActive(true);
        }
    }

    public void SetBlock (Vector3Int position, BlockData block)
    {
        SetBlock(position.x, position.y, position.z, block);
    }

    public void SetBlock (int x, int y, int z, BlockData block)
    {
        chunkData.blockDatas[x, y, z] = block;
        int index = y/World.ChunkSize;
        UpdateMeshChunk(index);
        Vector2Int chunkPos = posit;
        if (x >= World.ChunkSize - 1)
        {
            chunkPos.x += 1;
            World.chunkcolumns[chunkPos].UpdateMeshChunk(index);
        }
        if (x <= 0)
        {
            chunkPos.x -= 1;
            World.chunkcolumns[chunkPos].UpdateMeshChunk(index);
        }
        if (z >= World.ChunkSize - 1)
        {
            chunkPos.y += 1;
            World.chunkcolumns[chunkPos].UpdateMeshChunk(index);
        }
        if (z <= 0)
        {
            chunkPos.y -= 1;
            World.chunkcolumns[chunkPos].UpdateMeshChunk(index);
        }

        int newY = y - (index * World.ChunkSize);
        if (newY >= World.ChunkSize - 1)
        {
            index++;
            UpdateMeshChunk(index);
        }
        if (newY <= 0)
        {
            index--;
            UpdateMeshChunk(index);
        }
    }

}

[System.Serializable]
public struct ChunkData
{
    public BlockData[,,] blockDatas;
    public Vector2Int position;


    public ChunkData (Vector2Int position, BlockData[,,] blockDatas)
    {
        this.position = position;
        this.blockDatas = blockDatas;
    }

    public ChunkData (Vector2Int position)
    {
        this.position = position * World.ChunkSize;
        blockDatas = new BlockData[World.ChunkSize, World.HeightInBlocks, World.ChunkSize];
        for (int x = 0; x < blockDatas.GetLength(0); x++)
        {
            for (int y = 0; y < blockDatas.GetLength(1); y++)
            {
                for (int z = 0; z < blockDatas.GetLength(2); z++)
                {
                    blockDatas[x, y, z] = World.GenerateBlock(new Vector3Int((position.x * World.ChunkSize) + x, y, (position.y * World.ChunkSize) + z));
                }
            }
        }
       
    }

    public bool isSolid (int x, int y, int z)
    {
        return blockDatas[x, y, z].isSolid;
    }

    public BlockData GetBlockFromLocalV3Int (Vector3Int pos)
    {
        return blockDatas[pos.x,pos.y, pos.z];
    }

    public bool IsSolidNeighbourBlock (Direction direction, int x, int y, int z)
    {
        switch (direction)
        {
            case Direction.North:
                if (x > World.ChunkSize - 1 && x >= 0 && z > World.ChunkSize - 1 && z >= 0)
                {
                    return blockDatas[x, y, z].isSolid;
                }
                return World.GetBlockAtGlobalV3(new Vector3Int(x, y, z + 1) + new Vector3Int(position.x,0,position.y)).isSolid;

            case Direction.East:
                if (x > World.ChunkSize - 1 && x >= 0 && z > World.ChunkSize - 1 && z >= 0)
                {
                    return blockDatas[x, y, z].isSolid;
                }
                return World.GetBlockAtGlobalV3(new Vector3Int(x + 1, y, z) + new Vector3Int(position.x, 0, position.y)).isSolid;

            case Direction.South:
                if (x > World.ChunkSize - 1 && x >= 0 && z > World.ChunkSize - 1 && z >= 0)
                {
                    return blockDatas[x, y, z].isSolid;
                }
                return World.GetBlockAtGlobalV3(new Vector3Int(x, y, z - 1) + new Vector3Int(position.x, 0, position.y)).isSolid;

            case Direction.West:
                if (x > World.ChunkSize - 1 && x >= 0 && z > World.ChunkSize - 1 && z >= 0)
                {
                    return blockDatas[x, y, z].isSolid;
                }
                return World.GetBlockAtGlobalV3(new Vector3Int(x - 1, y, z) + new Vector3Int(position.x, 0, position.y)).isSolid;

            case Direction.Up:
                if (y + 1 >= World.HeightInBlocks)
                {
                    return false;
                }
                return blockDatas[x, y + 1, z].isSolid;
            case Direction.Down:
                if (y - 1 < 0)
                {
                    return false;
                }
                return blockDatas[x, y - 1, z].isSolid;

            default:
                break;
        }
        return false;
    }
}

