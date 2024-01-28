using UnityEngine;

namespace PixelMiner.Physics
{
    public class DynamicEntity
    {
        public Transform Transform;
        public AABB AABB;
        public Vector3 Velocity;
        public bool Simulate;
    }
}
