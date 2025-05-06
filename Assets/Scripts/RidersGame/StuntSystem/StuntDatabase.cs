using System.Collections.Generic;
using System.Linq;
using StuntKeys = RidersRuntime.Input.StuntButtonNamesShort;

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

            stunts.Add(new Stunt(4, "Frontside Pop Shuvit", StuntType.Flip, 2, 80, new string[] { "2", "3", "6", StuntKeys.StuntA }));
            stunts.Add(new Stunt(5, "Backside Pop Shuvit", StuntType.Flip, 2, 80, new string[] { "2", "1", "4", StuntKeys.StuntA }));
            stunts.Add(new Stunt(2, "Kickflip", StuntType.Flip, 1, 30, new string[] { "6", StuntKeys.StuntA }));
            stunts.Add(new Stunt(3, "Heelflip", StuntType.Flip, 1, 30, new string[] { "4", StuntKeys.StuntA }));
            stunts.Add(new Stunt(7, "Nose Slide", StuntType.Grind, 1, 50, new string[] { "4", StuntKeys.StuntB }));
            stunts.Add(new Stunt(8, "Tail Slide", StuntType.Grind, 1, 50, new string[] { "6", StuntKeys.StuntB }));
            stunts.Add(new Stunt(6, "50-50 Grind", StuntType.Grind, 1, 30, new string[] { "5", StuntKeys.StuntB }));
            // Add more stunts as needed
        }

        public Stunt QueryStuntByTime(StuntType stuntType, List<TimedInput> inputs)
        {
            List<Stunt> validStunts = stunts.FindAll(s => s.type == stuntType);

            foreach (var stunt in validStunts)
            {
                int comboIdx = 0;
                float startTime = -1f;

                foreach (var input in inputs)
                {
                    if (input.key == stunt.comboKeys[comboIdx])
                    {
                        if (comboIdx == 0) startTime = input.time;
                        comboIdx++;

                        if (comboIdx == stunt.comboKeys.Length)
                        {
                            if (input.time - startTime <= Values.timeWindowMax)
                                return stunt;
                            else
                                break;
                        }
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