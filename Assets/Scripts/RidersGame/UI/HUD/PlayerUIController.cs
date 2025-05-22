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

    private RaceManager raceManager; // Use RaceManager 
    public int playerIndex = 0;

    void Start()
    {
        raceManager = FindFirstObjectByType<RaceManager>();
    }

    void Update()
    {
        if (raceManager == null) return;

        PlayerUIInfo info = raceManager.GetPlayerUIInfo(playerIndex); // Call method from RaceManager

        positionText.text = $"{info.position + 1}{GetOrdinal(info.position + 1)}";
        lapText.text = $"Lap {info.lap}";
        timerText.text = FormatTime(info.time);
        speedText.text = $"{Mathf.RoundToInt(info.speed)} km/h";

        boostSlider.maxValue = maxBoost;
        boostSlider.value = info.boost;

        if (boostGlowEffect != null)
        {
            boostGlowEffect.enabled = info.isUsingBoost;
        }
    }

    private string GetOrdinal(int number)
    {
        if (number % 100 >= 11 && number % 100 <= 13)
            return "th";

        return (number % 10) switch
        {
            1 => "st",
            2 => "nd",
            3 => "rd",
            _ => "th",
        };
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        float seconds = time % 60f;
        return $"{minutes:00}:{seconds:00.00}";
    }
}
