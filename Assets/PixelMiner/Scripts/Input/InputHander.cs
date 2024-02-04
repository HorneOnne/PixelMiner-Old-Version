using UnityEngine;
using UnityEngine.InputSystem;
namespace PixelMiner
{
    public class InputHander : MonoBehaviour
    {
        public static InputHander Instance { get; private set; }
        public event System.Action<float> OnRotateDetected;

        private PlayerInput playerInput;

        [Header("Character Input Values")]
        public Vector2 Move;
        public Vector2 Look;
        public float MouseScrollY;
        public bool Cancel;
        public bool Fire1;
        public bool Fire2;
        public bool Fire3;
        public float Rot;


        private void Awake()
        {
            Instance = this;
            playerInput = new PlayerInput();

            playerInput.Player.Move.started += x => { Move = x.ReadValue<Vector2>().normalized; };
            playerInput.Player.Move.performed += x => { Move = x.ReadValue<Vector2>().normalized; };
            playerInput.Player.Move.canceled += x => { Move = x.ReadValue<Vector2>().normalized; };


            playerInput.Player.Look.started += x => { Look = x.ReadValue<Vector2>(); };
            playerInput.Player.Look.performed += x => { Look = x.ReadValue<Vector2>(); };
            playerInput.Player.Look.canceled += x => { Look = x.ReadValue<Vector2>(); };


            playerInput.Player.Cancel.started += x => { Cancel = x.ReadValue<float>() == 1 ? true: false ; };
            playerInput.Player.Cancel.canceled += x => { Cancel = x.ReadValue<float>() == 1 ? true : false; };


            playerInput.Player.Fire1.started += x => { Fire1 = x.ReadValue<float>() == 1 ? true : false; };
            playerInput.Player.Fire1.performed += x => { Fire1 = x.ReadValue<float>() == 1 ? true : false; };
            playerInput.Player.Fire1.canceled += x => { Fire1 = x.ReadValue<float>() == 1 ? true : false; };


            playerInput.Player.Rotate.performed += OnRotatePerformed;

            playerInput.UI.ScrollWheel.performed += x => { MouseScrollY = x.ReadValue<Vector2>().y; };
        }


        #region Enable / Disable
        private void OnEnable()
        {
            playerInput.Enable();
        
        }

        private void OnDisable()
        {
            playerInput.Disable();
        }
        #endregion


        private void Start()
        {
            ActivePlayerMap();
        }

        public void ActivePlayerMap()
        {
            playerInput.UI.Disable();
            playerInput.Player.Enable();
        }
        public void ActiveUIMap()
        {
            playerInput.Player.Disable();
            playerInput.UI.Enable();
        }


#if ENABLE_INPUT_SYSTEM
        //public void OnMove(InputValue value)
        //{
        //    MoveInput(value.Get<Vector2>());
        //}

        private void OnRotatePerformed(InputAction.CallbackContext context)
        {
            //Debug.Log("OnRotatePerformed");
            Rot = context.ReadValue<float>();
            OnRotateDetected?.Invoke(Rot);
        }
#endif

        //public void MoveInput(Vector2 newMoveDirection)
        //{
        //    Move = newMoveDirection;
        //}
    }
}
