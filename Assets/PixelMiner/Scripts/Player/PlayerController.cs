using PixelMiner.Cam;
using PixelMiner.Utilities;
using UnityEngine;
using PixelMiner.Physics;
using PixelMiner.DataStructure;
using PixelMiner.Miscellaneous;
using PixelMiner.Core;

namespace PixelMiner
{
    public class PlayerController : MonoBehaviour
    {
        private CameraLogicHandler _cameraLogicHandler;
        private InputHander _input;
        private Animator _anim;
        private Player _player;

        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _digMoveSpeed;
        [SerializeField] private Vector3 _moveDirection;
        private Vector3 _cameraIsometricRot = new Vector3(0, 45, 0);


        // Aiming
        private Vector3 _currentBCheckPos;
        private Vector3 _inputLookDir;
        private readonly Vector3 _upperHeadLookOffset = new Vector3(0,0,0);
        private readonly Vector3 _middleHeadLookOffset = new Vector3(0, -0.1f, 0);
        private readonly Vector3 _lowerHeadLookOffset = new Vector3(0,-0.25f,0);
        private Vector3 _currentHeadLookOffset;

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
        [SerializeField] private float _jumpForce;
        [SerializeField] private float _mass;


        // Animation
        private bool _hasAnimator;
        // animation IDs
        private int _animIDVelocity;



        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _hasAnimator = _anim != null;
        }


        private void Start()
        {
            _player = GetComponent<Player>();
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
            _entity.Mass = _mass;
            GamePhysics.AddDynamicEntity(_entity);


            WorldBuilding.WorldLoading.OnFirstLoadChunks += () =>
            {
                Simulate = true;
                _entity.Simulate = Simulate;
            };
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

            float currentMoveSpeed = _input.Fire1 ? _digMoveSpeed : _moveSpeed; 


            UpdatePosition(currentMoveSpeed);



            // Aiming
            // =======
            _inputLookDir = new Vector3(0, _input.LookHorizontal.y, 0).normalized;
            if(_inputLookDir.y > 0)
            {
                _player.CurrentBCheckTrans = _player.UpperBCheckTrans;
                _currentHeadLookOffset = _upperHeadLookOffset;
            }
            else if(_inputLookDir.y < 0)
            {
                _player.CurrentBCheckTrans = _player.LowerBCheckTrans;
                _currentHeadLookOffset = _lowerHeadLookOffset;
            }
            else
            {
                _player.CurrentBCheckTrans = _player.MiddleBCheckTrans;
                _currentHeadLookOffset = _middleHeadLookOffset;
            }
            _currentBCheckPos = _player.CurrentBCheckTrans.position;


            _forwardPosition = _currentBCheckPos + transform.TransformDirection(Vector3.forward);   
            _lookPosition = _forwardPosition;
            LookDirection = _lookPosition - _currentBCheckPos;

            //_aimTarrgetTrans.position = _player.CombatRayPointTrans.position + _currentHeadLookOffset;
        


            // Vector3 verticalV = MathHelper.RotateVectorUseMatrix(transform.forward, CurrentVerticalLookAngle, -transform.right);
            //Vector3 verticalH = MathHelper.RotateVectorUseMatrix(transform.forward, CurrentHorizontalLookAngle, transform.up);
            // _aimTarrgetTrans.position = _startLookPos +  verticalH + verticalV;


            //if (_input.Fire1 == false)
            {
                if (_input.Move != Vector2.zero)
                {
                    UpdateRotation(new Vector3(_input.Move.x, 0, _input.Move.y).ToGameDirection());
                }
            }



            // Jump
            if(_input.Jump && _entity.OnGround)
            {
                Debug.Log("Jump");
                _entity.SetVelocityY(_jumpForce);
            }



            // Animation
            // =========
            if (_hasAnimator)
            {
                if(_input.Fire1 == true)
                {
                    _anim.SetFloat(_animIDVelocity, Mathf.Min(_input.Move.magnitude, 0.3f));
                }
                else
                {
                    _anim.SetFloat(_animIDVelocity, _input.Move.magnitude);
                }
                
            }
        }

        private void FixedUpdate()
        {
            _entity.Mass = _mass;   
        }

        private void LateUpdate()
        {
            //DrawBounds.Instance.AddRay(_player.RaycastingPoint.position, transform.right, Color.red, 3);
            //DrawBounds.Instance.AddRay(_player.RaycastingPoint.position, transform.forward, Color.blue, 3);
            //DrawBounds.Instance.AddRay(_player.RaycastingPoint.position, transform.up, Color.green, 3);


            //Vector3 verticalV = MathHelper.RotateVectorUseMatrix(transform.forward, CurrentVerticalLookAngle, -transform.right);
            //Vector3 verticalH = MathHelper.RotateVectorUseMatrix(transform.forward, CurrentHorizontalLookAngle, transform.up);
            DrawBounds.Instance.AddRay(_currentBCheckPos, _lookPosition - _currentBCheckPos, Color.yellow);
        }


        private void UpdatePosition(float speed)
        {
            if (_input.Move != Vector2.zero)// && _input.Fire1 == false)
            {
                _moveDirection = new Vector3(_input.Move.x * speed, 0, _input.Move.y * speed);
                Vector3 _rotMoveDir = _moveDirection.ToGameDirection();

                _entity.SetVelocity(new Vector3(_rotMoveDir.x, _entity.Velocity.y, _rotMoveDir.z));
            }
            else
            {
                _entity.SetVelocity(new Vector3(0, _entity.Velocity.y, 0));
            }
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


    }

}
