using PixelMiner.DataStructure;

namespace PixelMiner.Core
{
    public static class MeshDataPool
    {
        public static ObjectPool<MeshData> Pool = new ObjectPool<MeshData>(20);

        public static MeshData Get()
        {
            return Pool.Get();
        }

        public static void Release(MeshData meshData)
        {
            meshData.Reset();
            Pool.Release(meshData);
        }
    }
}
