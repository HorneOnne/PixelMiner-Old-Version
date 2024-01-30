using PixelMiner.Cam;
using PixelMiner.Utilities;
using UnityEngine;
using PixelMiner.Physics;
using PixelMiner.DataStructure; 

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
        //[SerializeField] private AABB _entity.AABB;
        private DynamicEntity _entity;



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


            _entity = new DynamicEntity()
            {
                Transform = this.transform,
                AABB = new AABB()
                {
                    x = transform.position.x - 0.5f,
                    y = transform.position.y,
                    z = transform.position.z - 0.5f,
                    w = 1,
                    h = 2,
                    d = 1,
                    vx = _gravity.x,
                    vy = _gravity.y,
                    vz = _gravity.z
                },
                Simulate = true,
                
            };
            GamePhysics.AddDynamicEntity(_entity);
        }


        private void Update()
        {
            UpdatePosition();
            //_entity.AABB = new AABB()
            //{
            //    x = transform.position.x - 0.5f,
            //    y = transform.position.y,
            //    z = transform.position.z - 0.5f,
            //    w = 1,
            //    h = 2,
            //    d = 1,
            //    vx = 0,
            //    vy = 0,
            //    vz = 0
            //};



            if (_input.Fire1 == false)
            {
                UpdateRotation();
            }


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

        private void OnDrawGizmos()
        {
            //if (_entity != null && _entity.AABB.Equals(default) == false)
            //{
            //    Gizmos.color = Color.green;
            //    // Draw the bottom face
            //    Gizmos.DrawLine(new Vector3(_entity.AABB.x, _entity.AABB.y, _entity.AABB.z), new Vector3(_entity.AABB.x + _entity.AABB.w, _entity.AABB.y, _entity.AABB.z));
            //    Gizmos.DrawLine(new Vector3(_entity.AABB.x + _entity.AABB.w, _entity.AABB.y, _entity.AABB.z), new Vector3(_entity.AABB.x + _entity.AABB.w, _entity.AABB.y, _entity.AABB.z + _entity.AABB.d));
            //    Gizmos.DrawLine(new Vector3(_entity.AABB.x + _entity.AABB.w, _entity.AABB.y, _entity.AABB.z + _entity.AABB.d), new Vector3(_entity.AABB.x, _entity.AABB.y, _entity.AABB.z + _entity.AABB.d));
            //    Gizmos.DrawLine(new Vector3(_entity.AABB.x, _entity.AABB.y, _entity.AABB.z + _entity.AABB.d), new Vector3(_entity.AABB.x, _entity.AABB.y, _entity.AABB.z));

            //    // Draw the top face
            //    Gizmos.DrawLine(new Vector3(_entity.AABB.x, _entity.AABB.y + _entity.AABB.h, _entity.AABB.z), new Vector3(_entity.AABB.x + _entity.AABB.w, _entity.AABB.y + _entity.AABB.h, _entity.AABB.z));
            //    Gizmos.DrawLine(new Vector3(_entity.AABB.x + _entity.AABB.w, _entity.AABB.y + _entity.AABB.h, _entity.AABB.z), new Vector3(_entity.AABB.x + _entity.AABB.w, _entity.AABB.y + _entity.AABB.h, _entity.AABB.z + _entity.AABB.d));
            //    Gizmos.DrawLine(new Vector3(_entity.AABB.x + _entity.AABB.w, _entity.AABB.y + _entity.AABB.h, _entity.AABB.z + _entity.AABB.d), new Vector3(_entity.AABB.x, _entity.AABB.y + _entity.AABB.h, _entity.AABB.z + _entity.AABB.d));
            //    Gizmos.DrawLine(new Vector3(_entity.AABB.x, _entity.AABB.y + _entity.AABB.h, _entity.AABB.z + _entity.AABB.d), new Vector3(_entity.AABB.x, _entity.AABB.y + _entity.AABB.h, _entity.AABB.z));

            //    // Connect the corresponding points between the top and bottom faces
            //    Gizmos.DrawLine(new Vector3(_entity.AABB.x, _entity.AABB.y, _entity.AABB.z), new Vector3(_entity.AABB.x, _entity.AABB.y + _entity.AABB.h, _entity.AABB.z));
            //    Gizmos.DrawLine(new Vector3(_entity.AABB.x + _entity.AABB.w, _entity.AABB.y, _entity.AABB.z), new Vector3(_entity.AABB.x + _entity.AABB.w, _entity.AABB.y + _entity.AABB.h, _entity.AABB.z));
            //    Gizmos.DrawLine(new Vector3(_entity.AABB.x + _entity.AABB.w, _entity.AABB.y, _entity.AABB.z + _entity.AABB.d), new Vector3(_entity.AABB.x + _entity.AABB.w, _entity.AABB.y + _entity.AABB.h, _entity.AABB.z + _entity.AABB.d));
            //    Gizmos.DrawLine(new Vector3(_entity.AABB.x, _entity.AABB.y, _entity.AABB.z + _entity.AABB.d), new Vector3(_entity.AABB.x, _entity.AABB.y + _entity.AABB.h, _entity.AABB.z + _entity.AABB.d));

            //}
        }
    }

}
