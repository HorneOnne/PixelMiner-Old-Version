using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelMiner.DataStructure
{
    public class Octree<T>
    {
        public AABB Bound;
        public int Capacity;
        public Octree<T>[] Neigbors;
        private bool _divided = false;
        public List<T> Objects;


        public bool Insert(T obj, Vector3 position)
        {
            if(!this.Bound.Contains(position))
            {
                return false;
            }

            if(this.Objects.Count < this.Capacity)
            {
                Objects.Add(obj);
            }
            else
            {
                if(!_divided)
                {
                    _divided = true;
                    Subdivide();
                }

                for (int i = 0; i < Neigbors.Length; i++)
                {
                    if (Neigbors[i].Insert(obj, position))
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

     
        }


    }
}
