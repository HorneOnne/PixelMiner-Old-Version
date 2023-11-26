using PixelMiner.Enums;
using PixelMiner.WorldGen;
using UnityEngine;

namespace PixelMiner.Core
{
    public static class BlockUtils
    {
        public static Vector2[] GetUVs(BlockType type, BlockSide side)
        {
            Vector2Int tile = GetTile(type, side);
            float tileSizeX = Main.Instance.TileSizeX;
            float tileSizeY = Main.Instance.TileSizeY;
            
            float u = tile.y * tileSizeX;
            float v = 1f - ((tile.x + 1) * tileSizeY); // Flip the v-coordinate for Unity textures

            // Calculate UV coordinates for the four corners
            Vector2 bottomLeft = new Vector2(u, v + tileSizeY);
            Vector2 bottomRight = new Vector2(u + tileSizeX, v + tileSizeY);
            Vector2 topLeft = new Vector2(u, v);
            Vector2 topRight = new Vector2(u + tileSizeX, v);

            return new Vector2[] { bottomLeft, bottomRight, topLeft, topRight };
        }


        private static Vector2Int GetTile(BlockType blockType, BlockSide side)
        {
            Vector2Int tile;
            switch (side)
            {
                case BlockSide.Top:
                    tile = Main.Instance.BlockDataDict[blockType].Up;
                    break;
                case BlockSide.Bottom:
                    tile = Main.Instance.BlockDataDict[blockType].Up;
                    break;
                case BlockSide.Front:
                case BlockSide.Back:
                case BlockSide.Left:
                case BlockSide.Right:
                default:
                    tile = Main.Instance.BlockDataDict[blockType].Side;
                    break;
            }
            return tile;
        }
    }
}
