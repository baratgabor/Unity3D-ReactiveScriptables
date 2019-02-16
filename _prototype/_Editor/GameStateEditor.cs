using UnityEditor;
using UnityEngine;

namespace LeakyAbstraction.ReactiveScriptables.Prototyping
{

    // Current design doesn't require invocation helper

    //[CustomEditor(typeof(GameData), true)]
    //public class GameVariableEditor : Editor
    //{
    //    //string[] propertiesInBaseClass = new string[] { };

    //    public override void OnInspectorGUI()
    //    {
    //        //EditorGUILayout.LabelField("Special A Drawing");

    //        //DrawPropertiesExcluding(serializedObject, propertiesInBaseClass);

    //        base.OnInspectorGUI();

    //        GUI.enabled = Application.isPlaying;

    //        GameData v = target as GameData;
    //        if (GUILayout.Button("Invoke value changed event"))
    //            v.Notify();
    //    }
    //}
}