using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using PixelMiner.World;
using System.Collections.Generic;


namespace PixelMiner.UI.WorldGen
{
    public class TestPoissonDisc : MonoBehaviour
    {
        public Image Image;
        public int TextureWidth = 300;
        public int TextureHeight = 300;
        public int FrameX;
        public int FrameZ;

        FastNoiseLite noise;

        private async void Start()
        {
            noise = new FastNoiseLite(7);
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(1);
            noise.SetFrequency(0.0035f);

            Texture2D texture = await GetPoissonDiscTexture();
            texture.anisoLevel = 0;
            Image.sprite = GetSpriteFromTex(texture);
        }
        public Sprite GetSpriteFromTex(Texture2D texture)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
            return sprite;
        }


        private async Task<Texture2D> GetPoissonDiscTexture()
        {
            Debug.Log("GetPoissonDiscTexture");
            //int textureWidth = 1920;
            //int textureHeight = 1080;

            //int textureWidth = 160;
            //int textureHeight = 90;



            Texture2D texture = new Texture2D(TextureWidth, TextureHeight);
            Color[] pixels = new Color[TextureWidth * TextureHeight];

            var samplePoints = PoissonDisc(FrameX, FrameZ, TextureWidth, TextureHeight, noise);
            Debug.Log($"samplecount   {this.gameObject.name}  {samplePoints.Count}");

            await Task.Run(() =>
            {
                Parallel.For(0, pixels.Length, (i) =>
                {
                    pixels[i] = Color.black;
                });


                //Debug.Log($"Sample count: {samplePoints.Count}");
                Parallel.For(0, samplePoints.Count, (i) =>
                {
                    int index = Mathf.FloorToInt(samplePoints[i].x) + Mathf.FloorToInt(samplePoints[i].y) * TextureWidth;
                    pixels[index] = Color.white;
                });
            });

            texture.SetPixels(pixels);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;
            texture.Apply();
            return texture;
        }

        private List<Vector2Int> PoissonDisc(int frameX, int frameZ, int width, int height, FastNoiseLite noise, float minDistance = 50, float maxDistance = 200)
        {

            int k = 30; // limit of samples
            float cellSize = maxDistance / Mathf.Sqrt(2);

            int gridWidth = Mathf.CeilToInt(width / cellSize);
            int gridHeight = Mathf.CeilToInt(height / cellSize);
            List<Vector2Int>[,] grid = new List<Vector2Int>[gridWidth, gridHeight];

            Debug.Log($"grid cell size:  {this.gameObject.name}   {grid.GetLength(0)} {grid.GetLength(1)}");

            Queue<Vector2Int> processList = new Queue<Vector2Int>();
            List<Vector2Int> samplePoints = new List<Vector2Int>();
            Vector2Int currPoint;
            bool found;

            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    grid[x, y] = new List<Vector2Int>();
                }
            }


            //Vector2Int firstPoint = new Vector2Int((int)(width / 2f), (int)(height / 2.0f));
            //Vector2Int firstPoint = new Vector2Int(UnityEngine.Random.Range(0, width), (UnityEngine.Random.Range(0, height)));

            float noiseX = (noise.GetNoise(frameX * width, frameZ * height) + 1.0f) / 2.0f;
            float noiseY = (noise.GetNoise(frameX * width, frameZ * height) + 1.0f) / 2.0f;
            Vector2Int firstPoint = new Vector2Int(Mathf.FloorToInt(noiseX * (width - 1)), Mathf.FloorToInt(noiseY * (height - 1)));

            InsertGrid(firstPoint);
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
                    //float noiseValue = (noise.GetNoise(currPoint.x, currPoint.y) + 1.0f) / 2.0f;
                    float noiseValue = (noise.GetNoise((frameX * width) + currPoint.x, (frameZ * height) + currPoint.y) + 1.0f) / 2.0f;
                    float distance = Mathf.Lerp(minDistance, maxDistance, noiseValue);

                    Vector2Int newPoint = GenerateRandomPointAround(currPoint, (i + 1), distance);

                    if (IsValid(newPoint, distance))
                    {
                        processList.Enqueue(newPoint);
                        samplePoints.Add(newPoint);
                        InsertGrid(newPoint);
                        found = true;
                        break;
                    }
                }

                if (found == false)
                {
                    processList.Dequeue();
                }

                if (attempt++ > 100000)
                {
                    Debug.Log("Infinite loop");
                    break;
                }
            }


            void InsertGrid(Vector2Int point)
            {
                int maxCellX = Mathf.FloorToInt(point.x / cellSize);
                int maxCellY = Mathf.FloorToInt(point.y / cellSize);
                grid[maxCellX, maxCellY].Add(point);
            }


            Vector2Int GenerateRandomPointAround(Vector2Int point, int attempt, float minDistance)
            {
                float noiseValue = (noise.GetNoise((point.x + frameX * width) * attempt, (point.y + frameZ * height) * attempt) + 1) / 2.0f;

                float theta = noiseValue * Mathf.PI * 2f;
                // Generate random radius
                float newRadius = minDistance + noiseValue * minDistance;

                // Calculate new point
                int newX = Mathf.FloorToInt(point.x + newRadius * Mathf.Cos(theta));
                int newY = Mathf.FloorToInt(point.y + newRadius * Mathf.Sin(theta));

                Vector2Int newPoint = new Vector2Int(newX, newY);
                return newPoint;
            }




            bool IsValid(Vector2 point, float minDist)
            {
                if (point.x < 0 || point.x > width - 1 || point.y < 0 || point.y > height - 1) return false;

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
                        for (int i = 0; i < grid[x, y].Count; i++)
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
    }
}
