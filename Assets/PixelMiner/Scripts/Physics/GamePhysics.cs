using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelMiner.Core;

namespace PixelMiner.Physics
{
    public class GamePhysics : MonoBehaviour
    {
        private static List<DynamicEntity> _dynamicEntities = new List<DynamicEntity>();
        private Main _main;

        private Vector3 _gravity = Vector3.down;
        private float _gravityForce = 9.8f;

        private void Start()
        {
            _main = Main.Instance;

            _gravity *= _gravityForce;
        }

        public static void AddDynamicEntity(DynamicEntity entity)
        {
            _dynamicEntities.Add(entity);
        }

        private void Update()
        {
            for(int i = 0; i < _dynamicEntities.Count; i++)
            {
                DynamicEntity dEntity = _dynamicEntities[i];
                if (!dEntity.Simulate) continue;

                dEntity.Velocity += _gravity * Time.deltaTime;

            }
        }

    }
}
