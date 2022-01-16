using System;
using System.Collections.Generic;

namespace LogicGateSimBackend
{
    class Program
    {
        private static CircuitChip mainChip;
        private static List<Pin> allPins;
        private static List<Chip> allChips;

        private static void Main(string[] _)
        {
            mainChip = new CircuitChip();
            allPins = new List<Pin>();
            allChips = new List<Chip>();

            FileHelper.Initialize();

            RegeneratePinIds();
            RegenerateChipIds();

            while (true)
            {
                try
                {
                    DoCommand();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine();
                }

                RegeneratePinIds();
                RegenerateChipIds();
            }
        }

        private static void DoCommand()
        {
            string[] args;
            args = Console.ReadLine().Split(' ');
            var action = args[0];
            switch (action)
            {
                case "":
                    mainChip.OnTick();
                    Console.WriteLine("Tick");
                    break;
                case "load":
                case "l":
                    Load(args);
                    break;
                case "save":
                case "s":
                    Save();
                    break;
                case "add":
                case "a":
                    Add(args);
                    break;
                case "remove":
                case "r":
                    Remove(args);
                    break;
                case "connect":
                case "c":
                    Connect(args);
                    break;
                case "disconnect":
                case "dc":
                    Disconnect(args);
                    break;
                case "print":
                case "p":
                    Print();
                    break;
                case "name":
                    Name(args);
                    break;
                case "set":
                    Set(args);
                    break;
            }
        }

        private static void Load(string[] args)
        {
            mainChip = FileHelper.Deserialize(args[1]);
            Console.WriteLine($"{args[1]} file loaded");
        }

        private static void Save()
        {
            FileHelper.DeepSerialize(mainChip);
            Console.WriteLine($"{mainChip.Name} saved");
        }

        private static void Add(string[] args)
        {
            switch (args[1])
            {
                case "chip":
                    Chip chip;
                    if (Chip.IsPrimative(args[2], out PrimativeChipType chipType))
                    {
                        chip = new PrimativeChip(chipType);
                    }
                    else
                    {
                        chip = FileHelper.Deserialize(args[2]);
                    }
                    mainChip.AddChip(chip);
                    break;
                case "pin":
                    var name = "";
                    if (args.Length >= 3) name = args[2];
                    mainChip.AddPin(new Pin(name));
                    break;
            }
        }

        // remove [chip/pin] [id/name]
        private static void Remove(string[] args)
        {
            switch (args[1])
            {
                case "chip":
                    var chip = GetChipFromString(args[2]);
                    if (chip == mainChip)
                    {
                        Console.WriteLine("Cannot remove main chip");
                        return;
                    }
                    mainChip.RemoveChip(chip);
                    break;
                case "pin":
                    var pin = GetPinFromString(args[2]);
                    mainChip.RemovePin(pin);
                    break;
                default:
                    return;
            }
        }

        // connect [id/name] [id/name]
        private static void Connect(string[] args)
        {
            var pinA = GetPinFromString(args[1]);
            var pinB = GetPinFromString(args[2]);
            Pin.Connect(pinA, pinB);
            Console.WriteLine("Pins connected");
        }

        private static void Disconnect(string[] args)
        {
            var pin = GetPinFromString(args[1]);
            Pin.Disconnect(pin);
            Console.WriteLine("Pin disconnected");
        }

        private static void Print()
        {
            Console.WriteLine("\nMain Chip");
            PrintChip(mainChip);
            foreach (var pin in mainChip.Pins) PrintPin(pin);
            Console.WriteLine("\nAttached Chips");
            foreach (var chip in mainChip.Chips)
            {
                PrintChip(chip);
                foreach (var pin in chip.Pins) PrintPin(pin);
                Console.WriteLine();
            }
            Console.WriteLine("Pin States"); // TODO
            foreach (var pinState in mainChip.PinStates)
            {
                Console.WriteLine($"[{pinState.Id}]\tS\tPin State\t{pinState.State}");
                foreach (var pin in pinState.Pins)
                {
                    PrintPin(pin);
                }
                Console.WriteLine();
            }
        }

        private static void PrintPin(Pin pin)
        {
            var pinId = GetPinId(pin);
            var name = pin.Name == "" ? "(unnamed)" : pin.Name;
            var s = string.Format("[{0}]\tP\t{1,-10} {2}\t{3}", pinId, name, pin.PinState.Id, pin.InternalState);
            Console.WriteLine(s);
        }

        private static void PrintChip(Chip chip)
        {
            var chipId = GetChipId(chip);
            var name = chip.Name == "" ? "(unnamed)" : chip.Name;
            Console.WriteLine($"[{chipId}]\tC\t{name}");
        }

        private static int GetPinId(Pin pin)
        {
            return allPins.IndexOf(pin);
        }

        private static int GetChipId(Chip chip)
        {
            return allChips.IndexOf(chip);
        }

        // can parse either a name or an id to a pin
        private static Pin GetPinFromString(string str)
        {
            if (int.TryParse(str, out int pinId))
            {
                if (pinId >= allPins.Count) throw new InvalidOperationException("Invalid pin id");
                return allPins[pinId];
            }

            var pinsWithMatchingName = allPins.FindAll(pin => pin.Name == str);

            if (pinsWithMatchingName.Count > 1)
                throw new InvalidOperationException("Ambiguous pin name, use pin id instead");

            if (pinsWithMatchingName.Count == 0)
                throw new InvalidOperationException("Could not find pin");

            return pinsWithMatchingName[0];
        }

        private static Chip GetChipFromString(string str)
        {
            if (int.TryParse(str, out int chipId))
            {
                if (chipId >= allChips.Count) throw new InvalidOperationException("Invalid chip id");

                return allChips[chipId];
            }

            var chipsWithMatchingName = allChips.FindAll(chip => chip.Name == str);

            if (chipsWithMatchingName.Count > 1)
                throw new InvalidOperationException("Ambiguous chip name, use chip id instead");

            if (chipsWithMatchingName.Count == 0)
                throw new InvalidOperationException("Could not find chip");

            return chipsWithMatchingName[0];
        }

        // pins are reassigned ids which can be used to select pins
        private static void RegeneratePinIds()
        {
            allPins.Clear();

            foreach (var pin in mainChip.Pins)
            {
                allPins.Add(pin);
            }

            foreach (var chip in mainChip.Chips)
            {
                foreach (var pin in chip.Pins)
                {
                    allPins.Add(pin);
                }
            }
        }

        private static void RegenerateChipIds()
        {
            allChips.Clear();
            allChips.Add(mainChip);
            foreach (var chip in mainChip.Chips)
            {
                allChips.Add(chip);
            }
        }

        private static void Name(string[] args)
        {
            switch (args[1])
            {
                case "chip":
                    mainChip.Name = args[2];
                    break;
                case "pin":
                    var pin = GetPinFromString(args[2]);
                    pin.Name = args[3];
                    break;
                default:
                    return;
            }
            Console.WriteLine("Naming successful");
        }

        // set [pin number/pin name] [true/false]
        private static void Set(string[] args)
        {
            var pin = GetPinFromString(args[1]);
            var newValue = args[2];
            if (newValue == "true")
            {
                pin.State = true;
            }
            else if (newValue == "false")
            {
                pin.State = false;
            }
            else
            {
                throw new InvalidOperationException("Unknown pin state to set");
            }
            Console.WriteLine($"Pin set to {newValue}");
        }
    }
}
