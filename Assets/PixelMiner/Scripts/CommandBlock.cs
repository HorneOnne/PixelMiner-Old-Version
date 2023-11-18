using QFSW.QC;
using UnityEngine;
using PixelMiner.UI;

namespace PixelMiner
{
    public static class CommandBlock
    {
#if DEV_MODE
        [Command("/set_player_speed", "[value]", MonoTargetType.Single)]
        public static void SetPlayerSpeed(float value)
        {
            Player player = GameObject.FindAnyObjectByType<Player>();   
            if(player != null )
            {
                player.PlayerMovement.SetPlayerSpeed(value);
            }
        }

        [Command("/tp", MonoTargetType.Single)]
        public static void TeleportPlayer(float x, float y)
        {
            Player player = GameObject.FindAnyObjectByType<Player>();
            if (player != null)
            {
                player.transform.position = new Vector3(x, y, 0);
            }
        }


        [Command("/close_preview_map")]
        private static void ClosePreviewMap()
        {
            UIMapPreviewManager.Instance.CloseAllMap();
            InputHander.Instance.ActivePlayerMap();
        }

        [Command("/preview_heightmap")]
        private static void PreviewHeightmap(float zoom)
        {
            if(UIMapPreviewManager.Instance.HasHeightMap())
            {
                InputHander.Instance.ActiveUIMap();

                UIMapPreviewManager.Instance.SetActiveHeightMap();
            }
        }
#endif
    }
}

