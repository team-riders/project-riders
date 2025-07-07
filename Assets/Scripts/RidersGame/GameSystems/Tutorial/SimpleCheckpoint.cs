using UnityEngine;

namespace RidersRuntime
{
    public class SimpleCheckpoint : MonoBehaviour
    {
        // THIS EXISTS ONLY BECAUSE I CAN'T BE ASSED FIGURING OUT HOW TO GET THE TUTORIAL MANAGER TO REUSE THE CHECKPOINTS LIAM HAS ALREADY MADE. 
        // THEY'RE OVERLY COMPLEX FOR WHAT I WANT FOR THE TUTORIAL MANAGER AND I CAN'T EVEN BEGIN TO UNPACK HOW TO APPLY THEM.
        // DO NOT PUSH UNTIL I GO THROUGH LIAM TO FIND A BETTER SOLUTION.

        // tutorial assumes that only one player exists at all times. there is no reason for there to be multiple.

        private TutorialManager tutorialManager;

        private void Awake()
        {
            tutorialManager = GameObject.Find("Tutorial Manager").GetComponent<TutorialManager>();
        }

        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            tutorialManager.popUpIndex++;
        }
    }
}
