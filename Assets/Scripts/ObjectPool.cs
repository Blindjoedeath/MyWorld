using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool {

    private GameObject[] _instances;
    private Transform _transform;
    private int index;


    public ObjectPool(GameObject instance, int count, Transform transform = null)
    {
        _instances = new GameObject[count];
        for (int i = 0; i < count; ++i)
        {
             (_instances[i] = GameObject.Instantiate(instance, transform)).SetActive(false);
        }
    }

    public GameObject Get()
    {
        if (index == _instances.Length)
        {
            index = 0;
        }
        return _instances[index++];
    }

}
