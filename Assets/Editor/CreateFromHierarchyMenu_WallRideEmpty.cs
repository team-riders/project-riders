using UnityEditor;
using UnityEngine;

public class CreateFromHierarchyMenu_WallRideEmpty
{
    [MenuItem("GameObject/Project Riders/Wall Ride (Empty)", false, 0)]
    public static void CreateFromHierarchyMenu(MenuCommand menuCommand)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/WallRideEmpty.prefab");
        if (prefab == null)
        {
            Debug.LogError("WallRideEmpty.prefab not found at the specified path.");
            return;
        }

        GameObject parent = Selection.activeGameObject;

        if (menuCommand.context as GameObject != null)
        {
            parent = menuCommand.context as GameObject;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        if (parent != null)
        {
            GameObjectUtility.SetParentAndAlign(instance, parent);
        }
        else
        {
            Debug.LogWarning("No GameObject selected. Placing Wall Ride (Empty) in the root.");
        }

        Undo.RegisterCreatedObjectUndo(instance, "Create Wall Ride (Empty) Prefab");

        Selection.activeGameObject = instance;
    }
}
