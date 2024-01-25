using PixelMiner.DataStructure;

namespace PixelMiner.WorldBuilding
{
    public static class ChunkDataPool
    {
        public static ObjectPool<ChunkGenData> Pool = new ObjectPool<ChunkGenData>(10);

        public static ChunkGenData Get()
        {
            return Pool.Get();
        }

        public static void Release(ChunkGenData chunkData)
        {
            chunkData.Reset();
            Pool.Release(chunkData);
        }
    }
}
