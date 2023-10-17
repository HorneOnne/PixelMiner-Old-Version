using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoreMiner.WorldGen
{
    [CreateAssetMenu(menuName = "CoreMiner/Tiles/CustomTile")]
    public class CustomTileBase : TileBase
    {
        [Header("Tile Settings")]
        public Sprite tileSprite;
        public Color tileColor = Color.white;

        public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = tileSprite;
            tileData.color = tileColor;
            tileData.colliderType = UnityEngine.Tilemaps.Tile.ColliderType.Sprite;
        }
    }
}

