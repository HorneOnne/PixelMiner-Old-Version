using UnityEngine;
using PixelMiner.WorldGen;
using PixelMiner.Enums;
using PixelMiner.Utilities;
using Mono.CSharp;

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
        }


        [SerializeField] private Vector2 _predictMovePosition;
        [SerializeField] private Vector2 _predictMovePosition2;
        [SerializeField] private Vector2 _predictMovePosition3;
        [SerializeField] private HeightType _heightType;
        public Vector2 direction;
        private Tile _currentTile;
        private Tile _predictTile;



        private void Update()
        {
            _currentTile = Main.Instance.GetTile(transform.position, out Chunk chunk);
            Main.Instance.SetTileColor(transform.position, Color.red);


            //_predictMovePosition = Main.Instance.GetNeighborWorldPosition(transform.position, _input.Move);
            _predictMovePosition = (Vector2)transform.position + (_input.Move * 0.2f);
            
            if (_input.Move.x != 0 || _input.Move.y != 0)
            {
                SmartMove();
            }

         
            // Animation
            // =========
            Flip(_input.Move);
            if (_hasAnimator)
            {
                _anim.SetFloat(_animIDVelocityX, _input.Move.x);
                _anim.SetFloat(_animIDVelocityY, _input.Move.y);
            }
        }
        private void FixedUpdate()
        {           
            if (_predictTile != null && (
                _predictTile.HeightType == HeightType.DeepWater ||
                _predictTile.HeightType == HeightType.ShallowWater ||
                _predictTile.HeightType == HeightType.River))
            {

                
                Vector2 pos = transform.position;
           
                //_rb.position = pos;
                _rb.MovePosition(pos);

            }
            else
            {
                Movement();
            }
        }

        public Transform PredictObject;
        private void SmartMove()
        {
            _predictTile = Main.Instance.GetTile(_predictMovePosition, out Chunk chunk);
            Main.Instance.SetTileColor(_predictMovePosition, Color.blue);
            PredictObject.position = _predictMovePosition;

            if (_input.Move.x == 0 && _input.Move.y == 0)
                return;

            if (_input.Move.x > 0 && _input.Move.y == 0)
            {
                // Right
                _predictMovePosition2 = Main.Instance.GetNeighborWorldPosition(_predictMovePosition, MathHelper.UpLeftVector);
                _predictMovePosition3 = Main.Instance.GetNeighborWorldPosition(_predictMovePosition, MathHelper.DownLeftVector);
            }
            else if (_input.Move.x < 0 && _input.Move.y == 0)
            {
                // Left
                _predictMovePosition2 = Main.Instance.GetNeighborWorldPosition(_predictMovePosition, MathHelper.UpRightVector);
                _predictMovePosition3 = Main.Instance.GetNeighborWorldPosition(_predictMovePosition, MathHelper.DownRightVector);
            }
            else if (_input.Move.x == 0 && _input.Move.y > 0)
            {
                // Up
                _predictMovePosition2 = Main.Instance.GetNeighborWorldPosition(_predictMovePosition, MathHelper.DownLeftVector);
                _predictMovePosition3 = Main.Instance.GetNeighborWorldPosition(_predictMovePosition, MathHelper.DownRightVector);
            }
            else if (_input.Move.x == 0 && _input.Move.y < 0)
            {
                // Down
                _predictMovePosition2 = Main.Instance.GetNeighborWorldPosition(_predictMovePosition, MathHelper.UpLeftVector);
                _predictMovePosition3 = Main.Instance.GetNeighborWorldPosition(_predictMovePosition, MathHelper.UpRightVector);


            }
            else if (_input.Move.x > 0 && _input.Move.y > 0)
            {
                // Top Right
               

            }
            else if (_input.Move.x < 0 && _input.Move.y > 0)
            {
                // Top Left
              
            }

            else if (_input.Move.x > 0 && _input.Move.y < 0)
            {
                // Down Right
          

            }
            else if (_input.Move.x < 0 && _input.Move.y < 0)
            {
                // Down Left
              

            }


            Main.Instance.SetTileColor(_predictMovePosition2, Color.black);
            Main.Instance.SetTileColor(_predictMovePosition3, Color.black);

        }


        private void MyFormula(Vector2 p1, Vector2 p2, out Vector2 p3, out Vector2 p4)
        {
            if(p1.y == p2.y)
            {
                float a = (p1.x + p2.x) / 2.0f;
                float c = p1.y + 0.5f;
                float d = p1.y - 0.5f;

                p3 = new Vector2(a,c);
                p4 = new Vector2(a,d);
            }
            else if(p1.x == p2.x)
            {
                float a = (p1.y + p2.y) / 2.0f;
                float c = p1.y + 0.5f;
                float d = p1.y - 0.5f;

                p3 = new Vector2(c, a);
                p4 = new Vector2(d, a);
            }
            else
            {
                p3 = Vector2.zero;
                p4 = Vector2.zero;
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
