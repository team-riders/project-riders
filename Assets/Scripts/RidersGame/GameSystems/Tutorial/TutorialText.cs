using RidersRuntime.Input;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using static UnityEngine.UIElements.UxmlAttributeDescription;

namespace RidersRuntime
{

    public class TutorialText : MonoBehaviour
    {

        public TutorialManager tutorialManager;
        public TextMeshProUGUI text;

        // inputs used in text
        string Accelerate = "W";
        string Reverse = "S";
        string Turn = "A and D";
        string Jump = "Space";
        string Boost = ".";

        //KBM inputs
        string KBMAccelerate = "W";
        string KBMReverse = "S";
        string KBMTurn = "A and D";
        string KBMJump = "Space";
        string KBMBoost = ".";

        //PlayStation inputs
        string PSAccelerate = "Forward";
        string PSReverse = "Backward";
        string PSTurn = "Left and Right";
        string PSJump = "X";
        string PSBoost = "L1";

        //XBOX inputs
        string XBAccelerate = "Forward";
        string XBReverse = "Backward";
        string XBTurn = "Left and Right";
        string XBJump = "A";
        string XBBoost = "LB";

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {

            // this is disgusting
            ChangeInputText(UnityEngine.Input.GetJoystickNames());

            if (tutorialManager.popUpIndex == 0)
            {
                text.SetText("Hold " + Accelerate + " to move forward");
            }
            if (tutorialManager.popUpIndex == 1)
            {
                text.SetText("Hold " + Reverse + " to brake and move backwards"); 
            }
            if (tutorialManager.popUpIndex == 2)
            {
                text.SetText("Use " + Turn + " to turn\r\nFollow the path!");
            }
            if (tutorialManager.popUpIndex == 3)
            {
                text.SetText("Hold " + Jump + " to jump\r\nHold it for longer to jump higher!");
            }
            if (tutorialManager.popUpIndex == 4)
            {
                text.SetText("Jump on grind rails \r\nto start grinding\r\nHold " + Accelerate + " to move faster\r\nHold " + Reverse + " to move slower");
            }
            if (tutorialManager.popUpIndex == 5)
            {
                text.SetText("Press " + Boost + " to boost");
            }
            if (tutorialManager.popUpIndex == 6)
            {
                text.SetText("Congrats on completing the tutorial!\r\nReturn to the center checkpoint to leave.");
            }
        }

        void ChangeInputText(string[] inputs)
        {
            Debug.Log(inputs[0]);
            Debug.Log(inputs[0] == "Wireless Controller");
            if (inputs[0] == "Wireless Controller")
            {
                Accelerate = PSAccelerate;
                Reverse = PSReverse;
                Turn = PSTurn;
                Jump = PSJump;
                Boost = PSBoost;
            }
            else if (inputs[0] == "XBox Controller")
            {
                Accelerate = XBAccelerate;
                Reverse = XBReverse;
                Turn = XBTurn;
                Jump = XBJump;
                Boost = XBBoost;
            }
            else
            {
                Accelerate = KBMAccelerate;
                Reverse = KBMReverse;
                Turn = KBMTurn;
                Jump = KBMJump;
                Boost = KBMBoost;
            }
        }
    }
}
