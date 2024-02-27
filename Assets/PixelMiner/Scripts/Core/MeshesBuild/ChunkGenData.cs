namespace PixelMiner.Core
{
    public class ChunkGenData
    {
        public float[] HeightValues;
        public float[] HeatValues;
        public float[] MoistureValues;
        public float[] RiverValues;
        public int[] RiverDensity;
        public UnityEngine.Vector3Int[] RiverBfsNeighbors;
        private bool _isInit;

        public ChunkGenData()
        {
            UnityEngine.Debug.Log("Create ChunkGenData");
            _isInit = false;
        }

        public void Init(int width, int height, int depth)
        {
            if (_isInit) return;

            HeightValues = new float[width * depth];
            HeatValues = new float[width * depth];
            MoistureValues = new float[width * depth];
            RiverValues = new float[width * depth];
            RiverDensity = new int[width * height * depth];
            RiverBfsNeighbors = new UnityEngine.Vector3Int[5];

            _isInit = true; 
        }


        public void Reset()
        {
            System.Array.Clear(HeightValues, 0, HeightValues.Length);
            System.Array.Clear(HeatValues, 0, HeatValues.Length);
            System.Array.Clear(MoistureValues, 0, MoistureValues.Length);
            System.Array.Clear(RiverValues, 0, RiverValues.Length);
            System.Array.Clear(RiverDensity, 0, RiverDensity.Length);
        }
    }
}
