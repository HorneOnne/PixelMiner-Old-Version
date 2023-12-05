using Cinemachine;
using System.Collections.Generic;

namespace PixelMiner.Camera
{
    internal static class CameraSwitcher
    {
        private static List<CinemachineVirtualCamera> _cameras = new List<CinemachineVirtualCamera>();
        public static CinemachineVirtualCamera ActiveCam = null;

        public static void Register(CinemachineVirtualCamera cam)
        {
            _cameras.Add(cam);
        }

        public static void Unregister(CinemachineVirtualCamera cam)
        {
            _cameras.Remove(cam);
        }

        public static void SwitchCamera(CinemachineVirtualCamera cam)
        {
            cam.Priority = 10;
            ActiveCam = cam;

            for(int i =0;i < _cameras.Count;i++)
            {
                if(_cameras[i] != cam) 
                {
                    _cameras[i].Priority = 0;
                }
            }
        }
    }
}
