using UnityEngine;
using System.Threading.Tasks;
using PixelMiner.WorldGen;
using PixelMiner.WorldBuilding;
using PixelMiner.Enums;
using System.Xml.Resolvers;
using System.Runtime.InteropServices.WindowsRuntime;
using JetBrains.Annotations;
using Codice.Client.Common;

namespace PixelMiner.UI.WorldGen
{
    public class UIMapPreviewManager : MonoBehaviour
    {
        public static UIMapPreviewManager Instance { get; private set; }
        public UIMapPreview HeightMapPreview { get; private set; }
        public UIMapPreview HeatMapPreview { get; private set; }
        public UIMapPreview MoistureMapPreview { get; private set; }
        public UIMapPreview BiomeMapPreview { get; private set; }




        private void Awake()
        {
            Instance = this;
            HeightMapPreview = transform.Find("HeightMapPreview")?.GetComponent<UIMapPreview>();
            HeatMapPreview = transform.Find("HeatMapPreview")?.GetComponent<UIMapPreview>();
            MoistureMapPreview = transform.Find("MoistureMapPreview")?.GetComponent<UIMapPreview>();
            BiomeMapPreview = transform.Find("BiomeMapPreview")?.GetComponent<UIMapPreview>();
        }


        public bool HasHeightMap() => HeightMapPreview != null;
        public bool HasHeatMap() => HeatMapPreview != null;
        public bool HasMoistureMap() => MoistureMapPreview != null;
        public bool HasBiomeMap() => BiomeMapPreview != null;


        public void SetActiveHeightMap(bool digRiver)
        {
            CloseAllMap();

            UpdateHeightMapPreview(digRiver);
        }
        public void SetActiveHeatMap()
        {
            CloseAllMap(); ;

            UpdateHeatMapPreview();
        }
        public void SetActiveMoistureMap(bool applyHeight)
        {
            CloseAllMap();

            UpdateMoistureMapPreview(applyHeight);
        }
        public void SetActiveBiomeMap()
        {
            CloseAllMap();

            UpdateBiomeMapPreview();
        }

        public void CloseAllMap()
        {
            if (HasHeightMap()) HeightMapPreview.Image.enabled = false;
            if (HasHeatMap()) HeatMapPreview.Image.enabled = false;
            if (HasMoistureMap()) MoistureMapPreview.Image.enabled = false;
            if (HasBiomeMap()) BiomeMapPreview.Image.enabled = false;
        }





        private async void UpdateHeightMapPreview(bool digRiver)
        {
            Texture2D texture = await GetHeightmapTextureAsync(digRiver);
            HeightMapPreview.SetImage(texture);
        }
        private async void UpdateHeatMapPreview()
        {
            Texture2D texture = await GetHeatTextureAsync();
            HeatMapPreview.SetImage(texture);
        }
        private async void UpdateMoistureMapPreview(bool applyHeight)
        {
            Texture2D texture = await GetMoistureMapTextureAsync(applyHeight);
            MoistureMapPreview.SetImage(texture);
        }
        private async void UpdateBiomeMapPreview()
        {
            Texture2D texture = await GetBiomesMapTextureAsync();
            BiomeMapPreview.SetImage(texture);
        }



        private async Task<Texture2D> GetHeightmapTextureAsync(bool digRiver)
        {
            int textureWidth = 1920;
            int textureHeight = 1080;
            float[] heightValues = await WorldGeneration.Instance.GetHeightMapDataAsync(0, 0, textureWidth, textureHeight);

            if (digRiver)
            {
                Debug.Log("Dig river");
                float[] riverValues = await WorldGeneration.Instance.GetRiverDataAsync(0, 0, textureWidth, textureHeight);
                heightValues = await WorldGeneration.Instance.DigRiverAsync(heightValues, riverValues, textureWidth, textureHeight);

                //LogUtils.Log(riverValues, "River.txt");

            }

            Texture2D texture = new Texture2D(textureWidth, textureHeight);
            Color[] pixels = new Color[textureWidth * textureHeight];

            await Task.Run(() =>
            {
                Parallel.For(0, heightValues.Length, i =>
                {
                    float heightValue = heightValues[i];
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
        private async Task<Texture2D> GetHeatTextureAsync()
        {
            int textureWidth = 1920;
            int textureHeight = 1080;
            float[] heatValues = await WorldGeneration.Instance.GetHeatMapDataAysnc(0, 0, textureWidth, textureHeight);
            Texture2D texture = new Texture2D(textureWidth, textureHeight);
            Color[] pixels = new Color[textureWidth * textureHeight];

            await Task.Run(() =>
            {
                Parallel.For(0, heatValues.Length, i =>
                {
                    float heatValue = heatValues[i];

                    if (heatValue < WorldGeneration.Instance.ColdestValue)
                    {
                        pixels[i] = WorldGeneration.ColdestColor;
                    }
                    else if (heatValue < WorldGeneration.Instance.ColderValue)
                    {
                        pixels[i] = WorldGeneration.ColderColor;
                    }
                    else if (heatValue < WorldGeneration.Instance.ColdValue)
                    {
                        pixels[i] = WorldGeneration.ColdColor;
                    }
                    else if (heatValue < WorldGeneration.Instance.WarmValue)
                    {
                        pixels[i] = WorldGeneration.WarmColor;
                    }
                    else if (heatValue < WorldGeneration.Instance.WarmerValue)
                    {
                        pixels[i] = WorldGeneration.WarmerColor;
                    }
                    else
                    {
                        pixels[i] = WorldGeneration.WarmestColor;
                    }
                });
            });

            texture.SetPixels(pixels);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            texture.Apply();
            return texture;
        }
        private async Task<Texture2D> GetMoistureMapTextureAsync(bool applyHeight = true)
        {
            int textureWidth = 1920;
            int textureHeight = 1080;

            float[] moistureValues = await WorldGeneration.Instance.GetMoistureMapDataAsync(0, 0, textureWidth, textureHeight);

            if (applyHeight)
            {
                float[] heightValues = await WorldGeneration.Instance.GetHeightMapDataAsync(0, 0, textureWidth, textureHeight);
                moistureValues = await WorldGeneration.Instance.ApplyHeightDataToMoistureDataAsync(heightValues, moistureValues, textureWidth, textureHeight);
            }


            Texture2D texture = new Texture2D(textureWidth, textureHeight);
            Color[] pixels = new Color[textureWidth * textureHeight];
            await Task.Run(() =>
            {
                Parallel.For(0, moistureValues.Length, i =>
                {
                    float moistureValue = moistureValues[i];
                    if (moistureValue < WorldGeneration.Instance.DryestValue)
                    {
                        pixels[i] = WorldGeneration.Dryest;
                    }
                    else if (moistureValue < WorldGeneration.Instance.DryerValue)
                    {
                        pixels[i] = WorldGeneration.Dryer;
                    }
                    else if (moistureValue < WorldGeneration.Instance.DryValue)
                    {
                        pixels[i] = WorldGeneration.Dry;
                    }
                    else if (moistureValue < WorldGeneration.Instance.WetValue)
                    {
                        pixels[i] = WorldGeneration.Wet;
                    }
                    else if (moistureValue < WorldGeneration.Instance.WetterValue)
                    {
                        pixels[i] = WorldGeneration.Wetter;
                    }
                    else
                    {
                        pixels[i] = WorldGeneration.Wettest;
                    }
                });
            });

            texture.SetPixels(pixels);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            texture.Apply();
            return texture;
        }
        private async Task<Texture2D> GetBiomesMapTextureAsync()
        {
            int textureWidth = 1920*2;
            int textureHeight = 1080*2;
            Vector3Int dimension = new Vector3Int(textureWidth, 2, textureHeight);

            Chunk chunk = await WorldGeneration.Instance.GenerateNewChunk(0, 0, 0, dimension, applyDefaultData: true);
 
            Texture2D texture = new Texture2D(textureWidth, textureHeight);
            Color[] pixels = new Color[textureWidth * textureHeight];


            await Task.Run(() =>
            {
                Parallel.For(0, dimension[2], z =>
                {
                    for (int x = 0; x < dimension[0]; x++)
                    {
                        int i = x + z * dimension[0];

                        BiomeType biomeType = chunk.BiomesData[i];
                        switch (biomeType)
                        {
                            default:
                            case BiomeType.Desert:
                                pixels[i] = WorldGeneration.Desert;
                                break;
                            case BiomeType.Savanna:
                                pixels[i] = WorldGeneration.Savanna;
                                break;
                            case BiomeType.TropicalRainforest:
                                pixels[i] = WorldGeneration.TropicalRainforest;
                                break;
                            case BiomeType.Grassland:
                                pixels[i] = WorldGeneration.Grassland;
                                break;
                            case BiomeType.Woodland:
                                pixels[i] = WorldGeneration.Woodland;
                                break;
                            case BiomeType.SeasonalForest:
                                pixels[i] = WorldGeneration.SeasonalForest;
                                break;
                            case BiomeType.TemperateRainforest:
                                pixels[i] = WorldGeneration.TemperateRainforest;
                                break;
                            case BiomeType.BorealForest:
                                pixels[i] = WorldGeneration.BorealForest;
                                break;
                            case BiomeType.Tundra:
                                pixels[i] = WorldGeneration.Tundra;
                                break;
                            case BiomeType.Ice:
                                pixels[i] = WorldGeneration.Ice;
                                break;
                        }

                        if (chunk.ChunkData[i] == BlockType.Water)
                        {
                            pixels[i] = WorldGeneration.DeepColor;
                        }
                    }
                });
            });

            texture.SetPixels(pixels);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            texture.Apply();

            Destroy(chunk.gameObject);
            return texture;
        }
    }
}
