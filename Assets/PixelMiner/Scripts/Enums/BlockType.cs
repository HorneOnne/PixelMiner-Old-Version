namespace PixelMiner.Enums
{
    public enum BlockType : ushort
    {
        Air = 180,
        GrassTop = 40,
        GrassSide = 3,
        Dirt = 2,
        Stone = 1,
        
        Water = 207,
        Sand = 18,
        Glass = 49,
        Snow = 66,
        Ice = 67,

        Light = 105,

        Wood = 4,
    }

    public static class BlockTypeExtensions
    {
        public static bool IsSolid(this BlockType blockType)
        {
            return blockType != BlockType.Air &&
                   blockType != BlockType.Water;
        }
    }
}

