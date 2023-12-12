using QFSW.QC;
using UnityEngine;
using PixelMiner.UI.WorldGen;
using PixelMiner.Enums;
using PixelMiner.WorldGen;

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
        private static void PreviewHeightmap()
        {
            if(UIMapPreviewManager.Instance.HasHeightMap())
            {
                InputHander.Instance.ActiveUIMap();
                UIMapPreviewManager.Instance.SetActiveHeightMap();
            }
        }
        [Command("/preview_heatmap")]
        private static void PreviewHeatmap()
        {
            if (UIMapPreviewManager.Instance.HasHeatMap())
            {
                InputHander.Instance.ActiveUIMap();
                UIMapPreviewManager.Instance.SetActiveHeatMap();
            }
        }


        [Command("/getUV")]
        private static void GetUV(int u, int v, ushort blockType = 0)
        {
            float tileSize = 1 / 16f;
            Debug.Log($"\n\n/*{((BlockType)blockType).ToString().ToUpper()}*/" +
               $"\n{{new Vector2({tileSize * u}f, {tileSize * v}f), " +
               $"new Vector2({tileSize * u + tileSize}f, {tileSize * v}f)," +
               $"\nnew Vector2({tileSize * u}f, {tileSize * v + tileSize}f), " +
               $"new Vector2({tileSize * u + tileSize}f, {tileSize * v + tileSize}f)}}, ");
        }

        [Command("/getUV2")]
        private static void GetUV2(int u, int v, ushort blockType = 0)
        {
            float tileSize = 1 / 64f;
            Debug.Log($"\n\n/*{((ColorMapType)blockType).ToString().ToUpper()}*/" +
               $"\n{{new Vector2({tileSize * u}f, {tileSize * v}f), " +
               $"new Vector2({tileSize * u + tileSize}f, {tileSize * v}f)," +
               $"\nnew Vector2({tileSize * u}f, {tileSize * v + tileSize}f), " +
               $"new Vector2({tileSize * u + tileSize}f, {tileSize * v + tileSize}f)}}, ");
        }

        [Command("/getUV3")]
        private static void GetUV3(int u, int v, ushort blockType = 0)
        {
            float tileSize = 1 / 8f;
            Debug.Log($"\n\n/*{((ColorMapType)blockType).ToString().ToUpper()}*/" +
               $"\n{{new Vector2({tileSize * u}f, {tileSize * v}f), " +
               $"new Vector2({tileSize * u + tileSize}f, {tileSize * v}f)," +
               $"\nnew Vector2({tileSize * u}f, {tileSize * v + tileSize}f), " +
               $"new Vector2({tileSize * u + tileSize}f, {tileSize * v + tileSize}f)}}, ");
        }


        [CommandDescription("all commands")]
        [Command("/all")]
        private static string GenerateAllCommandList()
        {
            string output = "";
            foreach (CommandData command in QuantumConsoleProcessor.GetUniqueCommands())
            {
                output += $"\n   - {command.CommandName}";
            }

            return output;
        }


        [Command("/clear")]
        public static void ClearCommands()
        {
            QuantumConsole.Instance.ClearConsole();
        }


        [Command("/seed")]
        public static void GetSeed()
        {
            QuantumConsole.Instance.LogToConsole($"Seed: {WorldGeneration.Instance.Seed}");
        }


#endif
    }
}

