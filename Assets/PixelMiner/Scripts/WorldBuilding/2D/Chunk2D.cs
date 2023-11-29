using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading.Tasks;
using System.Collections.Generic;
using PixelMiner.DataStructure;

namespace PixelMiner.WorldBuilding
{
    [SelectionBase]
    public class Chunk2D : MonoBehaviour
    {
        public static System.Action<Chunk2D> OnChunkFarAway;

        [Header("Chunk Settings")]
        public Grid<Tile> ChunkData;
        public float FrameX;    // Used for calculate world position
        public float FrameY;    // Used for calculate world position
        public int IsometricFrameX; // Used for calculate world generation (coherent noise)
        public int IsometricFrameY; // Used for calculate world generation (coherent noise)
        [SerializeField] private byte _width;
        [SerializeField] private byte _height;
        private float _unloadChunkDistance = 200;
        private float _updateFrequency = 1.0f;
        private float _updateTimer = 0.0f;


        // Neighbors
        public Chunk2D Left;
        public Chunk2D Right;
        public Chunk2D Top;
        public Chunk2D Bottom;


        [Header("Tilemap visualization")]
        public Tilemap LandTilemap;
        public Tilemap WaterTilemap;
        public Tilemap Tilegroupmap;
        public Tilemap HeatTilemap;
        public Tilemap MoistureTilemap;
        public Grid Grid;

        public bool HasChunkNeighbors = false;
        public bool AllTileHasNeighbors = false;
        public bool ChunkHasDrawn;
        public bool Processing { get; set; } = false;


        public List<TileGroup> Waters = new List<TileGroup>();
        public List<TileGroup> Lands = new List<TileGroup>();

        private void Awake()
        {
            ChunkHasDrawn = false;
        }
        private void Start()
        {

        }
        private void Update()
        {
            if (Time.time - _updateTimer > _updateFrequency)
            {
                _updateTimer = Time.time;
                if (Vector2.Distance(Camera.main.transform.position, transform.position) > _unloadChunkDistance)
                {
                    OnChunkFarAway?.Invoke(this);
                }
            }
        }

        public async void Init(float frameX, float frameY, int isometricFrameX, int isometricFrameY, byte _width, byte height)
        {
            Processing = true;
            this.FrameX = frameX;
            this.FrameY = frameY;
            this.IsometricFrameX = isometricFrameX;
            this.IsometricFrameY = isometricFrameY;
            this._width = _width;
            this._height = height;
            ChunkData = new Grid<Tile>(_width, height, 1.0f);
     
            // Init empty chunk tile data
            await Task.Run(() =>
            {
                for (byte x = 0; x < _width; x++)
                {
                    for (byte y = 0; y < _height; y++)
                    {
                        Tile tile = new Tile(x, y);
                        ChunkData.SetValue(x, y, tile);
                    }
                }
            });
            Processing = false;
        }
   
        public void LoadChunk()
        {
            gameObject.SetActive(true);
        }

   



        // Test
        // ====
        public void PaintTileColor(Tile tile, Color color)
        {
            LandTilemap.SetColor(new Vector3Int(tile.FrameX, tile.FrameY), color);
        }
     


        public void SetTwoSidesChunkNeighbors(Chunk2D left, Chunk2D right, Chunk2D top, Chunk2D bottom)
        {
            this.Left = left;
            this.Right = right;
            this.Top = top;
            this.Bottom = bottom;

            if (left != null)
            {
                left.Right = this;
            }
            if (right != null)
            {
                right.Left = this;
            }
            if (top != null)
            {
                top.Bottom = this;
            }
            if (bottom != null)
            {
                bottom.Top = this;
            }
            HasChunkNeighbors = Left != null && Right != null && Top != null && Bottom != null;
        }
        public bool HasNeighbors()
        {
            HasChunkNeighbors = Left != null && Right != null && Top != null && Bottom != null;
            return HasChunkNeighbors;
        }
        public void UpdateAllTileNeighbors()
        {
            Debug.Log("UpdateAllTileNeighbors");
            AllTileHasNeighbors = true;
            for (var x = 0; x < _width; x++)
            {
                for (var y = 0; y < _height; y++)
                {
                    Tile t = ChunkData.GetValue(x, y);

                    t.Top = GetTop(t);
                    t.Bottom = GetBottom(t);
                    t.Left = GetLeft(t);
                    t.Right = GetRight(t);

                    if (t.HasNeighbors() == false)
                        AllTileHasNeighbors = false;
                }
            }
        }
        public async Task UpdateAllTileNeighborsAsync()
        {
            Processing = true;
            Debug.Log("UpdateAllTileNeighborsAsync");
            AllTileHasNeighbors = true;
            await Task.Run(() =>
            {
                for (var x = 0; x < _width; x++)
                {
                    for (var y = 0; y < _height; y++)
                    {
                        Tile t = ChunkData.GetValue(x, y);

                        t.Top = GetTop(t);
                        t.Bottom = GetBottom(t);
                        t.Left = GetLeft(t);
                        t.Right = GetRight(t);

                        if (t.HasNeighbors() == false)
                            AllTileHasNeighbors = false;
                    }
                }
            });
            Processing = false;
        }
        public void UpdateEdgeOfChunkTileNeighbors()
        {
            Debug.Log("UpdateEdgeTileNeighbors");

            // Loop through the first and last rows
            for (int col = 0; col < _width; col++)
            {
                Tile t1 = ChunkData.GetValue(0, col);
                t1.Top = GetTop(t1);
                t1.Bottom = GetBottom(t1);
                t1.Left = GetLeft(t1);
                t1.Right = GetRight(t1);

                Tile t2 = ChunkData.GetValue(_height - 1, col);
                t2.Top = GetTop(t2);
                t2.Bottom = GetBottom(t2);
                t2.Left = GetLeft(t2);
                t2.Right = GetRight(t2);
            }

            // Loop through the first and last columns (excluding corners)
            for (int row = 1; row < _height - 1; row++)
            {
                Tile t1 = ChunkData.GetValue(row, 0);
                t1.Top = GetTop(t1);
                t1.Bottom = GetBottom(t1);
                t1.Left = GetLeft(t1);
                t1.Right = GetRight(t1);

                Tile t2 = ChunkData.GetValue(row, _width - 1);
                t2.Top = GetTop(t2);
                t2.Bottom = GetBottom(t2);
                t2.Left = GetLeft(t2);
                t2.Right = GetRight(t2);
            }
        }



        #region Find Tile Neighbors
        public Tile GetTop(Tile t)
        {
            int newY = t.FrameY + 1;

            if (newY >= 0 && newY < _height)
            {
                return ChunkData.GetValue(t.FrameX, newY);
            }
            else if (newY == _height)
            {
                if (Top != null)
                {
                    return Top.ChunkData.GetValue(t.FrameX, 0);
                }
            }
            return null; // Handle the case where the index is out of range.   
        }
        public Tile GetBottom(Tile t)
        {
            int newY = t.FrameY - 1;
            if (newY >= 0 && newY < _height)
            {
                return ChunkData.GetValue(t.FrameX, newY);
            }
            else if (newY == -1)
            {
                if (Bottom != null)
                {
                    return Bottom.ChunkData.GetValue(t.FrameX, _height - 1);
                }
            }
            return null; // Handle the case where the index is out of range.
        }
        public Tile GetLeft(Tile t)
        {
            int newX = t.FrameX - 1;
            if (newX >= 0 && newX < _width)
            {
                return ChunkData.GetValue(newX, t.FrameY);
            }
            else if (newX == -1)
            {
                if (Left != null)
                {
                    return Left.ChunkData.GetValue(_width - 1, t.FrameY);
                }
            }
            return null; // Handle the case where the index is out of range.
        }
        public Tile GetRight(Tile t)
        {
            int newX = t.FrameX + 1;
            if (newX >= 0 && newX < _width)
            {
                return ChunkData.GetValue(newX, t.FrameY);
            }
            else if (newX == _width)
            {
                if (Right != null)
                {
                    return Right.ChunkData.GetValue(0, t.FrameY);
                }
            }
            return null; // Handle the case where the index is out of range.
        }


        public Tile GetTopWithinChunk(Tile t)
        {
            int newY = t.FrameY + 1;

            if (newY >= 0 && newY < _height)
            {
                return ChunkData.GetValue(t.FrameX, newY);
            }
            return null; // Handle the case where the index is out of range.   
        }
        public Tile GetBottomWithinChunk(Tile t)
        {
            int newY = t.FrameY - 1;
            if (newY >= 0 && newY < _height)
            {
                return ChunkData.GetValue(t.FrameX, newY);
            }
            return null; // Handle the case where the index is out of range.
        }
        public Tile GetLeftWithinChunk(Tile t)
        {
            int newX = t.FrameX - 1;
            if (newX >= 0 && newX < _width)
            {
                return ChunkData.GetValue(newX, t.FrameY);
            }
            return null; // Handle the case where the index is out of range.
        }
        public Tile GetRightWithinChunk(Tile t)
        {
            int newX = t.FrameX + 1;
            if (newX >= 0 && newX < _width)
            {
                return ChunkData.GetValue(newX, t.FrameY);
            }
            return null; // Handle the case where the index is out of range.
        }
        #endregion
        public void SetTile(Vector2 worldPosition, TileBase tileBase)
        {
            Vector3Int cellPosition = Grid.WorldToCell(worldPosition);
            cellPosition.z = 0;
            LandTilemap.SetTile(cellPosition, tileBase);
        }
        public void SetTile(byte frameX, byte frameY, TileBase tileBase)
        {
            LandTilemap.SetTile(new Vector3Int(frameX, frameY, 0), tileBase);
        }
        public Tile GetTile(byte frameX, byte frameY)
        {
            return ChunkData.GetValue(frameX, frameY);
        }
        public Vector3 GetTileWorldPosition(byte frameX, byte frameY)
        {
            return Grid.CellToWorld(new Vector3Int(frameX, frameY, 0));
        }
        public void SetDrawRenderMode(TilemapRenderer.Mode mode)
        {
            LandTilemap.GetComponent<TilemapRenderer>().mode = mode;
        }


     

        //#region Testing 
        //public Canvas Canvas;
        //public TextMeshProUGUI TextPrefab;
        //public void ShowTextTest()
        //{
        //    return;
        //    Canvas = GameObject.Find("Canvas_1").GetComponent<Canvas>();
        //    for (byte x = 0; x < _width; ++x)
        //    {
        //        for (byte y = 0; y < _height; ++y)
        //        {
        //            TextMeshProUGUI isoCoordText = Instantiate(TextPrefab, Canvas.transform);
        //            isoCoordText.hideFlags = HideFlags.HideInInspector;

        //            //Vector3 position = GetTileWorldPosition(x, y);
        //            //isoCoordText.text = $"[{position.x} , {position.y}]";

        //            Vector3 position = GetTileWorldPosition(x, y);
        //            isoCoordText.text = $"[{x + (IsometricFrameX * _width)} , {y + (IsometricFrameY * _height)}]";

        //            isoCoordText.fontSize = 15f;
        //            isoCoordText.transform.position = GetTileWorldPosition(x, y) + new Vector3(0,0.5f);
        //        }
        //    }
        //}
        //public TextMeshProUGUI CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 10, Color? color = null)
        //{
        //    if (color == null) color = Color.white;
        //    return CreateWorldText(parent, text, localPosition, fontSize, (Color)color);
        //}

        //// Create Text in the World
        //public TextMeshProUGUI CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color)
        //{
        //    TextMeshProUGUI textMesh = Instantiate(TextPrefab);
        //    textMesh.transform.SetParent(transform, false);
        //    textMesh.transform.position = localPosition;
        //    textMesh.text = text;
        //    textMesh.fontSize = fontSize;
        //    textMesh.color = color;
        //    return textMesh;
        //}  
        //#endregion
    }
}

