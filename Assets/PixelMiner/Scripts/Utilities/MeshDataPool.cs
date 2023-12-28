using PixelMiner.DataStructure;

namespace PixelMiner.Utilities
{
    public static class MeshDataPool
    {
        public static ObjectPool<MeshData> Pool = new ObjectPool<MeshData>(10);

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
