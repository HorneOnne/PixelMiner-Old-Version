using UnityEngine;
using UnityEditor;

namespace CoreMiner
{
    [CustomEditor(typeof(CustomTileBase))]
    public class CustomTileBaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            CustomTileBase customTile = (CustomTileBase)target;

            GUILayout.Label("Tile Preview:");

            // Display the selected sprite field
            customTile.tileSprite = EditorGUILayout.ObjectField("Tile Sprite", customTile.tileSprite, typeof(Sprite), false) as Sprite;

            EditorGUILayout.LabelField("Type: " + customTile.Type.ToString());
            EditorGUILayout.LabelField("Color: " + customTile.tileColor.ToString());

            // Other properties can be displayed here as well
            // ...

            // Continue with the rest of the inspector GUI elements
            DrawDefaultInspector();
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            CustomTileBase customTile = (CustomTileBase)target;

            if (customTile == null || customTile.tileSprite == null)
            {
                return null;
            }

            var texture = new Texture2D(width, height);
            EditorUtility.CopySerialized(customTile.tileSprite.texture, texture);
            return texture;

            //Color[] pixels = customTile.tileSprite.texture.GetPixels(
            //(int)customTile.tileSprite.textureRect.x,
            //(int)customTile.tileSprite.textureRect.y,
            //(int)customTile.tileSprite.textureRect.width,
            //(int)customTile.tileSprite.textureRect.height);
            //Texture2D texture = new Texture2D((int)customTile.tileSprite.textureRect.width, (int)customTile.tileSprite.textureRect.height);
            ////texture.SetPixels(pixels);
            ////texture.Apply();

            //EditorUtility.CopySerialized(customTile.tileSprite.texture, texture);
            //return texture;




            return texture;

        }
    }
}

