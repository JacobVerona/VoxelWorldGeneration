using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Collections.Concurrent;

public class World : MonoBehaviour
{
    public static readonly int ChunkSize = 16;
    public static readonly int Height = 16;
    public static readonly int ViewDistance = 16;
    public static int chunkcount = 0;
    public static int ViewDistanceInBlocks
    {
        get { return ViewDistance * ChunkSize; }
    }

    public static int HeightInBlocks
    {
        get { return ChunkSize * Height; }
    }

    public bool UseGizmos;

    public static Dictionary<Vector2Int, ChunkColumn> chunkcolumns = new Dictionary<Vector2Int, ChunkColumn>();
    public Material material;

    public Transform viewer;
    public static World world;

    public static List<Vector2Int> LoadedChunks = new List<Vector2Int>(100);
    public Vector2Int lastcoord;

    public BlockData[] blockData;

    private void Awake ()
    {
        world = FindObjectOfType<World>();
        lastcoord = GetChunkCoordFromV3(viewer.position);
    }

    public void OnDrawGizmos ()
    {
        if (UseGizmos)
        {
            foreach (var item in chunkcolumns)
            {
                item.Value.GizDraw();
            }
        }
    }
    bool igen = true;
    public IEnumerator UpdateVisibleChunk ()
    {
        Vector2Int coord = GetChunkCoordFromV3(viewer.position);

        if (lastcoord != coord)
        {
            for (int x = coord.x - ViewDistance; x <= ViewDistance + coord.x; x++)
            {
                for (int z = coord.y - ViewDistance; z <= ViewDistance + coord.y; z++)
                {
                    Vector2Int pos = new Vector2Int(x,z);
                    if (!chunkcolumns.ContainsKey(pos))
                    {
                        GenerateChunk(pos);
                        yield return null;
                    }
                    else
                    {
                        chunkcolumns[pos].CheckForViewDistance(coord);
                    }
                }
            }


            if (LoadedChunks.Count > 0)
            {
                for (int i = 0; i < LoadedChunks.Count; i++)
                {
                    if (chunkcolumns.ContainsKey(LoadedChunks[i]))
                    {
                        chunkcolumns[LoadedChunks[i]].CheckForViewDistance(coord);
                    }
                }
            }
        }
        yield return null;

        lastcoord = coord;
        igen = true;
    }

    public void UpdateVisibleChunk_ ()
    {
        Vector2Int coord = GetChunkCoordFromV3(viewer.position);

        if (lastcoord != coord)
        {
            for (int x = coord.x - ViewDistance; x <= ViewDistance + coord.x; x++)
            {
                for (int z = coord.y - ViewDistance; z <= ViewDistance + coord.y; z++)
                {
                    Vector2Int pos = new Vector2Int(x,z);
                    if (!chunkcolumns.ContainsKey(pos))
                    {
                        GenerateChunk(pos);
                    }
                    else
                    {
                        chunkcolumns[pos].CheckForViewDistance(coord);
                    }
                }
            }


            if (LoadedChunks.Count > 0)
            {
                for (int i = 0; i < LoadedChunks.Count; i++)
                {
                    if (chunkcolumns.ContainsKey(LoadedChunks[i]))
                    {
                        chunkcolumns[LoadedChunks[i]].CheckForViewDistance(coord);
                    }
                }
            }
        }

        lastcoord = coord;
        igen = true;
    }


    public void GenerateChunk (Vector2Int chunkPosition)
    {
        chunkcolumns.Add(chunkPosition, new ChunkColumn(chunkPosition, world.material, world));
        chunkcount++;
    }

    public void Update ()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            UpdateMeshAllChunks();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            UpdateDrawAllChunks();
        }

        if(igen)
        {
            //StartCoroutine(UpdateVisibleChunk());
            igen = false;
        }
        UpdateVisibleChunk_();

        UpdateDrawAllChunks();
    }


    public bool CheckForVoxel (float x, float y, float z)
    {
        return CheckForVoxel(new Vector3(x, y, z));
    }

    public ChunkColumn GetChunkFromV3 (Vector3 pos)
    {
        Vector2Int coord = GetChunkCoordFromV3(pos);
        return chunkcolumns[coord];
    }

    public static Vector3Int GlobalCoordToLocalCoord (Vector3 pos)
    {
        Vector2Int thisChunk = GetChunkCoordFromV3(pos);

        return new Vector3Int(Mathf.FloorToInt(pos.x) - (thisChunk.x * World.ChunkSize), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z) - (thisChunk.y * World.ChunkSize));
    }

    public bool CheckForVoxel (Vector3 position)
    {
        Vector2Int thisChunk = GetChunkCoordFromV3(position);

        if (chunkcolumns.ContainsKey(thisChunk))
        {
            if (chunkcolumns[thisChunk].isGenerated)
            {
                return chunkcolumns[thisChunk].chunkData.isSolid(Mathf.FloorToInt(position.x) - (thisChunk.x * World.ChunkSize), Mathf.FloorToInt(position.y), Mathf.FloorToInt(position.z) - (thisChunk.y * World.ChunkSize));
            }
        }
        return false;
    }

    public void UpdateMeshAllChunks ()
    {
        foreach (var item in chunkcolumns)
        {
            item.Value.UpdateMesh();
        }
    }

    public void UpdateDrawAllChunks ()
    {
        foreach (var item in chunkcolumns)
        {
            item.Value.Draw();
        }
    }

    public static bool DataIsGenerated (Vector2Int pos)
    {
        if (chunkcolumns.ContainsKey(pos))
        {
            return chunkcolumns[pos].isGenerated;
        }
        return false;
    }

    public static Vector2Int GetChunkCoordFromV3 (Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / World.ChunkSize);
        int z = Mathf.FloorToInt(position.z / World.ChunkSize);
        return new Vector2Int(x, z);
    }

    public static BlockData GetBlockAtGlobalV3 (Vector3Int BlockGlobalPos)
    {
        Vector2Int pos = GetChunkCoordFromV3(BlockGlobalPos);
      
        if (DataIsGenerated(pos))
        {
            return chunkcolumns[pos].chunkData.GetBlockFromLocalV3Int(BlockGlobalPos - (new Vector3Int(pos.x, 0,pos.y) * ChunkSize));
        }
        else
        {
            return GenerateBlock(BlockGlobalPos);
        }
    }

    /// <summary>
    /// Create block at position 22.68
    /// </summary>
    public static BlockData GenerateBlock (Vector3Int pos)
    {
        if (pos.y == 0)
        {
            return BlockDatabase.blockDatas["voxel:void"];
        }
        if (Noise.Perlin3D(pos.x, pos.y, pos.z, 0.05f, 3, 0.1f) < 0.4f)
        {
            return BlockDatabase.blockDatas["voxel:air"];
        }
        if (pos.y <= Noise.GenerateTerrainLayer(pos.x, pos.z, 4, 0.5f, 0.01f, 145))
        {
            if (Noise.Perlin3D(pos.x + 2000, pos.y, pos.z + 2000, 0.05f, 3, 0.1f) < 0.3f)
            {
                return BlockDatabase.blockDatas["voxel:coal"];
            }
            return BlockDatabase.blockDatas["voxel:stone"];
        }
        if (pos.y <= Noise.GenerateTerrainLayer(pos.x, pos.z, 4, 0.2f, 0.01f, 152))
        {
            if (pos.y <= Noise.GenerateTerrainLayer(pos.x, pos.z, 4, 0.2f, 0.01f, 150))
            {
                return BlockDatabase.blockDatas["voxel:dirt"];
            }
            return BlockDatabase.blockDatas["voxel:grass"];
        }
        
        return BlockDatabase.blockDatas["voxel:air"];
    }
}