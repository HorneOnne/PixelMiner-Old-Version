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
        public Vector2 LookHorizontal;
        public float LookVertical;
        public float MouseScrollY;
        public bool Cancel;
        public bool Fire0;
        public bool Fire1;
        public bool Fire2;
        public float Rot;
        public bool Jump;


        public float DirectionalHorbarInventory;
        public float AccessHorbarInventory;


        private void Awake()
        {
            Instance = this;
            playerInput = new PlayerInput();

            playerInput.Player.Move.started += x => { Move = x.ReadValue<Vector2>().normalized; };
            playerInput.Player.Move.performed += x => { Move = x.ReadValue<Vector2>().normalized; };
            playerInput.Player.Move.canceled += x => { Move = x.ReadValue<Vector2>().normalized; };


            playerInput.Player.LookHorizontal.started += x => { LookHorizontal = x.ReadValue<Vector2>(); };
            playerInput.Player.LookHorizontal.performed += x => { LookHorizontal = x.ReadValue<Vector2>(); };
            playerInput.Player.LookHorizontal.canceled += x => { LookHorizontal = x.ReadValue<Vector2>(); };

            playerInput.Player.LookVertical.started += x => { LookVertical = x.ReadValue<float>(); };
            playerInput.Player.LookVertical.performed += x => { LookVertical = x.ReadValue<float>(); };
            playerInput.Player.LookVertical.canceled += x => { LookVertical = x.ReadValue<float>(); };



            playerInput.Player.Cancel.started += x => { Cancel = x.ReadValue<float>() == 1 ? true: false ; };
            playerInput.Player.Cancel.canceled += x => { Cancel = x.ReadValue<float>() == 1 ? true : false; };


            playerInput.Player.Fire0.started += x => { Fire0 = x.ReadValue<float>() > 0.5f ? true : false; };
            playerInput.Player.Fire0.performed += x => { Fire0 = x.ReadValue<float>() > 0.5f ? true : false; };
            playerInput.Player.Fire0.canceled += x => { Fire0 = x.ReadValue<float>() > 0.5f ? true : false; };


            playerInput.Player.Fire1.started += x => { Fire1 = x.ReadValue<float>() > 0.5f ? true : false; };
            playerInput.Player.Fire1.performed += x => { Fire1 = x.ReadValue<float>() > 0.5f ? true : false; };
            playerInput.Player.Fire1.canceled += x => { Fire1 = x.ReadValue<float>() > 0.5f ? true : false; };





            playerInput.Player.Jump.started += x => { Jump = x.ReadValue<float>() == 1 ? true : false; };
            playerInput.Player.Jump.performed += x => { Jump = x.ReadValue<float>() == 1 ? true : false; };
            playerInput.Player.Jump.canceled += x => { Jump = x.ReadValue<float>() == 1 ? true : false; };


            playerInput.Player.InventoryHotbarDirectional.started += x => { DirectionalHorbarInventory = x.ReadValue<float>(); };
            playerInput.Player.InventoryHotbarDirectional.performed += x => { DirectionalHorbarInventory = x.ReadValue<float>(); };
            playerInput.Player.InventoryHotbarDirectional.canceled += x => { DirectionalHorbarInventory = x.ReadValue<float>(); };


            playerInput.Player.InventoryHotbar.started += x => { AccessHorbarInventory = x.ReadValue<float>(); };
            playerInput.Player.InventoryHotbar.performed += x => { AccessHorbarInventory = x.ReadValue<float>(); };
            playerInput.Player.InventoryHotbar.canceled += x => { AccessHorbarInventory = x.ReadValue<float>(); };


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
