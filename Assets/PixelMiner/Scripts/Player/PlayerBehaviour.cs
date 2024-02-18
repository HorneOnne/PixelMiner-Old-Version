using UnityEngine;
using PixelMiner.Core;
using PixelMiner.Miscellaneous;
namespace PixelMiner
{
    public class PlayerBehaviour : MonoBehaviour
    {
        private Player _player;
        private InputHander _input;
        private Animator _anim;
        private bool _hasAnimator;
        

        // Timer
        private float _lastFire1Time;
        private float _fire1Interval = 0.7f;



        // animation IDs
        private int _animIDRightHand;


        private Vector3 _blockOffsetOrigin = new Vector3(0.5f, 0.5f, 0.5f);
        public RaycastVoxelHit VoxelHit { get; private set; }
        private RayCasting _rayCasting;
        Vector3Int hitGlobalPosition;


        // Testing
        [SerializeField] private Transform _sampleBlockTrans;

        private void Start()
        {
            _player = GetComponent<Player>();
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


    
            if (RayCasting.Instance.DDAVoxelRayCast(_player.CurrentBCheckTrans.position,
                                                    _player.PlayerController.LookDirection,
                                                    out RaycastVoxelHit hitVoxel,
                                                    out RaycastVoxelHit preHitVoxel,
                                                    maxDistance: 1))
            {
                VoxelHit = hitVoxel;
                hitGlobalPosition = new Vector3Int(Mathf.FloorToInt(hitVoxel.point.x + 0.001f),
                                                                  Mathf.FloorToInt(hitVoxel.point.y + 0.001f),
                                                                  Mathf.FloorToInt(hitVoxel.point.z + 0.001f));
                _sampleBlockTrans.position = hitGlobalPosition + new Vector3(0.5f,0.5f,0.5f);                              
            }
            else
            {
                VoxelHit = default;

                Vector3 endPosition = _player.CurrentBCheckTrans.position + _player.PlayerController.LookDirection;
                _sampleBlockTrans.position = Main.Instance.GetBlockGPos(endPosition) + new Vector3(0.5f, 0.5f, 0.5f);
            }
        }

        private void LateUpdate()
        {
            if(!VoxelHit.Equals(default))
            {
                Vector3 hitCenter = hitGlobalPosition + _blockOffsetOrigin;
                DrawBounds.Instance.AddBounds(new Bounds(hitCenter, new Vector3(1.01f, 1.01f, 1.01f)), Color.grey);
            }
          
        }




        private void AssignAnimationIDs()
        {
            _animIDRightHand = Animator.StringToHash("RHand");
        }
    }
}
