using UnityEngine;
using UnityEngine.Pool;

namespace PixelMiner.DataStructure
{
    public class PhysicEntityOctreePool
    {
        public bool CollectionChecks = true;
        public int MaxPoolSize = 10;

        private static IObjectPool<PhysicEntityOctree> _pool;
        public static IObjectPool<PhysicEntityOctree> Pool
        {
            get
            {
                if (_pool == null)
                {
                    _pool = new UnityEngine.Pool.ObjectPool<PhysicEntityOctree>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject);
                }
                return _pool;
            }
        }

        private static PhysicEntityOctree CreatePooledItem()
        {
            //Debug.Log("create new");
            PhysicEntityOctree item = new PhysicEntityOctree();
            return item;
        }

 
        // Called when an item is returned to the pool using Release
        private static void OnReturnedToPool(PhysicEntityOctree octree)
        {
            //Debug.Log("return to pool");
            //system.gameObject.SetActive(false);
            octree.Clear();

        }

        // Called when an item is taken from the pool using Get
        private static void OnTakeFromPool(PhysicEntityOctree octree)
        {
            //Debug.Log("Take from pool");
            //system.gameObject.SetActive(true);
        }

        // If the pool capacity is reached then any items returned will be destroyed.
        // We can control what the destroy behavior does, here we destroy the GameObject.
        private static void OnDestroyPoolObject(PhysicEntityOctree octree)
        {
           // Destroy(system.gameObject);
        }
    }
}
