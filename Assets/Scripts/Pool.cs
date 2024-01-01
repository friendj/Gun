using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    [SerializeField]
    [Header("DisappearTime")]
    protected float disappearTime = 5;
    float endActiveTime;

    private Pool _pool;

    protected virtual void Update()
    {
        if (Time.time > endActiveTime)
            Disappear();
    }

    public void SetPool(Pool pool)
    {
        _pool = pool;
    }

    protected void Disappear()
    {
        _pool.PushObject(this);
        SetActive(false);
    }

    public virtual void SetActive(bool active)
    {
        gameObject.SetActive(active);
        if (active)
            endActiveTime = Time.time + disappearTime;
    }

    public virtual void Destroy()
    {
        Destroy(gameObject);
    }
}

public class Pool
{
    public Queue<PoolObject> queue = new Queue<PoolObject>();

    PoolObject _prefab;

    public Pool(PoolObject prefab)
    {
        if (prefab == null)
            Debug.LogError("pool prefab is null");
        _prefab = prefab;
    }

    public PoolObject CreateObject()
    {
        if (_prefab == null)
        {
            Debug.LogError("pool prefab is null");
            return null;
        }
        PoolObject o = MonoBehaviour.Instantiate(_prefab);
        o.SetPool(this);
        return o;
    }

    public PoolObject GetObject()
    {
        if (queue.Count <= 0)
        {
            return CreateObject();
        }
        return queue.Dequeue();
    }

    public void PushObject(PoolObject t)
    {
        queue.Enqueue(t);
    }

    public void ClearPool()
    {
        int dontNeedToClearCount = 0;
        while(queue.Count - dontNeedToClearCount > 0)
        {
            PoolObject poolObject = queue.Dequeue();
            if (poolObject.gameObject.activeSelf)
            {
                queue.Enqueue(poolObject);
                dontNeedToClearCount++;
            }
            else
            {
                poolObject.Destroy();
            }
        }
    }
}
