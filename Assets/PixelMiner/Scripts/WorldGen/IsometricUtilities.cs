using PixelMiner.Utilities;
using UnityEngine;

namespace PixelMiner.WorldGen
{
    public static class IsometricUtilities
    {
        public static float CELLSIZE_X = 2;
        public static float CELLSIZE_Y = 1;
        //public static float CELLSIZE_Y = 1.1547f;

        public static Vector2 IsometricFrameToWorldFrame(int isoFrameX, int isoFrameY)
        {
            float tileWidth = CELLSIZE_X;  // Width of an isometric tile
            float tileHeight = CELLSIZE_Y; // Height of an isometric tile

            float worldX = (isoFrameX - isoFrameY) * tileWidth * 0.5f;
            float worldY = (isoFrameX + isoFrameY) * tileHeight * 0.5f;

            return new Vector2(worldX, worldY);
        }

        public static Vector2 ConvertIsometricFrameToWorldPosition(int isoFrameX, int isoFrameY, int chunkWidth, int chunkHeight)
        {
            float worldX = (isoFrameX - isoFrameY) * CELLSIZE_X * 0.5f * chunkWidth;
            float worldY = (isoFrameX + isoFrameY) * CELLSIZE_Y * 0.5f * chunkHeight;

            return new Vector2(worldX, worldY);
        }

        public static Vector2Int ReverseConvertWorldPositionToIsometricFrame(Vector3 worldPosition, int chunkWidth, int chunkHeight)
        {
            float worldX = worldPosition.x;
            float worldY = worldPosition.y;

            int chunkX = Mathf.FloorToInt((worldX / (CELLSIZE_X * 0.5f * chunkWidth) + worldY / (CELLSIZE_Y * 0.5f * chunkHeight)) / 2);
            int chunkY = Mathf.FloorToInt((worldY / (CELLSIZE_Y * 0.5f * chunkHeight) - worldX / (CELLSIZE_X * 0.5f * chunkWidth)) / 2);

            return new Vector2Int(chunkX, chunkY);
        }

        public static Vector2 WorldToTileFrame(float x, float y, float tileWidth, float tileHeight, byte chunkWidth, byte chunkHeight)
        {
            x %= chunkWidth;
            y %= chunkHeight;

            //Debug.Log($"A: {x}");
            x = Mathf.FloorToInt(x / 2.0f) * 2;
            //Debug.Log($"B:{x}");
            y = Mathf.FloorToInt(y);
           
            int i = (int)Mathf.Floor((x / tileWidth + y / tileHeight));
            int j = (int)Mathf.Floor((y / tileHeight - x / tileWidth));
            //Debug.Log($"x: {x} \t y: {y} \t i: {i} \t j: {j}");
            //Debug.Log($"{(x / tileWidth + y / tileHeight) / 2.0f}");
            return new Vector2Int(i, j);
        }

    }
}

