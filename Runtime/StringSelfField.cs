using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class StringSelfField
{
    public Component targetComponent;
    public string targetField;
    FieldInfo cahedField; //cash for next fast access

    public void Invoke(string line)
    {
        if (!targetComponent || targetField == "")
        {
            MonoBehaviour.print("No find components !!!!!!!!!!!");
            return;
        }

        if(cahedField == null)
        {
            Type type = targetComponent.GetType();
            cahedField = type.GetField(targetField);
        }

        if (cahedField == null)
        {
            MonoBehaviour.print($"No find target field. {targetComponent.gameObject.name}/ {targetComponent.GetType()}/ {targetField}");
            return;
        }

        cahedField.SetValue(targetComponent, line);
    }
}