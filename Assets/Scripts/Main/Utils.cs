using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector2Int V3ToV2Int (ref Vector3 vector)
    {
        return new Vector2Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.z));
    }

    public static Vector3Int V3ToV3Int (ref Vector3 vector)
    {
        return new Vector3Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y), Mathf.FloorToInt(vector.z));
    }

    public static Vector2 V2IntToV2 (ref Vector2Int vector)
    {
        return new Vector2(vector.x, vector.y);
    }

    public static Vector2Int V3IntToV2Int (ref Vector3Int vector)
    {
        return new Vector2Int(vector.x, vector.z);
    }

    public static Vector3Int V2IntToV3Int (ref Vector2Int vector)
    {
        return new Vector3Int(vector.x, 0, vector.y);
    }

    public static Vector2Int FloatXYZtoV2Int (ref float x, ref float y, ref float z)
    {
        return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(z));
    }

    public static Vector3Int FloatXYZtoV3Int (ref float x, ref float y, ref float z)
    {
        return new Vector3Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y), Mathf.FloorToInt(z));
    }

    
}
