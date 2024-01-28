using PixelMiner.Cam;
using PixelMiner.Utilities;
using UnityEngine;

namespace PixelMiner.Character
{
    public class PlayerMovement3D : MonoBehaviour
    {
        private CameraLogicHandler _cameraLogicHandler;
        private InputHander _input;
        private Rigidbody _rb;
        private Animator _anim;


        [SerializeField] private float _moveSpeed;
        [SerializeField] private Vector3 _moveDirection;
        private Vector3 _cameraIsometricRot = new Vector3(0, 45, 0);


        // Physics
        [SerializeField] private Vector3 _gravity;
        private Bounds _aabbBounds;
        [SerializeField] private Vector3 _aabbBox;




        // Anim
        private bool _hasAnimator;
            // animation IDs
        private int _animIDVelocity;


        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _anim = GetComponent<Animator>();
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
            UpdatePosition();

            if (_input.Fire1 == false)
            {
                UpdateRotation();
            }


            //ApplyGravity();


            // Animation
            // =========
            if (_hasAnimator)
            {
                _anim.SetFloat(_animIDVelocity, _input.Move.magnitude);
            }
        }

        private void FixedUpdate()
        {
        
        }



        private void UpdatePosition()
        {
            if (_input.Move != Vector2.zero && _input.Fire1 == false)
            {
                _moveDirection = new Vector3(_input.Move.x, 0, _input.Move.y);
                Move(_moveDirection.Iso(new Vector3(0, _cameraLogicHandler.CurrentYRotAngle, 0)));
            }    
        }

        private void Move(Vector3 direction)
        {
            Vector3 movement = direction * _moveSpeed * UnityEngine.Time.deltaTime;
            //_rb.MovePosition(_rb.position + movement);
            transform.position += movement;
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


        private void ApplyGravity()
        {
            transform.position += _gravity * UnityEngine.Time.deltaTime;
        }

        private void AssignAnimationIDs()
        {
            _animIDVelocity = Animator.StringToHash("Velocity");

        }
    }

}
