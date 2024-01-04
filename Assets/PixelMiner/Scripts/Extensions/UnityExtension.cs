using UnityEngine;

namespace PixelMiner
{
    public static class UnityExtension
    {
        public static Vector3Int ToVector3Int(this Vector3 vector3)
        {
            return new Vector3Int(
                Mathf.FloorToInt(vector3.x),
                Mathf.FloorToInt(vector3.y),
                Mathf.FloorToInt(vector3.z)
            );
        }
    }
}
