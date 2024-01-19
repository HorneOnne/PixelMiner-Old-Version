using UnityEngine;

public static class NoiseExtension
{
    public static float ScaleNoise(float noiseValue, float oldMin, float oldMax, float newMin, float newMax)
    {
        return (noiseValue - oldMin) * (newMax - newMin) / (oldMax - oldMin) + newMin;
    }

    public static float DomainWarping(float x, float y, Vector2 offset, FastNoiseLite warpNoiseType, FastNoiseLite mainNoiseType)
    {
        Vector2 p = new Vector2(x, y);

        Vector2 q = new Vector2((float)warpNoiseType.GetNoise(p.x, p.y),
                                (float)warpNoiseType.GetNoise(p.x + offset.x, p.y + offset.y));


        //Vector2 l2p1 = (p + 40 * q) + new Vector2(77, 35);
        //Vector2 l2p2 = (p + 40 * q) + new Vector2(83, 28);

        //Vector2 r = new Vector3((float)simplex.GetNoise(l2p1.x, l2p1.y),
        //                        (float)simplex.GetNoise(l2p2.x, l2p2.y));


        //Vector2 l3 = p + 120 * r;
        Vector2 l3 = p + 40 * q;
        return mainNoiseType.GetNoise(l3.x, l3.y);
    }


}
