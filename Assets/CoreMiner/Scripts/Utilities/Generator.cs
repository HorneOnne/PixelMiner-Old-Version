using UnityEngine;
using LibNoise;
using LibNoise.Generator;

namespace CoreMiner.Utilities.NoiseGeneration
{

    public class Generator : MonoBehaviour
    {
        public int Width = 256;
        public int Height = 256;
        public int Octaves = 6;
        public double Frequency = 1.25f;
        public double Lacunarity = 2.0f;
        public double Persistence = 0.5f;
        public int Seed = 3;
        public Vector2 Offset;




        // Noise Generator Module
        private ModuleBase _heightNoiseModule;

        // Height Map Data
        private MapData _heightMapData;

        // Final Objects
        private Tile[,] _tiles;

        // Texture output
        [SerializeField] private MeshRenderer _heightMapMeshRenderer;

        private void Start()
        {
            _heightMapMeshRenderer = transform.Find("HeightTexture").GetComponent<MeshRenderer>();
            _heightNoiseModule = new Perlin(Frequency, Lacunarity, Persistence, Octaves, Seed, QualityMode.High);

            GetData();

            LoadTiles();

            //_heightMapMeshRenderer.materials[0].mainTexture = TextureGenerator.GetTexture(Width, Height, _tiles);
            
            //_heightMapMeshRenderer.materials[0].mainTexture = TextureGenerator.GenerateNoiseGradient(Width, Height);
            _heightMapMeshRenderer.materials[0].mainTexture = TextureGenerator.GenerateNoiseGradient(Width, Height, Offset.x, Offset.y);
        }



        private Texture2D GetTexture(ModuleBase module)
        {
            Noise2D noise2D = new Noise2D(Width, Height, module);
            noise2D.GeneratePlanar(0, Width, 0, Height, isSeamless: true);
            return noise2D.GetTexture();
        }

        private void GetData()
        {
            _heightMapData = new MapData(Width, Height);
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    float value = (float)_heightNoiseModule.GetValue(Offset.x * Width + x, Offset.y * Height + y, 0);

                    if (value > _heightMapData.Max) _heightMapData.Max = value;
                    if (value < _heightMapData.Min) _heightMapData.Min = value;

                    _heightMapData.Data[x, y] = value;

                }
            }

        }

        private void LoadTiles()
        {
            _tiles = new Tile[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tile t = new Tile();
                    t.X = x;
                    t.Y = y;

                    float value = _heightMapData.Data[x, y];

                    //normalize our value between 0 and 1
                    value = (value - _heightMapData.Min) / (_heightMapData.Max - _heightMapData.Min);
                    t.HeightValue = value;
                    _tiles[x, y] = t;

                }
            }
        }



    
    }
}