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
                //xEntry = dynamicBox.x - (staticBox.x + staticBox.w);
                //xExit = (dynamicBox.x + dynamicBox.w) - staticBox.x;

                xEntry = (staticBox.x + staticBox.w) - dynamicBox.x;
                xExit = staticBox.x - (dynamicBox.x + dynamicBox.w);
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
                //zEntry = dynamicBox.z - (staticBox.z + staticBox.d);
                //zExit = (dynamicBox.z + dynamicBox.d) - staticBox.z;

                zEntry = (staticBox.z + staticBox.d) - dynamicBox.z;
                zExit = staticBox.z - (dynamicBox.z + dynamicBox.d);
            }


            if(vel.x == 0)
            {
                xTimeEntry = float.NegativeInfinity;
                xTimeExit = float.PositiveInfinity;
            }
            else
            {
                xTimeEntry = xEntry / vel.x;
                xTimeExit = xExit / vel.x;
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
                zTimeEntry = zEntry / vel.z;
                zTimeExit = zExit / vel.z;
            }


            Debug.Log($"{xTimeEntry} {zTimeEntry}");
            //Debug.Log($"TimeEntry: {xTimeEntry}  {zTimeEntry}");

            time = Mathf.Max(xTimeEntry, yTimeEntry, zTimeEntry);
            float timeExit = Mathf.Min(xTimeExit, yTimeExit, zTimeExit);


            //Debug.Log($"{xTimeEntry} {yTimeEntry} {zTimeEntry}");
   
            if(time > timeExit || (xTimeEntry < 0 && yTimeEntry < 0 && zTimeEntry < 0) || xTimeEntry > 1.0f || yTimeEntry > 1.0f || zTimeEntry > 1.0f)
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
                        return 0;
                    }
                    else
                    {
                        // Z
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
                        return 2;
                    }
                }
                else
                {
                    if (yTimeEntry > zTimeEntry)
                    {
                        // Y
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
                        return 1;
                    }
                    else
                    {
                        // Z
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
                        return 2;
                    }
                }
            }
        }
        public static int SweepTest2(AABB dynamicBox, AABB staticBox, Vector3 vel, out float dtime)
        {
            // find distance between objects on near and far sides
            float invEntrX;
            float invEntrY;
            float invEntrZ;
            float invExitX;
            float invExitY;
            float invExitZ;

            if (vel.x > 0.0f)
            {
                invEntrX = staticBox.x - dynamicBox.x + dynamicBox.w;
                invExitX = staticBox.x + staticBox.w - dynamicBox.x;
            }
            else
            {
                invEntrX = staticBox.x + staticBox.w - dynamicBox.x;
                invExitX = staticBox.x - dynamicBox.x + dynamicBox.w;
            }
            if (vel.y > 0.0f)
            {
                invEntrY = staticBox.y - dynamicBox.y + dynamicBox.h;
                invExitY = staticBox.y + staticBox.h - dynamicBox.y;
            }
            else
            {
                invEntrY = staticBox.y + staticBox.h - dynamicBox.y;
                invExitY = staticBox.y - dynamicBox.y + dynamicBox.h;
            }
            if (vel.z > 0.0f)
            {
                invEntrZ = staticBox.z - dynamicBox.z + dynamicBox.d;
                invExitZ = staticBox.z + staticBox.d - dynamicBox.z;
            }
            else
            {
                invEntrZ = staticBox.z + staticBox.d - dynamicBox.z;
                invExitZ = staticBox.z - dynamicBox.z + dynamicBox.d;
            }

            float entrX;
            float entrY;
            float entrZ;
            float exitX;
            float exitY;
            float exitZ;

            if (vel.x == 0.0f)
            {
                entrX = float.NegativeInfinity;
                exitX = float.PositiveInfinity;
            }
            else
            {
                entrX = invEntrX / vel.x;
                exitX = invExitX / vel.x;
            }
            if (vel.y == 0.0f)
            {
                entrY = float.NegativeInfinity;
                exitY = float.PositiveInfinity;
            }
            else
            {
                entrY = invEntrY / vel.y;
                exitY = invExitY / vel.y;
            }
            if (vel.z == 0.0f)
            {
                entrZ = float.NegativeInfinity;
                exitZ = float.PositiveInfinity;
            }
            else
            {
                entrZ = invEntrZ / vel.z;
                exitZ = invExitZ / vel.z;
            }

            


            float entrTime = Mathf.Max(entrX, Mathf.Max(entrY, entrZ));
            float exitTime = Mathf.Min(exitX, Mathf.Min(exitY, exitZ));

            dtime = entrTime;
            // check if no collision
            if (entrTime > exitTime ||
                entrX < 0.0f && entrY < 0.0f && entrZ < 0.0f ||
                entrX > 1.0f || entrY > 1.0f || entrZ > 1.0f)
            {
                return -1;
            }

            if (entrX > entrY)
            {
                if (entrX > entrZ)
                {
                    return 0;
                }
                else
                {
                    return 2;
                }
            }
            else
            {
                if (entrY > entrZ)
                {
                    return 1;
                }
                else
                {
                    return 2;
                }
            }

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
