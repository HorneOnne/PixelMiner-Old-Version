using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelMiner.WorldBuilding
{
    [CreateAssetMenu]
    public class ModelData : ScriptableObject
    {
        public List<Vector3> Vertices;
        public List<int> Triangles;
    }
}
