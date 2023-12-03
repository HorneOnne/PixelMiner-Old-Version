using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PixelMiner.DataStructure
{
    public class ObjectPool<T> where T : new()
    {
        //private readonly ConcurrentBag<T> _objects;
        //private readonly Func<T> _objectGenerator;

        //public ObjectPool(Func<T> objectGenerator)
        //{
        //    _objectGenerator = objectGenerator ?? throw new ArgumentNullException(nameof(objectGenerator));
        //    _objects = new ConcurrentBag<T>();
        //}

        //public T Get() => _objects.TryTake(out T item) ? item : _objectGenerator();

        //public void Return(T item) => _objects.Add(item);

        // Maximum objects allowed!



        private readonly Queue<T> objectQueue = new Queue<T>();
        private readonly object lockObject = new object();

        public ObjectPool() 
        {
            Initialize(1000);
        }

        // Create objects and add them to the pool
        public void Initialize(int initialSize)
        {
            lock (lockObject)
            {
                for (int i = 0; i < initialSize; i++)
                {
                    objectQueue.Enqueue(new T());
                }
            }
        }

        // Acquire an object from the pool
        public T Get()
        {
            lock (lockObject)
            {
                if (objectQueue.Count > 0)
                {
                    return objectQueue.Dequeue();
                }
                else
                {
                    // If the pool is empty, create a new object
                    return new T();
                }
            }
        }

        // Release an object back to the pool
        public void Release(T obj)
        {
            lock (lockObject)
            {
                objectQueue.Enqueue(obj);
            }
        }
    }
}

