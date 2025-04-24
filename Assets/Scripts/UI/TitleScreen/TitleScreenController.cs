using UnityEngine;

public class TitleScreenController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject titleUI;   // Assign TitleScreenUI here
    public GameObject menuUI;    // Assign MenuScreenUI here

    private float idleTimer = 0f;
    private float idleThreshold = 45f;
    private bool hasPressedKey = false;

    void Start()
    {
        titleUI.SetActive(true);
        menuUI.SetActive(false);
    }

    void Update()
    {
        if (!hasPressedKey && Input.anyKeyDown)
        {
            ShowMenu();
        }

        idleTimer += Time.deltaTime;
        if (idleTimer >= idleThreshold && !hasPressedKey)
        {
            Debug.Log("Idle timeout reached - running placeholder function");
            idleTimer = 0f;
        }
    }

    void ShowMenu()
    {
        hasPressedKey = true;
        titleUI.SetActive(false);
        menuUI.SetActive(true);
    }
}
