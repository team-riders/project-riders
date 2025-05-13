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

        if (!string.IsNullOrEmpty(controller.SceneName))
        {
            scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(controller.SceneName + " t:Scene")[0]));
        }
        scene = (SceneAsset)EditorGUILayout.ObjectField(scene, typeof(SceneAsset), false);

        if (GUI.changed)
        {
            controller.ID = id;
            controller.Name = Name;
            controller.SceneName = scene != null ? scene.name : string.Empty;
        }
    }

    void OnValidate()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}