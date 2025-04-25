using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RidersRuntime.Data;

namespace RidersRuntime.StuntSystem
{
    public class StuntSystem : MonoBehaviour
    {
        VehicleType currentVehicleType;

        StuntDatabase stuntDatabase;

        void Start()
        {
            currentVehicleType = VehicleType.Skateboard;
            PopulateStunts();
        }

        void PopulateStunts()
        {
            stuntDatabase = new();
            stuntDatabase.PopulateStunts();
        }

        public void OnStuntRequestWithFrameData(List<FrameKeyData> frameData, StuntType stuntType)
        {
            // Handle the stunt request with frame data
            if (stuntType != StuntType.None)
            {
                // Query the stunt database for the stunt
                Stunt stunt = stuntDatabase.QueryStuntByFrames(stuntType, frameData);
                if (stunt != Stunt.None)
                {
                    // Execute the stunt
                    Debug.Log($"Executing stunt: {stunt.name} of type {stunt.type}");
                    // Add your stunt execution logic here
                }
                else
                {
                    Debug.Log("No valid stunt found for the given frame data.");
                    StuntPeripheralInputHandler.ShowSingleLineOutput(frameData);
                }
            }
        }
    }
}