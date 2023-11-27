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

        

       /// <summary>
       /// Global position to tile frame.
       /// </summary>
       /// <param name="globalX"></param>
       /// <param name="globalY"></param>
       /// <param name="offsetX"></param>
       /// <param name="offsetY"></param>
       /// <param name="tileWidth"></param>
       /// <param name="tileHeight"></param>
       /// <returns></returns>
        public static Vector2 GlobalToLocal(float globalX, float globalY, float offsetX, float offsetY, float tileWidth = 2.0f, float tileHeight = 1.0f)
        {
            float localX = (globalY - offsetY) / tileHeight + (globalX - offsetX) / tileWidth;
            float localY = (globalY - offsetY) / tileHeight - (globalX - offsetX) / tileWidth;

            return new Vector2(localX, localY);
        }


        /// <summary>
        /// Tile frame to global position.
        /// </summary>
        /// <param name="localX"></param>
        /// <param name="localY"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="tileWidth"></param>
        /// <param name="tileHeight"></param>
        /// <returns></returns>
        public static Vector2 LocalToGlobal(float localX, float localY, float offsetX, float offsetY, float tileWidth = 2.0f, float tileHeight = 1.0f)
        {
            float globalX = offsetX + (localX - localY) * tileWidth / 2.0f;
            float globalY = offsetY + (localX + localY) * tileHeight / 2.0f;

            return new Vector2(globalX, globalY);
        }

        /// <summary>
        /// Applies an isometric transformation to a <see cref="Vector3"/> using the provided Euler angles.
        /// </summary>
        /// <param name="input">The input <see cref="Vector3"/> to transform isometrically.</param>
        /// <param name="euler">The Euler angles specifying the rotation for the isometric transformation.</param>
        /// <returns>A new <see cref="Vector3"/> representing the result of the isometric transformation.</returns>
        public static Vector3 Iso(this Vector3 input, Vector3 euler) => Matrix4x4.Rotate(Quaternion.Euler(euler)).MultiplyPoint3x4(input); 
    }
}

