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
            //Time.timeScale = 0.1f;

            Debug.Log($"Dot Product: {Vector3.Dot(new Vector3(1, 0, 1), new Vector3(0, 0, -1))}");
            Debug.Log($"Dot Product: {Vector3.Dot(new Vector3(-1, 0, 1), new Vector3(0, 0, -1))}");
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
                DynamicEntity dEntity = _dynamicEntities[entity];
                if (!dEntity.Simulate) continue;
                dEntity.Position = dEntity.Transform.position;
               

                if (dEntity.Velocity.y > -5f)
                {
                    dEntity.AddVelocityY(-1f * Time.deltaTime);
                }


                _bounds.Clear();
                float normalX = 0, normalY = 0, normalZ = 0;
                float nearestCollisionTimeX = 1.0f;
                float nearestCollisionTimeY = 1.0f;
                float nearestCollisionTimeZ = 1.0f;
                int nearestAxis = -1;
                float remainingTimeX = 0;
                float remainingTimeY = 0;
                float remainingTimeZ = 0;

                AABB broadPhase;
                Vector3Int minBP, maxBP;
 

                // Y
                // ------------------------------------
                broadPhase = AABBExtensions.GetSweptBroadphaseBox(dEntity.AABB, new Vector3(0, dEntity.Velocity.y, 0) * Time.deltaTime);
                _drawer.AddPhysicBounds(broadPhase, Color.black);
                minBP = _main.GetBlockGPos(new Vector3(broadPhase.x , broadPhase.y, broadPhase.z));
                maxBP = _main.GetBlockGPos(new Vector3(broadPhase.x + broadPhase.w, broadPhase.y + broadPhase.h, broadPhase.z + broadPhase.d));
                for (int y = minBP.y; y <= maxBP.y; y++)
                {
                    for (int z = minBP.z; z <= maxBP.z; z++)
                    {
                        for (int x = minBP.x; x <= maxBP.x; x++)
                        {
                            if (_main.GetBlock(new Vector3(x, y, z)) != BlockType.Air)
                            {
                                AABB bound = GetBlockBound(new Vector3(x, y, z));
                                _drawer.AddPhysicBounds(bound, Color.red);
                                //_bounds.Add(b);

                                int axis = AABBExtensions.SweepTest(dEntity.AABB, bound, new Vector3(0, dEntity.Velocity.y, 0) * Time.deltaTime, out float t,
                                    out normalX, out normalY, out normalZ);

                                if (axis == -1 || t >= nearestCollisionTimeY) continue;

                                nearestCollisionTimeY = t;
                            }
                        }
                    }
                }
                remainingTimeY = 1.0f - nearestCollisionTimeY;
                if (remainingTimeY > 0.0f)
                {
                    dEntity.Position.y += dEntity.Velocity.y * (nearestCollisionTimeY - 1e-3f) * Time.deltaTime;
                }
                else
                {
                    dEntity.Position.y += dEntity.Velocity.y * nearestCollisionTimeY * Time.deltaTime;
                }





                // X
                // ------------------------------------
                broadPhase = AABBExtensions.GetSweptBroadphaseBox(dEntity.AABB, new Vector3(dEntity.Velocity.x, 0, 0) * Time.deltaTime);
                _drawer.AddPhysicBounds(broadPhase, Color.black);
                minBP = _main.GetBlockGPos(new Vector3(broadPhase.x, broadPhase.y, broadPhase.z));
                maxBP = _main.GetBlockGPos(new Vector3(broadPhase.x + broadPhase.w, broadPhase.y + broadPhase.h, broadPhase.z + broadPhase.d));
                for (int y = minBP.y; y <= maxBP.y; y++)
                {
                    for (int z = minBP.z; z <= maxBP.z; z++)
                    {
                        for (int x = minBP.x; x <= maxBP.x; x++)
                        {
                            if (_main.GetBlock(new Vector3(x, y, z)) != BlockType.Air)
                            {
                                AABB bound = GetBlockBound(new Vector3(x, y, z));
                                _drawer.AddPhysicBounds(bound, Color.red);
                                //_bounds.Add(b);

                                int axis = AABBExtensions.SweepTest(dEntity.AABB, bound, new Vector3(dEntity.Velocity.x, 0, 0) * Time.deltaTime, out float t,
                                    out normalX, out normalY, out normalZ);

                                if (axis == -1 || t >= nearestCollisionTimeX) continue;

                                nearestCollisionTimeX = t;
                            }
                        }
                    }
                }
                remainingTimeX = 1.0f - nearestCollisionTimeX;
                if (remainingTimeX > 0.0f)
                {
                    dEntity.Position.x += dEntity.Velocity.x * (nearestCollisionTimeX - 1e-1f) * Time.deltaTime;
                }
                else
                {
                    dEntity.Position.x += dEntity.Velocity.x * nearestCollisionTimeX * Time.deltaTime;
                }





                // Z
                // ------------------------------------
                broadPhase = AABBExtensions.GetSweptBroadphaseBox(dEntity.AABB, new Vector3(0, 0, dEntity.Velocity.z) * Time.deltaTime);
                _drawer.AddPhysicBounds(broadPhase, Color.black);
                minBP = _main.GetBlockGPos(new Vector3(broadPhase.x, broadPhase.y, broadPhase.z));
                maxBP = _main.GetBlockGPos(new Vector3(broadPhase.x + broadPhase.w, broadPhase.y + broadPhase.h, broadPhase.z + broadPhase.d));
                for (int y = minBP.y; y <= maxBP.y; y++)
                {
                    for (int z = minBP.z; z <= maxBP.z; z++)
                    {
                        for (int x = minBP.x; x <= maxBP.x; x++)
                        {
                            if (_main.GetBlock(new Vector3(x, y, z)) != BlockType.Air)
                            {
                                AABB bound = GetBlockBound(new Vector3(x, y, z));
                                _drawer.AddPhysicBounds(bound, Color.red);
                                //_bounds.Add(b);

                                int axis = AABBExtensions.SweepTest(dEntity.AABB, bound, new Vector3(0, 0, dEntity.Velocity.z) * Time.deltaTime, out float t,
                                    out normalX, out normalY, out normalZ);

                                if (axis == -1 || t >= nearestCollisionTimeZ) continue;

                                nearestCollisionTimeZ = t;
                            }
                        }
                    }
                }
                remainingTimeZ = 1.0f - nearestCollisionTimeZ;
                if (remainingTimeZ > 0.0f)
                {
                    dEntity.Position.z += dEntity.Velocity.z * (nearestCollisionTimeZ - 1e-1f) * Time.deltaTime;
                }
                else
                {
                    dEntity.Position.z += dEntity.Velocity.z * nearestCollisionTimeZ * Time.deltaTime;
                }





                //Push
                //if (remainingTime != 0)
                //{
                //    float magnitude = dEntity.Velocity.magnitude * remainingTime;
                //    float dotProduct = Mathf.Sign(dEntity.Velocity.x * normalZ + dEntity.Velocity.z * normalX);

                //    dEntity.Velocity.x = -10;
                //    dEntity.Velocity.z = 0;

                //    if (normalX != 0) dEntity.Velocity.x = 0;
                //    if (normalZ != 0) dEntity.Velocity.z = 0;

                //    //Debug.Log($"Loop: {loopCount}   {normalX}   {normalZ}   {dotProduct}   {dEntity.Velocity}");
                //    collisionTime = remainingTime * Time.deltaTime;
                //    dEntity.Position += dEntity.Velocity * collisionTime;
                //}


                dEntity.Transform.position = dEntity.Position;
                dEntity.AABB = new AABB()
                {
                    x = dEntity.Transform.position.x - 0.5f,
                    y = dEntity.Transform.position.y,
                    z = dEntity.Transform.position.z - 0.5f,
                    w = 0.9f,
                    h = 1.9f,
                    d = 0.9f
                };
                _drawer.AddPhysicBounds(dEntity.AABB, Color.green);

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
