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
        



        // animation IDs
        private int _animIDRightHand;


        private Vector3 _blockOffsetOrigin = new Vector3(0.5f, 0.5f, 0.5f);
        public RaycastVoxelHit VoxelHit { get; private set; }
        private RayCasting _rayCasting;
        Vector3Int hitGlobalPosition;
        private float _headLookSpeed = 5f;


        // Digging
        [SerializeField] private float _diggingTime = 0.2f;
        private bool _canDig = true;

        // Testing
        public Transform SampleBlockTrans;
        

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
            if (_input.Fire1)
            {
                if(_canDig)
                {
                    _anim.SetLayerWeight(1, 1.0f);
                    //_anim.SetTrigger(_animIDRightHand);
                    _canDig = false;
                    Invoke(nameof(ResetDig), _diggingTime);
                }              
            }
            else
            {
                _canDig = true;
                _anim.SetLayerWeight(1, 0.0f);
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
                SampleBlockTrans.position = hitGlobalPosition + new Vector3(0.5f, 0.5f, 0.5f);
            }
            else
            {
                VoxelHit = default;

                Vector3 endPosition = _player.CurrentBCheckTrans.position + _player.PlayerController.LookDirection;
                SampleBlockTrans.position = Main.Instance.GetBlockGPos(endPosition) + new Vector3(0.5f, 0.5f, 0.5f);
            }

            // Head look
            _player.AimTarrgetTrans.position = Vector3.Lerp(_player.AimTarrgetTrans.position, SampleBlockTrans.position, Time.deltaTime * _headLookSpeed);

        }


        //private void LateUpdate()
        //{
        //    if(!VoxelHit.Equals(default))
        //    {
        //        Vector3 hitCenter = hitGlobalPosition + _blockOffsetOrigin;
        //        DrawBounds.Instance.AddBounds(new Bounds(hitCenter, new Vector3(1.01f, 1.01f, 1.01f)), Color.grey);
        //    }
        //}


        private void ResetDig()
        {
            _canDig = true;
        }

        private void AssignAnimationIDs()
        {
            _animIDRightHand = Animator.StringToHash("RHand");
        }

    }
}
