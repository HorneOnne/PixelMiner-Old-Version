using UnityEngine;
using PixelMiner.Miscellaneous;

namespace PixelMiner.Character
{
    public class PlayerBehaviour : MonoBehaviour
    {
        private InputHander _input;
        private Animator _anim;
        private bool _hasAnimator;


       


        // Timer
        private float _lastFire1Time;
        private float _fire1Interval = 0.7f;



        // Look
        private Vector3 _inputLookDir;
        private Vector3 _handOffset = new Vector3(0, 1.5f, 0);
        private Vector3 _startLookPos;
        private Vector3 _forwardPosition;
        private Vector3 _lookPosition;

        // animation IDs
        private int _animIDRightHand;



        private void Start()
        {
            _input = InputHander.Instance;
            _anim = GetComponent<Animator>();
            _hasAnimator = _anim != null;
            AssignAnimationIDs();
        }


        private void Update()
        {
            if (_input.Fire1 && UnityEngine.Time.time - _lastFire1Time >= _fire1Interval)
            {
                Debug.Log("Fire 1");
                _lastFire1Time = UnityEngine.Time.time;

                _anim.SetTrigger(_animIDRightHand);
            }



            _inputLookDir = new Vector3(_input.Look.x, _input.Look.y, 0);
            _startLookPos = transform.position + _handOffset;
            _forwardPosition = _startLookPos + transform.TransformDirection(Vector3.forward) * 5;
            _lookPosition = _startLookPos + transform.TransformDirection(Vector3.forward + _inputLookDir) * 5;

            float angle = Vector3.Angle(_forwardPosition - _startLookPos, _lookPosition - _startLookPos);
            //Debug.Log(angle);

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


        private void AssignAnimationIDs()
        {
            _animIDRightHand = Animator.StringToHash("RHand");
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
        }
    }
}
