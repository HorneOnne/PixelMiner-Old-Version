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
        public float Mass;
        public bool Simulate;
        public bool OnGround;

        public DynamicEntity(Transform transform, AABB bound)
        {
            this.Transform = transform;
            this.AABB = bound;
            Position = default;
            Velocity = default;
            Mass = 1;
            Simulate = true;
        }

        public void SetVelocity(Vector3 vel)
        {
            Velocity = vel; 
        }
        public void SetVelocityX(float velX)
        {
            Velocity.x = velX;
        }
        public void SetVelocityY(float velY)
        {
            Velocity.y = velY;
        }
        public void SetVelocityZ(float velZ)
        {
            Velocity.z = velZ;
        }

        public void AddVelocity(Vector3 vel)
        {
            Velocity += vel;
        }
        public void AddVelocityX(float velX)
        {
            Velocity.x += velX;
        }
        public void AddVelocityY(float velY)
        {
            Velocity.y += velY;
        }
        public void AddVelocityZ(float velZ)
        {
            Velocity.z += velZ;
        }

    }
}
