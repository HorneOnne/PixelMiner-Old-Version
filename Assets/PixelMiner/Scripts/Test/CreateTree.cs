using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelMiner
{
    public class CreateTree : MonoBehaviour
    {
        public GameObject WoodPrefab;
        public GameObject LeavePrefab;
        HashSet<Vector3> Set = new HashSet<Vector3>();  
        private void Start()
        {
            int height = Random.Range(4, 6);
            Vector3 rootPosition = new Vector3(0, 0, 0);


            // Wood
            for (int i = 0; i < height; i++)
            {
                Vector3 woodPos = new Vector3(rootPosition.x, rootPosition.y + i, rootPosition.z);
                GameObject woord = Instantiate(WoodPrefab, woodPos, Quaternion.identity); 

                if(Set.Contains(woodPos) == false)
                {
                    Set.Add(woodPos);
                }
                else
                {
                    Debug.Log("Loop wood");
                }
            }


            // Leaves
            float radius = height / 3f * 2f;
            Vector3 center = new Vector3(rootPosition.x, rootPosition.y + height - 1, rootPosition.z);
            for(int i = -(int)radius; i < radius; i++)
            {
                for (int j = 0; j < radius; j++)
                {
                    for (int k = -(int)radius; k < radius; k++)
                    {
                        Vector3 leavePos =  center + new Vector3(i,j,k);

                        float distance = Vector3.Distance(center, leavePos);

                        if(distance < radius)
                        {
                            Instantiate(LeavePrefab, leavePos + Vector3.down, Quaternion.identity);
                        }


                        if (Set.Contains(leavePos) == false)
                        {
                            Set.Add(leavePos);
                        }
                        else
                        {
                            Debug.Log("Loop Leaves");
                        }
                    }
                }
            }

        }
    }
}
