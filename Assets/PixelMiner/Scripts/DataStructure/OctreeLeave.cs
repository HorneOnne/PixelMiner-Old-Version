using UnityEngine;
using System.Collections.Generic;

namespace PixelMiner.DataStructure
{
    public class  OctreeLeave
    {
        [HideInInspector] public AABB Bound;
        public int Capacity;
        public OctreeLeave[] Neighbors;
        public List<DynamicEntity> Entities;
        private int _level;
        private bool _divided;
        private Octree _root;

        private Color _boundsColor = Color.blue;

        public OctreeLeave() 
        {
            Entities = new List<DynamicEntity>();
        }

        public void Init(AABB bound, int capacity, int level, Octree root)
        {
            this.Bound = bound;
            this.Capacity = capacity;
            this._level = level;
            this._root = root;
            this._divided = false;    
        }


        public bool Insert(DynamicEntity entity)
        {
            if (!this.Bound.Contains(entity.Transform.position))
            {
                return false;
            }
            else
            {
                if (this.Entities.Count < this.Capacity || _level == Octree.MAX_LEVEL)
                {                
                    entity.Leave = this;
                    Entities.Add(entity);

                   if(Entities.Count == 1)
                    {
                        entity.EntityNodeIndex = 0;
                    }
                    else
                    {
                        entity.EntityNodeIndex = Entities.Count - 1;
                    }
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
            }

            return false;
        }

        public void Remove(DynamicEntity entity)
        {
            int entityLastIndex;
            if (Entities.Count > 0)
            {
                entityLastIndex = Entities.Count - 1;
                Entities[entityLastIndex].EntityNodeIndex = entity.EntityNodeIndex;
            }    

            Entities.RemoveAtUnordered(entity.EntityNodeIndex);
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
            Neighbors[0].Init(dsw, this.Capacity, nextLevel, _root);

            Neighbors[1] = OctreeLeavePool.Pool.Get();
            Neighbors[1].Init(dse, this.Capacity, nextLevel, _root);

            Neighbors[2] = OctreeLeavePool.Pool.Get();
            Neighbors[2].Init(dnw, this.Capacity, nextLevel, _root);

            Neighbors[3] = OctreeLeavePool.Pool.Get();
            Neighbors[3].Init(dne, this.Capacity, nextLevel, _root);

            Neighbors[4] = OctreeLeavePool.Pool.Get();
            Neighbors[4].Init(usw, this.Capacity, nextLevel, _root);

            Neighbors[5] = OctreeLeavePool.Pool.Get();
            Neighbors[5].Init(use, this.Capacity, nextLevel, _root);

            Neighbors[6] = OctreeLeavePool.Pool.Get();
            Neighbors[6].Init(unw, this.Capacity, nextLevel, _root);

            Neighbors[7] = OctreeLeavePool.Pool.Get();
            Neighbors[7].Init(une, this.Capacity, nextLevel, _root);
        }



        public void Query(AABB queryBound, ref List<DynamicEntity> entities, int maxSize = int.MaxValue)
        {
            if (this.Bound.Intersect(queryBound))
            {
                for (int i = 0; i < this.Entities.Count; i++)
                {
                    if (queryBound.Contains(Entities[i].Transform.position))
                    {
                        if(entities.Count < maxSize)
                        {
                            entities.Add(Entities[i]);
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



        public void TraverseRecursive(System.Action<AABB, Color> callback)
        {
            if(_divided)
            {
                for (int i = 0; i < Neighbors.Length; i++)
                {
                    Neighbors[i].TraverseRecursive(callback);

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
                    Neighbors[i].TraverseRecursive(callback);

                }
            }
            callback?.Invoke();
        }





        public bool Cleanup()
        {
            if (_divided)
            {
                for (int i = 0; i < Neighbors.Length; i++)
                {
                    if (Neighbors[i] != null)
                    {
                        Neighbors[i].Cleanup();
                    }
                }
            }

            if (Entities.Count == 0)
            {
                if(_divided)
                {
                    for (int i = 0; i < Neighbors.Length; i++)
                    {
                        OctreeLeavePool.Pool.Release(Neighbors[i]);
                    }
                    Neighbors = null;
                    _divided = false;  
                }
            
                return true;
            }

            return false;
        }



        public void Clear()
        {
            Entities.Clear();
            _divided = false;
        }
    }

    public static class ListExtensions
    {
        public static void RemoveAtUnordered<T>(this List<T> list, int index)
        {
            int lastElementIndex = list.Count - 1;
            var tempt = list[index];
            list[index] = list[lastElementIndex];
            list[lastElementIndex] = tempt;
            list.RemoveAt(lastElementIndex);
        }
    }
}
