using UnityEngine;

namespace PixelMiner.Character
{
    public class Player : MonoBehaviour
    {
        public PlayerBehaviour PlayerBehaviour { get; private set; }
 
        private void Awake()
        {
            PlayerBehaviour = GetComponent<PlayerBehaviour>();
        }
    }
}
