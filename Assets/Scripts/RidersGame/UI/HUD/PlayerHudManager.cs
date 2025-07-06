using UnityEngine;
using UnityEngine.UI;

namespace RidersRuntime.VehicleSystem
{
    [RequireComponent(typeof(GetPlayerInfo))]
    public class PlayerHUDManager : MonoBehaviour
    {
        [SerializeField] private Image boostBarImage;
        private GetPlayerInfo playerInfoSource;

        void Start()
        {
            playerInfoSource = GetComponent<GetPlayerInfo>();
            if (playerInfoSource == null)
            {
                Debug.LogError("GetPlayerInfo not found on this GameObject.");
                return;
            }

            if (boostBarImage == null)
            {
                Debug.LogError("Boost Bar Image not assigned in the inspector.");
                return;
            }
        }

        void Update()
        {
            var playerInfo = playerInfoSource.GetPlayerInfoBundle();
            float fillAmount = Mathf.Clamp01(playerInfo.Boost / playerInfo.BoostGaugeMax);
            boostBarImage.fillAmount = fillAmount;
        }
    }
}
