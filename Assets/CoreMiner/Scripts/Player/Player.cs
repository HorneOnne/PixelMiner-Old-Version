using UnityEngine;

namespace CoreMiner
{
    public class Player : MonoBehaviour
    {
        public PlayerMovement PlayerMovement { get; private set; }
 
        private void Awake()
        {
            PlayerMovement = GetComponent<PlayerMovement>();
        }
    }

}
