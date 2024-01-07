using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using PixelMiner.WorldBuilding;
using PixelMiner.Enums;
using PixelMiner.World;



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

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.F1))
            {
                SetActiveBiomeMap();
                Debug.Log("Preview Biome");
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                SetActiveHeatMap();
                Debug.Log("Preview Heat");
            }
            if (Input.GetKeyDown(KeyCode.F3))
            {
                SetActiveMoistureMap(true);
                Debug.Log("Preview Moisture");
            }
            if (Input.GetKeyDown(KeyCode.F4))
            {
                SetActiveHeightMap(true);
                Debug.Log("Preview Height");
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                WorldGeneration.Instance.UpdateNoiseModule();
                Debug.Log("Update Noise Modules");
            }
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

            //if (digRiver)
            //{
            //    Debug.Log("Dig river");
            //    float[] riverValues = await WorldGeneration.Instance.GetRiverDataAsync(0, 0, textureWidth, textureHeight);
            //    heightValues = await WorldGeneration.Instance.DigRiverAsync(heightValues, riverValues, textureWidth, textureHeight);

            //    //LogUtils.Log(riverValues, "River.txt");

            //}

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
            //float[] heatValues = await WorldGeneration.Instance.GetHeatMapDataAysnc(0, 0, textureWidth, textureHeight);
            float[] heatValues = await WorldGeneration.Instance.GetFractalHeatMapDataAsync(0, 0, textureWidth, textureHeight);
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
            int textureWidth = 1920;
            int textureHeight = 1080;
            Vector3Int dimension = new Vector3Int(textureWidth, 1, textureHeight);


            float[] heightValues = await WorldGeneration.Instance.GetHeightMapDataAsync(0, 0, textureWidth, textureHeight);
            float[] heatValues = await WorldGeneration.Instance.GetFractalHeatMapDataAsync(0, 0, textureWidth, textureHeight);
            float[] moistureValues = await WorldGeneration.Instance.GetMoistureMapDataAsync(0, 0, textureWidth, textureHeight);
            float[] riverValues = await WorldGeneration.Instance.GetRiverDataAsync(0, 0, textureWidth, textureHeight);


            BlockType[] blockData = await LoadHeightMapDataAsync(heightValues, textureWidth, 1, textureHeight);
            HeatType[] heatData = await LoadHeatMapDataAsync(heatValues, textureWidth, 1, textureHeight);
            MoistureType[] moistureData = await LoadMoistureMapDataAsync(moistureValues, textureWidth, 1, textureHeight);

            BiomeType[] biomeData = await GenerateBiomeMapDataAsync(moistureData, heatData, textureWidth, 1, textureHeight);
            //heightValues = await WorldGeneration.Instance.DigRiverAsync(heightValues, heatValues, moistureValues,
            //    biomeData, riverValues, textureWidth, 1, textureHeight);

            blockData = await LoadHeightMapDataAsync(heightValues, textureWidth, 1, textureHeight);
            bool[] biomesBorders = UpdateBiomesBorders(biomeData, textureWidth, textureHeight);


            Texture2D texture = new Texture2D(textureWidth, textureHeight);
            Color[] pixels = new Color[textureWidth * textureHeight];


            await Task.Run(() =>
            {
                Parallel.For(0, pixels.Length, (i) =>
                {
                    if (blockData[i] == BlockType.Water)
                    {
                        if (heightValues[i] < WorldGeneration.Instance.DeepWater)
                        {
                            pixels[i] = WorldGeneration.DeepColor;
                        }
                        else
                        {
                            pixels[i] = WorldGeneration.ShallowColor;
                        }
                    }
                    else
                    {
                        BiomeType biomeType = biomeData[i];
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

                        if (biomesBorders[i])
                        {
                            pixels[i] = Color.black;
                        }
                    }

                });
            });

            texture.SetPixels(pixels);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            texture.Apply();

            return texture;
        }





        #region Load data
        public async Task<BlockType[]> LoadHeightMapDataAsync(float[] heightValues, int width, int height, int depth)
        {
            BlockType[] chunkData = new BlockType[heightValues.Length];
            await Task.Run(() =>
            {
                for (int i = 0; i < heightValues.Length; i++)
                {
                    if (heightValues[i] < WorldGeneration.Instance.Water)
                    {
                        chunkData[i] = BlockType.Water;
                    }
                    else if (heightValues[i] < WorldGeneration.Instance.Sand)
                        chunkData[i] = BlockType.Sand;
                    else if (heightValues[i] < WorldGeneration.Instance.Grass)
                        chunkData[i] = BlockType.Dirt;
                    else if (heightValues[i] < WorldGeneration.Instance.Forest)
                        chunkData[i] = BlockType.GrassSide;
                    else if (heightValues[i] < WorldGeneration.Instance.Rock)
                        chunkData[i] = BlockType.Stone;
                    else
                        chunkData[i] = BlockType.Glass;
                }



            });

            return chunkData;
        }
        public async Task<HeatType[]> LoadHeatMapDataAsync(float[] heatValues, int width, int height, int depth)
        {
            HeatType[] heatData = new HeatType[heatValues.Length];
            await Task.Run(() =>
            {
                for (int z = 0; z < depth; z++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int index3D = WorldGenUtilities.IndexOf(x, y, z, width, height);
                            int flattenedIndex = WorldGenUtilities.IndexOf(x, z, width);
                            float heatValue = heatValues[flattenedIndex];

                            if (heatValue < WorldGeneration.Instance.ColdestValue)
                            {
                                heatData[index3D] = HeatType.Coldest;
                            }
                            else if (heatValue < WorldGeneration.Instance.ColderValue)
                            {
                                heatData[index3D] = HeatType.Colder;
                            }
                            else if (heatValue < WorldGeneration.Instance.ColdValue)
                            {
                                heatData[index3D] = HeatType.Cold;
                            }
                            else if (heatValue < WorldGeneration.Instance.WarmValue)
                            {
                                heatData[index3D] = HeatType.Warm;
                            }
                            else if (heatValue < WorldGeneration.Instance.WarmerValue)
                            {
                                heatData[index3D] = HeatType.Warmer;
                            }
                            else
                            {
                                heatData[index3D] = HeatType.Warmest;
                            }

                        }
                    }
                }
            });

            return heatData;
        }
        public async Task<MoistureType[]> LoadMoistureMapDataAsync(float[] moistureValues, int width, int height, int depth)
        {
            MoistureType[] moistureData = new MoistureType[moistureValues.Length];
            await Task.Run(() =>
            {
                for (int z = 0; z < depth; z++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int index2D = WorldGenUtilities.IndexOf(x, z, width);
                            int index3D = WorldGenUtilities.IndexOf(x, y, z, width, height);
                            float moistureValue = moistureValues[index2D];

                            if (moistureValue < WorldGeneration.Instance.DryestValue)
                            {
                                moistureData[index3D] = MoistureType.Dryest;
                            }
                            else if (moistureValue < WorldGeneration.Instance.DryerValue)
                            {
                                moistureData[index3D] = MoistureType.Dryer;
                            }
                            else if (moistureValue < WorldGeneration.Instance.DryValue)
                            {
                                moistureData[index3D] = MoistureType.Dry;
                            }
                            else if (moistureValue < WorldGeneration.Instance.WetValue)
                            {
                                moistureData[index3D] = MoistureType.Wet;
                            }
                            else if (moistureValue < WorldGeneration.Instance.WetterValue)
                            {
                                moistureData[index3D] = MoistureType.Wetter;
                            }
                            else
                            {
                                moistureData[index3D] = MoistureType.Wettest;
                            }
                        }
                    }
                }
            });

            return moistureData;
        }
        public async Task<BiomeType[]> GenerateBiomeMapDataAsync(MoistureType[] moistureData, HeatType[] heatData, int width, int height, int depth)
        {
            BiomeType[] biomesData = new BiomeType[width * depth];
            await Task.Run(() =>
            {
                for (int z = 0; z < depth; z++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int index = WorldGenUtilities.IndexOf(x, z, width);
                            biomesData[index] = WorldGeneration.Instance.BiomeTable[(int)moistureData[index], (int)heatData[index]];
                        }
                    }
                }
            });

            return biomesData;
        }
        #endregion


        private bool[] UpdateBiomesBorders(BiomeType[] biomeData, int width, int depth)
        {
            bool[] borderEdges = new bool[width * depth];   
            
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = x + z * width;
                    borderEdges[index] = false;
                    if (x == 0 || x == width - 1 || z == 0 || z == depth - 1) continue;

                    int bitmask = 0;
                    if (biomeData[x + (z+1) * width] == biomeData[index])
                        bitmask += 1;
                    if (biomeData[(x + 1) + z * width] == biomeData[index])
                        bitmask += 2;
                    if (biomeData[x + (z - 1) * width] == biomeData[index])
                        bitmask += 4;
                    if (biomeData[(x - 1) + z * width] == biomeData[index])
                        bitmask += 8;

                    if(bitmask != 15)
                    {
                        borderEdges[index] = true;
                    }
                }
            }
            return borderEdges;
        }

    }
}
