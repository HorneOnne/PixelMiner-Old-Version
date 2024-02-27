using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelMiner.DataStructure
{
    public class Octree<T> where T : DynamicEntity
    {
        public AABB Bound;
        public int Capacity;
        public Octree<T>[] Neighbors;
        private bool _divided = false;
        //public List<T> Entities;
        public HashSet<T> Entities;

        public Octree(AABB bound, int capacity)
        {
            Bound = bound;
            Capacity = capacity;
            //Entities = new List<T>();
            Entities = new HashSet<T>();
            Neighbors = new Octree<T>[8];
        }


        public bool Insert(T entity)
        {
            if (!this.Bound.Contains(entity.Transform.position))
            {
                return false;
            }

            if (this.Entities.Count < this.Capacity)
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


        public bool Remove(T entity)
        {
            if (!this.Bound.Contains(entity.Transform.position))
            {
                return false;
            }
            //if (Entities.Contains(entity))
            //{
            //    Debug.Log("0");
            //    Entities.Remove(entity);
            //    return true;
            //}

            if (!_divided)
            {
                if (Entities.Contains(entity))
                {
                    Entities.Remove(entity);
                    return true;
                }
            }
            else
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


            Neighbors[0] = new Octree<T>(dsw, this.Capacity);
            Neighbors[1] = new Octree<T>(dse, this.Capacity);
            Neighbors[2] = new Octree<T>(dnw, this.Capacity);
            Neighbors[3] = new Octree<T>(dne, this.Capacity);

            Neighbors[4] = new Octree<T>(usw, this.Capacity);
            Neighbors[5] = new Octree<T>(use, this.Capacity);
            Neighbors[6] = new Octree<T>(unw, this.Capacity);
            Neighbors[7] = new Octree<T>(une, this.Capacity);
        }


        public void TraverseRecursive(System.Action<AABB> callback)
        {
            callback?.Invoke(Bound);

            for (int i = 0; i < Neighbors.Length; i++)
            {
                if (Neighbors[i] != null)
                {
                    Neighbors[i].TraverseRecursive(callback);
                }
            }
        }

        public List<T> Query(AABB queryBound)
        {
            //List<T> l = new List<T>();
            //if (this.Bound.Intersect(queryBound))
            //{
            //    for (int i = 0; i < this.Entities.Count; i++)
            //    {
            //        if (queryBound.Contains(Entities[i].Transform.position))
            //        {
            //            l.Add(Entities[i]);
            //        }
            //    }

            //    if (_divided)
            //    {
            //        for (int i = 0; i < Neighbors.Length; i++)
            //        {
            //            l.AddRange(Neighbors[i].Query(queryBound));
            //        }
            //    }
            //}

            //return l;

            List<T> l = new List<T>();
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
    }
}
