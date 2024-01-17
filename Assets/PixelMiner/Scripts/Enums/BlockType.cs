namespace PixelMiner.Enums
{
    public enum BlockType : ushort
    {
        Air = 180,
        DirtGrass = 3,
        Dirt = 2,
        Stone = 1,

        Water = 207,
        Sand = 18,
        Glass = 49,
        Snow = 66,
        Ice = 67,

        Light = 105,

        Wood = 20,
        Leaves = 52,

        Grass = 39,
        TallGrass = 169,

        Bedrock = 17,
        Gravel = 0,
    }

    public static class BlockTypeExtensions
    {
        public static bool IsSolid(this BlockType blockType)
        {
            //return blockType != BlockType.Air;

            return blockType != BlockType.Air &&
                   blockType != BlockType.Water &&
                   blockType != BlockType.Grass &&
                   blockType != BlockType.Leaves &&
                   blockType != BlockType.TallGrass;
        }

        public static bool IsDirt(this BlockType blockType)
        {
            return blockType == BlockType.Dirt ||
                   blockType == BlockType.DirtGrass;
        }

        public static bool IsTransparentSolidBlock(this BlockType blockType)
        {
            return blockType == BlockType.Glass ||
                   blockType == BlockType.Leaves;
        }
    }
}

