using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelMiner
{
    public class ChunkBounds : MonoBehaviour
    {
        DrawBounds drawer;
        private void Awake()
        {
            drawer = GetComponent<DrawBounds>();
        }
        
        
    }
}
