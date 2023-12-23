using PixelMiner.WorldBuilding;
using PixelMiner.Core;
using System.Threading.Tasks;
using UnityEngine;

namespace PixelMiner
{
    public class IlluminateObject : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;

        private Color32[] _vertexColors;
        private Mesh _mesh;
        private byte _light;

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
            }
            else
            {
                Debug.LogWarning("MeshFilter component not found.");
            }
        }

        private void Update()
        {
            if (Time.time - _timer > _updateFrequency)
            {
                _timer = Time.time;

                Vector3Int position = new Vector3Int(Mathf.FloorToInt(transform.position.x),
                                                 Mathf.FloorToInt(transform.position.y + 0.001f),   // Add some threshold to y
                                                 Mathf.FloorToInt(transform.position.z));

                byte currentLightLevel = Main.Instance.GetBlockLight(position);

                if (currentLightLevel >= 0 && currentLightLevel <= 16 && (_light != currentLightLevel || _light + 1 != currentLightLevel))
                {
                    _light = currentLightLevel;
                    UpdateLightColorAsync();
                }
            } 
        }



        private async void UpdateLightColorAsync()
        {
            await Task.Run(() =>
            {
                Color32 lightColor = LightUtils.GetLightColor(_light);
                Parallel.For(0, _vertexColors.Length, (i) =>
                {
                    _vertexColors[i] = lightColor;
                   
                });
            });
            _mesh.colors32 = _vertexColors;
        }
    }

}
