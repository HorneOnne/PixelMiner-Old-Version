using PixelMiner.DataStructure;

namespace PixelMiner.WorldBuilding
{
    public static class QuadPool
    {
        public static ObjectPool<Quad> Pool = new ObjectPool<Quad>(100);


    }

    public static class BlockPool
    {
        public static ObjectPool<Block> Pool = new ObjectPool<Block>(23);

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
}
