using PixelMiner.Utilities;
using UnityEngine;

namespace PixelMiner
{
    public class PlayerMovement3D : MonoBehaviour
    {
        private InputHander _input;
        private Rigidbody _rb;
        private Animator _anim;
        private SpriteRenderer _sr;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private Vector3 _moveDirection;
        private Camera _mainCam;
        private Vector3 _cameraIsometricRot = new Vector3(0,45,0);

        public bool ContinuousMove;
        private bool _hasAnimator;
        private bool _facingRight;
        // animation IDs
        private int _animIDVelocityX;
        private int _animIDVelocityY;

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
            _mainCam = Camera.main;
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


            FaceToCamera();
        }

        private void FixedUpdate()
        {
            if (_moveDirection != Vector3.zero)
                _rb.velocity = _moveDirection.Iso(_cameraIsometricRot) * _moveSpeed;
            else
                _rb.velocity = new Vector3(0,_rb.velocity.y, 0);


            // Animation
            // =========
            Flip(_moveDirection);
            if (_hasAnimator)
            {
                _anim.SetFloat(_animIDVelocityX, _moveDirection.x);
                _anim.SetFloat(_animIDVelocityY, _moveDirection.z);
            }
        }


        private void FaceToCamera()
        {
            transform.forward = _mainCam.transform.forward;
        }

        private void AssignAnimationIDs()
        {
            _animIDVelocityX = Animator.StringToHash("VelocityX");
            _animIDVelocityY = Animator.StringToHash("VelocityY");

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
