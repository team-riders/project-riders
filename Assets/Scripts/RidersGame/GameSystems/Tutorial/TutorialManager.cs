using UnityEngine;

namespace RidersRuntime
{
    public class TutorialManager : MonoBehaviour
    {
        public GameObject[] popUps;
        private int popUpIndex;

        public PlayerInstanceController player;

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < popUps.Length; i++)
            {
                if (i == popUpIndex)
                {
                    popUps[popUpIndex].gameObject.SetActive(true);
                }
                else
                {
                    popUps[popUpIndex].gameObject.SetActive(false);
                }
            }
        }
    }
}
