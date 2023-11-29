using System.Collections.Generic;
using PixelMiner.Enums;

namespace PixelMiner.WorldBuilding
{
    [System.Serializable]
    public class TileGroup
    {
        public TileGroupType Type;
        public List<Tile> Tiles;

        public TileGroup()
        {
            Tiles = new List<Tile>();
        }
    }
}

