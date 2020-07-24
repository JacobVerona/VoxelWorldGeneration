using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Block data", menuName = "VoxelWorld/Datas/Block data")]
public class BlockData : ScriptableObject
{
    public string idName;
    public bool isSolid;
    public bool isTransparent;

    public int[] textureID = new int[6]{
        0,0,0,0,0,0
        };
}


public interface IBlock
{
   string[] TextureID ();
}
