using UnityEngine;
using UnityEngine.UI;

namespace RidersRuntime.VehicleSystem
{
    [RequireComponent(typeof(GetPlayerInfo))]
    public class PlayerHUDManager : MonoBehaviour
    {
        [SerializeField] private Image boostBarImage;
        [SerializeField] private Transform jumpChargeBarTransform;
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

            if (boostBarImage == null)
            {
                Debug.LogError("Boost Bar Image not assigned in the inspector.");
                return;
            }

            if (jumpChargeBarTransform == null)
            {
                Debug.LogError("Jump Charge Bar Transform not assigned in the inspector.");
                return;
            }
            else
            {
                jumpChargeBarImage = jumpChargeBarTransform.Find("Bar").GetComponent<Image>();
                if (jumpChargeBarImage == null)
                {
                    Debug.LogError("Jump Charge Bar Transform does not have a Bar Image component.");
                }
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
