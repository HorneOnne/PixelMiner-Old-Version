using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using PixelMiner.WorldBuilding;
using PixelMiner.Enums;
using PixelMiner.World;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;


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

        private async void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
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
                SetActiveHeightMap(true, false);
                Debug.Log("Preview Height");
            }
            if (Input.GetKeyDown(KeyCode.F5))
            {
                SetActiveHeightMap(true, true);
                Debug.Log("Preview Height");
            }

            if (Input.GetKeyDown(KeyCode.F7))
            {
                CloseAllMap();
                Texture2D texture = await GetMyNoiseTexture();
                HeightMapPreview.SetImage(texture);
            }

            if (Input.GetKeyDown(KeyCode.F8))
            {
                CloseAllMap();
                Texture2D texture = await GetPoissonDiscTexture();
                HeightMapPreview.SetImage(texture);
            }
        }


        public bool HasHeightMap() => HeightMapPreview != null;
        public bool HasHeatMap() => HeatMapPreview != null;
        public bool HasMoistureMap() => MoistureMapPreview != null;
        public bool HasBiomeMap() => BiomeMapPreview != null;


        public void SetActiveHeightMap(bool digRiver, bool domainWarping)
        {
            CloseAllMap();

            UpdateHeightMapPreview(digRiver, domainWarping);
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





        private async void UpdateHeightMapPreview(bool digRiver, bool applyDomainWarping)
        {
            Texture2D texture = await GetHeightmapTextureAsync(digRiver, applyDomainWarping);
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



        private async Task<Texture2D> GetHeightmapTextureAsync(bool digRiver, bool applyDomainWarping)
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

            bool greyScale = false;

            Texture2D texture = new Texture2D(textureWidth, textureHeight);
            Color[] pixels = new Color[textureWidth * textureHeight];

            await Task.Run(() =>
            {
                Parallel.For(0, heightValues.Length, i =>
                {
                    float heightValue = heightValues[i];

                    if (greyScale)
                    {
                        pixels[i] = new Color(heightValue, heightValue, heightValue, 1.0f);

                        //if (heightValue > 0.5f && heightValue < 0.6f)
                        //{
                        //    pixels[i] = new Color(heightValue, heightValue, heightValue, 1.0f);
                        //}
                        //if (heightValue > 0.7f)
                        //{
                        //    pixels[i] = new Color(heightValue, heightValue, heightValue, 1.0f);
                        //}
                        //else if (heightValue < 0.25f)
                        //{
                        //    pixels[i] = Color.red;
                        //}
                        //else
                        //{
                        //    pixels[i] = Color.blue;
                        //}

                    }
                    else
                    {
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


                    //float modN = heatValue % 0.1f;
                    //if (modN < 0.015f)
                    //{
                    //    pixels[i] = WorldGeneration.ColdestColor;
                    //}
                    //else if (modN < 0.03f)
                    //{
                    //    pixels[i] = WorldGeneration.ColderColor;
                    //}
                    //else if (modN < 0.045f)
                    //{
                    //    pixels[i] = WorldGeneration.ColdColor;
                    //}
                    //else if (modN < 0.06f)
                    //{
                    //    pixels[i] = WorldGeneration.WarmColor;
                    //}
                    //else if (modN < 0.08f)
                    //{
                    //    pixels[i] = WorldGeneration.WarmerColor;
                    //}
                    //else
                    //{
                    //    pixels[i] = WorldGeneration.WarmestColor;
                    //}

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

            //if (applyHeight)
            //{
            //    float[] heightValues = await WorldGeneration.Instance.GetHeightMapDataAsync(0, 0, textureWidth, textureHeight);
            //    moistureValues = await WorldGeneration.Instance.ApplyHeightDataToMoistureDataAsync(heightValues, moistureValues, textureWidth, textureHeight);
            //}


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
            Texture2D texture = new Texture2D(textureWidth, textureHeight);
            Color[] pixels = new Color[textureWidth * textureHeight];

            Debug.Log($"Texture: {textureWidth} {textureHeight}");

            Vector3Int dimension = new Vector3Int(textureWidth, 1, textureHeight);

            Task<float[]> heightTask = WorldGeneration.Instance.GetHeightMapDataAsync(0, 0, textureWidth, textureHeight);
            Task<float[]> heatTask = WorldGeneration.Instance.GetHeatMapDataAysnc(0, 0, textureWidth, textureHeight);
            Task<float[]> moistureTask = WorldGeneration.Instance.GetMoistureMapDataAsync(0, 0, textureWidth, textureHeight);
            Task<float[]> riverTask = WorldGeneration.Instance.GetRiverDataAsync(0, 0, textureWidth, textureHeight);
            await Task.WhenAll(heightTask, heatTask, moistureTask, riverTask);
            float[] heightValues = heightTask.Result;
            float[] heatValues = heatTask.Result;
            float[] moistureValues = moistureTask.Result;
            float[] riverValues = riverTask.Result;

            TextureType[] blockData = await LoadHeightMapDataAsync(heightValues, textureWidth, 1, textureHeight);
            HeatType[] heatData = await LoadHeatMapDataAsync(heatValues, textureWidth, 1, textureHeight);
            MoistureType[] moistureData = await LoadMoistureMapDataAsync(moistureValues, textureWidth, 1, textureHeight);
            BiomeType[] biomeData = await GenerateBiomeMapDataAsync(moistureData, heatData, heightValues, textureWidth, 1, textureHeight);


            blockData = await LoadHeightMapDataAsync(heightValues, textureWidth, 1, textureHeight);
            BiomeType[] riverBiomes = new BiomeType[riverValues.Length];
            for (int i = 0; i < riverValues.Length; i++)
            {
                if (riverValues[i] > 0.6f && biomeData[i] != BiomeType.Ocean && biomeData[i] != BiomeType.Desert)
                {
                    riverBiomes[i] = BiomeType.River;
                }
                else
                {
                    riverBiomes[i] = BiomeType.Other;
                }
            }



            Queue<RiverNode> riverNodes = GetRiverNodes(riverBiomes, textureWidth, textureHeight);
            Debug.Log($"{riverNodes.Count}");
            int[] riversIndices = DigRiver(riverNodes, textureWidth, textureHeight);



            await Task.Run(() =>
            {
                Parallel.For(0, pixels.Length, (i) =>
                {
                    if (blockData[i] == TextureType.Water)
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
                            case BiomeType.Plains:
                                pixels[i] = WorldGeneration.Grassland;
                                break;
                            case BiomeType.Snow:
                                pixels[i] = WorldGeneration.Ice;
                                break;
                            case BiomeType.Forest:
                                pixels[i] = WorldGeneration.ForestColor;
                                break;
                        }


                        // Dig river
                        if (riversIndices[i] == 1)
                        {
                            pixels[i] = new Color32(64, 94, 195, 255);
                        }
                        else if (riversIndices[i] == 2)
                        {
                            pixels[i] = new Color32(45, 65, 134, 255);
                        }
                        else if (riversIndices[i] == 3)
                        {
                            pixels[i] = new Color32(24, 42, 100, 255);
                        }
                        else if (riversIndices[i] == 4)
                        {
                            pixels[i] = new Color32(15, 29, 74, 255);
                        }
                        else if (riversIndices[i] == 5)
                        {
                            pixels[i] = Color.black;
                        }
                    }
                });


                while (riverNodes.TryDequeue(out RiverNode node))
                {
                    pixels[node.RelativePosition.x + node.RelativePosition.y * textureWidth] = Color.blue;
                }
            });




            texture.SetPixels(pixels);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            texture.Apply();

            return texture;
        }

        private async Task<Texture2D> GetPoissonDiscTexture()
        {
            Debug.Log("GetPoissonDiscTexture");
            int textureWidth = 1920;
            int textureHeight = 1080;

            //int textureWidth = 160;
            //int textureHeight = 90;



            Texture2D texture = new Texture2D(textureWidth, textureHeight);
            Color[] pixels = new Color[textureWidth * textureHeight];

            var samplePoints = PoissonDisc(textureWidth, textureHeight);
            await Task.Run(() =>
            {
                Parallel.For(0, pixels.Length, (i) =>
                {
                    pixels[i] = Color.black;
                });


                //Debug.Log($"Sample count: {samplePoints.Count}");
                Parallel.For(0, samplePoints.Count, (i) =>
                {
                    int index = Mathf.FloorToInt(samplePoints[i].x) + Mathf.FloorToInt(samplePoints[i].y) * textureWidth;
                    pixels[index] = Color.white;
                });
            });

            texture.SetPixels(pixels);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;
            texture.Apply();
            return texture;
        }

        // Poisson Disc Reference: https://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph07-poissondisk.pdf
        private List<Vector2> PoissonDisc(int width, int height)
        {
            FastNoiseLite noise = new FastNoiseLite(0);
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFrequency(0.0045f);
            noise.SetFractalOctaves(1);

            float minR = 5;  // minimum distance R
            float maxR = 20;  // minimum distance R
            int k = 15; // limit of samples
                     
            float cellSize = maxR / Mathf.Sqrt(2);


            //Vector2 a = default;
            //Debug.Log($"Check: {Vector2.zero == default}   {a == default}    {a == Vector2.zero}");

            // Create a grid
           
            int gridWidth = Mathf.CeilToInt(width / cellSize);
            int gridHeight = Mathf.CeilToInt(height / cellSize);
            List<Vector2>[,] grid = new List<Vector2>[gridWidth, gridHeight];

            Debug.Log($"Grid length: {grid.Length}  {(int)width/minR * (int)height/minR}");

            Queue<Vector2> processList = new Queue<Vector2>();
            List<Vector2> samplePoints = new List<Vector2>();
            Vector2 currPoint;
            bool found;

            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    grid[x, y] = new List<Vector2>();
                }
            }


            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            Vector2 firstPoint = new Vector2(width / 2f, height / 2.0f);
            Insert(firstPoint);
            processList.Enqueue(firstPoint);
            samplePoints.Add(firstPoint);

            int attempt = 0;
            // Generate other points from points in processList
            while (processList.Count > 0)
            {
                currPoint = processList.Peek();
                found = false;
                for (int i = 0; i < k; i++)
                {
                    float noiseValue = (noise.GetNoise(currPoint.x , currPoint.y) + 1.0f) / 2.0f;
                    float minDist = Mathf.Lerp(minR, maxR, noiseValue);

                    //float minDist = maxR;
                    Vector2 newPoint = GenerateRandomPointAround(currPoint, (i + 1), minDist);
                    
                    //Debug.Log($"{currPoint.x +0} {currPoint.y + 0}");

                    if (IsValid(newPoint, minDist))
                    {
                        processList.Enqueue(newPoint);
                        samplePoints.Add(newPoint);
                        Insert(newPoint);
                        found = true;
                        break;
                    }
                }

                if (found == false)
                {
                    processList.Dequeue();
                }

                if (attempt++ > 500000)
                {
                    Debug.Log("Infinite loop");
                    break;
                }

           
            }


            sw.Stop();
            Debug.Log($"Elapsed time: {sw.ElapsedMilliseconds / 1000f} s");

            void Insert(Vector2 point)
            {
                int maxCellX = Mathf.FloorToInt(point.x / cellSize);
                int maxCellY = Mathf.FloorToInt(point.y / cellSize);
                grid[maxCellX, maxCellY].Add(point);
            }

            Vector2 GenerateRandomPointAround(Vector2 point, int attempt, float minDistance)
            {
                float noiseValue = (noise.GetNoise(point.x * attempt, point.y * attempt) + 1) / 2.0f;
                float theta = noiseValue * Mathf.PI * 2f;
                // Generate random radius
                float newRadius = minDistance + noiseValue * minDistance;

                // Calculate new point
                float newX = point.x + newRadius * Mathf.Cos(theta);
                float newY = point.y + newRadius * Mathf.Sin(theta);

                Vector2 newPoint = new Vector2(newX, newY);
                return newPoint;
            }


           

            bool IsValid(Vector2 point, float minDist)
            {
                if (point.x < 0 || point.x > width || point.y < 0 || point.y > height) return false;
                int maxCellX = Mathf.FloorToInt(point.x / cellSize);
                int maxCellY = Mathf.FloorToInt(point.y / cellSize);


                int maxStartX = Mathf.Max(0, maxCellX - 1);
                int maxEndX = Mathf.Min(maxCellX + 1, gridWidth - 1);
                int maxStartY = Mathf.Max(0, maxCellY - 1);
                int maxEndY = Mathf.Min(maxCellY + 1, gridHeight - 1);
                Vector2 gridPoint;

                for (int y = maxStartY; y <= maxEndY; y++)
                {
                    for (int x = maxStartX; x <= maxEndX; x++)
                    {
                        for(int i = 0; i < grid[x,y].Count; i++)
                        {
                            gridPoint = grid[x, y][i];
                            float dist = (point - gridPoint).sqrMagnitude;
                            if (dist < minDist * minDist)
                            {
                                return false;
                            }

                        }
                    }
                }
  

                return true;
            }

            return samplePoints;
        }



        // Density generation algorithm by https://www.redblobgames.com/maps/terrain-from-noise/
        public float Frequency = 0.01f;
        public float Lacunarity = 2.0f;
        public float Persistence = 0.5f;
        public int R = 3;
        private async Task<Texture2D> GetMyNoiseTexture()
        {
            int textureWidth = 1920;
            int textureHeight = 1080;
            Texture2D texture = new Texture2D(textureWidth, textureHeight);
            Color[] pixels = new Color[textureWidth * textureHeight];

            FastNoiseLite noise = new FastNoiseLite(0);
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFrequency(0.0045f);
            noise.SetFractalOctaves(3);

            await Task.Run(() =>
            {
                Parallel.For(0, pixels.Length, (i) =>
                {
                    int x = i % textureWidth;
                    int y = i / textureWidth;
                    float noiseValue = (noise.GetNoise(x, y) + 1.0f) / 2.0f;
                    pixels[i] = new Color(noiseValue, noiseValue, noiseValue, 1.0f);
                });



                //float[][] bluenoise = new float[textureHeight][];
                //for (int i = 0; i < textureHeight; i++)
                //{
                //    bluenoise[i] = new float[textureWidth];
                //}

                //for (int y = 0; y < textureHeight; y++)
                //{
                //    for (int x = 0; x < textureWidth; x++)
                //    {
                //        float nx = x / (float)textureWidth - 0.5f;
                //        float ny = y / (float)textureHeight - 0.5f;

                //        // Blue noise is high frequency; adjust this value based on your needs
                //        bluenoise[y][x] = noise.GetNoise(50 * nx, 50 * ny);
                //    }
                //}


                //for (int yc = 0; yc < textureHeight; yc++)
                //{
                //    for (int xc = 0; xc < textureWidth; xc++)
                //    {
                //        float max = 0;

                //        for (int dy = -R; dy <= R; dy++)
                //        {
                //            for (int dx = -R; dx <= R; dx++)
                //            {
                //                int xn = dx + xc;
                //                int yn = dy + yc;

                //                if (0 <= yn && yn < textureHeight && 0 <= xn && xn < textureWidth)
                //                {
                //                    float e = bluenoise[yn][xn];
                //                    if (e > max) { max = e; }
                //                }
                //            }
                //        }

                //        if (bluenoise[yc][xc] == max)
                //        {
                //            // Place tree at xc, yc
                //            Debug.Log("Trrree");
                //            pixels[xc + yc * textureWidth] = Color.white;
                //        }
                //    }
                //}
            });



            texture.SetPixels(pixels);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            texture.Apply();
            return texture;
        }




        #region Load data
        public async Task<TextureType[]> LoadHeightMapDataAsync(float[] heightValues, int width, int height, int depth)
        {
            TextureType[] chunkData = new TextureType[heightValues.Length];
            await Task.Run(() =>
            {
                for (int i = 0; i < heightValues.Length; i++)
                {
                    if (heightValues[i] < WorldGeneration.Instance.Water)
                    {
                        chunkData[i] = TextureType.Water;
                    }
                    else if (heightValues[i] < WorldGeneration.Instance.Sand)
                        chunkData[i] = TextureType.Sand;
                    else if (heightValues[i] < WorldGeneration.Instance.Grass)
                        chunkData[i] = TextureType.Dirt;
                    else if (heightValues[i] < WorldGeneration.Instance.Forest)
                        chunkData[i] = TextureType.GrassSide;
                    else if (heightValues[i] < WorldGeneration.Instance.Rock)
                        chunkData[i] = TextureType.Stone;
                    else
                        chunkData[i] = TextureType.Ice;
                }



            });

            return chunkData;
        }
        public async Task<HeatType[]> LoadHeatMapDataAsync(float[] heatValues, int width, int height, int depth)
        {
            HeatType[] heatData = new HeatType[heatValues.Length];
            await Task.Run(() =>
            {
                Parallel.For(0, heatValues.Length, (i) =>
                {
                    float heatValue = heatValues[i];

                    //if (heatValue < WorldGeneration.Instance.ColdestValue)
                    //{
                    //    heatData[i] = HeatType.Coldest;
                    //}
                    //else if (heatValue < WorldGeneration.Instance.ColderValue)
                    //{
                    //    heatData[i] = HeatType.Colder;
                    //}
                    //else if (heatValue < WorldGeneration.Instance.ColdValue)
                    //{
                    //    heatData[i] = HeatType.Cold;
                    //}
                    //else if (heatValue < WorldGeneration.Instance.WarmValue)
                    //{
                    //    heatData[i] = HeatType.Warm;
                    //}
                    //else if (heatValue < WorldGeneration.Instance.WarmerValue)
                    //{
                    //    heatData[i] = HeatType.Warmer;
                    //}
                    //else
                    //{
                    //    heatData[i] = HeatType.Warmest;
                    //}

                    float modN = heatValue % 0.1f;
                    if (modN < 0.015f)
                    {
                        heatData[i] = HeatType.Coldest;
                    }
                    else if (modN < 0.03f)
                    {
                        heatData[i] = HeatType.Colder;
                    }
                    else if (modN < 0.045f)
                    {
                        heatData[i] = HeatType.Cold;
                    }
                    else if (modN < 0.06)
                    {
                        heatData[i] = HeatType.Warm;
                    }
                    else if (modN < 0.08)
                    {
                        heatData[i] = HeatType.Warmer;
                    }
                    else
                    {
                        heatData[i] = HeatType.Warmest;
                    }


                });
            });

            return heatData;
        }
        public async Task<MoistureType[]> LoadMoistureMapDataAsync(float[] moistureValues, int width, int height, int depth)
        {
            MoistureType[] moistureData = new MoistureType[moistureValues.Length];
            await Task.Run(() =>
            {
                Parallel.For(0, moistureValues.Length, (i) =>
                {
                    float moistureValue = moistureValues[i];

                    if (moistureValue < WorldGeneration.Instance.DryestValue)
                    {
                        moistureData[i] = MoistureType.Dryest;
                    }
                    else if (moistureValue < WorldGeneration.Instance.DryerValue)
                    {
                        moistureData[i] = MoistureType.Dryer;
                    }
                    else if (moistureValue < WorldGeneration.Instance.DryValue)
                    {
                        moistureData[i] = MoistureType.Dry;
                    }
                    else if (moistureValue < WorldGeneration.Instance.WetValue)
                    {
                        moistureData[i] = MoistureType.Wet;
                    }
                    else if (moistureValue < WorldGeneration.Instance.WetterValue)
                    {
                        moistureData[i] = MoistureType.Wetter;
                    }
                    else
                    {
                        moistureData[i] = MoistureType.Wettest;
                    }
                });
            });

            return moistureData;
        }
        public async Task<BiomeType[]> GenerateBiomeMapDataAsync(MoistureType[] moistureData, HeatType[] heatData, float[] heightValues, int width, int height, int depth)
        {
            BiomeType[] biomesData = new BiomeType[width * depth];
            await Task.Run(() =>
            {
                Parallel.For(0, biomesData.Length, (i) =>
                {
                    if (heightValues[i] < WorldGeneration.Instance.Water)
                    {
                        ;
                        biomesData[i] = BiomeType.Ocean;
                    }
                    else
                    {
                        biomesData[i] = WorldGeneration.Instance.BiomeTable[(int)moistureData[i], (int)heatData[i]];
                    }
                });
            });

            return biomesData;
        }
        #endregion



        private Queue<RiverNode> GetRiverNodes(BiomeType[] biomeData, int width, int depth)
        {
            Queue<RiverNode> bfsRiverQueue = new Queue<RiverNode>();

            int size = width * depth;
            for (int i = 0; i < size; i++)
            {
                int x = i % width;
                int z = i / width;

                if (x == 0 || x == width - 1 || z == 0 || z == depth - 1) continue;

                int bitmask = 0;
                if (biomeData[x + (z + 1) * width] == biomeData[i])
                    bitmask += 1;
                if (biomeData[(x + 1) + z * width] == biomeData[i])
                    bitmask += 2;
                if (biomeData[x + (z - 1) * width] == biomeData[i])
                    bitmask += 4;
                if (biomeData[(x - 1) + z * width] == biomeData[i])
                    bitmask += 8;

                if (bitmask != 15)
                {
                    RiverNode riverNode = new RiverNode()
                    {
                        RelativePosition = new Vector3Int(x, z, 0),
                        Density = 5
                    };

                    bfsRiverQueue.Enqueue(riverNode);
                }
            }

            return bfsRiverQueue;
        }



        public float DomainWarpingFbmPerlinNoise(float x, float y, FastNoiseLite perlin, FastNoiseLite voronoi)
        {
            Vector2 p = new Vector2(x, y);

            Vector2 q = new Vector2((float)perlin.GetNoise(p.x, p.y),
                                    (float)perlin.GetNoise(p.x + 52.0f, p.y + 13.0f));


            //Vector2 l2p1 = (p + 40 * q) + new Vector2(77, 35);
            //Vector2 l2p2 = (p + 40 * q) + new Vector2(83, 28);

            //Vector2 r = new Vector3((float)perlin.GetNoise(l2p1.x, l2p1.y),
            //                        (float)perlin.GetNoise(l2p2.x, l2p2.y));


            //Vector2 l3 = p + 120 * r;
            Vector2 l3 = p + 40 * q;
            return (float)voronoi.GetNoise(l3.x, l3.y);
        }

        public float Map(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            value = Mathf.Clamp(value, fromMin, fromMax);
            return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
        }


        private int[] DigRiver(Queue<RiverNode> riverSpreadQueue, int width, int height)
        {
            int attempts = 0;
            int[] riversDensity = new int[width * height];
            for (int i = 0; i < riversDensity.Length; i++)
            {
                riversDensity[i] = 0;
            }


            int ToIndex(int x, int y)
            {
                return x + (y * width);
            }

            RiverNode startNode = riverSpreadQueue.Peek();
            riversDensity[ToIndex(startNode.RelativePosition.x, startNode.RelativePosition.y)] = 0;

            while (riverSpreadQueue.Count > 0)
            {
                RiverNode currentNode = riverSpreadQueue.Dequeue();
                var neighbors = GetNeighbors(currentNode.RelativePosition);
                for (int i = 0; i < neighbors.Length; i++)
                {
                    if (neighbors[i].x < 0 || neighbors[i].x > width - 1 || neighbors[i].y < 0 || neighbors[i].y > height - 1)
                    {
                        Debug.Log("Invalid");
                        continue;
                    }

                    if (riversDensity[ToIndex(neighbors[i].x, neighbors[i].y)] + 1 < currentNode.Density && currentNode.Density > 0)
                    {
                        RiverNode nbRiverNode = new RiverNode()
                        {
                            RelativePosition = neighbors[i],
                            Density = currentNode.Density - 1
                        };

                        riverSpreadQueue.Enqueue(nbRiverNode);
                        riversDensity[ToIndex(nbRiverNode.RelativePosition.x, nbRiverNode.RelativePosition.y)] = nbRiverNode.Density;
                    }
                }



                attempts++;
                if (attempts > 500000)
                {
                    Debug.Log("Infinite loop");
                    break;
                }
            }


            Debug.Log($"Spread river attempt: {attempts}");

            return riversDensity;
        }


        private Vector3Int[] _neighborsPosition = new Vector3Int[4];
        public Vector3Int[] GetNeighbors(Vector3Int position)
        {
            _neighborsPosition[0] = position + new Vector3Int(1, 0, 0);
            _neighborsPosition[1] = position + new Vector3Int(-1, 0, 0);
            _neighborsPosition[2] = position + new Vector3Int(0, 0, 1);
            _neighborsPosition[3] = position + new Vector3Int(0, 0, -1);

            return _neighborsPosition;
        }

    }
}
