using UnityEngine;

namespace PixelMiner.Character
{
    public class Player : MonoBehaviour
    {
        public PlayerMovement PlayerMovement { get; private set; }
        public PlayerBehaviour PlayerBehaviour { get; private set; }
 
        private void Awake()
        {
            PlayerMovement = GetComponent<PlayerMovement>();
            PlayerBehaviour = GetComponent<PlayerBehaviour>();
        }
    }
}
