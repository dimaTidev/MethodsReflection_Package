using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

[System.Serializable]
public class ActionEvent<T>
{
    public Component targetComponent;
    public string targetMethod;
    Action action;

    public void Invoke()
    {
        if (action == null)
            GetAction();
        action?.Invoke();
    }

    void GetAction()
    {
        if (!targetComponent || targetMethod == "")
        {
            MonoBehaviour.print("No find components !!!!!!!!!!!");
            return;
        }
        Type type = targetComponent.GetType();
        MethodInfo method = type.GetMethod(targetMethod);
        action = (Action)Delegate.CreateDelegate(typeof(Action), targetComponent as object, method);
    }
}