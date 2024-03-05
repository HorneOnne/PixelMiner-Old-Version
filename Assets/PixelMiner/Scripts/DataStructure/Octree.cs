using System.Collections.Generic;
using UnityEngine;

namespace PixelMiner.DataStructure
{
    public class Octree
    {
        public const int MAX_LEVEL = 2;

        public AABB Bound;
        public int Capacity;
        public OctreeLeave[] Neighbors;
        private bool _divided;
        private int _level;
        public List<DynamicEntity> AllEntities;
        private List<DynamicEntity> EntityInRoot;

        private Color _boundsColor = Color.blue;

        public bool Divided { get => _divided; }
        public Octree()
        {
            AllEntities = new List<DynamicEntity>();
            EntityInRoot = new List<DynamicEntity>();
        }

        public void Init(AABB bound, int capacity, int level)
        {

            this.Bound = bound;
            this.Capacity = capacity;
            this._level = level;
            this._divided = false;
        }

        public bool Insert(DynamicEntity entity)
        {
            if (!this.Bound.Contains(entity.Transform.position))
            {
                //Debug.Log("Not contain");
                return false;
            }

            entity.Root = this;
            if (this.AllEntities.Count < this.Capacity || _level == MAX_LEVEL)
            {
                AllEntities.Add(entity);             
                entity.EntitiesIndex = AllEntities.Count - 1;
                entity.EntityNodeIndex = -1;
                entity.Leave = null;


                EntityInRoot.Add(entity);
                entity.EntityRootIndex = EntityInRoot.Count - 1;
                return true;
            }
            else
            {
                if (!_divided)
                {
                    _divided = true;
                    Subdivide();
                }

                for (int i = 0; i < Neighbors.Length; i++)
                {
                    if (Neighbors[i].Insert(entity))
                    {
                        AllEntities.Add(entity);
                        entity.EntitiesIndex = AllEntities.Count - 1;
                        entity.EntityRootIndex = -1;
                        return true;
                    }
                }
            }
            return false;
        }


        public void Remove(DynamicEntity entity)
        {
            int entityLastIndex = AllEntities.Count - 1;
            AllEntities[entityLastIndex].EntitiesIndex = entity.EntitiesIndex;

            if (_divided)
            {
                if (entity.Leave != null)
                {
                    entity.Leave.Remove(entity);
                }
            }
            AllEntities.RemoveAtUnordered(entity.EntitiesIndex);


            if (entity.EntityRootIndex != -1)
            {
                int lastRootIndex = EntityInRoot.Count - 1;
                EntityInRoot[lastRootIndex].EntityRootIndex = entity.EntityRootIndex;
                EntityInRoot.RemoveAtUnordered(entity.EntityRootIndex);
            }
        }

        public void Query(AABB queryBound, ref List<DynamicEntity> entities, int maxSize = int.MaxValue)
        {
            if (this.Bound.Intersect(queryBound))
            {
                for (int i = 0; i < EntityInRoot.Count; i++)
                {
                    if (queryBound.Contains(EntityInRoot[i].Transform.position))
                    {
                        if(entities.Count < maxSize)
                        {
                            entities.Add(EntityInRoot[i]);
                        }     
                        else
                        {
                            return;
                        }
                    }     
                }

                if (_divided)
                {
                    for (int i = 0; i < Neighbors.Length; i++)
                    {
                        Neighbors[i].Query(queryBound, ref entities, maxSize);
                    }
                }
            }
        }


        public void Subdivide()
        {
            Neighbors = new OctreeLeave[8];
            float halfWidth = this.Bound.w / 2.0f;
            float halfHeight = this.Bound.h / 2.0f;
            float halfDepth = this.Bound.d / 2.0f;

            AABB dsw = new AABB(Bound.x, Bound.y, Bound.z, halfWidth, halfHeight, halfDepth);
            AABB dse = new AABB(Bound.x + halfWidth, Bound.y, Bound.z, halfWidth, halfHeight, halfDepth);
            AABB dnw = new AABB(Bound.x, Bound.y, Bound.z + halfDepth, halfWidth, halfHeight, halfDepth);
            AABB dne = new AABB(Bound.x + halfWidth, Bound.y, Bound.z + halfDepth, halfWidth, halfHeight, halfDepth);

            AABB usw = new AABB(Bound.x, Bound.y + halfHeight, Bound.z, halfWidth, halfHeight, halfDepth);
            AABB use = new AABB(Bound.x + halfWidth, Bound.y + halfHeight, Bound.z, halfWidth, halfHeight, halfDepth);
            AABB unw = new AABB(Bound.x, Bound.y + halfHeight, Bound.z + halfDepth, halfWidth, halfHeight, halfDepth);
            AABB une = new AABB(Bound.x + halfWidth, Bound.y + halfHeight, Bound.z + halfDepth, halfWidth, halfHeight, halfDepth);

            int nextLevel = _level + 1;

            Neighbors[0] = OctreeLeavePool.Pool.Get();
            Neighbors[0].Init(dsw, this.Capacity, nextLevel, this);

            Neighbors[1] = OctreeLeavePool.Pool.Get();
            Neighbors[1].Init(dse, this.Capacity, nextLevel, this);

            Neighbors[2] = OctreeLeavePool.Pool.Get();
            Neighbors[2].Init(dnw, this.Capacity, nextLevel, this);

            Neighbors[3] = OctreeLeavePool.Pool.Get();
            Neighbors[3].Init(dne, this.Capacity, nextLevel, this);

            Neighbors[4] = OctreeLeavePool.Pool.Get();
            Neighbors[4].Init(usw, this.Capacity, nextLevel, this);

            Neighbors[5] = OctreeLeavePool.Pool.Get();
            Neighbors[5].Init(use, this.Capacity, nextLevel, this);

            Neighbors[6] = OctreeLeavePool.Pool.Get();
            Neighbors[6].Init(unw, this.Capacity, nextLevel, this);

            Neighbors[7] = OctreeLeavePool.Pool.Get();
            Neighbors[7].Init(une, this.Capacity, nextLevel, this);
        }

        public void TraverseRecursive(System.Action<AABB, Color> callback)
        {
            if (_divided)
            {
                for (int i = 0; i < Neighbors.Length; i++)
                {
                    if (Neighbors[i] != null)
                    {
                        Neighbors[i].TraverseRecursive(callback);
                    }
                }
            }
            else
            {
                callback?.Invoke(Bound, _boundsColor);
            }               
        }
        public void TraverseRecursive(System.Action callback)
        {
            if (_divided)
            {
                for (int i = 0; i < Neighbors.Length; i++)
                {
                    if (Neighbors[i] != null)
                    {
                        Neighbors[i].TraverseRecursive(callback);
                    }
                }
            }

            callback?.Invoke();
        }


        public bool Cleanup()
        {
            int neighborEmpty = 0;
            if (_divided)
            {
                for (int i = 0; i < Neighbors.Length; i++)
                {
                    if (Neighbors[i] != null)
                    {
                        if (Neighbors[i].Cleanup())
                        {
                            neighborEmpty++;
                        }
                    }
                }
            }

            if (_divided && neighborEmpty == 8)
            {
                _divided = false;
                for(int i = 0; i < Neighbors.Length; i++)
                {
                    OctreeLeavePool.Pool.Release(Neighbors[i]);
                }
                Neighbors = null;
            }

            if (AllEntities.Count == 0)
            {
                _divided = false;
                if(Neighbors != null)
                {
                    for (int i = 0; i < Neighbors.Length; i++)
                    {
                        OctreeLeavePool.Pool.Release(Neighbors[i]);
                    }
                    Neighbors = null;
                }
               
                return true;
            }


            return false;
        }





        public void Clear()
        {
            AllEntities.Clear();
            EntityInRoot.Clear();
            _divided = false;
        }
    }
}
