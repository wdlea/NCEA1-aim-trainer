using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(ReadOnlyEditorAttribute))]
class ReadOnlyEditorDrawer: PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string str;

        switch (property.propertyType)
        {
            case SerializedPropertyType.Integer:
                str = property.intValue.ToString();
                break;
            case SerializedPropertyType.Float:
                str = property.floatValue.ToString("0.0000");
                break;
            case SerializedPropertyType.Boolean:
                str = property.boolValue.ToString();
                break;
            case SerializedPropertyType.String:
                str = property.stringValue;
                break;
            case SerializedPropertyType.Color:
                str = property.colorValue.ToString();
                break;
            case SerializedPropertyType.Enum:
                str = property.enumDisplayNames[property.enumValueIndex];
                break;
            case SerializedPropertyType.Vector2:
                str = property.vector2Value.ToString();
                break;
            case SerializedPropertyType.Vector3:
                str = property.vector3Value.ToString();
                break;
            case SerializedPropertyType.Vector4:
                str = property.vector4Value.ToString();
                break;
            case SerializedPropertyType.Vector2Int:
                str = property.vector2IntValue.ToString();
                break;
            case SerializedPropertyType.Vector3Int:
                str = property.vector3IntValue.ToString();
                break;
            default:
                throw new System.InvalidOperationException("Invalid type provided to ReadOnlyAttribute");
        }

        EditorGUI.LabelField(position, label.text, str);
    }
}