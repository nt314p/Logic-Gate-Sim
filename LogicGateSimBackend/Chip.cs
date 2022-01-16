using System.Collections.Generic;

namespace LogicGateSimBackend
{
    public abstract class Chip
    {
        public int Id { get; }
        public string Name { get; set; }
        public List<Pin> Pins => pins;
        private readonly List<Pin> pins;
        public HashSet<PinState> PinStates => pinStates;
        private readonly HashSet<PinState> pinStates;

        private static int nextId = 0;

        private static readonly Dictionary<string, PrimativeChipType> primativeChipDict =
            new Dictionary<string, PrimativeChipType>
            {
                { "And", PrimativeChipType.And },
                { "Or", PrimativeChipType.Or },
                { "Not", PrimativeChipType.Not },
                { "Xor", PrimativeChipType.Xor }
            };

        public Chip(string name)
        {
            Name = name;
            Id = GetNextId();
            pins = new List<Pin>();
            pinStates = new HashSet<PinState>();
        }

        public Chip(string name, int pinCount)
        {
            Name = name;
            Id = GetNextId();
            pins = new List<Pin>();
            pinStates = new HashSet<PinState>();

            for (var i = 0; i < pinCount; i++)
            {
                AddPin(new Pin());
            }
        }

        public void AddPin(Pin pin)
        {
            pins.Add(pin);
            pinStates.Add(pin.PinState);
        }

        public void RemovePin(Pin pin)
        {
            pins.Remove(pin);
            pinStates.Clear();
            foreach (var p in pins)
            {
                pinStates.Add(p.PinState);
            }
        }

        public abstract void OnChipUpdate();

        public static bool IsPrimative(Chip chip)
        {
            return chip is PrimativeChip;
        }

        public static bool IsPrimative(string name, out PrimativeChipType primativeChipType)
        {
            if (primativeChipDict.TryGetValue(name, out PrimativeChipType type))
            {
                primativeChipType = type;
                return true;
            }

            primativeChipType = PrimativeChipType.None;
            return false;
        }

        public override string ToString()
        {
            var ret = GetType().Name + $"; Id: {Id}\n";
            for (int i = 0; i < pins.Count; i++)
            {
                ret += $"{i}: {pins[i]}\n";
            }
            return ret;
        }

        private static int GetNextId()
        {
            int id = nextId;
            nextId++;
            return id;
        }
    }

}