using UnityEngine;

namespace CoreMiner
{
    public class PlayerMovement : MonoBehaviour
    {
        private InputHander _input;
        private Rigidbody2D _rb;
        private Animator _anim;

        [SerializeField] private float moveSpeed;

      
        private bool _hasAnimator;
        private bool _facingRight;

        // animation IDs
        private int _animIDVelocityX;
        private int _animIDVelocityY;

        void Start()
        {
            _input = InputHander.Instance;
            _rb = GetComponent<Rigidbody2D>();
            _anim = GetComponent<Animator>();
            AssignAnimationIDs();      
        }

        private void FixedUpdate()
        {
            Movement();
        }

        private void AssignAnimationIDs()
        {
            _animIDVelocityX = Animator.StringToHash("VelocityX");
            _animIDVelocityY = Animator.StringToHash("VelocityY");

            _hasAnimator = TryGetComponent(out _anim);
        }

        private void Movement()
        {
            _rb.velocity = _input.Move * moveSpeed;
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
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (move.x < 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }

#if DEV_MODE
        public void SetPlayerSpeed(float value)
        {
            this.moveSpeed = value;
        }
#endif
    }

}
