using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelMiner.Physics
{
    public struct AABB 
    {
        public float MinX;
        public float MaxX;
        public float MinY;
        public float MaxY;
        public float MinZ;
        public float MaxZ;


        public static AABB GetSwept(AABB b, Vector3 vel)
        {
            AABB swept;

            swept.MinX = vel.x > 0 ? b.MinX : b.MinX + vel.x;
            swept.MinY = vel.y > 0 ? b.MinY : b.MinY + vel.y;
            swept.MinZ = vel.z > 0 ? b.MinZ : b.MinZ + vel.z;

            swept.MaxX = vel.x > 0 ? b.MaxX + vel.x : b.MaxX;
            swept.MaxY = vel.y > 0 ? b.MaxY + vel.y : b.MaxY;
            swept.MaxZ = vel.z > 0 ? b.MaxZ + vel.z : b.MaxZ;

            return swept;
        }
        public override string ToString()
        {
            return $"{MinX}, {MinY}, {MinZ}, {MaxX}, {MaxY}, {MaxZ}";
        }

        // Base on this source: https://github.com/minetest/minetest/blob/master/src/collision.cpp
        // returns -1 if no collision, 0 if against x axis, 1 on y, 2 on z
        // also returns time of collision based on amount of velocity
        public const float d = 0.01f;  // some sort of threshold

        public static int AxisAlignedCollision(AABB dynamicBox, AABB staticBox, Vector3 vel, out float time)
        {
            const float COL_ZERO = 0;

            float sizeX = staticBox.MaxX - staticBox.MinX;
            float sizeY = staticBox.MaxY - staticBox.MinY;
            float sizeZ = staticBox.MaxZ - staticBox.MinZ;

            AABB relBox;
            relBox.MinX = dynamicBox.MinX - staticBox.MinX;
            relBox.MinY = dynamicBox.MinY - staticBox.MinY;
            relBox.MinZ = dynamicBox.MinZ - staticBox.MinZ;
            relBox.MaxX = dynamicBox.MaxX - staticBox.MinX;
            relBox.MaxY = dynamicBox.MaxY - staticBox.MinY;
            relBox.MaxZ = dynamicBox.MaxZ - staticBox.MinZ;

            time = 0f;

            // Check y first due to gravity
            if(vel.y > 0)
            {
                if(relBox.MaxY <= d)
                {
                    time = -relBox.MaxY / vel.y;
                    if((relBox.MinX + vel.x * time < sizeX) &&
                       (relBox.MaxX + vel.x * time > COL_ZERO) &&
                       (relBox.MinZ + vel.z * time < sizeZ) &&
                       (relBox.MaxZ + vel.z * time > COL_ZERO))
                    {
                        return 1;
                    }                      
                }
                else if(relBox.MinY > sizeY)
                {
                    return -1;
                }
            }
            else if(vel.y < 0)
            {

            }


            return 0;
        }
    }


}
