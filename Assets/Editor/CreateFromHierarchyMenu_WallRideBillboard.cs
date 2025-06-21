using UnityEditor;
using UnityEngine;

public class CreateFromHierarchyMenu_WallRideBillboard
{
    [MenuItem("GameObject/Project Riders/Wall Ride (Billboard)", false, 0)]
    public static void CreateFromHierarchyMenu(MenuCommand menuCommand)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/WallRideBillBoard.prefab");
        if (prefab == null)
        {
            Debug.LogError("WallRideBillBoard.prefab not found at the specified path.");
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
            Debug.LogWarning("No GameObject selected. Placing Wall Ride (Billboard) in the root.");
        }

        Undo.RegisterCreatedObjectUndo(instance, "Create Wall Ride (Billboard) Prefab");

        Selection.activeGameObject = instance;
    }
}
