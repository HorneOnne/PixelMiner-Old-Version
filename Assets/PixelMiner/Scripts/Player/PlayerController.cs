using PixelMiner.Cam;
using PixelMiner.Utilities;
using UnityEngine;
using PixelMiner.Physics;
using PixelMiner.DataStructure;
using PixelMiner.Miscellaneous;

namespace PixelMiner
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
        public Vector3 EyePosition { get; private set; }
        public Vector3 LookDirection { get; private set; }
        private Vector3 _forwardPosition;
        private Vector3 _lookPosition;
        public float CurrentVerticalLookAngle = 0;  // Angle in degrees
        public float CurrentHorizontalLookAngle = 0;  // Angle in degrees
        [SerializeField] private float _verticalSensitive = 20f;


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
            Vector3 worldForward = transform.TransformDirection(Vector3.forward);
            //CurrentLookAngle += _input.LookVertical * _verticalSensitive * UnityEngine.Time.deltaTime;
            CurrentHorizontalLookAngle = _input.LookHorizontal.x * _verticalSensitive;
            CurrentVerticalLookAngle = _input.LookHorizontal.y * _verticalSensitive;
            float maxAngle = 90;
            if (CurrentHorizontalLookAngle > maxAngle) CurrentHorizontalLookAngle = maxAngle;
            if (CurrentHorizontalLookAngle < -maxAngle) CurrentHorizontalLookAngle = -maxAngle;
            if (CurrentVerticalLookAngle > maxAngle) CurrentVerticalLookAngle = maxAngle;
            if (CurrentVerticalLookAngle < -maxAngle) CurrentVerticalLookAngle = -maxAngle;

            if (Simulate)
            {
                _entity.Simulate = Simulate;
            }

            UpdatePosition();



            // Aiming
            // =======
            _inputLookDir = new Vector3(_input.LookHorizontal.x, _input.LookHorizontal.y, 0);
            EyePosition = transform.position + _handOffset;
            Vector3 offset = _inputLookDir == Vector3.zero ? transform.forward : Vector3.zero;
            _forwardPosition = EyePosition + transform.TransformDirection(Vector3.forward) * 5;
            if(_inputLookDir.sqrMagnitude > 0.005f)
            {
                _lookPosition = EyePosition + transform.TransformDirection(_inputLookDir + new Vector3(0,0,0.025f));
            }
            else
            {
                _lookPosition = EyePosition +  transform.TransformDirection(Vector3.forward);
            }
            LookDirection = _lookPosition - EyePosition;
            _aimTarrgetTrans.position = _lookPosition;


            // Vector3 verticalV = MathHelper.RotateVectorUseMatrix(transform.forward, CurrentVerticalLookAngle, -transform.right);
            //Vector3 verticalH = MathHelper.RotateVectorUseMatrix(transform.forward, CurrentHorizontalLookAngle, transform.up);
            // _aimTarrgetTrans.position = _startLookPos +  verticalH + verticalV;


            if (_input.Fire1 == false)
            {
                //if (_input.LookHorizontal != Vector2.zero)
                //{
                //    UpdateRotation(new Vector3(_input.LookHorizontal.x, 0, _input.LookHorizontal.y).ToGameDirection());
                //}
                //else if (_input.Move != Vector2.zero)
                //{
                //    UpdateRotation(new Vector3(_input.Move.x, 0, _input.Move.y).ToGameDirection());
                //}

                if (_input.Move != Vector2.zero)
                {
                    UpdateRotation(new Vector3(_input.Move.x, 0, _input.Move.y).ToGameDirection());
                }
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

        private void LateUpdate()
        {
            DrawBounds.Instance.AddRay(EyePosition, transform.right, Color.red, 3);
            DrawBounds.Instance.AddRay(EyePosition, transform.forward, Color.blue, 3);
            DrawBounds.Instance.AddRay(EyePosition, transform.up, Color.green, 3);


            //DrawBounds.Instance.AddRay(_startLookPos, MathHelper.RotateVectorUseMatrix(transform.forward, CurrentVerticalLookAngle, -transform.right), Color.yellow);
            Vector3 verticalV = MathHelper.RotateVectorUseMatrix(transform.forward, CurrentVerticalLookAngle, -transform.right);
            Vector3 verticalH = MathHelper.RotateVectorUseMatrix(transform.forward, CurrentHorizontalLookAngle, transform.up);
            //DrawBounds.Instance.AddRay(_startLookPos, verticalH + verticalV, Color.yellow);
            DrawBounds.Instance.AddRay(EyePosition, _aimTarrgetTrans.transform.position - EyePosition, Color.yellow);
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
                _entity.SetVelocity(new Vector3(0, _entity.Velocity.y, 0));
            }
        }

        private void Move(Vector3 direction)
        {
            Vector3 movement = direction * _moveSpeed * UnityEngine.Time.deltaTime;
            //_rb.MovePosition(_rb.position + movement);
            transform.position += movement;
        }

        private void UpdateRotation(Vector3 gameDirection)
        {
            RotateTowardMoveDirection(gameDirection);
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
