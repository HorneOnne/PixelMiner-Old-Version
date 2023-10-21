namespace CoreMiner
{
    public class Tile
    {
        public TileType Type { get; set; }
        public float HeightValue { get; set; }
        public int FrameX, FrameY;
        public Tile() { }
        public Tile(int x, int y) 
        {
            this.FrameX = x;
            this.FrameY = y;
        }
    }

    public enum TileType : ushort
    {
        Dirt,
        DirtGrass,
        Stone,
        Other
    }
}

