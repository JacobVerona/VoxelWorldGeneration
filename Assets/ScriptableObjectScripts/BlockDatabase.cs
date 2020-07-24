using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;


    [CreateAssetMenu(fileName = "Block database", menuName = "VoxelWorld/Databases")]
    public class BlockDatabase : ScriptableObject
    {
        public static Dictionary<string, BlockData> blockDatas = new Dictionary<string, BlockData>();

        public void Init ()
        {
            BlockData[] blocksLoad = Resources.LoadAll<BlockData>("BlockDatas");

            for (int i = 0; i < blocksLoad.Length; i++)
            {
                if (!blockDatas.ContainsKey(blocksLoad[i].idName))
                {
                    blockDatas.Add(blocksLoad[i].idName, blocksLoad[i]);
                }
            }
        }
    }
