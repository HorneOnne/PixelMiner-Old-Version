using UnityEngine;
using System.Collections.Generic;
namespace PixelMiner.Enums
{
    public static class BlockTypeExtensions
    {
        public static byte[] BlockProperties;

        private static HashSet<BlockType> _solidVoxelSet;
        private static HashSet<BlockType> _solidTransparentVoxelSet;
        private static HashSet<BlockType> _solidNonVoxelSet;


        static BlockTypeExtensions()
        {
            InitializeSolidBlocksSet();
            InitializeSolidTransparentBlocksSet();
            InitializeSolidNonvoxelSet();

            BlockProperties = new byte[(int)BlockType.Count];

            // bit 0: Solid block (Set bit 0: TRUE if solid else FALSE)
            foreach (var solidBlock in _solidVoxelSet)
            {
                BlockProperties[(byte)solidBlock] = (byte)(BlockProperties[(byte)solidBlock] | (1 << 0));
            }

            // bit 1: Solid transparent block (Set bit 1: TRUE if solid else FALSE)
            foreach (var solidBlock in _solidTransparentVoxelSet)
            {
                BlockProperties[(byte)solidBlock] = (byte)(BlockProperties[(byte)solidBlock] | (1 << 1));
            }

            // bit 2: Solid model (not block) (Set bit 2: TRUE if solid else FALSE)
            foreach (var solidBlock in _solidNonVoxelSet)
            {
                BlockProperties[(byte)solidBlock] = (byte)(BlockProperties[(byte)solidBlock] | (1 << 2));
            }

            _solidVoxelSet = null;
            _solidTransparentVoxelSet = null;
            _solidNonVoxelSet = null;
        }



        #region  INITIALIZE DATA
        private static void InitializeSolidBlocksSet()
        {
            _solidVoxelSet = new HashSet<BlockType>()
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
            _solidTransparentVoxelSet = new HashSet<BlockType>()
            {
                    {BlockType.Glass},
                    {BlockType.Leaves},
            };
        }

        private static void InitializeSolidNonvoxelSet()
        {
            _solidNonVoxelSet = new HashSet<BlockType>()
            {
                    {BlockType.Torch},
            };
        }
        #endregion


        public static bool IsSolidVoxel(this BlockType blockType)
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

        public static bool IsTransparentVoxel(this BlockType blockType)
        {
            //return blockType == BlockType.Glass ||
            //       blockType == BlockType.Leaves;

            // Get bit 1.
            return (BlockProperties[(byte)blockType] & (1 << 1)) != 0;
        }

        public static bool IsGrassType(this BlockType blockType)
        {
            return blockType == BlockType.Grass ||
                   blockType == BlockType.TallGrass ||
                   blockType == BlockType.Shrub;
        }

        public static bool AffectedByColorMap(this BlockType blockType)
        {
            return blockType == BlockType.Grass ||
                   blockType == BlockType.DirtGrass ||
                   blockType == BlockType.TallGrass ||
                   blockType == BlockType.Leaves;
        }

        public static bool IsSolidNonvoxel(this BlockType blockType)
        {
            // Get bit 2.
            return (BlockProperties[(byte)blockType] & (1 << 2)) != 0;
        }
    }
}


