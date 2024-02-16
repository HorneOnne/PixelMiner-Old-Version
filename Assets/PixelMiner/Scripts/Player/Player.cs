using UnityEngine;

namespace PixelMiner
{
    public class Player : MonoBehaviour
    {
        public PlayerBehaviour PlayerBehaviour { get; private set; }
        public PlayerController PlayerController { get; private set; }
 
        private void Awake()
        {
            PlayerBehaviour = GetComponent<PlayerBehaviour>();
            PlayerController = GetComponent<PlayerController>();
        }
    }
}
