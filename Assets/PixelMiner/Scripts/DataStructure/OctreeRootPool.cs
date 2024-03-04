using UnityEngine.Pool;

namespace PixelMiner.DataStructure
{
    public class OctreeRootPool
    {
        //public bool CollectionChecks = true;
        //public int MaxPoolSize = 10;

        //private static IObjectPool<Octree> _pool;
        //public static IObjectPool<Octree> Pool
        //{
        //    get
        //    {
        //        if (_pool == null)
        //        {
        //            _pool = new UnityEngine.Pool.ObjectPool<Octree>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject);
        //        }
        //        return _pool;
        //    }
        //}

        //private static Octree CreatePooledItem()
        //{
        //    //Debug.Log("create new");
        //    Octree item = new Octree();
        //    return item;
        //}


        //// Called when an item is returned to the pool using Release
        //private static void OnReturnedToPool(Octree octree)
        //{
        //    //Debug.Log("return to pool");
        //    //system.gameObject.SetActive(false);
        //    octree.Clear();

        //}

        //// Called when an item is taken from the pool using Get
        //private static void OnTakeFromPool(Octree octree)
        //{
        //    //Debug.Log("Take from pool");
        //    //system.gameObject.SetActive(true);
        //}

        //// If the pool capacity is reached then any items returned will be destroyed.
        //// We can control what the destroy behavior does, here we destroy the GameObject.
        //private static void OnDestroyPoolObject(Octree octree)
        //{
        //    // Destroy(system.gameObject);
        //}
    }

    public class OctreeLeavePool
    {
        //public bool CollectionChecks = true;
        //public int MaxPoolSize = 10;

        //private static IObjectPool<OctreeNode> _pool;
        //public static IObjectPool<Octree> Pool
        //{
        //    get
        //    {
        //        if (_pool == null)
        //        {
        //            _pool = new UnityEngine.Pool.ObjectPool<Octree>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject);
        //        }
        //        return _pool;
        //    }
        //}

        //private static Octree CreatePooledItem()
        //{
        //    //Debug.Log("create new");
        //    Octree item = new Octree();
        //    return item;
        //}


        //// Called when an item is returned to the pool using Release
        //private static void OnReturnedToPool(Octree octree)
        //{
        //    //Debug.Log("return to pool");
        //    //system.gameObject.SetActive(false);
        //    octree.Clear();

        //}

        //// Called when an item is taken from the pool using Get
        //private static void OnTakeFromPool(Octree octree)
        //{
        //    //Debug.Log("Take from pool");
        //    //system.gameObject.SetActive(true);
        //}

        //// If the pool capacity is reached then any items returned will be destroyed.
        //// We can control what the destroy behavior does, here we destroy the GameObject.
        //private static void OnDestroyPoolObject(Octree octree)
        //{
        //    // Destroy(system.gameObject);
        //}
    }
}
