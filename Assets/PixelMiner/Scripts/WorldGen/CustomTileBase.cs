using UnityEngine;
using UnityEngine.Tilemaps;
using Sirenix.OdinInspector;

namespace PixelMiner.WorldGen
{
    [CreateAssetMenu(menuName = "PixelMiner/Tiles/CustomTile")]
    public class CustomTileBase : TileBase
    {
        [PreviewField(60), HideLabel]
        [HorizontalGroup("Split", 60)]
        public Sprite tileSprite;

        [VerticalGroup("Split/Right"), LabelWidth(60)]
        public TileType Type;

        [VerticalGroup("Split/Right"), LabelWidth(60)]
        public Color tileColor = Color.white;

        public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = tileSprite;
            tileData.color = tileColor;
            tileData.colliderType = UnityEngine.Tilemaps.Tile.ColliderType.Sprite;
        }
    }
}

