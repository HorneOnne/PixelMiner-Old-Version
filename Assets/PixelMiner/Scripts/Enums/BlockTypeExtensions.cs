using UnityEngine;
using System.Collections.Generic;
namespace PixelMiner.Enums
{
    public static class BlockTypeExtensions
    {
        public static byte[] BlockProperties;
        private static HashSet<BlockType> _solidBlocksSet;
        private static HashSet<BlockType> _solidTransparentBlocksSet;


        static BlockTypeExtensions()
        {
            InitializeSolidBlocksSet();
            InitializeSolidTransparentBlocksSet();
            BlockProperties = new byte[(int)BlockType.Count];

            // bit 0: Solid block (Set bit 0: TRUE if solid else FALSE)
            foreach (var solidBlock in _solidBlocksSet)
            {
                BlockProperties[(byte)solidBlock] = (byte)(BlockProperties[(byte)solidBlock] | (1 << 0));
            }

            // bit 1: Solid transparent block (Set bit 1: TRUE if solid else FALSE)
            foreach (var solidBlock in _solidTransparentBlocksSet)
            {
                BlockProperties[(byte)solidBlock] = (byte)(BlockProperties[(byte)solidBlock] | (1 << 1));
            }


            _solidBlocksSet = null;
            _solidTransparentBlocksSet = null;
        }



        #region  INITIALIZE DATA
        private static void InitializeSolidBlocksSet()
        {
            _solidBlocksSet = new HashSet<BlockType>()
            {
                    {BlockType.DirtGrass},
                    {BlockType.Dirt},
                    {BlockType.Stone},
                    {BlockType.Sand},
                    {BlockType.Glass},
                    {BlockType.Snow},
                    {BlockType.Ice},
                    {BlockType.Light},
                    {BlockType.Wood},
                    {BlockType.Bedrock},
                    {BlockType.Gravel},
                    {BlockType.Cactus},
                    {BlockType.PineWood},
                    {BlockType.SnowDritGrass},
                    {BlockType.PineLeaves},
            };
        }
        private static void InitializeSolidTransparentBlocksSet()
        {
            _solidTransparentBlocksSet = new HashSet<BlockType>()
            {
                    {BlockType.Glass},
                    {BlockType.Leaves},
            };
        }
        #endregion


        public static bool IsSolid(this BlockType blockType)
        {
            //return blockType != BlockType.Air &&
            //       blockType != BlockType.Water &&
            //       blockType != BlockType.Grass &&
            //       blockType != BlockType.Leaves &&
            //       blockType != BlockType.Shrub &&
            //       blockType != BlockType.TallGrass;

            // Get bit 0.
            return (BlockProperties[(byte)blockType] & (1 << 0)) != 0;
        }

        public static bool IsDirt(this BlockType blockType)
        {
            return blockType == BlockType.Dirt ||
                   blockType == BlockType.DirtGrass ||
                   blockType == BlockType.SnowDritGrass;
        }

        public static bool IsTransparentSolidBlock(this BlockType blockType)
        {
            //return blockType == BlockType.Glass ||
            //       blockType == BlockType.Leaves;

            // Get bit 1.
            return (BlockProperties[(byte)blockType] & (1 << 1)) != 0;
        }

        public static bool IsGrassType(this BlockType blockType)
        {
            return blockType == BlockType.Grass ||
                   blockType == BlockType.Shrub;
        }

        public static bool AffectedByColorMap(this BlockType blockType)
        {
            return blockType == BlockType.Grass ||
                   blockType == BlockType.DirtGrass ||
                   blockType == BlockType.TallGrass ||
                   blockType == BlockType.Leaves;
        }
    }
}


