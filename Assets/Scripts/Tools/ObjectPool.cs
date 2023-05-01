using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

//Uses reflection to dynamically find all the classes derived from the generic type T
public class ObjectPool<T> where T : Component
{
    private Queue<T> pool = new Queue<T>();

    public T GetObjectFromPool()
    {
        if (pool.Count > 0)
        {
            T obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
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
