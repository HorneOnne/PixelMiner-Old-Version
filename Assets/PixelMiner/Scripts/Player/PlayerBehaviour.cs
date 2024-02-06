using UnityEngine;


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
        }

        


        private void AssignAnimationIDs()
        {
            _animIDRightHand = Animator.StringToHash("RHand");
        }

    }
}
