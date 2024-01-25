using PixelMiner.Cam;
using PixelMiner.Utilities;
using UnityEngine;

namespace PixelMiner
{
    public class PlayerMovement3D : MonoBehaviour
    {
        private CameraLogicHandler _cameraLogicHandler;
        private InputHander _input;
        private Rigidbody _rb;
        private Animator _anim;
        private SpriteRenderer _sr;


        [SerializeField] private float _moveSpeed;
        [SerializeField] private Vector3 _moveDirection;
        private Vector3 _cameraIsometricRot = new Vector3(0, 45, 0);

        public bool ContinuousMove;
        private bool _hasAnimator;
        private bool _facingRight;
        // animation IDs
        private int _animIDVelocityX;
        private int _animIDVelocityY;
        private int _animIDVelocity;


        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _anim = GetComponentInChildren<Animator>();
            _sr = GetComponentInChildren<SpriteRenderer>();
            _hasAnimator = _anim != null;
        }


        private void Start()
        {
            _input = InputHander.Instance;
            _cameraLogicHandler = CameraLogicHandler.Instance;
            AssignAnimationIDs();
        }


        private void Update()
        {
            if (ContinuousMove)
            {
                _moveDirection = _input.Move == Vector2.zero ? _moveDirection : new Vector3(_input.Move.x, 0, _input.Move.y);
            }
            else
            {
                _moveDirection.x = _input.Move.x;
                _moveDirection.z = _input.Move.y;
            }

            if (_input.Cancel)
            {
                _moveDirection = Vector3.zero;
            }

            UpdatePosition();
            //FaceToCamera();

            UpdateRotation();


            // Animation
            // =========
            //Flip(_moveDirection);
            if (_hasAnimator)
            {
                //_anim.SetFloat(_animIDVelocityX, _moveDirection.x);
                //_anim.SetFloat(_animIDVelocityY, _moveDirection.z);
                _anim.SetFloat(_animIDVelocity, _input.Move.magnitude);
            }
        }

        private void FixedUpdate()
        {
        

        }


        private void FaceToCamera()
        {
            transform.forward = _cameraLogicHandler.MainCam.transform.forward;
        }

        private void UpdatePosition()
        {
            Move(); 
        }

        private void Move()
        {
            Vector3 movement = _moveDirection.Iso(new Vector3(0, _cameraLogicHandler.CurrentYRotAngle, 0)) * _moveSpeed * UnityEngine.Time.deltaTime; ;
            _rb.MovePosition(_rb.position + movement);
            //if (_moveDirection != Vector3.zero)
            //{
            //    _rb.velocity = _moveDirection.Iso(new Vector3(0, _cameraLogicHandler.CurrentYRotAngle, 0)) * _moveSpeed * UnityEngine.Time.fixedDeltaTime;
            //}
            //else
            //{
            //    _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
            //}

        }

        private void UpdateRotation()
        {
            RotateTowardMoveDirection(_moveDirection.Iso(new Vector3(0, _cameraLogicHandler.CurrentYRotAngle, 0)));
        }

        private void RotateTowardMoveDirection(Vector3 direction)
        {
            if (direction.sqrMagnitude >= 0.1f)
            {
                // Calculate the rotation to look towards the move direction
                Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);

                // Apply the rotation to the character
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, UnityEngine.Time.deltaTime * 10f);
            }
        }


        private void AssignAnimationIDs()
        {
            _animIDVelocityX = Animator.StringToHash("VelocityX");
            _animIDVelocityY = Animator.StringToHash("VelocityY");
            _animIDVelocity = Animator.StringToHash("Velocity");

        }

        private void Flip(Vector2 move)
        {
            if (move.x > 0)
            {
                _sr.flipX = true;
            }
            else if (move.x < 0)
            {
                _sr.flipX = false;
            }
        }


    }

}
