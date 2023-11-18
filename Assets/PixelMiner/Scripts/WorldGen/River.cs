using System.Collections.Generic;

namespace PixelMiner
{
    [System.Serializable]
    public class River
    {
        public int Length;
        public List<Tile> Tiles;
        public int ID;

        public int Insersections;
        public float TurnCount;
        public Direction CurrentDirection;

        public River(int id)
        {
            this.ID = id;
            Tiles = new List<Tile>();   
        }

        public void AddTile(Tile tile)
        {
            tile.SetRiverPath(this);
            Tiles.Add(tile);
        }
    }

    [System.Serializable]
    public class RiverGroup
    {
        public List<River> Rivers = new List<River>();
    }
}

