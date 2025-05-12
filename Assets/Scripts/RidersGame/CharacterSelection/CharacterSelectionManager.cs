using System;
using System.Collections.Generic;
using RidersRuntime.Data;
using UnityEngine;

namespace RidersRuntime.GameSystems
{
    public class CharacterSelectionManager : MonoBehaviour
    {
        List<RiderSelection> playerSelectedRiders = new();

        Action<List<RiderSelection>> onCharacterSelectionComplete;

        public void GrabHostInformation(RaceMeetConfiguration meetConfiguration, Action<List<RiderSelection>> callback)
        {
            onCharacterSelectionComplete = callback;


            RiderConfig riderConfig = Resources.LoadAll<RiderConfig>(Paths.Riders)[0];
            playerSelectedRiders.Add(new RiderSelection()
            {
                rider = riderConfig,
                vehicleType = VehicleType.Skateboard,
                isPlayer = true
            });
        }

        public void DispatchToMaster()
        {
            onCharacterSelectionComplete?.Invoke(playerSelectedRiders);
        }
    }
}