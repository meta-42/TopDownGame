using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

//非继承MonoBehaviour的单例继承此类，并加入私有构造函数
public class Singleton<T> where T : class//, new()
{
    private static T _instance;
    private static readonly object syslock = new object();
    public static readonly Type[] EmptyTypes = new Type[0];
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (syslock)
                {
                    if (_instance == null)
                    {
                        ConstructorInfo ci = typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, EmptyTypes, null);
                        if (ci == null) { throw new InvalidOperationException("class must contain a private constructor"); }                    
                        _instance = (T)ci.Invoke(null);
                        //_instance = new T();
                    }
                }
            }
            return _instance;
        }
    }
}
