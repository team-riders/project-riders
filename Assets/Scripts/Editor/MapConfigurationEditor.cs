using RidersRuntime.Data;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(MapConfiguration))]
public class MapConfigurationEditor : Editor
{
    int id;
    string Name;
    SceneAsset scene;

    public override void OnInspectorGUI()
    {
        MapConfiguration controller = (MapConfiguration)target;

        EditorGUILayout.LabelField("Map Configuration", EditorStyles.boldLabel);
        id = EditorGUILayout.IntField("ID", controller.ID);
        Name = EditorGUILayout.TextField("Name", controller.Name);
        // Get the scene asset from the project

        if (!string.IsNullOrEmpty(controller.ScenePath))
        {
            scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(controller.ScenePath);
        }
        scene = (SceneAsset)EditorGUILayout.ObjectField("Scene Asset", scene, typeof(SceneAsset), false);
        controller.ID = id;
        controller.Name = Name;
        controller.ScenePath = scene != null ? AssetDatabase.GetAssetPath(scene) : string.Empty;

        if (GUI.changed)
        {
            EditorUtility.SetDirty(controller);
        }
    }

    void OnValidate()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}