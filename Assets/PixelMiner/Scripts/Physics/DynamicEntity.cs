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

        public DynamicEntity(Transform transform, AABB bound)
        {
            this.Transform = transform;
            this.AABB = bound;
            Position = default;
            Velocity = default;
            Simulate = true;
        }

        public void SetVelocity(Vector3 vel)
        {
            Velocity = vel; 
        }

        public void AddVelocity(Vector3 vel)
        {
            Velocity += vel;
        }
    }
}
