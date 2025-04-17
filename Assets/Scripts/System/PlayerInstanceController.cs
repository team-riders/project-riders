using UnityEngine.InputSystem;
using UnityEngine;
[RequireComponent(typeof(UnityEngine.InputSystem.PlayerInput))]
public class PlayerInstanceController : MonoBehaviour
{
    public UnityEngine.InputSystem.PlayerInput playerInput;
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
        playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();

        // We can get the player index from playerInput.playerIndex
    }
}