using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;

namespace CoreMiner
{
    [CreateAssetMenu(menuName = "CoreMiner/Tiles/CustomAnimatedTile")]
    public class CustomAnimatedTileBase : AnimatedTile
    {
        [Header("Tile Settings")]
        public Sprite tileSprite;
        public TileType Type;
        public Color tileColor = Color.white;

        public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = tileSprite;
            tileData.color = tileColor;
            tileData.colliderType = UnityEngine.Tilemaps.Tile.ColliderType.Sprite;
        }
    }
}

