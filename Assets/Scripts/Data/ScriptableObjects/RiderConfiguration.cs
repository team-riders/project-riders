using UnityEngine;


[CreateAssetMenu(fileName = "New Rider", menuName = "Project Riders/New Rider", order = 0)]
public class RiderConfig : ScriptableObject
{
    public Racer racerInformation;
    public GameObject characterModelPrefab;
}