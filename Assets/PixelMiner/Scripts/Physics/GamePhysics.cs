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
            //Time.timeScale = 0.4f;
        }

        public static void AddDynamicEntity(DynamicEntity entity)
        {
            _dynamicEntities.Add(entity);
        }
        private int currentAxis = -1;
        private void Update()
        {
            Vector3 gravityForce = _gravity * _gravityForce;
            for (int entity = 0; entity < _dynamicEntities.Count; entity++)
            {
                _collisionTimes.Clear();

                DynamicEntity dEntity = _dynamicEntities[entity];
                if (!dEntity.Simulate) continue;
                dEntity.Position = dEntity.Transform.position;
              
                //dEntity.SetVelocity(new Vector3(1.0f, 0.0f, -1.0f));         
                  
                if (dEntity.Velocity == Vector3.zero) continue;

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
                                if(AABBExtensions.AABBCheck(broadPhase, b))
                                {
                                    _drawer.AddPhysicBounds(b, Color.red);
                                    _bounds.Add(b);
                                }
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
                    //int axis = AABBExtensions.SweepTest2(dEntity.AABB, bound, dEntity.Velocity, out float t);
 
         
                    if (axis == -1)
                    {
                        continue;
                    }


                    if (nearestEntryTime > t)
                    {
                        nearestEntryTime = t;
                        nearestAxis = axis;
                    }
                }
                Vector3 offset = new Vector3(0, 2, 0);
                if(nearestAxis == 0)
                {
                    _drawer.AddLine(dEntity.Position + offset, (dEntity.Position + offset) + Vector3.right * normalX * nearestEntryTime, Color.red);
                }
                else if(nearestAxis == 1)
                {
                    _drawer.AddLine(dEntity.Position + offset, (dEntity.Position + offset) + Vector3.up * normalY * nearestEntryTime, Color.green);
                }
                else if(nearestAxis == 2)
                {
                    _drawer.AddLine(dEntity.Position + offset, (dEntity.Position + offset) + Vector3.forward * normalZ * nearestEntryTime, Color.blue);
                }

                float remainingTime = 1.0f - nearestEntryTime;

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


                //if (currentAxis == -1)
                //{
                //    currentAxis = nearestAxis;
                //}
                //else
                //{
                //    if (currentAxis != nearestAxis)
                //    {
                //        Debug.Break();
                //    }
                //}

          

                if (nearestEntryTime > Time.deltaTime)
                    nearestEntryTime = Time.deltaTime;


                dEntity.Position += dEntity.Velocity * nearestEntryTime;
                dEntity.Transform.position = dEntity.Position;

                dEntity.AABB = new AABB()
                {
                    x = dEntity.Transform.position.x - 0.5f,
                    y = dEntity.Transform.position.y,
                    z = dEntity.Transform.position.z - 0.5f,
                    w = 1,
                    h = 2,
                    d = 1,
                };
                _drawer.AddPhysicBounds(dEntity.AABB, Color.green);

                //if (nearestAxis != -1)
                //{ 
                //    Debug.Break();
                //}

                //Debug.Log($"{dEntity.Velocity} {nearestEntryTime} {remainingDelta}");


             
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


        private void OnApplicationQuit()
        {
#if UNITY_EDITOR
            _collisionTimes.Clear();
            _bounds.Clear();
            _dynamicEntities.Clear();
#endif
        }
    }
}
