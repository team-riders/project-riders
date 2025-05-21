using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RidersRuntime.StuntSystem
{
    public class StuntDatabase
    {
        public List<Stunt> stunts = new List<Stunt>();

        public void PopulateStunts()
        {
            StuntSO[] stunts = Resources.LoadAll<StuntSO>("Data/Stunts");

            foreach (var stuntSO in stunts)
            {
                Stunt newStunt = new Stunt(stuntSO.stunt.id, stuntSO.stunt.name, stuntSO.stunt.type, stuntSO.stunt.difficulty, stuntSO.stunt.reward, stuntSO.stunt.key);
                this.stunts.Add(newStunt);
            }
        }

        public Stunt QueryStuntByTime(StuntType stuntType, List<TimedInput> inputs)
        {
            List<Stunt> validStunts = stunts.FindAll(s => s.type == stuntType);

            foreach (var stunt in validStunts)
            {
                float startTime = -1f;

                // Find the correct StuntButton based on the StuntType
                string stuntKey = Values.stuntBindings.FirstOrDefault(x => x.Value == stunt.type).Key;
                TimedInput? input = inputs.FirstOrDefault(input => input.key == stunt.key.ToString());

                // Check if the stuntKey is pressed and then the stunt key is pressed
                if (input.HasValue)
                {
                    // Get the time of the stunt key press
                    startTime = input.Value.time;

                    // Find the first key that matches the stuntKey
                    var keyInput = inputs.FirstOrDefault(input => input.key == stuntKey);
                    if (keyInput.time - startTime <= Values.timeWindowMax)
                    {
                        return stunt;
                    }
                }
            }

            return Stunt.None;
        }

        public List<string> GetLastPressedKeys(List<List<string>> keys)
        {
            List<string> lastPressedKeys = new();
            List<string> previous = new();

            foreach (var keystrokes in keys)
            {
                var newKeys = keystrokes.Except(previous).ToList();
                if (newKeys.Count > 0) lastPressedKeys.Add(newKeys[0]);

                previous = keystrokes;
            }

            return lastPressedKeys;
        }
    }
}