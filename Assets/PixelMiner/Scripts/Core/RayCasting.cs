using PixelMiner.Enums;
using PixelMiner.World;
using System.Collections.Generic;
using UnityEngine;

namespace PixelMiner.Core
{
    public struct RaycastVoxelHit
    {
        public Vector3Int point;
    }


    public class RayCasting : MonoBehaviour
    {
        private Main _main;

        private List<Vector3> _ddaVoxelVisualizationList = new List<Vector3>();
        private Vector3 _origin;
        private Vector3 _target;
        private Vector3 _rayDir;
        private Vector3 _blockOffsetOrigin = new Vector3(0.5f, 0.5f, 0.5f);
        private Chunk _currentChunk;

        private void Start()
        {
            _main = Main.Instance;
        }

        public bool DDAVoxelRayCast(Vector3 origin, Vector3 dir, out RaycastVoxelHit hit, out RaycastVoxelHit preHit)
        {
            dir.Normalize();
            _rayDir = dir;
            _origin = origin;
            bool hitVoxel = false;
            hit.point = default;
            preHit.point = default;


            //const float ff = 0.001f;
            //if (origin.x % (1.0f / BPU) == 0)
            //{
            //    origin.x += ff;
            //}
            //if (origin.y % (1.0f / BPU) == 0)
            //{
            //    origin.y += ff;
            //}


        

            Vector3 radius = Vector3.zero;
            float maxDistance = 40000; // 200 * 200
            Vector3Int step = Vector3Int.zero;
            // ray distance it takes to equal one block unit in each direction (this one doesnt change in loop)
            Vector3 tDelta = Vector3.positiveInfinity;
            // ray distance it takes to move to next block boundary in each direction (this changes)
            Vector3 tMax = Vector3.positiveInfinity;
            Vector3 voxelPosition = origin;

      
            if (dir.x > 0.0f)
            {
                step.x = 1;
                tDelta.x = 1.0f / dir.x;
                tMax.x = (Mathf.Ceil(origin.x) - origin.x) / dir.x;           
            }
            else if (dir.x < 0.0f)
            {
                step.x = -1;
                tDelta.x = -1.0f / dir.x;
                tMax.x = -(origin.x - Mathf.Floor(origin.x)) / dir.x;
            }

            if (dir.y > 0.0f)
            {
                step.y = 1;
                tDelta.y = 1.0f / dir.y;
                tMax.y = (Mathf.Ceil(origin.y) - origin.y) / dir.y;
            }
            else if (dir.y < 0.0f)
            {
                step.y = -1;
                tDelta.y = - 1.0f / dir.y;
                tMax.y = - (origin.y - Mathf.Floor(origin.y)) / dir.y;
            }

            if(dir.z > 0.0f)
            {
                step.z = 1;
                tDelta.z = 1.0f / dir.z;
                tMax.z = (Mathf.Ceil(origin.z) - origin.z) / dir.z;
            }
            else if(dir.z < 0.0f)
            {
                step.z = -1;
                tDelta.z = -1.0f / dir.z;
                tMax.z = -(origin.z - Mathf.Floor(origin.z))/ dir.z;    
            }


#if UNITY_EDITOR
            _ddaVoxelVisualizationList.Clear();
            _ddaVoxelVisualizationList.Add(voxelPosition.ToVector3Int());
#endif


            int attempts = 0;
            while (radius.x * radius.x + radius.y * radius.y + radius.z * radius.z < maxDistance)
            {
                if (tMax.x < tMax.y)
                {                
                    if (tMax.x < tMax.z)
                    {
                        // Increment X
                        tMax.x += tDelta.x;
                        voxelPosition.x += step.x;
                        radius.x++;
                    }
                    else
                    {
                        // Increment Z
                        tMax.z += tDelta.z;
                        voxelPosition.z += step.z;
                        radius.z++;
                    }

                }
                else
                {
                    if (tMax.y < tMax.z)
                    {
                        //Increment Y
                        tMax.y += tDelta.y;
                        voxelPosition.y += step.y;
                        radius.y++;
                    }
                    else
                    {
                        // Increment Z
                        tMax.z += tDelta.z;
                        voxelPosition.z += step.z;
                        radius.z++;
                    }
                   

                }

#if UNITY_EDITOR
                _ddaVoxelVisualizationList.Add(voxelPosition.ToVector3Int());
#endif


                if (_main.GetBlock(voxelPosition).IsSolid() || _main.GetBlock(voxelPosition).IsTransparentSolidBlock())
                {
                    hit.point = voxelPosition.ToVector3Int();
                    hitVoxel = true;
                    break;
                }
                preHit.point = voxelPosition.ToVector3Int();


                attempts++;
                if (attempts > 10000)
                {
                    Debug.Log("Loop");
                    break;
                }
            }

            return hitVoxel;
        }

       

        private void OnDrawGizmos()
        {
            if (_ddaVoxelVisualizationList.Count > 0)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawRay(_origin, _rayDir * 200);
     

                Gizmos.color = Color.blue;
                for (int i = 0; i < _ddaVoxelVisualizationList.Count; i++)
                {
                    if (i == 0)
                    {
                        Gizmos.color = Color.yellow;
                    }
                    else
                    {
                        Gizmos.color = new Color(1.0f - (float)i / _ddaVoxelVisualizationList.Count, 0, 1.0f);
                    }
                    Gizmos.DrawWireCube(_ddaVoxelVisualizationList[i] + _blockOffsetOrigin, Vector3.one);
                }
            }
        }
    }
}
