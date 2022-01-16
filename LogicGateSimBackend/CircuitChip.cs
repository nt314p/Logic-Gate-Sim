using System.Collections.Generic;
using System.IO;

namespace LogicGateSimBackend
{
    public class CircuitChip : Chip
    {
        public List<Chip> Chips { get => chips; }
        private readonly List<Chip> chips;

        public CircuitChip() : base("")
        {
            chips = new List<Chip>();
        }

        public CircuitChip(string name) : base(name)
        {
            chips = new List<Chip>();
        }

        public void AddChip(Chip chip)
        {
            chips.Add(chip);
            //pinStates.UnionWith(chip.GetPinStates());
        }

        public void RemoveChip(Chip chip)
        {
            chips.Remove(chip);
            /*pinStates.Clear();
            foreach (var c in chips)
            {
                pinStates.UnionWith(c.GetPinStates());
            }*/
        }

        public void OnTick()
        {
            OnChipUpdate();
        }

        public override void OnChipUpdate()
        {
            foreach (var pinState in PinStates)
            {
                pinState.OnPinStateUpdate();
            }
            foreach (var chip in chips)
            {
                chip.OnChipUpdate();
            }
        }


    }
}