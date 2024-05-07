using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Singleton class that can be inherited by any MonoBehaviour class.
/// This class will make sure that only one instance of the class is created.
/// Use f.e. for Managers, Managers should be singletons.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    protected virtual void Awake() => Instance = this as T;
    protected virtual void OnDestroy() => Instance = null;


}
/// <summary>
/// Singleton that persists through scenes.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SingletonPersistent<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            base.Awake();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
