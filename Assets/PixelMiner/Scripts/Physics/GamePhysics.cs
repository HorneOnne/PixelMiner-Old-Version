using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelMiner.Physics
{
    public static class GamePhysics 
    {
        public static bool Swept(Bounds dynamicBox, Bounds staticBox, out Vector3 overlap)
        {

            // Horizontal overlap
            bool overlapHorizontal = false;
            if (dynamicBox.min.x < staticBox.min.x + (staticBox.max.x - staticBox.min.x) &&
               staticBox.min.x < dynamicBox.min.x + (dynamicBox.max.x - dynamicBox.min.x))
            {
                overlapHorizontal = true;
            }


            // Vertical overlap
            bool overlapVertical = false;
            if (dynamicBox.min.y < staticBox.min.y + (staticBox.max.y - staticBox.min.y) &&
              staticBox.min.y < dynamicBox.min.y + (dynamicBox.max.y - dynamicBox.min.y))
            {
                overlapVertical = true;
            }


            float dx = Mathf.Abs(dynamicBox.center.x - staticBox.center.x);
            float dy = Mathf.Abs(dynamicBox.center.y - staticBox.center.y);


            float ox = (dynamicBox.extents.x + staticBox.extents.x) - dx;
            float oy = (dynamicBox.extents.y + staticBox.extents.y) - dy;

            overlap = default;
            if (overlapHorizontal && overlapVertical)
            {
                overlap = new Vector2(ox, oy);
            }

            return (overlapHorizontal && overlapVertical);
        }
    }
}
