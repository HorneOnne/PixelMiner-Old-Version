using System.Collections.Generic;
using UnityEngine;

namespace PixelMiner.DataStructure
{
    public class Octree
    {
        public const int MAX_LEVEL = 3;

        public AABB Bound;
        public int Capacity;
        public OctreeNode[] Neighbors;
        private bool _divided;
        private int _level;
        public List<DynamicEntity> Entities;


        public Octree(AABB bound, int capacity, int level)
        {
            this.Bound = bound;
            this.Capacity = capacity;
            this._level = level;
            this._divided = false;
            Entities = new List<DynamicEntity>();
        }

        //public void Init(AABB bound, int capacity, int level)
        //{
           
        //}

        public bool Insert(DynamicEntity entity)
        {
            if (!this.Bound.Contains(entity.Transform.position))
            {
                Debug.Log("Not contain");
                return false;
            }

            entity.Root = this;
            if (this.Entities.Count < this.Capacity || _level == MAX_LEVEL)
            {
                Entities.Add(entity);
                entity.EntityRootIndex = Entities.Count - 1;
                entity.EntityNodeIndex = -1;
                entity.Node = null;
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
                        Entities.Add(entity);
                        entity.EntityRootIndex = Entities.Count - 1;
                        return true;
                    }
                }
            }
            return false;
        }


        public void Remove(DynamicEntity entity)
        {
            int entityLastIndex = Entities.Count - 1;
            Entities[entityLastIndex].EntityRootIndex = entity.EntityRootIndex;
           
            if (_divided)
            {
                if (entity.Node != null)
                {
                    entity.Node.Remove(entity);
                }
            }

            //Entities.Remove(entity);
            Entities.RemoveAtUnordered(entity.EntityRootIndex);
        }

        public void Query(AABB queryBound, ref List<DynamicEntity> entities)
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

                //if (!_divided)
                //{
                //    foreach (var e in this.Entities)
                //    {
                //        if (queryBound.Contains(e.Transform.position))
                //        {
                //            entities.Add(e);
                //        }
                //    }
                //}
                //else
                //{
                //    for (int i = 0; i < Neighbors.Length; i++)
                //    {
                //        Neighbors[i].Query(queryBound, ref entities);
                //    }
                //}
            }
        }


        public void Subdivide()
        {
            Neighbors = new OctreeNode[8];
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
            Debug.Log($"Subdivide Root: {nextLevel}");

            Neighbors[0] = new OctreeNode(dsw, this.Capacity, nextLevel, this);
            Neighbors[1] = new OctreeNode(dse, this.Capacity, nextLevel, this);
            Neighbors[2] = new OctreeNode(dnw, this.Capacity, nextLevel, this);
            Neighbors[3] = new OctreeNode(dne, this.Capacity, nextLevel, this);

            Neighbors[4] = new OctreeNode(usw, this.Capacity, nextLevel, this);
            Neighbors[5] = new OctreeNode(use, this.Capacity, nextLevel, this);
            Neighbors[6] = new OctreeNode(unw, this.Capacity, nextLevel, this);
            Neighbors[7] = new OctreeNode(une, this.Capacity, nextLevel, this);
        }

        public void TraverseRecursive(System.Action<AABB> callback)
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

            callback?.Invoke(Bound);
        }

        //public void TraverseRecursive(System.Action callback)
        //{
        //    if (_divided)
        //    {
        //        for (int i = 0; i < Neighbors.Length; i++)
        //        {
        //            if (Neighbors[i] != null)
        //            {
        //                Neighbors[i].TraverseRecursive(callback);
        //            }
        //        }
        //    }

        //    callback?.Invoke();
        //}

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
                Neighbors = null;
            }

            if (Entities.Count == 0)
            {
                _divided = false;
                Neighbors = null;
                return true;
            }


            return false;
        }


        public void Clear()
        {
            Entities.Clear();
            _divided = false;
            System.Array.Clear(Neighbors, 0, Neighbors.Length);
        }
    }
}
