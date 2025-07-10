using UnityEngine;
using UnityEngine.UI;

namespace RidersRuntime.VehicleSystem
{
    [RequireComponent(typeof(GetPlayerInfo))]
    public class PlayerHUDManager : MonoBehaviour
    {
        private Image boostBarImage;
        private Transform jumpChargeBarTransform;
        private Image jumpChargeBarImage;
        private GetPlayerInfo playerInfoSource;

        void Start()
        {
            playerInfoSource = GetComponent<GetPlayerInfo>();
            if (playerInfoSource == null)
            {
                Debug.LogError("GetPlayerInfo not found on this GameObject.");
                return;
            }

            // Find InGameHud on the parent
            Transform inGameHud = transform.parent.Find("InGameHUD");
            if (inGameHud == null)
            {
                Debug.LogError("InGameHud not found on the parent.");
                return;
            }

            // Boost Bar
            Transform boostGauge = inGameHud.Find("BoostGauge");
            if (boostGauge == null)
            {
                Debug.LogError("BoostGauge not found in InGameHUD.");
                return;
            }
            Transform boostBar = boostGauge.Find("Bar");
            if (boostBar == null || (boostBarImage = boostBar.GetComponent<Image>()) == null)
            {
                Debug.LogError("Bar Image not found in BoostGauge.");
                return;
            }

            // Jump Charge Bar
            jumpChargeBarTransform = inGameHud.Find("JumpCharge");
            if (jumpChargeBarTransform == null)
            {
                Debug.LogError("JumpCharge not found in InGameHUD.");
                return;
            }
            Transform jumpBar = jumpChargeBarTransform.Find("Bar");
            if (jumpBar == null || (jumpChargeBarImage = jumpBar.GetComponent<Image>()) == null)
            {
                Debug.LogError("Bar Image not found in JumpCharge.");
                return;
            }
        }

        void Update()
        {
            var playerInfo = playerInfoSource.GetPlayerInfoBundle();
            SetBoostFillAmount(playerInfo);
            SetJumpChargeFillAmount(playerInfo);
        }

        void SetBoostFillAmount(GetPlayerInfo.PlayerInfo playerInfo)
        {
            float fillAmount = Mathf.Clamp01(playerInfo.Boost / playerInfo.BoostGaugeMax);
            boostBarImage.fillAmount = fillAmount;
        }

        void SetJumpChargeFillAmount(GetPlayerInfo.PlayerInfo playerInfo)
        {
            float fillAmount = Mathf.Clamp01(playerInfo.JumpCharge);
            jumpChargeBarImage.fillAmount = fillAmount;

            // Hide the jump charge bar if the charge is zero
            jumpChargeBarTransform.gameObject.SetActive(fillAmount > 0f);
        }
    }
}
