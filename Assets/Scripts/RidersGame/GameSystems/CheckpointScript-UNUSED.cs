using System.Collections.Generic;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] List<GameObject> checkpoints;
    [SerializeField] Vector3 vectorPoint;
    [SerializeField] int checkpointsPassed;
    [SerializeField] int checkpointNum;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Reset button for player to spawn at checkpoint
        if (Input.GetKeyDown(KeyCode.R))
        {
            player.transform.position = vectorPoint;
        }

    }

    // On colliding, checkpoint object will delete so it cant be retriggered by the same player 
    // issue becomes that this will delete for other players aswell
    // need to find a way to ensure this doesnt happen


    // Add checkpoint to a integer variable and only allow access to certain checkpoints based on this value
    private void OnTriggerEnter(Collider other)
    {
        if (checkpointsPassed == checkpointNum)
        {
            vectorPoint = player.transform.position;
            checkpointsPassed += 1;
        }

    }

}
