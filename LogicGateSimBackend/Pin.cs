using System;
using System.Collections.Generic;

namespace LogicGateSimBackend
{
    public class Pin
    {
        public bool State
        {
            get => state || pinState.State; // internal state is not affected 

            set { state = value; }
        }

        public bool InternalState { get => state; }
        public PinState PinState { get => pinState; }
        public string Name { get; set; }

        private PinState pinState;
        private bool state;

        public Pin(PinState pinState)
        {
            Name = "";
            this.pinState = pinState;
            pinState.AddPin(this);
        }

        public Pin()
        {
            Name = "";
            pinState = new PinState();
            pinState.AddPin(this);
        }

        public Pin(string name)
        {
            pinState = new PinState();
            pinState.AddPin(this);
            Name = name;
        }

        // Connects pin b to pin a
        public static void Connect(Pin a, Pin b)
        {
            if (a == b) throw new ArgumentException("Pins are the same");
            PinState.Connect(a.PinState, b.PinState);
        }

        public static void Connect(Pin a, PinState pinState)
        {
            if (a.pinState == pinState) throw new ArgumentException("Pin already has that pin state");
            a.pinState.RemovePin(a);
            a.pinState = pinState;
            a.pinState.AddPin(a);
        }

        // Disconnects pin from previous pin state, then connected to passed in pin state
        public static void Disconnect(Pin pin, PinState pinState)
        {
            pin.pinState.RemovePin(pin);
            pin.pinState = pinState;
            pinState.AddPin(pin);
        }

        // Disconnects pin from previous pin state and is connected to a new pin state
        public static void Disconnect(Pin pin)
        {
            Disconnect(pin, new PinState());
        }

        public static HashSet<PinState> GetPinStates(List<Pin> pins)
        {
            var pinStates = new HashSet<PinState>();
            foreach (var pin in pins)
            {
                pinStates.Add(pin.pinState);
            }

            return pinStates;
        }

        public override string ToString()
        {
            string name = Name == null ? "" : Name + "; ";
            return name + $"Id: {pinState.Id}; Internal state: {InternalState}; PinState: {pinState.State}";
        }
    }
}