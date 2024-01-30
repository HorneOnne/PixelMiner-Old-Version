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
            for (int i = 0; i < _dynamicEntities.Count; i++)
            {
                _collisionTimes.Clear();

                DynamicEntity dEntity = _dynamicEntities[i];
                if (!dEntity.Simulate) continue;
                dEntity.AABB = new AABB()
                {
                    x = dEntity.Transform.position.x - 0.5f,
                    y = dEntity.Transform.position.y,
                    z = dEntity.Transform.position.z - 0.5f,
                    w = 1,
                    h = 2,
                    d = 1,
                    vx = 0,
                    vy = 0,
                    vz = 0
                };

                _drawer.AddPhysicBounds(dEntity.AABB, Color.green);


                AABB broadPhase = AABBExtensions.GetSweptBroadphaseBox(dEntity.AABB);
                float d = 0.01f;
                int count = 0;
                for (float z = dEntity.Transform.position.z - d; z <= dEntity.Transform.position.z + d; z++)
                {
                    for (float y = dEntity.Transform.position.y - d; y <= dEntity.Transform.position.y + d; y++)
                    {
                        for (float x = dEntity.Transform.position.x - d; x <= dEntity.Transform.position.x + d; x++)
                        {
                            count++;
                            AABB b = GetBlockBound(new Vector3(x, y, z));
                            _drawer.AddPhysicBounds(b, Color.red);
                            float collisionTime = 1;
                            if (_main.GetBlock(new Vector3(x, y, z)) == BlockType.Air)
                            {                                
                                if (!b.Equals(default))
                                {
                                    if (AABBExtensions.AABBCheck(broadPhase, b))
                                    {
                                        //Debug.Log($"dEntity: {dEntity.AABB}");
                                        //Debug.Log($"b : {b}");
                                        //Debug.Break();
                                        AABBExtensions.SweptAABB(dEntity.AABB, b, out float normalX, out float normalY, out float normalZ, out collisionTime);
                                    }
                                }

                                _collisionTimes.Add(collisionTime);
                            }
                        }
                    }
                }

  
                if(_collisionTimes.Count > 0)
                {
                    float lowesetTime = _collisionTimes[0];

                    for (int j = 1; j < _collisionTimes.Count; j++)
                    {
                        if (lowesetTime > _collisionTimes[i])
                            lowesetTime = _collisionTimes[i];
                    }


                    //Debug.Log(lowesetTime);


                    if (lowesetTime > Time.deltaTime)
                        lowesetTime = Time.deltaTime;


                    ApplyGravity(dEntity, lowesetTime);
                }      
            }
        }


        private void ApplyGravity(DynamicEntity entity, float collisionTime)
        {
            //entity.Transform.position += new Vector3(_gravity.x, _gravity.y, _gravity.z) * collisionTime;
            entity.Transform.position += _gravity * Time.deltaTime;
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
                vx = 0,
                vy = 0,
                vz = 0,
            };
            return bound;
        }
    }
}
