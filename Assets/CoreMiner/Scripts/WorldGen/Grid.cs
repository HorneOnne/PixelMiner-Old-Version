using UnityEngine;

namespace CoreMiner.Utilities
{
    public class Grid<T>
    {
        public static event System.Action<Vector3> OnGridChanged;

        private int width;
        private int height;
        private float cellSize;
        private Vector3 originPosition;
        private T[,] gridMap;

        public Grid(int width, int height, float cellSize, Vector3 originPosition = default(Vector3))
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.originPosition = originPosition;

            gridMap = new T[width, height];

            bool showDebug = false;
            TextMesh[,] debugTextArray = new TextMesh[width, height];
            if (showDebug)
            {
                for (int x = 0; x < gridMap.GetLength(0); x++)
                {
                    for (int y = 0; y < gridMap.GetLength(1); y++)
                    {
                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                    }
                }
                Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
            }
        }

        public void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
            y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, y) * cellSize + originPosition;
        }

        public void SetValue(int x, int y, T value)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                gridMap[x, y] = value;
                OnGridChanged?.Invoke(new Vector3(x, y));
            }
        }

        public void SetValue(Vector3 worldPosition, T value)
        {
            int x, y;
            GetXY(worldPosition, out x, out y);
            SetValue(x, y, value);
        }

        public T GetValue(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                return gridMap[x, y];
            }
            else
            {
                return default(T);
            }
        }

        public T GetValue(Vector3 worldPosition)
        {
            int x, y;
            GetXY(worldPosition, out x, out y);
            return GetValue(x, y);
        }
    }
}