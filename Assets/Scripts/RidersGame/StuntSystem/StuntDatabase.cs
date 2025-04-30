using System.Collections.Generic;
using System.Linq;

namespace RidersRuntime.StuntSystem
{
    public class StuntDatabase
    {
        public List<Stunt> stunts = new List<Stunt>();

        public void PopulateStunts()
        {
            // Example of populating the stunt database
            // If for efficiency, we can populate into a tree structure

            // TODO: Add frame limit & priorities
            // TODO: Single slides are broken atm. Absolutely needs priority

            stunts.Add(new Stunt(4, "Pop Shuvit", StuntType.Flip, 2, 80, 2));
            // stunts.Add(new Stunt(5, "Backside Pop Shuvit", StuntType.Flip, 2, 80, new string[] { "2", "1", "4", StuntKeys.StuntA }));
            stunts.Add(new Stunt(2, "Kickflip", StuntType.Flip, 1, 30, 4));
            stunts.Add(new Stunt(3, "Heelflip", StuntType.Flip, 1, 30, 6));
            stunts.Add(new Stunt(7, "Nose Slide", StuntType.Grind, 1, 50, 4));
            stunts.Add(new Stunt(8, "Tail Slide", StuntType.Grind, 1, 50, 6));
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