using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(ScrollRectEx), true)]
public class ScrollRectExEditor : ScrollRectEditor
{

    SerializedProperty m_GameModeId;
    protected override void OnEnable()
    {
        base.OnEnable();
        m_GameModeId = serializedObject.FindProperty("m_GameModeId");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.PropertyField(m_GameModeId);
        serializedObject.ApplyModifiedProperties();
    }
}
