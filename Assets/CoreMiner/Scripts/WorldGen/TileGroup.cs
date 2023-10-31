using System.Collections.Generic;

namespace CoreMiner
{
    public class TileGroup
    {
        public TileGroupType Type;
        public List<Tile> Tiles;

        public List<TileGroup> Waters;
        public List<TileGroup> Lands;

        public TileGroup()
        {
            Tiles = new List<Tile>();
            Waters = new List<TileGroup>();
            Lands = new List<TileGroup>();
        }
    }
}

