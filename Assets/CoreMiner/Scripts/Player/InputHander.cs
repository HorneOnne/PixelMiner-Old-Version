using UnityEngine;
using UnityEngine.InputSystem;

namespace CoreMiner
{
    public class InputHander : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 Move;


#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }
#endif

        public void MoveInput(Vector2 newMoveDirection)
        {
            Move = newMoveDirection;
        }
    }
}
