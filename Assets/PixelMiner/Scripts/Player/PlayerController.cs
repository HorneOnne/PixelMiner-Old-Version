using PixelMiner.Cam;
using PixelMiner.Utilities;
using UnityEngine;
using PixelMiner.Physics;
using PixelMiner.DataStructure;
using PixelMiner.Miscellaneous;

namespace PixelMiner.Character
{
    public class PlayerController : MonoBehaviour
    {
        private CameraLogicHandler _cameraLogicHandler;
        private InputHander _input;
        private Rigidbody _rb;
        private Animator _anim;


        [SerializeField] private float _moveSpeed;
        [SerializeField] private Vector3 _moveDirection;
        private Vector3 _cameraIsometricRot = new Vector3(0, 45, 0);


        // Aiming
        [SerializeField] private Transform _aimTarrgetTrans;
        private Vector3 _inputLookDir;
        private Vector3 _handOffset = new Vector3(0, 1.5f, 0);
        private Vector3 _startLookPos;
        private Vector3 _forwardPosition;
        private Vector3 _lookPosition;


        // Physics
        [SerializeField] private Vector3 _gravity;
        //[SerializeField] private AABB _entity.AABB;
        private DynamicEntity _entity;
        public bool Simulate = false;


        // Animation
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

            AABB bound = new AABB()
            {
                x = transform.position.x - 0.5f,
                y = transform.position.y,
                z = transform.position.z - 0.5f,
                w = 1,
                h = 1.9f,
                d = 1,
            };

            _entity = new DynamicEntity(this.transform, bound);
            _entity.Simulate = Simulate;    
            GamePhysics.AddDynamicEntity(_entity);
        }


        private void Update()
        {
            if(Simulate)
            {
                _entity.Simulate = Simulate;
            }

            UpdatePosition();

            if (_input.Fire1 == false && _input.Move != Vector2.zero)
            {
                UpdateRotation();
            }


            // Aiming
            // =======
            _inputLookDir = new Vector3(_input.Look.x, _input.Look.y, 0);
            _startLookPos = transform.position + _handOffset;
            _forwardPosition = _startLookPos + transform.TransformDirection(Vector3.forward) * 5;
            _lookPosition = _startLookPos + transform.TransformDirection(Vector3.forward + _inputLookDir.ToGameDirection()) * 5;
            _aimTarrgetTrans.position = _lookPosition;
            float angle = Vector3.Angle(_forwardPosition - _startLookPos, _lookPosition - _startLookPos);




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

        private void LateUpdate()
        {
            Vector3 worldForward = transform.TransformDirection(Vector3.forward);
            DrawBounds.Instance.AddLine(_startLookPos, _forwardPosition, Color.red);
            DrawBounds.Instance.AddLine(_startLookPos, _lookPosition, Color.cyan);

            Quaternion rotation = Quaternion.Euler(60f, 0, 0f);
            Vector3 rotateVector = rotation * (_forwardPosition - _startLookPos);
            DrawBounds.Instance.AddLine(_startLookPos, rotateVector, Color.gray);
        }


        private void UpdatePosition()
        {
            if (_input.Move != Vector2.zero && _input.Fire1 == false)
            {
                _moveDirection = new Vector3(_input.Move.x * _moveSpeed, 0, _input.Move.y * _moveSpeed);
                Vector3 _rotMoveDir = _moveDirection.ToGameDirection();
              
                _entity.SetVelocity(new Vector3(_rotMoveDir.x, _entity.Velocity.y, _rotMoveDir.z));
            }
            else
            {
                _entity.SetVelocity(new Vector3(0, _entity.Velocity.y,0));
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
            RotateTowardMoveDirection(_moveDirection.ToGameDirection());
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
