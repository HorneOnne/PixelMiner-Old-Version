using System.Threading.Tasks;
using UnityEngine;
using PixelMiner.Core;
using PixelMiner.Lighting;


namespace PixelMiner.WorldInteraction
{
    public class IlluminateObject : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;

        private Color32[] _vertexColors;
        private Vector2[] _uv3s;
        private Mesh _mesh;
        private byte _blockLight;
        private byte _ambientLight;

        private float _timer;
        private float _updateFrequency = 0.2f;

        private void Start()
        {
            if (_meshFilter != null)
            {
                _mesh = _meshFilter.sharedMesh;
                if (_mesh.vertices.Length > 0)
                {
                    _vertexColors = new Color32[_mesh.vertices.Length];
                    //Debug.Log($"object has {_vertexColors.Length} vertices.");
                }
                else
                {
                    Debug.LogWarning("Mesh has no vertices.");
                }

                _mesh.SetUVs(2, _uv3s);
            }
            else
            {
                Debug.LogWarning("MeshFilter component not found.");
            }
        }

        private void Update()
        {
            if (UnityEngine.Time.time - _timer > _updateFrequency)
            {
                _timer = UnityEngine.Time.time;

                Vector3Int position = new Vector3Int(Mathf.FloorToInt(transform.position.x),
                                                 Mathf.FloorToInt(transform.position.y + 0.001f),   // Add some threshold to y
                                                 Mathf.FloorToInt(transform.position.z));

                _blockLight = Main.Instance.GetBlockLight(position);
                UpdateBlockLightColorAsync();

                //if (currentLightBlockLevel >= 0 && currentLightBlockLevel < 16 && (_blockLight != currentLightBlockLevel || _blockLight + 1 != currentLightBlockLevel))
                //{
                //    _blockLight = currentLightBlockLevel;
                //    UpdateBlockLightColorAsync();
                //}
            } 
        }



        private async void UpdateBlockLightColorAsync()
        {
            await Task.Run(() =>
            {
                Color32 lightColor = LightUtils.GetLightColor(_blockLight);
                Parallel.For(0, _vertexColors.Length, (i) =>
                {
                    _vertexColors[i] = lightColor;
                   
                });
            });
            _mesh.colors32 = _vertexColors;
        }
    }

}
