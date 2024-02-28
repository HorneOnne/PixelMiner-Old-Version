using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PixelMiner.DataStructure
{
    public struct AABB
    {
        public float x;
        public float y;
        public float z;
        public float w;
        public float h;
        public float d;

        public Vector3 Min { get => new Vector3(x,y,z); }
        public Vector3 Max { get => new Vector3(x + w,y + h,z + d); }
        public Vector3 Center { get => new Vector3(x + w / 2.0f, y + h / 2.0f, z + d / 2.0f); }

        public AABB(float x, float y, float z, float width, float height, float depth)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = width;
            this.h = height;
            this.d = depth;
        }



        public override string ToString()
        {
            return $"{x} {y} {z}   {w} {h} {d}";
        }


        public bool Contains(Vector3 p)
        {
            return p.x >= x && p.x <= x + w && p.y >= y && p.y <= y + h && p.z >= z && p.z <= z + d;
        }
        

        public bool Intersect(AABB other)
        {
            return !(x + w < other.x || x > other.x + other.w ||
                     y + h < other.y || y > other.y + other.h ||
                     z + d < other.z || z > other.z + other.d);
        }
    }
}
