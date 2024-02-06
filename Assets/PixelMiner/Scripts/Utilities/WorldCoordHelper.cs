using UnityEngine;

namespace PixelMiner.Utilities
{
    public static class WorldCoordHelper
    {
        private static Transform _mainCamTrans;
        static WorldCoordHelper()
        {
            _mainCamTrans = Camera.main.transform;
        }

        public static Vector3Int GlobalToRelativeBlockPosition(Vector3 globalPosition, 
            int chunkWidth = 32, int chunkHeight = 10, int chunkDepth = 32)
        {
            // Calculate the relative position within the chunk
            int relativeX = Mathf.FloorToInt(globalPosition.x) % chunkWidth;
            int relativeY = Mathf.FloorToInt(globalPosition.y) % chunkHeight;
            int relativeZ = Mathf.FloorToInt(globalPosition.z) % chunkDepth;

            // Ensure that the result is within the chunk's dimensions
            if (relativeX < 0) relativeX += chunkWidth;
            if (relativeY < 0) relativeY += chunkHeight;
            if (relativeZ < 0) relativeZ += chunkDepth;

            return new Vector3Int(relativeX, relativeY, relativeZ);
        }

        public static Vector3 ToGameDirection(this Vector3 worldDirection)
        {
            return worldDirection.Iso(new Vector3(0, _mainCamTrans.eulerAngles.y, 0));
        }
    }
}