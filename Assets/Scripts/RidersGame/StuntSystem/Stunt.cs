using System;

namespace RidersRuntime.StuntSystem
{
    [Serializable]
    public struct Stunt
    {
        public int id;
        public string name;
        public StuntType type;

        public int difficulty;
        public int reward;

        public int key;

        public static Stunt None = new Stunt(0, "None", StuntType.None, 0, 0, 0);

        public Stunt(int id, string name, StuntType type, int difficulty, int reward, int key)
        {
            this.id = id;
            this.name = name;
            this.type = type;
            this.difficulty = difficulty;
            this.reward = reward;
            this.key = key;
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
}