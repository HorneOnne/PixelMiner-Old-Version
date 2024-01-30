using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelMiner.Core;
using PixelMiner.Enums;
using PixelMiner.DataStructure;
using PixelMiner.Miscellaneous;

namespace PixelMiner.Physics
{
    public class GamePhysics : MonoBehaviour
    {
        private static List<DynamicEntity> _dynamicEntities = new List<DynamicEntity>();
        private Main _main;

        private Vector3 _gravity = Vector3.down;
        [SerializeField] private float _gravityForce = 9.8f;
        private List<float> _collisionTimes = new List<float>();
        private List<AABB> _bounds = new List<AABB>();

        private DrawBounds _drawer;
        private void Start()
        {
            _main = Main.Instance;
            _drawer = DrawBounds.Instance;
        }

        public static void AddDynamicEntity(DynamicEntity entity)
        {
            _dynamicEntities.Add(entity);
        }

        private void Update()
        {
            Vector3 gravityForce = _gravity * _gravityForce;
            for (int entity = 0; entity < _dynamicEntities.Count; entity++)
            {
                _collisionTimes.Clear();

                DynamicEntity dEntity = _dynamicEntities[entity];
                if (!dEntity.Simulate) continue;
                dEntity.Position = dEntity.Transform.position;
                dEntity.AABB = new AABB()
                {
                    x = dEntity.Transform.position.x - 0.5f,
                    y = dEntity.Transform.position.y,
                    z = dEntity.Transform.position.z - 0.5f,
                    w = 1,
                    h = 2,
                    d = 1,
                };
                dEntity.SetVelocity(new Vector3(0.0f, 0.0f,-1.0f));
                _drawer.AddPhysicBounds(dEntity.AABB, Color.green);


                AABB broadPhase = AABBExtensions.GetSweptBroadphaseBox(dEntity.AABB, dEntity.Velocity);
                _drawer.AddPhysicBounds(broadPhase, Color.black);
                float threshold = 0.01f;
                Vector3Int minBP = _main.GetBlockGPos(new Vector3(broadPhase.x - threshold, broadPhase.y - threshold, broadPhase.z - threshold));
                Vector3Int maxBP = _main.GetBlockGPos(new Vector3((broadPhase.x + broadPhase.w) + threshold, (broadPhase.y + broadPhase.h) + threshold, (broadPhase.z + broadPhase.d) + threshold));
                //Debug.Log($"{minBP}   {maxBP}");
                
                _bounds.Clear();
                for (int y = minBP.y; y <= maxBP.y; y++)
                {
                    for (int z = minBP.z; z <= maxBP.z; z++)
                    {
                        for (int x = minBP.x; x <= maxBP.x; x++)
                        {
                            if (_main.GetBlock(new Vector3(x, y, z)) != BlockType.Air)
                            {
                                AABB b = GetBlockBound(new Vector3(x, y, z));
                                _drawer.AddPhysicBounds(b, Color.red);
                                _bounds.Add(b);  
                            }
                        }
                    }
                }


                int loopCount = 0;
                int maxLoops = 10;
                float remainingDelta = Time.deltaTime;
                //Debug.Log($"Bound count: {_bounds.Count}");
                float nearestEntryTime = 1.0f;
                int nearestAxis = -1;
                float normalX = 0, normalY = 0, normalZ = 0;
                for (int i = 0; i < _bounds.Count; i++)
                {
                    AABB bound = _bounds[i];
                    int axis = AABBExtensions.SweepTest(dEntity.AABB, bound, dEntity.Velocity, out float t, 
                        out normalX, out normalY, out normalZ);
          
                    if (axis == -1)
                    {
                        continue;
                    }

   
                    if(nearestEntryTime > t)
                    {
                        nearestEntryTime = t;
                        nearestAxis = axis;
                    }
                   
                }



                // Zero out velocity on collided axis
                //if (nearestAxis == 0)
                //{
                //    dEntity.Velocity.x = 0;
                //}
                //else if (nearestAxis == 1)
                //{
                //    dEntity.Velocity.y = 0;
                //}
                //else if (nearestAxis == 2)
                //{
                //    dEntity.Velocity.z = 0;
                //}

                //Debug.Log(nearestEntryTime);
                //Debug.Log($"{normalX} {normalY} {normalZ}");
    
                if(nearestEntryTime > Time.deltaTime)
                    nearestEntryTime = Time.deltaTime;

                dEntity.Position += dEntity.Velocity * nearestEntryTime;
                dEntity.Transform.position = dEntity.Position;
                //Debug.Log($"{dEntity.Velocity} {nearestEntryTime} {remainingDelta}");


                //while (remainingDelta > 1e-10f && loopCount++ < maxLoops)
                //{
                //    float nearestEntryTime = 1.0f;
                //    int nearestAxis = -1;

                //    for(int i = 0; i < _bounds.Count; i++)
                //    {
                //        AABB bound = _bounds[i];


                //        int axis = AABBExtensions.SweepTest(dEntity.AABB, bound, dEntity.Velocity * remainingDelta, out float t);

                //        if(axis == -1 || t >= nearestEntryTime)
                //        {
                //            continue;
                //        }
                //        nearestEntryTime = t;
                //        nearestAxis = axis;
                //    }

                //    // No collision
                //    if (nearestAxis == -1)
                //    {
                //        dEntity.Position += dEntity.Velocity * remainingDelta;
                //        remainingDelta = 0;
                //        break;
                //    }
                //    else
                //    {
                //        // Handle multiple collision
                //        if(nearestEntryTime < 0)
                //        {
                //            // Handle negateive nearest caused by d
                //            if (nearestEntryTime < 0)
                //                Debug.Log($"nearestEntryTime < 0");

                //        }
                //        else
                //        {
                //            // Move to the point of collision and reduce time remaining
                //            dEntity.Position += dEntity.Velocity * nearestEntryTime * remainingDelta;
                //            remainingDelta -= nearestEntryTime * remainingDelta;
                //        }

                //        // Zero out velocity on collided axis
                //        if(nearestAxis == 0)
                //        {
                //            dEntity.Velocity.x = 0;
                //        }
                //        else if(nearestAxis == 1)
                //        {
                //            dEntity.Velocity.y = 0;
                //        }
                //        else if(nearestAxis == 2)
                //        {
                //            dEntity.Velocity.z = 0;
                //        }
                //    }
                //}
                //if (loopCount >= maxLoops)
                //{
                //    Debug.LogWarning("physics loop count exceeded!");
                //}
                //dEntity.Transform.position = dEntity.Position;
            }
        }

        private void LateUpdate()
        {
            //for (int i = 0; i < _dynamicEntities.Count; i++)
            //{
            //    _dynamicEntities[i].SetVelocity(Vector3.zero);
            //}
        }


        private void ApplyGravity(DynamicEntity entity, float collisionTime)
        {
            //entity.Transform.position += new Vector3(_gravity.x, _gravity.y, _gravity.z) * collisionTime;
            entity.Transform.position += new Vector3(entity.Velocity.x, entity.Velocity.y, entity.Velocity.z) * collisionTime;
            //Debug.Log($"{collisionTime} {entity.Velocity}");
        }

        private AABB GetBlockBound(Vector3 globalPosition)
        {
            int x = Mathf.FloorToInt(globalPosition.x);
            int y = Mathf.FloorToInt(globalPosition.y);
            int z = Mathf.FloorToInt(globalPosition.z);

            AABB bound = new AABB()
            {
                x = x,
                y = y,
                z = z,
                w = 1,
                h = 1,
                d = 1,
            };
            return bound;
        }
    }
}
