using System.Collections;
using NUnit.Framework;
using RidersRuntime.VehicleSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PlayModeMovement
{
    RidersRuntime.Input.PlayerInput pInput;
    Blackboard blackboard;

    [SetUp]
    public void SetUp()
    {
        pInput = new RidersRuntime.Input.PlayerInput();
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator PlayModeMovementWithEnumeratorPasses()
    {
        SceneManager.LoadScene("Assets/Scenes/environment/map_rooftop.unity", LoadSceneMode.Single);
        yield return waitForSceneLoad();
        yield return GoRight();
        yield return new WaitForSeconds(3);
        yield return Jump();
        yield return new WaitForSeconds(3);
        yield return GoLeft();
        yield return Jump();
        yield return new WaitForSeconds(2);
    }

    private IEnumerator Jump()
    {
        blackboard.SetValue<bool>("JumpIntention", true);
        yield return null;
        blackboard.SetValue<bool>("JumpIntention", false);
    }

    private IEnumerator GoRight()
    {
        blackboard.SetValue<float>("TurningPower", 1.0f);
        yield return null;
        blackboard.SetValue<float>("TurningPower", 0.0f);
    }

    private IEnumerator GoLeft()
    {
        blackboard.SetValue<float>("TurningPower", -1.0f);
        yield return null;
        blackboard.SetValue<float>("TurningPower", 0.0f);
    }

    private IEnumerator waitForSceneLoad()
    {
        while (SceneManager.GetActiveScene().buildIndex > 0)
        {
            yield return null;
        }
    }
}
