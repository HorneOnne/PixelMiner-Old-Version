using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelMiner.DataStructure
{


    public class PhysicEntityOctree
    {
        public AABB Bound;
        public int Capacity;
        public PhysicEntityOctree[] Neighbors;
        private bool _divided = false;

        public HashSet<DynamicEntity> Entities;
        //public List<DynamicEntity> Entities;

        private const int MAX_LEVEL = 5;
        private int _level;
        public int Level { get => _level; }

        public PhysicEntityOctree()
        {
            Entities = new HashSet<DynamicEntity>();
            //Entities = new List<DynamicEntity>();
            Neighbors = new PhysicEntityOctree[8];
        }

        public void Init(AABB bound, int capacity, int level)
        {
            Bound = bound;
            Capacity = capacity;     
            this._level = level;

            if (_level > MAX_LEVEL)
            {
                Debug.Log($"{_level} \t {MAX_LEVEL}");
                throw new System.ArgumentOutOfRangeException();
            }
        }


        public bool Insert(DynamicEntity entity)
        {
            if (!this.Bound.Contains(entity.Transform.position))
            {
                return false;
            }

            if (this.Entities.Count < this.Capacity || _level == MAX_LEVEL)
            {
                Entities.Add(entity);
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
                        return true;
                    }
                }
            }
            return false;
        }


        public bool Remove(DynamicEntity entity)
        {
            if (Entities.Contains(entity))
            {
                Entities.Remove(entity);
                return true;
            }

            if (_divided)
            {
                for (int i = 0; i < Neighbors.Length; i++)
                {
                    if (Neighbors[i].Remove(entity))
                    {
                        return true;
                    }
                }
            }
     
            return false;

        }

        public void Subdivide()
        {
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

            int nextLevel = _level+1;

            Neighbors[0] = PhysicEntityOctreePool.Pool.Get();
            Neighbors[0].Init(dsw, this.Capacity, nextLevel);

            Neighbors[1] = PhysicEntityOctreePool.Pool.Get();
            Neighbors[1].Init(dse, this.Capacity, nextLevel);

            Neighbors[2] = PhysicEntityOctreePool.Pool.Get();
            Neighbors[2].Init(dnw, this.Capacity, nextLevel);

            Neighbors[3] = PhysicEntityOctreePool.Pool.Get();
            Neighbors[3].Init(dne, this.Capacity, nextLevel);

            Neighbors[4] = PhysicEntityOctreePool.Pool.Get();
            Neighbors[4].Init(usw, this.Capacity, nextLevel);

            Neighbors[5] = PhysicEntityOctreePool.Pool.Get();
            Neighbors[5].Init(use, this.Capacity, nextLevel);

            Neighbors[6] = PhysicEntityOctreePool.Pool.Get();
            Neighbors[6].Init(unw, this.Capacity, nextLevel);

            Neighbors[7] = PhysicEntityOctreePool.Pool.Get();
            Neighbors[7].Init(une, this.Capacity, nextLevel);
        }


        public void TraverseRecursive(System.Action<AABB> callback)
        {
            for (int i = 0; i < Neighbors.Length; i++)
            {
                if (Neighbors[i] != null)
                {
                    Neighbors[i].TraverseRecursive(callback);
                }
            }
            callback?.Invoke(Bound);
        }
        public void TraverseRecursive(System.Action<PhysicEntityOctree> callback)
        {
           

            for (int i = 0; i < Neighbors.Length; i++)
            {
                if (Neighbors[i] != null)
                {
                    Neighbors[i].TraverseRecursive(callback);
                }
            }
            callback?.Invoke(this);
        }


        public List<DynamicEntity> Query(AABB queryBound)
        {
            List<DynamicEntity> l = new List<DynamicEntity>();
            if (this.Bound.Intersect(queryBound))
            {
                foreach (var e in this.Entities)
                {
                    if (queryBound.Contains(e.Transform.position))
                    {
                        l.Add(e);
                    }
                }
                if (_divided)
                {
                    for (int i = 0; i < Neighbors.Length; i++)
                    {
                        l.AddRange(Neighbors[i].Query(queryBound));
                    }
                }
            }

            return l;
        }

        public void QueryNonAlloc(AABB queryBound, ref List<DynamicEntity> entities)
        {
            if (this.Bound.Intersect(queryBound))
            {
                foreach (var e in this.Entities)
                {
                    if (queryBound.Contains(e.Transform.position))
                    {
                        entities.Add(e);
                    }
                }
                if (_divided)
                {
                    for (int i = 0; i < Neighbors.Length; i++)
                    {
                        Neighbors[i].QueryNonAlloc(queryBound, ref entities);
                    }
                }
            }
        }
        public void QueryNonAlloc(AABB queryBound, ref List<DynamicEntity> entities, LayerMask layer)
        {
            if (this.Bound.Intersect(queryBound))
            {
                foreach (var e in this.Entities)
                {
                    if (queryBound.Contains(e.Transform.position) && (layer & e.PhysicLayer) != 0)
                    {
                        entities.Add(e);
                    }
                }
                if (_divided)
                {
                    for (int i = 0; i < Neighbors.Length; i++)
                    {
                        Neighbors[i].QueryNonAlloc(queryBound, ref entities,layer);
                    }
                }
            }
        }

        public void Clear()
        {
            Entities.Clear();
            _divided = false;
            System.Array.Clear(Neighbors, 0, Neighbors.Length); 
        }
    }
}
