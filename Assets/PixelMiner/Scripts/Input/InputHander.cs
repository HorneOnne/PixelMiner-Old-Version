using UnityEngine;

namespace PixelMiner
{
    public class InputHander : MonoBehaviour
    {
        public static InputHander Instance { get; private set; }
 
        private PlayerInput playerInput;

        [Header("Character Input Values")]
        public Vector2 Move;
        public float MouseScrollY;
        public bool Cancel;
        public float Fire;

        private void Awake()
        {
            Instance = this;
            playerInput = new PlayerInput();

            playerInput.Player.Move.started += x => { Move = x.ReadValue<Vector2>(); };
            playerInput.Player.Move.performed += x => { Move = x.ReadValue<Vector2>(); };
            playerInput.Player.Move.canceled += x => { Move = x.ReadValue<Vector2>(); };


            playerInput.Player.Cancel.started += x => { Cancel = x.ReadValue<float>() == 1 ? true: false ; };
            playerInput.Player.Cancel.canceled += x => { Cancel = x.ReadValue<float>() == 1 ? true : false; };


            playerInput.Player.Fire.performed += x => { Fire = x.ReadValue<float>(); };

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
#endif

        //public void MoveInput(Vector2 newMoveDirection)
        //{
        //    Move = newMoveDirection;
        //}
    }
}
