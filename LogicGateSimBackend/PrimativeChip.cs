using System;
using System.Collections.Generic;

namespace LogicGateSimBackend
{
    public enum PrimativeChipType
    {
        None,
        And,
        Or,
        Not,
        Xor
    }

    public class PrimativeChip : Chip
    {
        private readonly PrimativeChipType primativeChipType;

        public PrimativeChip(PrimativeChipType primativeChipType)
            : base(primativeChipType.ToString(), PrimativeChipPinCount(primativeChipType))
        {
            this.primativeChipType = primativeChipType;
        }

        public override void OnChipUpdate()
        {
            switch (primativeChipType)
            {
                case PrimativeChipType.And:
                    Pins[0].State = Pins[1].State && Pins[2].State;
                    break;
                case PrimativeChipType.Or:
                    Pins[0].State = Pins[1].State || Pins[2].State;
                    break;
                case PrimativeChipType.Not:
                    Pins[0].State = !Pins[1].State;
                    break;
                case PrimativeChipType.Xor:
                    Pins[0].State = Pins[1].State ^ Pins[2].State;
                    break;
            }
        }

        private static int PrimativeChipPinCount(PrimativeChipType chipType)
        {
            switch(chipType)
            {
                case PrimativeChipType.Not: 
                    return 2;
                case PrimativeChipType.And:
                case PrimativeChipType.Or:
                case PrimativeChipType.Xor:
                    return 3;
            }
            return -1;
        }
    }
}