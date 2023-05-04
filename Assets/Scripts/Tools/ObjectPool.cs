using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

//Uses reflection to dynamically find all the classes derived from the generic type T
public class ObjectPool<T> where T : Component
{
    //Using a queue data structure ensures that objects that have been in the pool the longest are returned first, which helps to avoid situations where objects
    //are kept in the pool for a long time without being reused. In addition the operations of enqueueing and dequeuing items in a queue are O(1) time complexity,
    //which makes it an efficient data structure for this purpose.
    private Queue<T> pool = new Queue<T>();

    public T GetObjectFromPool()
    {
        if (pool.Count > 0)
        {
            T obj = pool.Dequeue();
            return obj;
        }
        else
        {
            return null;
        } 
    }

    public void ReturnObjectToPool(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}
