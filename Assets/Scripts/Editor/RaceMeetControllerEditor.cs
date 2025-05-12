using System.Collections.Generic;
using UnityEditor;
using RidersRuntime.Data;
using RidersRuntime.RaceManager;

[CustomEditor(typeof(RaceMeetController))]
public class RaceMeetControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RaceMeetController controller = (RaceMeetController)target;
        RaceMeetConfiguration rm = controller.raceMeet;


        DrawDefaultInspector();
        EditorGUILayout.Space(10);
        if (rm)
        {
            EditorGUILayout.LabelField("Race Meet", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("ID: " + rm.ID.ToString() + " Name: " + rm.Name_Meet, EditorStyles.label);

            EditorGUILayout.LabelField("Race Events", EditorStyles.boldLabel);
            List<RaceEventConfiguration> re = controller.raceMeet.raceEvents;

            for (int i = 0; i < re.Count; i++)
            {
                EditorGUILayout.LabelField("ID: " + re[i].ID.ToString() + " Name: " + re[i].RaceName, EditorStyles.label);
                EditorGUILayout.LabelField("Type: " + re[i].RaceType.ToString(), EditorStyles.label);
                EditorGUILayout.LabelField("Number of Laps: " + re[i].NumberOfLaps.ToString(), EditorStyles.label);
                EditorGUILayout.LabelField("Map: " + re[i].map.name, EditorStyles.label);
            }
        }
    }
}