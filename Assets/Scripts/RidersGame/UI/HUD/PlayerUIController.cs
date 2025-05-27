using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RidersRuntime.RaceManager;

public class PlayerUIController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI positionText;
    public TextMeshProUGUI lapText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI speedText;
    public Slider boostSlider;
    public Image boostGlowEffect;

    [Header("Boost Settings")]
    public float maxBoost = 100f;

    private RaceUIController raceUIController;
    public int playerIndex = 0;

    void Start()
    {
        // Locate the RaceUIController in the scene
        raceUIController = FindFirstObjectByType<RaceUIController>();
        if (raceUIController == null)
        {
            Debug.LogWarning("[PlayerUIController] RaceUIController not found in scene.");
        }
    }

    void Update()
    {
        if (raceUIController == null) return;

        PlayerUIInfo info = raceUIController.GetPlayerUIInfo(playerIndex);

        Debug.Log($"[PlayerUIController] Speed: {info.speed} | Boost: {info.boost} | Lap: {info.lap}");

        // Update position display
        if (positionText != null)
            positionText.text = $"{info.position + 1}{GetOrdinal(info.position + 1)}";

        // Update lap display
        if (lapText != null)
            lapText.text = $"Lap {info.lap}";

        // Update race timer
        if (timerText != null)
            timerText.text = FormatTime(info.time);

        // Update speed display
        if (speedText != null)
            speedText.text = $"{Mathf.RoundToInt(info.speed)} km/h";

        // Update boost UI
        if (boostSlider != null)
        {
            boostSlider.maxValue = maxBoost;
            boostSlider.value = info.boost;
        }

        // Toggle boost glow effect if applicable
        if (boostGlowEffect != null)
            boostGlowEffect.enabled = info.isUsingBoost;
    }

    // Converts a number into an ordinal string (e.g., 1st, 2nd, 3rd)
    private string GetOrdinal(int number)
    {
        if (number % 100 >= 11 && number % 100 <= 13)
            return "th";

        return (number % 10) switch
        {
            1 => "st",
            2 => "nd",
            3 => "rd",
            _ => "th"
        };
    }

    // Converts float time into MM:SS.ss format
    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        float seconds = time % 60f;
        return $"{minutes:00}:{seconds:00.00}";
    }
}
