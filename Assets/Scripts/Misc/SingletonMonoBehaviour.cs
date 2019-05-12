using UnityEngine;

public class SingletonMonoBehaviour<T>: MonoBehaviour where T: SingletonMonoBehaviour<T>
{
    public static T Instance { get; private set; }

    public static U Get<U>() where U: T
    {
        return Instance as U;
    }

    protected void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this as T;
        } 
    }
}