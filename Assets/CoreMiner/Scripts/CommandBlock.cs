using QFSW.QC;
using UnityEngine;

namespace CoreMiner
{
    public static class CommandBlock
    {
#if DEV_MODE

        [Command("/set_player_speed", "[value]", MonoTargetType.Single)]
        private static void SetPlayerSpeed(float value)
        {
            Player player = GameObject.FindAnyObjectByType<Player>();   
            if(player != null )
            {
                player.PlayerMovement.SetPlayerSpeed(value);
            }
        }

        [Command("/tp", MonoTargetType.Single)]
        private static void TeleportPlayer(float x, float y)
        {
            Player player = GameObject.FindAnyObjectByType<Player>();
            if (player != null)
            {
                player.transform.position = new Vector3(x, y, 0);
            }
        }
#endif
    }
}

