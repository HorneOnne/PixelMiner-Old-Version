using UnityEngine;
using PixelMiner.DataStructure;

namespace PixelMiner.Physics
{
    public class DynamicEntity
    {
        public Transform Transform;
        public Vector3 Position;
        public AABB AABB;
        public Vector3 Velocity;
        public bool Simulate;

        public void SetVelocity(Vector3 vel)
        {
            AABB.vx = vel.x;
            AABB.vy = vel.y;
            AABB.vz = vel.z;
        }
    }
}
