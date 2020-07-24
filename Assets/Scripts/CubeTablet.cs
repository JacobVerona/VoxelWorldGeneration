using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CubeTablet
{
	public static readonly int TextureAtlasSizeInBlocks = 4;
	public static float NormalizeBlockTextureSize
	{
		get { return (1f / (float)TextureAtlasSizeInBlocks); }
	}


	/// <summary>
	/// 3-2
	/// |/|
	/// 0-1
	/// 
	/// 0-3-2
	/// ->->->
	/// 0-2-1
	/// </summary>


	public static Vector3[] vertices = new Vector3[8]
	{
		new Vector3(1f, 1f, 1f),
		new Vector3(-1f, 1f, 1f),
		new Vector3(-1f, -1f, 1f),
		new Vector3(1f, -1f, 1f),
		new Vector3(-1f, 1f, -1f),
		new Vector3(1f, 1f, -1f),
		new Vector3(1f, -1f, -1f),
		new Vector3(-1f, -1f, -1f)
	};

	public static int[][] faceTriangles = new int[6][]
	{
		new int[4]
		{
			0,
			1,
			2,
			3
		},
		new int[4]
		{
			5,
			0,
			3,
			6
		},
		new int[4]
		{
			4,
			5,
			6,
			7
		},
		new int[4]
		{
			1,
			4,
			7,
			2
		},
		new int[4]
		{
			5,
			4,
			1,
			0
		},
		new int[4]
		{
			3,
			2,
			7,
			6
		}
	};

	public static Vector3[] FaceVertices (Direction direction, float scale, Vector3 facePos)
	{
		Vector3[] array = new Vector3[4];

		for (int i = 0; i < array.Length; i++)
		{
			array[i] = vertices[faceTriangles[(int)direction][i]] * scale + facePos;
		}

		return array;
	}
}
