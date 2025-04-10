using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct FrameKeyData
{
    public int frameCount;
    public List<string> inputs;
}

public enum StuntType
{
    None,
    Jump,
    Spin,
    Flip,
    Grind
}

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

        stunts.Add(new Stunt(4, "Frontside Pop Shuvit", StuntType.Flip, 2, 80, new string[] { "2", "3", "6", "StuntA" }));
        stunts.Add(new Stunt(5, "Backside Pop Shuvit", StuntType.Flip, 2, 80, new string[] { "2", "1", "4", "StuntA" }));
        stunts.Add(new Stunt(2, "Kickflip", StuntType.Flip, 1, 30, new string[] { "6", "StuntA" }));
        stunts.Add(new Stunt(3, "Heelflip", StuntType.Flip, 1, 30, new string[] { "4", "StuntA" }));
        stunts.Add(new Stunt(6, "50-50 Grind", StuntType.Grind, 1, 30, new string[] { "5", "StuntB" }));
        stunts.Add(new Stunt(7, "Nose Slide", StuntType.Grind, 1, 50, new string[] { "4", "StuntB" }));
        stunts.Add(new Stunt(8, "Tail Slide", StuntType.Grind, 1, 50, new string[] { "6", "StuntB" }));
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

public struct Stunt
{
    public int id;
    public string name;
    public StuntType type;

    public int difficulty;
    public int reward;

    // Change to int but we also need to change the trick buttons to match it
    public string[] comboKeys;

    public static Stunt None = new Stunt(0, "None", StuntType.None, 0, 0, new string[] { });

    public Stunt(int id, string name, StuntType type, int difficulty, int reward, string[] comboKeys)
    {
        this.id = id;
        this.name = name;
        this.type = type;
        this.difficulty = difficulty;
        this.reward = reward;
        this.comboKeys = comboKeys;
    }

    public static bool operator ==(Stunt a, Stunt b) => a.id == b.id;
    public static bool operator !=(Stunt a, Stunt b) => a.id != b.id;

    public override bool Equals(object obj)
    {
        if (obj is Stunt otherStunt)
        {
            return this == otherStunt;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return id.GetHashCode() ^ name.GetHashCode() ^ type.GetHashCode() ^ difficulty.GetHashCode() ^ reward.GetHashCode();
    }
}


