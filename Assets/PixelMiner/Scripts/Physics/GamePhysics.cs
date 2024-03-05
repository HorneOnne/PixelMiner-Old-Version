using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using PixelMiner.Core;
using PixelMiner.Enums;
using PixelMiner.DataStructure;
using PixelMiner.Miscellaneous;
using Sirenix.OdinInspector;

namespace PixelMiner.Physics
{
    public class GamePhysics : SerializedMonoBehaviour
    {
        public static GamePhysics Instance { get; private set; }
        public Dictionary<Vector3Int, Octree> SpatialOctrees;
        private Main _main;
        private Vector3Int OctreeDimensions = new Vector3Int(256, 256, 256);
        private const int OCTREE_CAPACITY = 3;

        [SerializeField] private Vector3 _gravity = Vector3.down;
        private List<float> _collisionTimes = new List<float>();
        private List<AABB> _bounds = new List<AABB>();
        private Queue<DynamicEntity> _removeEntityQueue = new Queue<DynamicEntity>();
        private Queue<DynamicEntity> _addEntityQueue = new Queue<DynamicEntity>();


        // Physics clamp
        [SerializeField] private float _minFallForce = -3;
        [SerializeField] private float _maxJumpForce = 10;


        [SerializeField] private AnimationCurve _physicWaterFloatingCurve;
        private DrawBounds _drawer;

        private List<DynamicEntity> _allEntitiesQueryList = new List<DynamicEntity>();
        private List<DynamicEntity> _octreeEntitiesQueryList = new List<DynamicEntity>();
        private List<DynamicEntity> _entitiesQueryList;
        private Vector3[] _corners = new Vector3[8];
        private List<Octree> _overlapBoxOctrees = new List<Octree>();
        private List<DynamicEntity> _overlapEntitiesFound = new List<DynamicEntity>();


        private Queue<Vector3Int> _treeRemovalKey = new Queue<Vector3Int>();

        public int TotalEntities;


        [Header("Gizmos")]
        [SerializeField] private bool _showEntityBounds;
        [SerializeField] private bool _showOctreeBounds;

        private void Awake()
        {
            Instance = this;
            SpatialOctrees = new Dictionary<Vector3Int, Octree>();
            //_octreePhysics = new Octree<DynamicEntity>(new AABB(-700, -700, -700, 1400, 1400, 1400), OCTREE_CAPACITY, level: 0);
        }

        public GameObject Prefab;
        public LayerMask ItemLayer;
        private void Start()
        {
            _main = Main.Instance;
            _drawer = DrawBounds.Instance;
            //Time.timeScale = 0.1f;

            _entitiesQueryList = new List<DynamicEntity>();


            // Test
            //int entityCount = 10000;
            //for (int i = 0; i < entityCount; i++)
            //{
            //    Vector3 randomPosition = new Vector3(Random.Range(-300, 300), Random.Range(-300, 300), Random.Range(-300, 300));
            //    var entityObject = Instantiate(Prefab, randomPosition, Quaternion.identity);
            //    AABB bound = GetBlockBound(randomPosition);
            //    DynamicEntity dEntity = new DynamicEntity(entityObject.transform, bound, Vector2.zero, ItemLayer);
            //    dEntity.Simulate = false;
            //    dEntity.SetConstraint(Constraint.X, true);
            //    dEntity.SetConstraint(Constraint.Y, true);
            //    dEntity.SetConstraint(Constraint.Z, true);
            //    HandleAddDynamicEntity(dEntity);
            //}


            if (_addEntityQueue.Count > 0)
            {
                while (_addEntityQueue.Count > 0)
                {
                    HandleAddDynamicEntity(_addEntityQueue.Dequeue());
                }
            }
        }


        public int AddEachFrame = 0;
        public int RemoveEachFrame = 0;
        [Space(10)]
        public int NumOfOctreeRootPool;
        public int ActiveTreeRoot;
        public int InactiveTreeRoot;
        [Space(10)]
        public int NumOfOctreeLeavePool;
        public int ActivetreeLeaves;
        public int InactivetreeLeaves;
        private void Update()
        {
            NumOfOctreeRootPool = OctreeRootPool.Pool.CountAll;
            ActiveTreeRoot = OctreeRootPool.Pool.CountActive;
            InactiveTreeRoot = OctreeRootPool.Pool.CountInactive;

            NumOfOctreeLeavePool = OctreeLeavePool.Pool.CountAll;
            ActivetreeLeaves = OctreeLeavePool.Pool.CountActive;
            InactivetreeLeaves = OctreeLeavePool.Pool.CountInactive;

            if (Input.GetKeyDown(KeyCode.I))
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector3 randomPosition = new Vector3(Random.Range(-300, 300), Random.Range(10, 64), Random.Range(-300, 300));
                    var entityObject = Instantiate(Prefab, randomPosition, Quaternion.identity);
                    AABB bound = GetBlockBound(randomPosition);
                    DynamicEntity dEntity = new DynamicEntity(entityObject.transform, bound, Vector2.zero, ItemLayer);
                    dEntity.Mass = 10;
                    dEntity.SetConstraint(Constraint.X, true);
                    dEntity.SetConstraint(Constraint.Y, true);
                    dEntity.SetConstraint(Constraint.Z, true);

                    AddDynamicEntity(dEntity);
                }
            }



            foreach (var octree in SpatialOctrees)
            {
                if (_showEntityBounds)
                {
                    _octreeEntitiesQueryList.Clear();
                    int entitiesFound = GetAllEntitiesInOctreeNonAlloc(octree.Key, ref _octreeEntitiesQueryList);
                    for (int i = 0; i < _octreeEntitiesQueryList.Count; i++)
                    {
                        _drawer.AddPhysicBounds(_octreeEntitiesQueryList[i].AABB, Color.green);
                    }
                }


                if (_showOctreeBounds)
                {
                    octree.Value.TraverseRecursive((AABB, Color) =>
                    {
                        _drawer.AddPhysicBounds(AABB, Color);
                    });
                }

            }
        }

        private void FixedUpdate()
        {
            UnityEngine.Profiling.Profiler.BeginSample("Recreate octree sample");
            _allEntitiesQueryList.Clear();
            TotalEntities = 0;
            foreach (var octree in SpatialOctrees)
            {
                _octreeEntitiesQueryList.Clear();
                int entitiesFound = GetAllEntitiesInOctreeNonAlloc(octree.Key, ref _octreeEntitiesQueryList);
                TotalEntities += entitiesFound;
                for (int entity = 0; entity < entitiesFound; entity++)
                {
                    DynamicEntity dEntity = _octreeEntitiesQueryList[entity];
                    if (!dEntity.Simulate) continue;

                    bool entityUpdateByEditor = false;
                    bool entityIdle;
                    AABB broadPhase;

                    Vector3 currPos = dEntity.Position;
                    dEntity.Position = dEntity.Transform.position;


                    if (currPos != dEntity.Position)
                    {
                        //Debug.Log("Not update physics this entity due editor.");
                        entityUpdateByEditor = true;
                    }


                    if (dEntity.Velocity.y > _minFallForce)
                    {
                        dEntity.AddVelocity(_gravity * dEntity.Mass * Time.fixedDeltaTime);
                    }




                    // Y
                    // ------------------------------------
                    if (!dEntity.GetConstraint(Constraint.Y))
                    {
                        broadPhase = AABBExtensions.GetSweptBroadphaseBox(dEntity.AABB, new Vector3(0, dEntity.Velocity.y, 0) * Time.fixedDeltaTime);
                        ResolveResolutionY(dEntity, broadPhase, Time.fixedDeltaTime);
                    }




                    // X
                    // ------------------------------------
                    if (!dEntity.GetConstraint(Constraint.X))
                    {
                        broadPhase = AABBExtensions.GetSweptBroadphaseBox(dEntity.AABB, new Vector3(dEntity.Velocity.x, 0, 0) * Time.fixedDeltaTime);
                        ResolveResolutionX(dEntity, broadPhase, Time.fixedDeltaTime);
                    }





                    // Z
                    // ------------------------------------
                    if (!dEntity.GetConstraint(Constraint.Z))
                    {
                        broadPhase = AABBExtensions.GetSweptBroadphaseBox(dEntity.AABB, new Vector3(0, 0, dEntity.Velocity.z) * Time.fixedDeltaTime);
                        ResolveResolutionZ(dEntity, broadPhase, Time.fixedDeltaTime);
                    }

                    //Debug.Log($"{dEntity.Position}    {dEntity.Transform.position}");

                    if (dEntity.Position != dEntity.Transform.position || entityUpdateByEditor)
                    {
                        //Debug.Log($"Entity move");
                        if (dEntity.Leave != null)
                        {
                            if (dEntity.Leave.Bound.Contains(dEntity.Position) == false)
                            {
                                _removeEntityQueue.Enqueue(dEntity);
                                _addEntityQueue.Enqueue(dEntity);
                            }
                        }
                        else if (dEntity.Root.Bound.Contains(dEntity.Position) == false)
                        {
                            _removeEntityQueue.Enqueue(dEntity);
                            _addEntityQueue.Enqueue(dEntity);
                        }
                        entityIdle = false;


                        //_removeEntityQueue.Enqueue(dEntity);
                        //_addEntityQueue.Enqueue(dEntity);
                        //entityIdle = false;
                    }
                    else
                    {
                        entityIdle = true;
                        //Debug.Log("Entity idle");
                    }



                    // Update physics position
                    if (!entityUpdateByEditor)
                    {
                        dEntity.Transform.position = dEntity.Position;
                    }
                    else
                    {
                        dEntity.Position = dEntity.Transform.position;
                    }

                    if (entityIdle == false || entityUpdateByEditor)
                    {
                        dEntity.AABB = new AABB()
                        {
                            x = dEntity.Transform.position.x + dEntity.BoxOffset.x,
                            y = dEntity.Transform.position.y + dEntity.BoxOffset.y,
                            z = dEntity.Transform.position.z + dEntity.BoxOffset.z,
                            w = dEntity.AABB.w,
                            h = dEntity.AABB.h,
                            d = dEntity.AABB.d
                        };
                    }
                }

               
            }


         
            AddEachFrame = _addEntityQueue.Count;
            RemoveEachFrame = _removeEntityQueue.Count;


            if (_removeEntityQueue.Count > 0)
            {
                while (_removeEntityQueue.Count > 0)
                {
                    HandleRemoveDynamicEntity(_removeEntityQueue.Dequeue());
                }
            }


          
            // Return pools
            foreach (var octree in SpatialOctrees)
            {
                bool canClean = octree.Value.Cleanup();
                if (canClean)
                {
                    _treeRemovalKey.Enqueue(octree.Key);
                }
            }
            if (_treeRemovalKey.Count > 0)
            {
                while (_treeRemovalKey.Count > 0)
                {
                    var treeKey = _treeRemovalKey.Dequeue();
                    OctreeRootPool.Pool.Release(SpatialOctrees[treeKey]);
                    SpatialOctrees.Remove(treeKey);
                }
            }


            if (_addEntityQueue.Count > 0)
            {
                while (_addEntityQueue.Count > 0)
                {
                    HandleAddDynamicEntity(_addEntityQueue.Dequeue());
                }
            }


            UnityEngine.Profiling.Profiler.EndSample();
        }



        private void ResolveResolutionX(DynamicEntity dEntity, AABB broadPhase, float dTime)
        {
            _drawer.AddPhysicBounds(broadPhase, Color.black);
            Vector3Int minBP = _main.GetBlockGPos(new Vector3(broadPhase.x, broadPhase.y, broadPhase.z));
            Vector3Int maxBP = _main.GetBlockGPos(new Vector3(broadPhase.x + broadPhase.w, broadPhase.y + broadPhase.h, broadPhase.z + broadPhase.d));
            int axis;
            int nearestAxis = -1;
            float nearestCollisionTimeX = 1;
            float remainingTimeX;
            float normalX = -999;
            float normalY = -999;
            float normalZ = -999;


            for (int y = minBP.y; y <= maxBP.y; y++)
            {
                for (int z = minBP.z; z <= maxBP.z; z++)
                {
                    for (int x = minBP.x; x <= maxBP.x; x++)
                    {
                        BlockType currBlock = _main.GetBlock(new Vector3(x, y, z));
                        AABB bound = GetBlockBound(new Vector3(x, y, z));
                        if (currBlock.IsSolidVoxel() || currBlock.IsTransparentVoxel() && z < (dEntity.AABB.z + dEntity.AABB.d))
                        {
                            if (dEntity.AABB.Intersect(bound))
                            {
                                float overlapX = Mathf.Max(0, Mathf.Min(dEntity.AABB.Max.x, bound.Max.x) - Mathf.Max(dEntity.AABB.x, bound.x));
                                float direction = dEntity.AABB.x < bound.x ? 1 : -1;

                                if (direction == -1)
                                {
                                    dEntity.Position.x += overlapX;
                                }
                                else
                                {
                                    dEntity.Position.x -= overlapX;
                                }

                                return;
                            }

                            axis = AABBExtensions.SweepTest(dEntity.AABB, bound, new Vector3(dEntity.Velocity.x, 0, 0) * dTime, out float t,
                                out normalX, out normalY, out normalZ);

                            if (axis == -1 || t >= nearestCollisionTimeX) continue;

                            nearestAxis = axis;
                            nearestCollisionTimeX = t;
                        }
                    }
                }
            }
            remainingTimeX = 1.0f - nearestCollisionTimeX;
            if (remainingTimeX > 0.0f)
            {
                dEntity.Position.x += dEntity.Velocity.x * (nearestCollisionTimeX - 1e-1f) * dTime;
            }
            else
            {
                dEntity.Position.x += dEntity.Velocity.x * nearestCollisionTimeX * dTime;
            }
        }
        private void ResolveResolutionZ(DynamicEntity dEntity, AABB broadPhase, float dTime)
        {
            _drawer.AddPhysicBounds(broadPhase, Color.black);
            Vector3Int minBP = _main.GetBlockGPos(new Vector3(broadPhase.x, broadPhase.y, broadPhase.z));
            Vector3Int maxBP = _main.GetBlockGPos(new Vector3(broadPhase.x + broadPhase.w, broadPhase.y + broadPhase.h, broadPhase.z + broadPhase.d));
            int axis;
            int nearestAxis = -1;
            float nearestCollisionTimeZ = 1;
            float remainingTimeZ;
            float normalX = -999;
            float normalY = -999;
            float normalZ = -999;


            for (int y = minBP.y; y <= maxBP.y; y++)
            {
                for (int z = minBP.z; z <= maxBP.z; z++)
                {
                    for (int x = minBP.x; x <= maxBP.x; x++)
                    {
                        BlockType currBlock = _main.GetBlock(new Vector3(x, y, z));
                        AABB bound = GetBlockBound(new Vector3(x, y, z));
                        if (currBlock.IsSolidVoxel() || currBlock.IsTransparentVoxel() && x < (dEntity.AABB.x + dEntity.AABB.w))
                        {
                            //if (dEntity.AABB.Intersect(bound))
                            //{
                            //    float overlapX = Mathf.Max(0, Mathf.Min(dEntity.AABB.Max.z, bound.Max.z) - Mathf.Max(dEntity.AABB.z, bound.z));
                            //    float direction = dEntity.AABB.z < bound.z ? 1 : -1;

                            //    if (direction == -1)
                            //    {
                            //        dEntity.Position.z += overlapX;
                            //    }
                            //    else
                            //    {
                            //        dEntity.Position.z -= overlapX;
                            //    }

                            //    return;
                            //}


                            axis = AABBExtensions.SweepTest(dEntity.AABB, bound, new Vector3(0, 0, dEntity.Velocity.z) * dTime, out float t,
                                out normalX, out normalY, out normalZ);

                            if (axis == -1 || t >= nearestCollisionTimeZ) continue;
                            nearestAxis = axis;
                            nearestCollisionTimeZ = t;
                        }
                    }
                }
            }
            remainingTimeZ = 1.0f - nearestCollisionTimeZ;
            if (remainingTimeZ > 0.0f)
            {
                dEntity.Position.z += dEntity.Velocity.z * (nearestCollisionTimeZ - 1e-1f) * dTime;
            }
            else
            {
                dEntity.Position.z += dEntity.Velocity.z * nearestCollisionTimeZ * dTime;
            }
        }

        private void ResolveResolutionY(DynamicEntity dEntity, AABB broadPhase, float dTime)
        {
            _drawer.AddPhysicBounds(broadPhase, Color.black);
            Vector3Int minBP = _main.GetBlockGPos(new Vector3(broadPhase.x, broadPhase.y, broadPhase.z));
            Vector3Int maxBP = _main.GetBlockGPos(new Vector3(broadPhase.x + broadPhase.w, broadPhase.y + broadPhase.h, broadPhase.z + broadPhase.d));
            int axis;
            int nearestAxis = -1;
            float nearestCollisionTimeY = 1;
            float remainingTimeY;
            float normalX = -999;
            float normalY = -999;
            float normalZ = -999;

            int yResolution = -999;

            for (int y = minBP.y; y <= maxBP.y; y++)
            {
                for (int z = minBP.z; z <= maxBP.z; z++)
                {
                    for (int x = minBP.x; x <= maxBP.x; x++)
                    {
                        BlockType currBlock = _main.GetBlock(new Vector3(x, y, z));
                        AABB bound = GetBlockBound(new Vector3(x, y, z));
                        if (currBlock.IsSolidVoxel() || currBlock.IsTransparentVoxel())
                        {
                            if (dEntity.AABB.Intersect(bound) && yResolution != 1)
                            {
                                float overlapY = Mathf.Max(0, Mathf.Min(dEntity.AABB.Max.y, bound.Max.y) - Mathf.Max(dEntity.AABB.y, bound.y));

                                float direction = dEntity.AABB.y < bound.y ? 1 : -1;

                                if (direction == -1)
                                {
                                    dEntity.Position.y += overlapY;
                                    dEntity.OnGround = true;
                                    dEntity.SetVelocityY(0);

                                    yResolution = -1;
                                    return;

                                }
                                else
                                {
                                    dEntity.Position.y -= overlapY;
                                    dEntity.OnGround = false;

                                    yResolution = 1;
                                }
                            }


                            axis = AABBExtensions.SweepTest(dEntity.AABB, bound, new Vector3(0, dEntity.Velocity.y, 0) * dTime, out float t,
                                out normalX, out normalY, out normalZ);

                            if (axis == -1 || t >= nearestCollisionTimeY) continue;

                            nearestCollisionTimeY = t;
                            nearestAxis = axis;
                        }
                        else if (currBlock == BlockType.Water)
                        {
                            AABBExtensions.AABBOverlapVolumnCheck(dEntity.AABB, bound, out float w, out float h, out float d);
                            //dEntity.AddVelocityY(2f * dEntity.Mass * h * Time.deltaTime);
                            dEntity.AddVelocityY(2f * dEntity.Mass * _physicWaterFloatingCurve.Evaluate(h) * dTime);

                        }
                    }
                }
            }

            remainingTimeY = 1.0f - nearestCollisionTimeY;
            if (remainingTimeY > 0.0f)
            {
                dEntity.Position.y += dEntity.Velocity.y * (nearestCollisionTimeY) * dTime;
                if (nearestAxis == 1)
                {
                    dEntity.Position.y = Mathf.FloorToInt(dEntity.Position.y);

                    if (normalY == 1)
                    {
                        dEntity.OnGround = true;
                        dEntity.SetVelocityY(0);
                    }
                }
            }
            else
            {
                dEntity.Position.y += dEntity.Velocity.y * nearestCollisionTimeY * dTime;
                dEntity.OnGround = false;
            }
        }





        private void ApplyGravity(DynamicEntity entity, float collisionTime)
        {
            //entity.Transform.position += new Vector3(_gravity.x, _gravity.y, _gravity.z) * collisionTime;
            entity.Transform.position += new Vector3(entity.Velocity.x, entity.Velocity.y, entity.Velocity.z) * collisionTime;
            //Debug.Log($"{collisionTime} {entity.Velocity}");
        }

        private void HandleAddDynamicEntity(DynamicEntity entity)
        {
            //_dynamicEntities.Add(entity);  
            Vector3Int octreeFrame = GetSpatialFrame(entity.Transform.position);
            if (SpatialOctrees.TryGetValue(octreeFrame, out Octree octree))
            {
                bool canInsert = octree.Insert(entity);
            }
            else
            {
                AABB octreeBounds = GetOctreeBounds(octreeFrame);
                //Octree newOctree = new Octree();
                Octree newOctree = OctreeRootPool.Pool.Get();
                newOctree.Init(octreeBounds, OCTREE_CAPACITY, level: 0);
                SpatialOctrees.Add(octreeFrame, newOctree);
                bool canInsert = newOctree.Insert(entity);
            }
        }

        private void HandleRemoveDynamicEntity(DynamicEntity entity)
        {

            var root = entity.Root;
            if (root != null)
            {
                root.Remove(entity);
            }
            else
            {
                Debug.Log($"HandleRemoveDynamicEntity       Node = null");
            }

        }

        public void AddDynamicEntity(DynamicEntity entity)
        {
            _addEntityQueue.Enqueue(entity);
        }

        public void RemoveDynamicEntity(DynamicEntity entity, System.Action afterRemoveCallback = null)
        {
            _removeEntityQueue.Enqueue(entity);
            Destroy(entity.Transform.gameObject);
        }




        public int OverlapBox(Vector3 center, Vector3 halfExtents, ref List<DynamicEntity> entities)
        {
            //_overlapBoxOctrees.Clear();

            AABB bounds = new AABB(center.x - halfExtents.x, center.y - halfExtents.y, center.z - halfExtents.z,
                                   halfExtents.x * 2, halfExtents.y * 2, halfExtents.z * 2);

            _corners[0] = new Vector3(bounds.x, bounds.y, bounds.z);
            _corners[1] = new Vector3(bounds.Max.x, bounds.y, bounds.z);
            _corners[2] = new Vector3(bounds.x, bounds.Max.y, bounds.z);
            _corners[3] = new Vector3(bounds.Max.x, bounds.Max.y, bounds.z);
            _corners[4] = new Vector3(bounds.x, bounds.y, bounds.Max.z);
            _corners[5] = new Vector3(bounds.Max.x, bounds.y, bounds.Max.z);
            _corners[6] = new Vector3(bounds.x, bounds.Max.y, bounds.Max.z);
            _corners[7] = new Vector3(bounds.Max.x, bounds.Max.y, bounds.Max.z);


            for (int i = 0; i < _corners.Length; i++)
            {
                Vector3Int octreeGridFrame = GetSpatialFrame(_corners[i]);
                if (SpatialOctrees.TryGetValue(octreeGridFrame, out Octree octree))
                {
                    if (!_overlapBoxOctrees.Contains(octree))
                    {
                        _overlapBoxOctrees.Add(octree);
                    }
                }
            }

            for (int i = 0; i < _overlapBoxOctrees.Count; i++)
            {
                //_overlapEntitiesFound = _overlapBoxOctrees[i].Query(bounds);
                _overlapBoxOctrees[i].Query(bounds, ref entities);
            }
            return entities.Count;
        }

        public int OverlapBoxNonAlloc(Vector3 center, Vector3 halfExtents, DynamicEntity[] entities, int mask)
        {
            _overlapEntitiesFound.Clear();
            _overlapBoxOctrees.Clear();

            AABB bounds = new AABB(center.x - halfExtents.x, center.y - halfExtents.y, center.z - halfExtents.z,
                                   halfExtents.x * 2, halfExtents.y * 2, halfExtents.z * 2);

            _corners[0] = new Vector3(bounds.x, bounds.y, bounds.z);
            _corners[1] = new Vector3(bounds.Max.x, bounds.y, bounds.z);
            _corners[2] = new Vector3(bounds.x, bounds.Max.y, bounds.z);
            _corners[3] = new Vector3(bounds.Max.x, bounds.Max.y, bounds.z);
            _corners[4] = new Vector3(bounds.x, bounds.y, bounds.Max.z);
            _corners[5] = new Vector3(bounds.Max.x, bounds.y, bounds.Max.z);
            _corners[6] = new Vector3(bounds.x, bounds.Max.y, bounds.Max.z);
            _corners[7] = new Vector3(bounds.Max.x, bounds.Max.y, bounds.Max.z);


            for (int i = 0; i < _corners.Length; i++)
            {
                Vector3Int octreeGridFrame = GetSpatialFrame(_corners[i]);
                if (SpatialOctrees.TryGetValue(octreeGridFrame, out Octree octree))
                {
                    if (!_overlapBoxOctrees.Contains(octree))
                    {
                        _overlapBoxOctrees.Add(octree);
                    }
                }
            }

            for (int i = 0; i < _overlapBoxOctrees.Count; i++)
            {
                _overlapBoxOctrees[i].Query(bounds, ref _overlapEntitiesFound, entities.Length);
            }

            int actualSize = Mathf.Min(_overlapEntitiesFound.Count, entities.Length);
            if (actualSize > 0)
            {
                for (int i = 0; i < actualSize; i++)
                {
                    entities[i] = _overlapEntitiesFound[i];
                }
            }
            return actualSize;
        }

        private int GetAllEntitiesInOctreeNonAlloc(Vector3Int octreeFrame, ref List<DynamicEntity> entities)
        {
            var octree = SpatialOctrees[octreeFrame];
            //octree.Query(GetOctreeBounds(octreeFrame), ref entities);
            for (int i = 0; i < octree.AllEntities.Count; i++)
            {
                entities.Add(octree.AllEntities[i]);
            }


            return entities.Count;
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




        #region Girds
        public Vector3Int GetSpatialFrame(Vector3 globalPosition)
        {
            Vector3Int gridFrame = new Vector3Int(Mathf.FloorToInt(globalPosition.x / OctreeDimensions.x),
                                                  Mathf.FloorToInt(globalPosition.y / OctreeDimensions.y),
                                                  Mathf.FloorToInt(globalPosition.z / OctreeDimensions.z));

            return gridFrame;
        }

        private AABB GetOctreeBounds(Vector3Int octreeFrame)
        {
            return new AABB(octreeFrame.x * OctreeDimensions.x,
                             octreeFrame.y * OctreeDimensions.y,
                             octreeFrame.z * OctreeDimensions.z,
                             OctreeDimensions.x,
                             OctreeDimensions.y,
                             OctreeDimensions.z);


        }

        private Vector3Int[] GetAdjacentNeighborsNonAlloc(Vector3Int position, Vector3Int[] neighborPosition)
        {
            int index = 0;

            for (int xOffset = -1; xOffset <= 1; xOffset++)
            {
                for (int yOffset = -1; yOffset <= 1; yOffset++)
                {
                    for (int zOffset = -1; zOffset <= 1; zOffset++)
                    {
                        // Skip the case where all offsets are zero (current position)
                        if (xOffset == 0 && yOffset == 0 && zOffset == 0)
                            continue;

                        neighborPosition[index] = position + new Vector3Int(xOffset, yOffset, zOffset);
                        index++;
                    }
                }
            }

            return neighborPosition;
        }

        #endregion

        private void OnApplicationQuit()
        {
#if UNITY_EDITOR
            _collisionTimes.Clear();
            _bounds.Clear();
            //_dynamicEntities.Clear();
#endif
        }
    }
}
