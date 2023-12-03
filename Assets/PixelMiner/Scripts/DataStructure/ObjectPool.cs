using System.Collections.Generic;
namespace PixelMiner.DataStructure
{
    public class ObjectPool<T> where T : new()
    {
        // list to hold the objects
        private List<T> _objectsList = new List<T>();
        //counter keeps track of the number of objects in the pool
        private int _counter = 0;
        // max objects allowed in the pool
        private int _maxObjects = 1000;

        // returns the number of objects in the pool
        public int GetCount()
        {
            return _counter;
        }

        // method to get object from the pool
        public T GetObj()
        {
            // declare item
            T objectItem;
            // check if pool has any objects
            // if yes, remove the first object and return it
            // also, decrease the count
            if (_counter > 0)
            {
                objectItem = _objectsList[0];
                _objectsList.RemoveAt(0);
                _counter--;
                return objectItem;
            }
            // if the pool has no objects, create a new one and return it
            else
            {
                T obj = new T();
                return obj;
            }
        }

        // method to return object to the pool
        // if counter is less than the max objects allowed, add object to the pool
        // also, increment counter
        public void ReleaseObj(T item)
        {
            if (_counter < _maxObjects)
            {
                _objectsList.Add(item);
                _counter++;
            }
        }
    }

    
}

