using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

[CustomPropertyDrawer(typeof(StringSelfField))]
public class StringSelfField_Drawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //  if (property.isExpanded)
        //      EditorGUI.DrawRect(new Rect(position.x - 15, position.y - 2, position.width, position.height), new Color(0.2f, 0.2f, 0.2f, 1));
        //
        //  Rect expandRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        //  property.isExpanded = EditorGUI.Foldout(expandRect, property.isExpanded, label);
        //
        //  if (!property.isExpanded) return;
        //
        //
        EditorGUI.BeginProperty(position, label, property);
        //   position.x += position.width - 250;
        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label); // Draw label
        var indent = EditorGUI.indentLevel;  // Don't make child fields be indented
        EditorGUI.indentLevel = 0;

        float widthStep = (position.width - EditorGUIUtility.labelWidth) / 2;

        var rect_label = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

        EditorGUI.LabelField(rect_label, property.displayName);
        //EditorGUI.DrawRect(new Rect(position.x - 5, position.y - 2, position.width, position.height), new Color(0.3f, 0.3f, 0.3f, 1));

        var rect_left = new Rect(rect_label.x + rect_label.width, position.y, widthStep, EditorGUIUtility.singleLineHeight);
        var rect_right = new Rect(rect_left.x + widthStep, position.y, widthStep, EditorGUIUtility.singleLineHeight);

        SerializedProperty sp_targetComponent = property.FindPropertyRelative("targetComponent");
        SerializedProperty sp_targetMethod = property.FindPropertyRelative("targetField");


        // EditorGUI.PropertyField(rect_left, sp_targetComponent, GUIContent.none);

        //List of target scripts

        if (sp_targetComponent.objectReferenceValue == null)
            sp_targetComponent.objectReferenceValue = property.serializedObject.targetObject;

        if (sp_targetComponent.objectReferenceValue != null)
            ActionEvent_SubscribeDrawer.DrawComponent(rect_left, sp_targetComponent);

        //List of target fields
        if (sp_targetComponent.objectReferenceValue != null)
        {
            //rect_right.y += rect_right.height;
            DrawFields(rect_right, sp_targetComponent, sp_targetMethod, typeof(string));
        }

        property.serializedObject.ApplyModifiedProperties();
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
    void DrawFields(Rect rect, SerializedProperty property, SerializedProperty property_out, Type type)
    {
        var flags =
            BindingFlags.Instance |
              BindingFlags.Public;// | 
                                  // BindingFlags.NonPublic;// | 
                                  //  BindingFlags.Default; 
        var mb = property.objectReferenceValue as Component as MonoBehaviour;

        List<FieldInfo> fields = mb.GetFields(type, flags);

        if (fields != null)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < fields.Count; i++)
                list.Add(fields[i].Name);

            int selectedID = -1;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == property_out.stringValue)
                {
                    selectedID = i;
                    break;
                }
            }

            bool isMissing = false;

            if (selectedID == -1)
            {
                isMissing = true;
                selectedID = 0;
                list.Insert(0, "{Missing reference: " + property_out.stringValue + "}");
            }

            EditorGUI.BeginChangeCheck();
            selectedID = EditorGUI.Popup(rect, selectedID, list.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                if (selectedID >= list.Count) selectedID = 0;
                if (selectedID < list.Count)
                {
                    if (!isMissing || (isMissing && selectedID != 0))
                        if (property_out.stringValue != list[selectedID])
                            property_out.stringValue = list[selectedID];
                }
            }
        }
        else
        {
            EditorGUI.LabelField(rect, $"No Find fields with type: {type}");
        }
    }

    //  public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    //  {
    //      return property.isExpanded ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
    //  }
}