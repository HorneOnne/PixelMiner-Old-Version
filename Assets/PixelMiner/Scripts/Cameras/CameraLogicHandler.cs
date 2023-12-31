using UnityEngine;
using Cinemachine;


namespace PixelMiner.Cam
{
    public class CameraLogicHandler : MonoBehaviour
    {
        public static CameraLogicHandler Instance {  get; private set; }    

        private InputHander _input;
        [SerializeField] private CinemachineVirtualCamera _isometricCam;
        [SerializeField] private CinemachineVirtualCamera _topDownCam;

        public float CurrentYRotAngle { get; private set; }
        public UnityEngine.Camera MainCam{get; private set;}
        public CameraViewStyle Style;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            CameraSwitcher.Register(_isometricCam);
            CameraSwitcher.Register(_topDownCam);
        }

        private void OnDisable()
        {
            CameraSwitcher.Unregister(_isometricCam);
            CameraSwitcher.Unregister(_topDownCam);
        }

        void Start()
        {
            CameraSwitcher.SwitchCamera(_isometricCam);
            MainCam = UnityEngine.Camera.main;
            if (_isometricCam == null)
            {
                // Try to find virtual camera.
                _isometricCam = FindFirstObjectByType<CinemachineVirtualCamera>();
                if ( _isometricCam == null )
                {
                    Debug.LogError("Virtual camera is null. Please assign a virtual camera.");
                }                 
            }

            _input = InputHander.Instance;
            _input.OnRotateDetected += OnRotate;

            Style = CameraViewStyle.IsometricOrthographic;
            CurrentYRotAngle = CameraSwitcher.ActiveCam.transform.eulerAngles.y;
        }

        private void OnDestroy()
        {
            _input.OnRotateDetected -= OnRotate;
        }


        private void OnRotate(float rot)
        {
            if (_input.Rot == -1)
            {
                CurrentYRotAngle += 45;
            }
            else if (_input.Rot == 1)
            {
                CurrentYRotAngle -= 45;
            }

            // Clamp the _currentYRotAngle within the range -360 to 360
            CurrentYRotAngle = (CurrentYRotAngle + 360) % 360;
            if (CurrentYRotAngle > 180)
            {
                CurrentYRotAngle -= 360;
            }

            if(CurrentYRotAngle % 90 == 0)
            {
                CameraSwitcher.SwitchCamera(_topDownCam);
                CameraSwitcher.ActiveCam.transform.eulerAngles = new Vector3(30, CurrentYRotAngle, 0);
            }
            else
            {
                CameraSwitcher.SwitchCamera(_isometricCam);
                CameraSwitcher.ActiveCam.transform.eulerAngles = new Vector3(30, CurrentYRotAngle, 0);
            }
        }


        public enum CameraViewStyle
        {
            TopDownPerspective, IsometricOrthographic
        }
    }
}
