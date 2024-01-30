using UnityEngine;

namespace PixelMiner.DataStructure
{
    public static class AABBExtensions
    {
        public static int SweptAABB(AABB dynamicBox, AABB staticBox, Vector3 vel, out float normalX, out float normalY, out float normalZ, out float entryTime)
        {
            float xEntry, yEntry, zEntry; // Specify how far away the closest edges of the objects are from each other.
            float xExit, yExit, zExit;   // The distance to the far side of the object

            // Find the distance between the objects on the near and far sides for both x and y
            if (vel.x > 0.0f)
            {
                xEntry = staticBox.x - (dynamicBox.x + dynamicBox.w);
                xExit = (staticBox.x + staticBox.w) - dynamicBox.x;
            }
            else
            {
                xEntry = dynamicBox.x - (staticBox.x + staticBox.w);
                xExit = (dynamicBox.x + dynamicBox.w) - staticBox.x;
            }

            if (vel.y > 0.0f)
            {
                yEntry = staticBox.y - (dynamicBox.y + dynamicBox.h);
                yExit = (staticBox.y + staticBox.h) - dynamicBox.y;
            }
            else
            {
                yEntry = dynamicBox.y - (staticBox.y + staticBox.h);
                yExit = (dynamicBox.y + dynamicBox.h) - staticBox.y;
            }

            if (vel.z > 0.0f)
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

            if (vel.x == 0.0f)
            {
                xTimeEntry = float.NegativeInfinity;
                xTimeExit = float.PositiveInfinity;
                //Debug.Log("A");
            }
            else
            {
                xTimeEntry = xEntry / Mathf.Abs(vel.x);
                xTimeExit = xExit / Mathf.Abs(vel.x);
               // Debug.Log("B");
            }

            if (vel.y == 0.0f)
            {
                yTimeEntry = float.NegativeInfinity;
                yTimeExit = float.PositiveInfinity;
                //Debug.Log("C");
            }
            else
            {
                yTimeEntry = yEntry / Mathf.Abs(vel.y);
                yTimeExit = yExit / Mathf.Abs(vel.y);
                //Debug.Log("D");
            }



            if (vel.z == 0.0f)
            {
                zTimeEntry = float.NegativeInfinity;
                zTimeExit = float.PositiveInfinity;
                //Debug.Log("E");
            }
            else
            {
                zTimeEntry = zEntry / Mathf.Abs(vel.z);
                zTimeExit = zExit / Mathf.Abs(vel.z);
                //Debug.Log("F");
            }

            // Find the earliest/lastest times of collision
            //float entryTime = Mathf.Max(xEntry, yEntry);
            entryTime = Mathf.Max(xTimeEntry, yTimeEntry, zTimeEntry);       // entryTime tell when the collision occured
            float exitTime = Mathf.Min(xTimeExit, yTimeExit, zTimeExit);         // exitTime tell when collision exited the object from other side.
            //Debug.Log($"{entryTime} {xTimeEntry} {yTimeEntry} {zTimeEntry}");
            //Debug.Break();
            // If there was no collision
            if (entryTime > exitTime || xTimeEntry < 0.0f && yTimeEntry < 0.0f && zTimeEntry < 0.0f || xTimeEntry > 1.0f && yTimeEntry > 1.0f && zTimeEntry > 1.0f)
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

        public static int SweepTest(AABB dynamicBox, AABB staticBox, Vector3 vel, out float time, out float normalX, out float normalY, out float normalZ)
        {
            const float COL_ZERO = 0; // also


            //AABB rel;   // relative aabb
            //rel.x = dynamicBox.x - staticBox.x;
            //rel.y = dynamicBox.y - staticBox.y;
            //rel.z = dynamicBox.z - staticBox.z;
            //rel.w = (dynamicBox.x + dynamicBox.w) - staticBox.x;
            //rel.h = (dynamicBox.y + dynamicBox.h) - staticBox.y;
            //rel.d = (dynamicBox.z + dynamicBox.d) - staticBox.z;

            float xEntry, yEntry, zEntry;
            float xExit, yExit, zExit;  

            float xTimeEntry, yTimeEntry, zTimeEntry;
            float xTimeExit, yTimeExit, zTimeExit;

            if(vel.x > 0)
            {
                xEntry = staticBox.x - (dynamicBox.x + dynamicBox.w);
                xExit = (staticBox.x + staticBox.w) - dynamicBox.x;
            }
            else
            {
                xEntry = dynamicBox.x - (staticBox.x + staticBox.w);
                xExit = (dynamicBox.x + dynamicBox.w) - staticBox.x;

                //Debug.Log($"{dynamicBox.x}  {staticBox.x}  {xEntry}");
            }

            if(vel.y > 0)
            {
                yEntry = staticBox.y - (dynamicBox.y + dynamicBox.h);
                yExit = (staticBox.y + staticBox.h) - dynamicBox.y;
            }
            else
            {
                yEntry = dynamicBox.y - (staticBox.y + staticBox.h);
                yExit = (dynamicBox.y + dynamicBox.h) - staticBox.y;
            }

            if(vel.z > 0)
            {
                zEntry = staticBox.z - (dynamicBox.z + dynamicBox.d);
                zExit = (staticBox.z + staticBox.d) - dynamicBox.z;
            }
            else
            {
                zEntry = dynamicBox.z - (staticBox.z + staticBox.d);
                zExit = (dynamicBox.z + dynamicBox.z) - staticBox.z;
            }


            if(vel.x == 0)
            {
                xTimeEntry = float.NegativeInfinity;
                xTimeExit = float.PositiveInfinity;
            }
            else
            {
                xTimeEntry = Mathf.Abs(xEntry / vel.x);
                xTimeExit = Mathf.Abs(xExit / vel.x);

                //Debug.Log($"{xEntry} {vel.x} {xTimeEntry}");
            }

            if(vel.y == 0)
            {
                yTimeEntry = float.NegativeInfinity;
                yTimeExit = float.PositiveInfinity;
            }
            else
            {
                yTimeEntry = Mathf.Abs(yEntry / vel.y);
                yTimeExit = Mathf.Abs(yExit / vel.y);
            }

            if(vel.z == 0)
            {
                zTimeEntry = float.NegativeInfinity;
                zTimeExit = float.PositiveInfinity;
            }
            else
            {
                zTimeEntry = Mathf.Abs(zEntry / vel.z);
                zTimeExit = Mathf.Abs(zExit / vel.z);
            }

            time = Mathf.Max(xTimeEntry, yTimeEntry, zTimeEntry);
            float timeExit = Mathf.Min(xTimeExit, yTimeExit, zTimeExit);


            //Debug.Log($"{xTimeEntry} {yTimeEntry} {zTimeEntry}");
   
            if(time > timeExit || xEntry < 0 && yEntry < 0 && zTimeEntry < 0 || xEntry > 1.0f && yEntry > 1.0f && zEntry > 1.0f)
            {
                // No collision
                normalX = 0.0f;
                normalY = 0.0f;
                normalZ = 0.0f;
                return -1;
            }
            else
            {
                if(xTimeEntry > yTimeEntry)
                {
                    if (xTimeEntry > zTimeEntry)
                    {
                        // X
                        Debug.Log("X");

                        if(xEntry < 0)
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
                        // Z
                        Debug.Log("Z 1");
                        if (zEntry < 0)
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
                    if (yTimeEntry > zTimeEntry)
                    {
                        // Y
                        Debug.Log("Y");

                        if (yEntry < 0)
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
                        // Z
                        Debug.Log("Z 2");
                        if (zEntry < 0)
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
            }

            return 1;

            time = 0.0f;
            //float d = 0.01f;
            //// checking y first because prob happens most often due to gravity
            //if (vel.y > 0)
            //{
             
            //}
            //else if (vel.y < 0)
            //{
            //    if (rel.y + rel.h >= rel.h - d)
            //    {
            //        time = (rel.h - rel.y) / vel.y;
            //        Debug.Log(time);
            //        if ((rel.x + vel.x * time < rel.w) &&
            //            ((rel.x + rel.w) + vel.x * time > COL_ZERO) &&
            //            (rel.z + vel.z * time < rel.d) &&
            //            ((rel.z + rel.d) + vel.z * time > COL_ZERO))
            //        {
            //            return 1;
            //        }
            //    }
            //    else if (rel.y + rel.h < 0)
            //    {
            //        return -1;
            //    }
            //}

            return -1;
        }

        public static AABB GetSweptBroadphaseBox(this AABB b, Vector3 vel)
        {
            AABB broadphasebox = new AABB()
            {
                x = vel.x > 0 ? b.x : b.x + vel.x,
                y = vel.y > 0 ? b.y : b.y + vel.y,
                z = vel.z > 0 ? b.z : b.z + vel.z,
                w = vel.x > 0 ? vel.x + b.w : b.w - vel.x,
                h = vel.y > 0 ? vel.y + b.h : b.h - vel.y,
                d = vel.z > 0 ? vel.z + b.d : b.d - vel.z
            };


            return broadphasebox;
        }

        public static bool AABBCheck(AABB b1, AABB b2)
        {
            return !(b1.x + b1.w < b2.x || b1.x > b2.x + b2.w || 
                    b1.y + b1.h < b2.y || b1.y > b2.y + b2.h ||
                    b1.z + b1.d < b2.z || b1.z > b2.z + b2.d);
        }
    }
}
