using UnityEngine;

namespace PixelMiner
{
    public class Player : MonoBehaviour
    {
        public PlayerBehaviour PlayerBehaviour { get; private set; }
        public PlayerController PlayerController { get; private set; }
        public PlayerInventory PlayerInventory { get; private set; }

        public Transform AimTarrgetTrans;

        public Transform UpperBCheckTrans;
        public Transform MiddleBCheckTrans;
        public Transform LowerBCheckTrans;

        public Transform CurrentBCheckTrans;

        public Transform CombatRayPointTrans;


        private void Awake()
        {
            PlayerBehaviour = GetComponent<PlayerBehaviour>();
            PlayerController = GetComponent<PlayerController>();
            PlayerInventory = GetComponent<PlayerInventory>();
        }
    }
}
