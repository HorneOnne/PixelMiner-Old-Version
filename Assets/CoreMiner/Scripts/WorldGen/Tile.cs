namespace CoreMiner
{
    [System.Serializable]
    public class Tile
    {
        public TileType Type { get; set; }
        public HeightType HeightType { get; set; }
        public HeatType HeatType { get; set; }
        public float HeightValue { get; set; }
        public float HeatValue { get; set; }
        public int FrameX, FrameY;

        // Neighbors
        public Tile Left;
        public Tile Right;
        public Tile Top;
        public Tile Bottom;

        public bool Collidable;
        public bool FloodFilled;

        public Tile() { }
        public Tile(int x, int y) 
        {
            this.FrameX = x;
            this.FrameY = y;
            FloodFilled = false;
        }

        public bool HasNeighbors()
        {
            return Left != null && Right != null && Top != null && Bottom != null;
        }

    }

    public enum TileType : ushort
    {
        Dirt,
        DirtGrass,
        ForestGrass,
        Stone,
        Water,
        DeepWater,
        Sand,
        Rock,
        Snow,
        Heat = 999,
        Other
    }

    public enum HeightType
    {
        DeepWater = 1,
        ShallowWater = 2,
        Shore = 3,
        Sand = 4,
        Grass = 5,
        Forest = 6,
        Rock = 7,
        Snow = 8,
        River = 9,
    }

    public enum HeatType
    {
        Coldest = 0,
        Colder = 1,
        Cold = 2,
        Warm = 3,
        Warmer = 4,
        Warmest = 5
    }
}

