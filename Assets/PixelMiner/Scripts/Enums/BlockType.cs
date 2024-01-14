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
        Wood = 4,

        Grass = 39,
    }

    public static class BlockTypeExtensions
    {
        public static bool IsSolid(this BlockType blockType)
        {
            //return blockType != BlockType.Air;

            return blockType != BlockType.Air &&
                   blockType != BlockType.Water &&
                    blockType != BlockType.Grass;
        }
    }
}

