using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelMiner.Physics
{
    public struct AABB
    {
        public float x;
        public float y;
        public float z;
        public float w;
        public float h;
        public float d;
        public float vx;
        public float vy;
        public float vz;
    }

    public static class ABBBExtensions
    {
        public static int SweptAABB(AABB dynamicBox, AABB staticBox, out float normalX, out float normalY, out float normalZ, out float entryTime)
        {
            float xEntry, yEntry, zEntry; // Specify how far away the closest edges of the objects are from each other.
            float xExit, yExit, zExit;   // The distance to the far side of the object

            // Find the distance between the objects on the near and far sides for both x and y
            if (dynamicBox.vx > 0.0f)
            {
                xEntry = staticBox.x - (dynamicBox.x + dynamicBox.w);
                xExit = (staticBox.x + staticBox.w) - dynamicBox.x;
            }
            else
            {
                xEntry = dynamicBox.x - (staticBox.x + staticBox.w);
                xExit = (dynamicBox.x + dynamicBox.w) - staticBox.x;
            }

            if (dynamicBox.vy > 0.0f)
            {
                yEntry = staticBox.y - (dynamicBox.y + dynamicBox.h);
                yExit = (staticBox.y + staticBox.h) - dynamicBox.y;
            }
            else
            {
                yEntry = dynamicBox.y - (staticBox.y + staticBox.h);
                yExit = (dynamicBox.y + dynamicBox.h) - staticBox.y;
            }

            if (dynamicBox.vz > 0.0f)
            {
                zEntry = staticBox.z - (dynamicBox.z + dynamicBox.d);
                zExit = (staticBox.z + staticBox.d) - dynamicBox.z;
            }
            else
            {
                zEntry = dynamicBox.z - (staticBox.z + staticBox.d);
                zExit = (dynamicBox.z + dynamicBox.d) - staticBox.z;
            }


            // Find time of the collision and time leaving for each axis
            float xTimeEntry, yTimeEntry, zTimeEntry;
            float xTimeExit, yTimeExit, zTimeExit;

            if (dynamicBox.vx == 0.0f)
            {
                xTimeEntry = float.NegativeInfinity;
                xTimeExit = float.PositiveInfinity;
            }
            else
            {
                xTimeEntry = xEntry / Mathf.Abs(dynamicBox.vx);
                xTimeExit = xExit / Mathf.Abs(dynamicBox.vx);
            }

            if (dynamicBox.vy == 0.0f)
            {
                yTimeEntry = float.NegativeInfinity;
                yTimeExit = float.PositiveInfinity;
            }
            else
            {
                yTimeEntry = yEntry / Mathf.Abs(dynamicBox.vy);
                yTimeExit = yExit / Mathf.Abs(dynamicBox.vy);
            }



            if (dynamicBox.vz == 0.0f)
            {
                zTimeEntry = float.NegativeInfinity;
                zTimeExit = float.PositiveInfinity;
            }
            else
            {
                zTimeEntry = zEntry / Mathf.Abs(dynamicBox.vz);
                zTimeExit = zExit / Mathf.Abs(dynamicBox.vz);
            }

            // Find the earliest/lastest times of collision
            //float entryTime = Mathf.Max(xEntry, yEntry);
            entryTime = Mathf.Max(xTimeEntry, yTimeEntry);       // entryTime tell when the collision occured
            float exitTime = Mathf.Min(xTimeExit, yTimeExit);         // exitTime tell when collision exited the object from other side.


            // If there was no collision
            if (entryTime > exitTime || xTimeEntry < 0.0f && yTimeEntry < 0.0f || xTimeEntry > 1.0f && yTimeEntry > 1.0f)
            {
                Debug.Log($"No collision");
                normalX = 0.0f;
                normalY = 0.0f;
                normalZ = 0.0f;
                return -1;
            }
            else      // if there was a collision
            {
                Debug.Log("Collision");
                // Calculate normal of collided surface
                if (xTimeEntry > yTimeEntry)
                {
                    if(xTimeEntry > zTimeEntry)
                    {
                        if (xEntry < 0.0f)
                        {
                            normalX = 1.0f;
                            normalY = 0.0f;
                            normalZ = 0.0f;
                        }
                        else
                        {
                            normalX = -1.0f;
                            normalY = 0.0f;
                            normalZ = 0.0f;
                        }
                    }
                    else
                    {
                        if(zEntry < 0.0f)
                        {
                            normalX = 0.0f;
                            normalY = 0.0f;
                            normalZ = 1.0f;
                        }
                        else
                        {
                            normalX = 0.0f;
                            normalY = 0.0f;
                            normalZ = -1.0f;
                        }
                    }
                   
                }
                else
                {
                    if(yTimeEntry > zTimeEntry)
                    {
                        if (yEntry < 0.0f)
                        {
                            normalX = 0.0f;
                            normalY = 1.0f;
                            normalZ = 0.0f;
                        }
                        else
                        {
                            normalX = 0.0f;
                            normalY = -1.0f;
                            normalZ = 0.0f;
                        }
                    }
                    else
                    {
                        if (zEntry < 0.0f)
                        {
                            normalX = 0.0f;
                            normalY = 0.0f;
                            normalZ = 1.0f;
                        }
                        else
                        {
                            normalX = 0.0f;
                            normalY = 0.0f;
                            normalZ = -1.0f;
                        }
                    }
                   
                }
            }   // return the time of collision return entryTime


            return 1;
        }

        public static AABB GetSweptBroadphaseBox(this AABB b)
        {
            AABB broadphasebox = new AABB()
            {
                x = b.vx > 0 ? b.x : b.x + b.vx,
                y = b.vy > 0 ? b.y : b.y + b.vy,
                w = b.vx > 0 ? b.vx + b.w : b.w - b.vx,
                h = b.vy > 0 ? b.vy + b.h : b.h - b.vy
            };


            return broadphasebox;
        }

        public static bool AABBCheck(AABB b1, AABB b2)
        {
            return !(b1.x + b1.w < b2.x || b1.x > b2.x + b2.w || b1.y + b1.h < b2.y || b1.y > b2.y + b2.h);
        }
    }
}
