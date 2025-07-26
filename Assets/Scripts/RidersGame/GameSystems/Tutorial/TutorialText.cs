using RidersRuntime.Input;
using System.IO;
using TMPro;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

namespace RidersRuntime
{
    public class TutorialText : MonoBehaviour
    {

        public TutorialManager tutorialManager;
        public TextMeshProUGUI text;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {


            // this part i feel like could be better but honestly i can't be assed thinking of a better way
            // enables appropriate inputs at step
            if (tutorialManager.popUpIndex == 0)
            {
                text.SetText("Hold W to move forward");
            }
            if (tutorialManager.popUpIndex == 1)
            {
                text.SetText("Hold S to brake and move backwards"); 
            }
            if (tutorialManager.popUpIndex == 2)
            {
                text.SetText("Use A and D to turn\r\nFollow the path!");
            }
            if (tutorialManager.popUpIndex == 3)
            {
                text.SetText("Hold Space to jump\r\nHold it for longer to jump higher!");
            }
            if (tutorialManager.popUpIndex == 4)
            {
                text.SetText("Jump on grind rails \r\nto start grinding\r\nHold W to move faster\r\nHold S to move slower");
            }
            if (tutorialManager.popUpIndex == 5)
            {
                text.SetText("Press . to boost");
            }
            if (tutorialManager.popUpIndex == 6)
            {
                text.SetText("Congrats on completing the tutorial!\r\nReturn to the center checkpoint to leave.");
            }
        }
    }
}
