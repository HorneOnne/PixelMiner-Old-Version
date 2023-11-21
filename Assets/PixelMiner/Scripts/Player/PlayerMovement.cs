using UnityEngine;
using PixelMiner.WorldGen;

namespace PixelMiner
{
    public class PlayerMovement : MonoBehaviour
    {
        private InputHander _input;
        private Rigidbody2D _rb;
        private Animator _anim;
         private Transform _model;
        [SerializeField] private float moveSpeed;
        public Vector2 _moveDirection;

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
        [SerializeField] private Vector2 _predictMovePostition;
        [SerializeField] private Vector2 _predictMovePostition2;
        [SerializeField] private Vector2 _predictMovePostition3;
        [SerializeField] private HeightType _heightType;
        private Tile _currentTile;
        private Tile _predictTile;

        private void Update()
        {
            _predictMovePostition = (Vector2)transform.position + _input.Move;

            //_currentTile = Main.Instance.GetTile(transform.position);
            //_predictTile = Main.Instance.GetTile(_predictMovePostition);

            //_currentTile.SetColor(Color.red);
            //_predictTile.SetColor(Color.blue);

            //if(_predictTile != null)
            //{
            //    // Predict
            //    if (_input.Move.x != 0 && _input.Move.y == 0)
            //    {
            //        //_predictTile.Top.SetColor(Color.green);
            //        //_predictTile.Bottom.SetColor(Color.green);

            //    }
            //    else if (_input.Move.x == 0 && _input.Move.y != 0)
            //    {

            //    }
            //    else if (_input.Move.x != 0 && _input.Move.y != 0)
            //    {

            //    }
            //    else
            //    {

            //    }
            //}




            if (_predictTile != null && (
                _predictTile.HeightType == HeightType.DeepWater || 
                _predictTile.HeightType == HeightType.ShallowWater ||
                _predictTile.HeightType == HeightType.River))
            {
                _previousPostition = transform.position;
            }

        }
        private void FixedUpdate()
        {
            //if(_currentTile != null && (_currentTile.HeightType != HeightType.DeepWater && _currentTile.HeightType != HeightType.ShallowWater && _currentTile.HeightType != HeightType.River))
            //{
            //    Movement();
            //}
            //else
            //{
            //    _rb.MovePosition(_previousPostition);
            //}

            if (_predictTile != null && (
                _predictTile.HeightType == HeightType.DeepWater ||
                _predictTile.HeightType == HeightType.ShallowWater ||
                _predictTile.HeightType == HeightType.River))
            {
                _rb.MovePosition(_previousPostition);
            }
            else
            {
                Movement();
            }
        }

        private void AssignAnimationIDs()
        {
            _animIDVelocityX = Animator.StringToHash("VelocityX");
            _animIDVelocityY = Animator.StringToHash("VelocityY");

        }

       
        private void Movement()
        {
            _moveDirection = _input.Move;
            // Move diagonally
            if (_input.Move.x != 0 && _input.Move.y != 0)
            {
                _moveDirection = ConvertDiagonalVectorToDimetricProjection(_input.Move, WorldGeneration.Instance.IsometricAngle);
            }
    
            _rb.velocity = _moveDirection * moveSpeed;


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

        /// <summary>
        /// Converts a 2D diagonal vector (45 degrees) in a manner consistent with dimetric projection based on the specified angle.
        /// </summary>
        private Vector2 ConvertDiagonalVectorToDimetricProjection(Vector2 originalVector, float desiredAngle)
        {
            float radians = Mathf.Deg2Rad * desiredAngle;
            float magnitude = originalVector.magnitude;

            // Calculate the new vector components, considering the signs of the original components
            float newX = originalVector.x >= 0 ? magnitude * Mathf.Cos(radians) : -magnitude * Mathf.Cos(radians);
            float newY = originalVector.y >= 0 ? magnitude * Mathf.Sin(radians) : -magnitude * Mathf.Sin(radians);
            return new Vector2(newX, newY);
        }



#if DEV_MODE
        public void SetPlayerSpeed(float value)
        {
            this.moveSpeed = value;
        }
#endif
    }

}
