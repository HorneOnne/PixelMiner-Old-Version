using PixelMiner.Enums;
using PixelMiner.Time;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PixelMiner.WorldInteraction;


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
        private WorldTime _worldTime;

        //
        private string _fpsString;
        private string _targetString;
        private string _blockString;
        private string _blockLightString;
        private string _ambientLightString;
        private string _worldTimeString;

        
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
            _worldTime = WorldTime.Instance;
        }

        private void UpdateLogText()
        {
            _fpsString = string.Format("FPS: {0:F2}  ({1:F2} m/s)", fps, msec);
            _targetString = $"Target: {_targetPosition}";
            _blockString = $"Block: {_targetBlock}";
            _blockLightString = $"Block Light: {_blockLight}";
            _ambientLightString = $"Ambient Light: {_ambientLight}";
            UpdateWorldTime();

            sb.Clear();
            sb.AppendLine(_fpsString);
            sb.AppendLine(_targetString);
            sb.AppendLine(_blockString);
            sb.AppendLine(_blockLightString);
            sb.AppendLine(_ambientLightString);
            sb.AppendLine(_worldTimeString);

            
            _debugLogText.text = $"{sb}";
        }

        private void Update()
        {
            deltaTime += (UnityEngine.Time.unscaledDeltaTime - deltaTime) * 0.1f;
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

        private void UpdateWorldTime()
        {
            _worldTimeString = $"Hours: ({_worldTime.Hours.ToString("00")}:{_worldTime.Minutes.ToString("00")})  Day({_worldTime.Days})";

        }
    }
}
