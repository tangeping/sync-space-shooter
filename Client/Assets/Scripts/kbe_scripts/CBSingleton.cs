using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CBSingleton<T> where T : CBSingleton<T>
{
    private static T _instance = null;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Activator.CreateInstance<T>();
                _instance.OnInit();
            }
            return _instance;
        }
    }

    protected virtual void OnInit()
    {
    }
}
