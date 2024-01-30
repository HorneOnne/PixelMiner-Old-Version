using System.Collections;
using System.Collections.Generic;

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
        public float vx;
        public float vy;
        public float vz;


        public override string ToString()
        {
            return $"{x} {y} {z}   {w} {h} {d}";
        }
    }
}
