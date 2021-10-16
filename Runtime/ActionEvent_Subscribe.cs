using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

[System.Serializable]
public class ActionEvent_Subscribe
{
    public enum InvokeType{Null, Int, Float, String, GameObject};
    public InvokeType type;

    public Component targetComponent;
    public string targetMethod;

    public Component argumentComponent;
    public string argumentMethod;

    public void Invoke()
    {
        if (type == InvokeType.Null)
            InvokeNull();
        else if (type == InvokeType.Int)
            Invoke<int>();
        else if (type == InvokeType.Float)
            Invoke<float>();
        else if (type == InvokeType.String)
            Invoke<string>();
        else if (type == InvokeType.GameObject)
            Invoke<GameObject>();
    }
    void Invoke<T>()
    {
        if (!targetComponent || !argumentComponent || targetMethod == "" || argumentMethod == "")
        {
            MonoBehaviour.print("No find components !!!!!!!!!!!");
            return;
        }
        Type type = targetComponent.GetType();
        MethodInfo method = type.GetMethod(targetMethod);
        if (method == null)
        {
            MonoBehaviour.print("No find target method !!!!!!!!!!!");
            return;
        }
        Action<T> act = CreateOpenDelegate<T> (argumentComponent as object, argumentMethod);
        if (act == null)
        {
            MonoBehaviour.print("No find argumentAction !!!!!!!!!!!");
            return;
        }
        object[] arguments = { act as object };
        method.Invoke(targetComponent as object, arguments);
    }
    void InvokeNull()
    {
        if (!targetComponent || !argumentComponent || targetMethod == "" || argumentMethod == "")
        {
            MonoBehaviour.print("No find components !!!!!!!!!!!");
            return;
        }
        Type type = targetComponent.GetType();
        MethodInfo method = type.GetMethod(targetMethod);
        if (method == null)
        {
            MonoBehaviour.print("No find target method !!!!!!!!!!!");
            return;
        }
        Action act = CreateOpenDelegate(argumentComponent as object, argumentMethod);
        if (act == null)
        {
            MonoBehaviour.print("No find argumentAction !!!!!!!!!!!");
            return;
        }
        object[] arguments = { act as object };
        method.Invoke(targetComponent as object, arguments);
    }
    Action<T> CreateOpenDelegate<T>(object target, string method)
    {
        try
        {
            var openDelegate = Delegate.CreateDelegate(typeof(Action<T>), target, method);
            return (Action<T>)openDelegate;
        }
        catch
        {
            MonoBehaviour.print("Can't CreateOpenDelegate !!!!!!!!!!!");
        }
        return null;
    }
    Action CreateOpenDelegate(object target, string method)
    {
        try
        {
            var openDelegate = Delegate.CreateDelegate(typeof(Action), target, method);
            return (Action)openDelegate;
        }
        catch
        {
            MonoBehaviour.print("Can't CreateOpenDelegate !!!!!!!!!!!");
        }
        return null;
    }
}