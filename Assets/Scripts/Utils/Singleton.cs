using UnityEngine;


public class Singleton<T> : MonoBehaviour where T: Component
{
    private static T instance;

    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this as T;
    }

    protected virtual void OnDestroy()
    {
    }
}


public class NormalSingleton<T> where T: class, new()
{
    private static T instance = null;

    protected NormalSingleton() { this.Init(); }

    protected virtual void Init() { }

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
            }
            return instance;
        }
    }
}
