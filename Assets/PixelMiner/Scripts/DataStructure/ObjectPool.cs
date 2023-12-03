using System.Collections.Generic;

namespace PixelMiner.DataStructure
{
    public class ObjectPool<T> where T : new()
    {
        private readonly Queue<T> objectQueue = new Queue<T>();
        private readonly object lockObject = new object();

        public ObjectPool(int size = 10) 
        {
            Initialize(size);
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

        // Get the current size of the pool
        public int Count()
        {
            lock (lockObject)
            {
                return objectQueue.Count;
            }
        }
    }
}

