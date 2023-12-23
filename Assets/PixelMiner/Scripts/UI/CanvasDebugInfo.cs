using PixelMiner.Enums;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace PixelMiner.UI
{
    public class CanvasDebugInfo : CustomCanvas
    {
        [SerializeField] private TextMeshProUGUI _debugLogText;
        float deltaTime = 0.0f;
        float msec;
        float fps;
        private Vector3Int _targetPosition;
        private BlockType _targetBlock;
        private byte _blockLight;
        private byte _ambientLight;

        private StringBuilder sb = new StringBuilder();

        //
        private string _fpsString;
        private string _targetString;
        private string _blockString;
        private string _blockLightString;
        private string _ambientLightString;

        private void OnEnable()
        {
            Tool.OnTarget += UpdateTarget;
        }

        private void OnDisable()
        {
            Tool.OnTarget -= UpdateTarget;
        }

        private void Start()
        {
            InvokeRepeating(nameof(UpdateLogText), 1.0f, 0.02f);    
        }

        private void UpdateLogText()
        {
            _fpsString = string.Format("FPS: {0:F2}  ({1:F2} m/s)", fps, msec);
            _targetString = $"Target: {_targetPosition}";
            _blockString = $"Block: {_targetBlock}";
            _blockLightString = $"Block Light: {_blockLight}";
            _ambientLightString = $"Ambient Light: {_ambientLight}";

            sb.Clear();
            sb.AppendLine(_fpsString);
            sb.AppendLine(_targetString);
            sb.AppendLine(_blockString);
            sb.AppendLine(_blockLightString);
            sb.AppendLine(_ambientLightString);

            
            _debugLogText.text = $"{sb}";
        }

        private void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            msec = deltaTime * 1000.0f;
            fps = 1.0f / deltaTime;
        }

        private void UpdateTarget(Vector3Int target, BlockType blockType, byte blockLight, byte ambientLight)
        {
            this._targetPosition = target;
            this._targetBlock = blockType;
            this._blockLight = blockLight;
            this._ambientLight = ambientLight;
        }
    }
}
