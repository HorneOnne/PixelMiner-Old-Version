using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using PixelMiner.WorldGen;

namespace PixelMiner.UI.WorldGen
{
    public class UIMapPreviewManager : MonoBehaviour
    {
        public static UIMapPreviewManager Instance { get; private set; }
        public UIMapPreview HeightMapPreview { get; private set; }
        public UIMapPreview HeatMapPreview { get; private set; }
        public UIMapPreview MoistureMapPreview { get; private set; }




        private void Awake()
        {
            Instance = this;
            HeightMapPreview = transform.Find("HeightMapPreview")?.GetComponent<UIMapPreview>();
            HeatMapPreview = transform.Find("HeatMapPreview")?.GetComponent<UIMapPreview>();
            MoistureMapPreview = transform.Find("MoistureMapPreview")?.GetComponent<UIMapPreview>();
        }


        public bool HasHeightMap() => HeightMapPreview != null;
        public bool HasHeatMap() => HeatMapPreview != null;
        public bool HasMoistureMap() => MoistureMapPreview != null;


        public void SetActiveHeightMap()
        {
            if (HasHeightMap()) HeightMapPreview.gameObject.SetActive(true);
            if (HasHeatMap()) HeatMapPreview.gameObject.SetActive(false);
            if (HasMoistureMap()) MoistureMapPreview.gameObject.SetActive(false);

            UpdateHeightMapPreview();
        }

        public void SetActiveHeatMap()
        {
            if (HasHeightMap()) HeightMapPreview.gameObject.SetActive(false);
            if (HasHeatMap()) HeatMapPreview.gameObject.SetActive(true);
            if (HasMoistureMap()) MoistureMapPreview.gameObject.SetActive(false);
        }

        public void SetActiveMoistureMap()
        {
            if (HasHeightMap()) HeightMapPreview.gameObject.SetActive(false);
            if (HasHeatMap()) HeatMapPreview.gameObject.SetActive(false);
            if (HasMoistureMap()) MoistureMapPreview.gameObject.SetActive(true);
        }


        public void CloseAllMap()
        {
            if (HasHeightMap()) HeightMapPreview.gameObject.SetActive(false);
            if (HasHeatMap()) HeatMapPreview.gameObject.SetActive(false);
            if (HasMoistureMap()) MoistureMapPreview.gameObject.SetActive(false);
        }





        private async void UpdateHeightMapPreview()
        {
            Texture2D texture = await GetHeightmapTextureAsync();
            HeightMapPreview.SetImage(texture);
        }


        private async Task<Texture2D> GetHeightmapTextureAsync()
        {
            int textureWidth = 960;
            int textureHeight = 540;
            float[] heightValues = await WorldGeneration.Instance.GetHeightMapDataAsync(0, 0, textureWidth, textureHeight);
            Texture2D texture = new Texture2D(textureWidth, textureHeight);
            Color[] pixels = new Color[textureWidth * textureHeight];
            int x;
            int y;
            await Task.Run(() =>
            {
                Parallel.For(0, heightValues.Length, i =>
                {
                    float heightValue = heightValues[i];
                    x = i % textureWidth;
                    y = i / textureHeight;
                    if (heightValue < WorldGeneration.Instance.DeepWater)
                    {
                        pixels[i] = WorldGeneration.DeepColor;
                    }
                    else if (heightValue < WorldGeneration.Instance.Water)
                    {
                        pixels[i] = WorldGeneration.ShallowColor;
                    }
                    else if (heightValue < WorldGeneration.Instance.Sand)
                    {
                        pixels[i] = WorldGeneration.SandColor;
                    }
                    else if (heightValue < WorldGeneration.Instance.Grass)
                    {
                        pixels[i] = WorldGeneration.GrassColor;
                    }
                    else if (heightValue < WorldGeneration.Instance.Forest)
                    {
                        pixels[i] = WorldGeneration.ForestColor;
                    }
                    else if (heightValue < WorldGeneration.Instance.Rock)
                    {
                        pixels[i] = WorldGeneration.RockColor;
                    }
                    else
                    {
                        pixels[i] = WorldGeneration.SnowColor;
                    }
                });
            });

            texture.SetPixels(pixels);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            texture.Apply();
            return texture;
        }
    }
}
