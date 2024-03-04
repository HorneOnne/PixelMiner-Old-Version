using UnityEngine;
using System.Collections.Generic;

namespace PixelMiner.DataStructure
{
    public class  OctreeNode
    {
        [HideInInspector] public AABB Bound;
        public int Capacity;
        public OctreeNode[] Neighbors;
        public List<DynamicEntity> Entities;
        private int _level;
        private bool _divided;
        private Octree _root;


        public OctreeNode(AABB bound, int capacity, int level, Octree root)
        {
            this.Bound  = bound;
            this.Capacity = capacity;
            this._level = level;
            this._root = root;
            this._divided = false;
            Entities = new List<DynamicEntity>();
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
                    entity.Node = this;
                    Entities.Add(entity);

                    if(Entities.Count == 0)
                    {
                        Debug.Log("Why 0?");
                    }
                    else if(Entities.Count == 1)
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
            //Debug.Log($"Remove node index: {entity.EntityNodeIndex}");
            //Entities.Remove(entity);
            //return;

            int entityLastIndex;
            if (Entities.Count > 0)
            {
                entityLastIndex = Entities.Count - 1;
                Entities[entityLastIndex].EntityNodeIndex = entity.EntityNodeIndex;
            }    
            //else
            //{
            //    entityLastIndex = 0;
            //}
            Entities.RemoveAtUnordered(entity.EntityNodeIndex);

           // if (entity.EntityNodeIndex == Entities.Count)
           // {
           //     Debug.LogWarning($"Break remove node: {entity.EntityNodeIndex}   max: {Entities.Count}");
           //     Debug.Break();  
           // }

           // try
           // {
           //     Entities.RemoveAtUnordered(entity.EntityNodeIndex);
           // }
           //catch
           // {
           //     Debug.LogWarning($"Error remove node: {entity.EntityNodeIndex}   max: {Entities.Count}");
           // }
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
            //Debug.Log($"Subdivide node: {nextLevel}");
            Neighbors[0] = new OctreeNode(dsw, this.Capacity, nextLevel, _root); 
            Neighbors[1] = new OctreeNode(dse, this.Capacity, nextLevel, _root); 
            Neighbors[2] = new OctreeNode(dnw, this.Capacity, nextLevel, _root); 
            Neighbors[3] = new OctreeNode(dne, this.Capacity, nextLevel, _root); 

            Neighbors[4] = new OctreeNode(usw, this.Capacity, nextLevel, _root); 
            Neighbors[5] = new OctreeNode(use, this.Capacity, nextLevel, _root); 
            Neighbors[6] = new OctreeNode(unw, this.Capacity, nextLevel, _root); 
            Neighbors[7] = new OctreeNode(une, this.Capacity, nextLevel, _root); 
        }



        //public void Query(AABB queryBound, ref List<DynamicEntity> entities)
        //{
        //    if (this.Bound.Intersect(queryBound))
        //    {
        //        if (!_divided)
        //        {
        //            for(int i = 0; i < this.EntityIndices.Count; i++)
        //            {
        //                if (queryBound.Contains(_root.Entities[EntityIndices[i]].Transform.position))
        //                {
        //                    entities.Add(_root.Entities[EntityIndices[i]]);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            for (int i = 0; i < Neighbors.Length; i++)
        //            {
        //                Neighbors[i].Query(queryBound, ref entities);
        //            }
        //        }
        //    }
        //}

        public void TraverseRecursive(System.Action<AABB> callback)
        {
            if(_divided)
            {
                for (int i = 0; i < Neighbors.Length; i++)
                {
                    Neighbors[i].TraverseRecursive(callback);
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
        //            Neighbors[i].TraverseRecursive(callback);
        //        }
        //    }

        //    callback?.Invoke();
        //}


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
                _divided = false;
                Neighbors = null;
                return true;
            }

            return false;
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
