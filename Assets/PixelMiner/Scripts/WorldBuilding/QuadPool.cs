using PixelMiner.DataStructure;

namespace PixelMiner.WorldBuilding
{
    public static class QuadPool
    {
        public static ObjectPool<Quad> Pool = new ObjectPool<Quad>(30);

        public static Quad Get()
        {
            return Pool.Get();
        }

        public static void Release(Quad quad)
        {
            quad.Reset();
            Pool.Release(quad);

        }

    }

    public static class BlockPool
    {
        public static ObjectPool<Block> Pool = new ObjectPool<Block>(30);

        public static Block Get()
        {
            return Pool.Get();
        }

        public static void Release(Block block)
        {
            block.Reset();
            Pool.Release(block);
        }
    }

    public static class ChunkMeshDataPool
    {
        public static ObjectPool<ChunkMeshData> Pool = new ObjectPool<ChunkMeshData>(25);

        public static ChunkMeshData Get()
        {
            return Pool.Get();
        }

        public static void Release(ChunkMeshData chunkMeshData)
        {
            chunkMeshData.Reset();
            Pool.Release(chunkMeshData);
        }
    }
}
