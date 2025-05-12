using UnityEngine.InputSystem;
using UnityEngine;

namespace RidersRuntime
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInstanceController : MonoBehaviour
    {
        public PlayerInput playerInput;
        public bool isRacingMode = false;

        public void SwitchToUI()
        {
            isRacingMode = false;
        }

        public void SwitchToRacing()
        {
            isRacingMode = true;
        }

        // When we create the racers, we will need to directly attach the racer to the player

        void Awake()
        {
            playerInput = GetComponent<PlayerInput>();

            // We can get the player index from playerInput.playerIndex
        }
    }
}