using UnityEngine;
namespace PixelMiner.DataStructure
{
    public class UnityPoolManager : MonoBehaviour
    {
        private void OnApplicationQuit()
        {
            OctreeRootPool.Pool.Clear();
            OctreeLeavePool.Pool.Clear();
        }
    }
}
