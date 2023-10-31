using System.Collections.Generic;

namespace CoreMiner
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

