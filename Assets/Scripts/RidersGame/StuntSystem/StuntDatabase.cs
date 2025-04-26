using System.Collections.Generic;
using System.Linq;
using StuntKeys = RidersRuntime.Input.StuntButtonNamesShort;

namespace RidersRuntime.StuntSystem
{
    public class StuntDatabase
    {
        public List<Stunt> stunts = new List<Stunt>();
        public int frameDelayMax = 15;

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

        public Stunt QueryStuntByFrames(StuntType stuntType, List<FrameKeyData> data)
        {
            // dogshit o(n!))
            // Copy the whole list over that match the stunt type
            List<Stunt> currentValidStunt = stunts.FindAll(stunt => stunt.type == stuntType);
            List<string> keys = GetLastPressedKeys(data.Select(frame => frame.inputs).ToList());

            foreach (var stunt in stunts)
            {
                int comboIndex = 0;
                for (int i = 0; i < data.Count; i++)
                {
                    // Check if the current frame contains the next combo key
                    if (comboIndex < stunt.comboKeys.Length && data[i].inputs.Contains(stunt.comboKeys[comboIndex]))
                    {
                        comboIndex++;
                    }

                    if (comboIndex == stunt.comboKeys.Length)
                    {
                        // From here we then check the frame count
                        // Check if the frame count is within the allowed range
                        int frameCount = 0;
                        int startingFrame = i - stunt.comboKeys.Length + 1;
                        for (int j = startingFrame; j < data.Count; j++)
                        {
                            frameCount += data[j].frameCount;
                        }

                        if (frameCount <= frameDelayMax) return stunt;
                        else comboIndex = 0;
                    }
                }
            }

            return Stunt.None; // No matching stunt combo found
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