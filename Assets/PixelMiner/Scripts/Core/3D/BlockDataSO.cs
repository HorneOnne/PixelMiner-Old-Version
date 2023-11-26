using PixelMiner.Enums;
using UnityEngine;
using System.Collections.Generic;


namespace PixelMiner.Core
{
    [CreateAssetMenu(fileName = "BlockData", menuName = "PixelMiner/BlockData", order = 1)]
    public class BlockDataSO : ScriptableObject
    {
        public float TileSizeX, TileSizeY;
        public List<BlockData> BLockDataList;
    }

    [System.Serializable]
    public class BlockData
    {
        public BlockType BlockType;
        // Cell uvs (block sides)
        public Vector2Int Up, Down, Side;
        public bool IsSolid;
        public bool GenerateCollider = true;
    }
}
