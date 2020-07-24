using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    static FastNoise fastNoise = new FastNoise(0);
    public static float GenerateTerrainLayer (float x, float z, int octaves, float persistence, float smooth, int heightOfBlocks, int minHeightOfBlocks = 0)
    {
        float height = Noise.Normalize(minHeightOfBlocks, heightOfBlocks, BrownianMotion(x*smooth,z*smooth, octaves, persistence));
        return (int)height;
    }

    public static float Normalize (float min, float max, float value)
    {
        return Mathf.Lerp(min, max, Mathf.InverseLerp(0, 1, value));
    }

    public static float BrownianMotion (float x, float z, int octaves, float persistence)
    {
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;
        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise((x + 32000f) * frequency, (z + 32000f) * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= 2;
        }
        return total / maxValue;
    }

    public static float Perlin3D (float x, float y, float z, float smooth, int octaves, float persistence)
    {
        float XY = Noise.BrownianMotion(x*smooth, y*smooth,octaves,persistence);
        float YZ = Noise.BrownianMotion(y*smooth, z*smooth,octaves,persistence);
        float XZ = Noise.BrownianMotion(x*smooth, z*smooth,octaves,persistence);

        float YX = Noise.BrownianMotion(y*smooth, x*smooth,octaves,persistence);
        float ZY = Noise.BrownianMotion(z*smooth, y*smooth,octaves,persistence);
        float ZX = Noise.BrownianMotion(z*smooth, x*smooth,octaves,persistence);
        return (XY + YZ + XZ + YX + ZY + ZX) / 6f;
    }
}
