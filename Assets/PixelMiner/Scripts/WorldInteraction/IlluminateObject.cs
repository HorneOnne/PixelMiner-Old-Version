using UnityEngine;
using PixelMiner.Core;

namespace PixelMiner.WorldInteraction
{
    public class IlluminateObject : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        private Material _mat;

        private byte _blockLight;
        private byte _ambientLight;
        private float _ambientLightIntensity;
        private float _timer;
        private float _updateFrequency = 0.02f;

        private void Start()
        {
            _mat = _meshRenderer.material;       
        }

        private void Update()
        {          
            if (Time.time - _timer > _updateFrequency)
            {
                _timer = Time.time;

                _blockLight = Main.Instance.GetBlockLight(transform.position);
                _ambientLight = Main.Instance.GetAmbientLight(transform.position);
                _ambientLightIntensity = Main.Instance.GetAmbientLightIntensity();

                _mat.SetInt("_BlockLightValue", _blockLight);
                _mat.SetInt("_AmbientLightValue", _ambientLight);
                _mat.SetFloat("_AmbientIntensity", _ambientLightIntensity);
            } 
        }
    }

}
