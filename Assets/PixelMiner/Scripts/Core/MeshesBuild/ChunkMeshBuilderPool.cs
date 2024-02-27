using PixelMiner.DataStructure;
using UnityEngine;

namespace PixelMiner.Core
{
    public static class ChunkMeshBuilderPool
    {
        public static ObjectPool<ChunkMeshBuilder> Pool = new ObjectPool<ChunkMeshBuilder>(5);

        public static ChunkMeshBuilder Get()
        {
            return Pool.Get();
        }

        public static void Release(ChunkMeshBuilder chunkMeshData)
        {
            chunkMeshData.Reset();
            Pool.Release(chunkMeshData);
        }
    }
}
