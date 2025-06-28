using System.Collections.Generic;
using UnityEngine;

namespace RidersRuntime.StuntSystem
{
    public class StuntSystem : MonoBehaviour
    {
        // VehicleType currentVehicleType;

        StuntDatabase stuntDatabase;
        List<Stunt> stuntsRecorded = new List<Stunt>();

        void Start()
        {
            // currentVehicleType = VehicleType.Skateboard;
            PopulateStunts();
        }

        void PopulateStunts()
        {
            stuntDatabase = new();
            stuntDatabase.PopulateStunts();
        }

        public void OnStuntRequestTimed(List<TimedInput> timedInputs, StuntType stuntType)
        {
            // Handle the stunt request with frame data
            if (stuntType != StuntType.None)
            {
                // Query the stunt database for the stunt
                Stunt stunt = stuntDatabase.QueryStuntByTime(stuntType, timedInputs);
                if (stunt != Stunt.None)
                {
                    // Execute the stunt
                    Debug.Log($"Executing stunt: {stunt.name} of type {stunt.type}");
                    // Add your stunt execution logic here
                    RecordStunt(stunt);
                }
                else
                {
                    Debug.Log("No valid stunt found for the given frame data.");
                }
            }
        }

        // Josh - handling landing

        void RecordStunt(Stunt stunt)
        {
            stuntsRecorded.Add(stunt);
        }

        public int ReturnRewardOnLanding()
        {
            int totalReward = 0;
            foreach (Stunt stunt in stuntsRecorded)
            {
                totalReward += stunt.reward;
            }
            stuntsRecorded.RemoveAll(st => st.reward > 0);
            Debug.Log($"Total reward on landing: {totalReward}");
            return totalReward;
        }
    }
}