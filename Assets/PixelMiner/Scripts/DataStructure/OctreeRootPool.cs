using UnityEngine.Pool;
namespace PixelMiner.DataStructure
{

    public  static class OctreeRootPool
    {
        public static bool CollectionChecks = true;
        public static int MaxPoolSize = 2048;

        private static UnityEngine.Pool.ObjectPool<Octree> _pool;
        public static UnityEngine.Pool.ObjectPool<Octree> Pool
        {
            get
            {
                if (_pool == null)
                {
                    _pool = new UnityEngine.Pool.ObjectPool<Octree>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, maxSize: MaxPoolSize);
                }
                return _pool;
            }
        }

        private static Octree CreatePooledItem()
        {
            //Debug.Log("create new");
            Octree item = new Octree();
            return item;
        }


        // Called when an item is returned to the pool using Release
        private static void OnReturnedToPool(Octree octree)
        {
            //Debug.Log("return to pool");
            //system.gameObject.SetActive(false);
            
            octree.Clear();

        }

        // Called when an item is taken from the pool using Get
        private static void OnTakeFromPool(Octree octree)
        {
            //Debug.Log("Take from pool");
            //system.gameObject.SetActive(true);
        }

        // If the pool capacity is reached then any items returned will be destroyed.
        // We can control what the destroy behavior does, here we destroy the GameObject.
        private static void OnDestroyPoolObject(Octree octree)
        {
            // Destroy(system.gameObject);
        }
    }

    public static class OctreeLeavePool
    {
        public static bool CollectionChecks = true;
        public static int MaxPoolSize = 65536;

        private static UnityEngine.Pool.ObjectPool<OctreeLeave> _pool;

        public static UnityEngine.Pool.ObjectPool<OctreeLeave> Pool
        {
            get
            {
                if (_pool == null)
                {
                    _pool = new UnityEngine.Pool.ObjectPool<OctreeLeave>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, maxSize: MaxPoolSize);
                }
                return _pool;
            }
        }

        private static OctreeLeave CreatePooledItem()
        {
            //Debug.Log("create new");
            OctreeLeave item = new OctreeLeave();
            return item;
        }


        // Called when an item is returned to the pool using Release
        private static void OnReturnedToPool(OctreeLeave leave)
        {
            //Debug.Log("return to pool");
            //system.gameObject.SetActive(false);
            
            //leave.Clear();

        }

        // Called when an item is taken from the pool using Get
        private static void OnTakeFromPool(OctreeLeave leave)
        {
            //Debug.Log("Take from pool");
            //system.gameObject.SetActive(true);
        }

        // If the pool capacity is reached then any items returned will be destroyed.
        // We can control what the destroy behavior does, here we destroy the GameObject.
        private static void OnDestroyPoolObject(OctreeLeave leave)
        {
            // Destroy(system.gameObject);
        }
    }
}
