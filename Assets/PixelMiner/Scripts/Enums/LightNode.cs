using UnityEngine;

namespace PixelMiner.Enums
{
    public struct LightNode
    {
        public Vector3Int GlobalPosition;
        public byte Intensity;

        public LightNode(Vector3Int globalPosition, byte intensity)
        {
            this.GlobalPosition = globalPosition;
            this.Intensity = intensity;
        }
    }
}
