using UnityEngine;

namespace PixelMiner
{
    public class PlayerMovement : MonoBehaviour
    {
        private InputHander _input;
        private Rigidbody2D _rb;
        private Animator _anim;
         private Transform _model;
        [SerializeField] private float moveSpeed;

      
        private bool _hasAnimator;
        private bool _facingRight;

        // animation IDs
        private int _animIDVelocityX;
        private int _animIDVelocityY;


        private void Awake()
        {
            _model = transform.Find("Model");
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponentInChildren<Animator>();

            _hasAnimator = _anim != null;
        }
        void Start()
        {
            _input = InputHander.Instance;
            AssignAnimationIDs();

            _previousPostition = transform.position;
        }

        [SerializeField] private Vector2 _previousPostition;
        [SerializeField] private HeightType _heightType;
        private Tile _currentTile;

        private void Update()
        {
            _currentTile = Main.Instance.GetTile(transform.position);
            if (_currentTile != null && (_currentTile.HeightType != HeightType.DeepWater && _currentTile.HeightType != HeightType.ShallowWater && _currentTile.HeightType != HeightType.River))
            {
                //_previousPostition = Main.Instance.GetWorldTilePosition(transform.position);
                _previousPostition = transform.position;
            }

        }
        private void FixedUpdate()
        {
            //Movement();
      
            if(_currentTile != null && (_currentTile.HeightType != HeightType.DeepWater && _currentTile.HeightType != HeightType.ShallowWater && _currentTile.HeightType != HeightType.River))
            {

                Movement();
            }
            else
            {
                //_rb.position = Main.Instance.GetWorldTilePosition(_previousPostition);
                _rb.MovePosition(_previousPostition);
            }
        }

        private void AssignAnimationIDs()
        {
            _animIDVelocityX = Animator.StringToHash("VelocityX");
            _animIDVelocityY = Animator.StringToHash("VelocityY");

        }

        private void Movement()
        {
            _rb.velocity = _input.Move * moveSpeed;

            // Assuming _input.Move is a Vector2 input from the player
            //Vector2 isometricMove = ConvertToIsometric(_input.Move);
            //_rb.velocity = isometricMove * moveSpeed;


            Flip(_input.Move);
            if (_hasAnimator)
            {
                _anim.SetFloat(_animIDVelocityX, _input.Move.x);
                _anim.SetFloat(_animIDVelocityY, _input.Move.y);
            }
        }

        private void Flip(Vector2 move)
        {
            if (move.x > 0)
            {
                _model.localScale = new Vector3(-1, 1, 1);
            }
            else if (move.x < 0)
            {
                _model.localScale = new Vector3(1, 1, 1);
            }
        }



        #region Isometric helper
        // Function to convert Cartesian coordinates to isometric coordinates
        private Vector2 ConvertToIsometric(Vector2 cartesianInput)
        {
            // Assuming your isometric tile has a 45-degree angle
            float angle = 30f;

            // Convert degrees to radians
            float radians = Mathf.Deg2Rad * angle;

            // Calculate the isometric transformation matrix
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);

            // Apply the transformation matrix to convert Cartesian to isometric coordinates
            float isoX = cartesianInput.x * cos - cartesianInput.y * sin;
            float isoY = cartesianInput.x * sin + cartesianInput.y * cos;

            return new Vector2(isoX, isoY).normalized; // Normalize to ensure consistent movement speed in all directions
        }
        #endregion

#if DEV_MODE
        public void SetPlayerSpeed(float value)
        {
            this.moveSpeed = value;
        }
#endif
    }

}
