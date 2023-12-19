using System;
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
    
    public struct Light
    {
        public ushort Torque;
        public byte Sun;
    }

    public struct LightRemovalNode
    {
        public int Index;
        public byte Light;
    }
    
    public class LightCalculator
    {
        // Direction in 3D space around block
        private byte[] _wL; 
        private byte[] _eL;  
        private byte[] _uL; 
        private byte[] _dL;  
        private byte[] _nL; 
        private byte[] _sL;      
        
        private byte[] _uwL; 
        private byte[] _ueL; 
        private byte[] _unL;
        private byte[] _usL; 
        private byte[] _dwL; 
        private byte[] _deL; 
        private byte[] _dnL; 
        private byte[] _dsL;
        private byte[] _swL;
        private byte[] _seL;
        private byte[] _nwL;
        private byte[] _neL;
        
        // Corner directions
        private byte[] _uneL;
        private byte[] _unwL;
        private byte[] _useL;
        private byte[] _uswL;
        private byte[] _dneL;
        private byte[] _dnwL;
        private byte[] _dseL;
        private byte[] _dswL;

        private Queue<LightOp> _lightOps; // A list of placement a deletion operations to make within this chunk.
        private Queue<int> _lightBFS;

        public byte GetLight(int x, int y, int z)
        {
            int chunkWidth = 32;
            int chunkHeight = 32;
            int chunkDepth = 32;

            int s;

            int indexX = Mathf.Clamp(x + chunkWidth, 0, 2 * chunkWidth - 1);
            int indexY = Mathf.Clamp(x + chunkHeight, 0, 2 * chunkHeight - 1);
            int indexZ = Mathf.Clamp(x + chunkDepth, 0, 2 * chunkDepth - 1);




            return 0;
        }
    }
}
