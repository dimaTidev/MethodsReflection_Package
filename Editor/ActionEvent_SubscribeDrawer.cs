using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using System.Linq;
using UnityEditor;

// IngredientDrawer
[CustomPropertyDrawer(typeof(ActionEvent_Subscribe))]
public class ActionEvent_SubscribeDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
            EditorGUI.DrawRect(new Rect(position.x - 15, position.y - 2, position.width, position.height), new Color(0.2f, 0.2f, 0.2f, 1));

        Rect expandRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(expandRect, property.isExpanded, label);

        if (!property.isExpanded) return;


        EditorGUI.BeginProperty(position, label, property);
        position.x += position.width - 250;
        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label); // Draw label
        var indent = EditorGUI.indentLevel;  // Don't make child fields be indented
        EditorGUI.indentLevel = 0;

        EditorGUI.DrawRect(new Rect(position.x - 5, position.y - 2, position.width, position.height), new Color(0.3f, 0.3f, 0.3f, 1));

        var rect_left = new Rect(position.x, position.y + 2, 100, EditorGUIUtility.singleLineHeight);
        var rect_right = new Rect(position.x + 100, position.y + 2, 150, EditorGUIUtility.singleLineHeight);

        SerializedProperty sp_targetComponent = property.FindPropertyRelative("targetComponent");
        SerializedProperty sp_targetMethod = property.FindPropertyRelative("targetMethod");
        SerializedProperty sp_enumTypes = property.FindPropertyRelative("type");
        SerializedProperty sp_ArgumentComponent = property.FindPropertyRelative("argumentComponent");
        SerializedProperty sp_ArgumentMethod = property.FindPropertyRelative("argumentMethod");


        EditorGUI.PropertyField(rect_left, sp_targetComponent, GUIContent.none);

        //List of target scripts
        if (sp_targetComponent.objectReferenceValue != null)
            DrawComponent(rect_right, sp_targetComponent);
        //Popup enum of types
        if (sp_targetComponent.objectReferenceValue != null)
        {
            rect_left.y += rect_left.height;
            EditorGUI.PropertyField(rect_left, sp_enumTypes, GUIContent.none);
        }
        //List of target methods
        if (sp_targetComponent.objectReferenceValue != null)
        {
            rect_right.y += rect_right.height;
            string enumValue = sp_enumTypes.enumNames[sp_enumTypes.enumValueIndex];
            Type type = enumValue == "Int" ? typeof(Action<int>) :
                enumValue == "Float" ? typeof(Action<float>) :
                enumValue == "String" ? typeof(Action<string>) :
                enumValue == "GameObject" ? typeof(Action<GameObject>) :
                typeof(Action);

            DrawMethods(rect_right, sp_targetComponent, sp_targetMethod, type);
        }

        //Component of target
        if (sp_targetComponent.objectReferenceValue != null && sp_targetMethod.stringValue != "")
        {
            rect_left.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(rect_left, "Argument");
            rect_left.y += EditorGUIUtility.singleLineHeight;
            rect_right.y += EditorGUIUtility.singleLineHeight * 2;

            EditorGUI.PropertyField(rect_left, sp_ArgumentComponent, GUIContent.none);
            if (sp_ArgumentComponent.objectReferenceValue != null)
            {
                DrawComponent(rect_right, sp_ArgumentComponent);
                if (sp_ArgumentComponent.objectReferenceValue != null)
                {
                    rect_right.y += rect_right.height;
                    string enumValue = sp_enumTypes.enumNames[sp_enumTypes.enumValueIndex];
                    Type type = enumValue == "Int" ? typeof(int) :
                        enumValue == "Float" ? typeof(float) :
                        enumValue == "String" ? typeof(string) :
                        enumValue == "GameObject" ? typeof(GameObject) :
                        null;

                    DrawMethods(rect_right, sp_ArgumentComponent, sp_ArgumentMethod, type);
                }
            }
        }

        property.serializedObject.ApplyModifiedProperties();
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public static void DrawComponent(Rect rect, SerializedProperty property)
    {
        GameObject target = ((Component)property.objectReferenceValue).gameObject;
        // GameObject target = ((MonoBehaviour)property.serializedObject.targetObject).gameObject;

        int selectedID = 0;

        Component[] components = target.GetComponents<Component>();

        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] == property.objectReferenceValue as Component)
            {
                selectedID = i;
                break;
            }
        }

        string[] allComponents = new string[components.Length];
        for (int i = 0; i < components.Length; i++)
            allComponents[i] = components[i].GetType().Name;

        EditorGUI.BeginChangeCheck();
        selectedID = EditorGUI.Popup(rect, selectedID, allComponents);
        if (EditorGUI.EndChangeCheck())
            property.objectReferenceValue = components[selectedID];
    }
    void DrawMethods(Rect rect, SerializedProperty property, SerializedProperty property_out, Type type)
    {
        var flags =
            BindingFlags.Instance |
              BindingFlags.Public;// | 
                                  // BindingFlags.NonPublic;// | 
                                  //  BindingFlags.Default; 
        var mb = property.objectReferenceValue as Component as MonoBehaviour;
        // Debug.Log("mb " + property.objectReferenceValue.GetType());
        Type[] types = { type };
        List<MethodInfo> methods = mb.GetMethods(typeof(void), type == null ? null : types, flags);

        if (methods != null)
        {
            string[] allMethods = new string[methods.Count];
            for (int i = 0; i < allMethods.Length; i++)
                allMethods[i] = methods[i].Name;

            int selectedID = 0;

            for (int i = 0; i < allMethods.Length; i++)
            {
                if (allMethods[i] == property_out.stringValue)
                {
                    selectedID = i;
                    break;
                }
            }

            //EditorGUI.BeginChangeCheck();
            selectedID = EditorGUI.Popup(rect, selectedID, allMethods);
            //if (EditorGUI.EndChangeCheck())
            if (selectedID >= allMethods.Length) selectedID = 0;
            if (selectedID < allMethods.Length)
                if (property_out.stringValue != allMethods[selectedID])
                    property_out.stringValue = allMethods[selectedID];
        }
        else
        {
            EditorGUI.LabelField(rect, "No Find methods with argument");
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return property.isExpanded ? EditorGUIUtility.singleLineHeight * 5 + 6 : EditorGUIUtility.singleLineHeight;
    }
}