using System.Collections.Generic;
using UnityEngine;

namespace CoreMiner
{
    public class Tile
    {
        public TileType Type { get; set; }
        public HeightType HeightType { get; set; }
        public HeatType HeatType { get; set; }
        public MoistureType MoistureType { get; set; }
        public float HeightValue { get; set; }
        public float HeatValue { get; set; }
        public float MoistureValue { get; set; }
        public byte FrameX, FrameY;

        // Neighbors
        public Tile Left;
        public Tile Right;
        public Tile Top;
        public Tile Bottom;

        public bool Collidable;
        public bool FloodFilled;


        // Rivers
        public List<River> Rivers = new List<River>();
        public int RiverSize { get; set; }

        public Tile() { }
        public Tile(byte x, byte y)
        {
            this.FrameX = x;
            this.FrameY = y;
            FloodFilled = false;
        }

        public bool HasNeighbors()
        {
            return Left != null && Right != null && Top != null && Bottom != null;
        }


        #region River
        public Direction GetLowestNeighbors()
        {
            float leftNbHeight = Left.HeightValue;
            float rightNbHeight = Right.HeightValue;
            float topNbHeight = Top.HeightValue;
            float bottomNbHeight = Bottom.HeightValue;

            if (leftNbHeight < rightNbHeight && leftNbHeight < topNbHeight && leftNbHeight < bottomNbHeight)
                return Direction.Left;
            else if (rightNbHeight < leftNbHeight && rightNbHeight < topNbHeight && rightNbHeight < bottomNbHeight)
                return Direction.Right;
            else if (topNbHeight < leftNbHeight && topNbHeight < rightNbHeight && topNbHeight < bottomNbHeight)
                return Direction.Top;
            else if (bottomNbHeight < leftNbHeight && bottomNbHeight < topNbHeight && bottomNbHeight < rightNbHeight)
                return Direction.Bottom;
            else
                return Direction.Bottom; // If all values are equal, returning any direction or a default direction.
        }
        public int GetRiverNeighborCount(River river)
        {
            int count = 0;
            if (Left.Rivers.Count > 0 && Left.Rivers.Contains(river))
                count++;
            if (Right.Rivers.Count > 0 && Right.Rivers.Contains(river))
                count++;
            if (Top.Rivers.Count > 0 && Top.Rivers.Contains(river))
                count++;
            if (Bottom.Rivers.Count > 0 && Bottom.Rivers.Contains(river))
                count++;
            return count;
        }
        public void SetRiverPath(River river)
        {
            if (Collidable == false)
                return;

            if (Rivers.Contains(river) == false)
            {
                Rivers.Add(river);
            }
        }
        public void SetRiverTile(River river)
        {
            SetRiverPath(river);
            HeightType = HeightType.River;
            HeightValue = 0;
            Collidable = false;
        }
        #endregion
    }

    public enum TileType : byte
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
        Color = 254,
        Other
    }

    public enum HeightType : byte
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

    public enum HeatType : byte
    {
        Coldest = 0,
        Colder = 1,
        Cold = 2,
        Warm = 3,
        Warmer = 4,
        Warmest = 5
    }

    public enum MoistureType : byte
    {
        Dryest = 0,
        Dryer = 1,
        Dry = 2,
        Wet = 3,
        Wetter = 4,
        Wettest = 5,
    }
}

