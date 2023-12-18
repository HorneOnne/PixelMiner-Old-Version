using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelMiner.Lighting
{
    public struct LightOp
    {
        public int x;
        public int y;
        public int z;
        public byte val;
    }

    public class LightCalculator
    {
        // Direction in 3D space around block
        byte[] wL;  // West (Left)
        byte[] eL;  // East (Right)
        byte[] uL;  // Up (Top)
        byte[] dL;  // Down (Bottm)
        byte[] nL;  // North (Forward)
        byte[] sL;  // South (Backward)       


        // Diagonal directions
        byte[] uwL; // Up West
        byte[] ueL; // Up East
        byte[] unL; // Up North
        byte[] usL; // Up South
        byte[] dwL; // Down West
        byte[] deL; // Down East
        byte[] dnL; // Down North
        byte[] dsL; // Down South


        // Corner directions
    }
}
