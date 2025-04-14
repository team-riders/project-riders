using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "MapConfiguration", menuName = "Project Riders/New Map")]
public class MapConfiguration : ScriptableObject
{
    public int ID;
    public string Name;
    public string SceneName;
    // The scene itself typically contain the following information
    // If we want to do a drag and drop of the scene, we will need to create a custom editor that will compile back to the asset
}
