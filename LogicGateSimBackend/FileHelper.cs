using System;
using System.Collections.Generic;
using System.IO;

namespace LogicGateSimBackend
{
    public class FileHelper
    {
        public const string ChipFileExtension = ".txt";
        public const string ChipFolder = "Chips";

        public FileHelper()
        {
        }

        public static void Initialize()
        {
            if (!Directory.Exists(ChipFolder))
                Directory.CreateDirectory(ChipFolder);
        }

       
        // recursively serializes all chips uses in the chip arg
        public static void DeepSerialize(Chip chip)
        {
            DeepSerialize(chip, new HashSet<string>());
        }

        private static void DeepSerialize(Chip chip, HashSet<string> alreadySerialized)
        {
            if (alreadySerialized.Contains(chip.Name)) return; // do not serialize the same chip

            if (chip is CircuitChip circuitChip) // only serialize chip if it is not primative
            {
                Serialize(circuitChip);
                alreadySerialized.Add(chip.Name);
                foreach (var c in circuitChip.Chips)
                {
                    DeepSerialize(c, alreadySerialized);
                }
            }
        }

        // serializes a single chip (shallowly)
        public static void Serialize(CircuitChip chip)
        {
            var writer = new StreamWriter(NameToPath(chip.Name));

            writer.WriteLine("v:1");
            writer.WriteLine(chip.Name);
            writer.WriteLine($"Pins:{chip.Pins.Count}");
            SerializePins(chip.Pins, writer);

            writer.WriteLine($"Chips:{chip.Chips.Count}");
            foreach (var c in chip.Chips)
            {
                writer.WriteLine(c.Name);
                SerializePins(c.Pins, writer);
            }

            writer.Close();
        }

        private static void SerializePins(List<Pin> pins, StreamWriter writer)
        {
            foreach (var pin in pins) writer.WriteLine(PinToPinField(pin));
        }

        public static CircuitChip Deserialize(string name)
        {
            var path = NameToPath(name);
            if (!File.Exists(path)) throw new FileNotFoundException();
            var reader = new StreamReader(path);

            var version = reader.ReadLine();
            ParseField(version, "v", out _, out int versionNumber);

            if (versionNumber != 1)
            {
                throw new InvalidDataException("Invalid version number");
            }

            var circuitName = reader.ReadLine();
            var circuitChip = new CircuitChip(circuitName);
            var pinStateDict = new Dictionary<int, PinState>();

            var pinsIdentifier = reader.ReadLine();
            ParseField(pinsIdentifier, "Pins", out _, out int pinCount);

            for (var i = 0; i < pinCount; i++)
            {
                var hasPinName = ParseField(reader.ReadLine(), out string pinName, out int pinStateId);

                Pin pinToAdd;
                if (pinStateDict.TryGetValue(pinStateId, out PinState pinState))
                {
                    pinToAdd = new Pin(pinState); // id exists, connect pin to existing pin state
                }
                else
                {
                    pinToAdd = new Pin(); // id does not exist, create new pin state and add to dict 
                    pinStateDict.Add(pinStateId, pinToAdd.PinState);
                }

                if (hasPinName) pinToAdd.Name = pinName;

                circuitChip.AddPin(pinToAdd);
            }

            var chipsIdentifier = reader.ReadLine();
            ParseField(chipsIdentifier, "Chips", out _, out int chipCount);

            for (var i = 0; i < chipCount; i++)
            {
                var chipName = reader.ReadLine();
                Chip chip;
                if (Chip.IsPrimative(chipName, out PrimativeChipType primativeChipType))
                {
                    chip = new PrimativeChip(primativeChipType);
                }
                else
                {
                    chip = Deserialize(chipName);
                }

                for (var pinIndex = 0; pinIndex < chip.Pins.Count; pinIndex++)
                {
                    var currentPin = chip.Pins[pinIndex];

                    var hasPinName = ParseField(reader.ReadLine(), out string pinName, out int pinStateId);
                    if (hasPinName) currentPin.Name = pinName;

                    if (pinStateDict.TryGetValue(pinStateId, out PinState pinState))
                    {
                        PinState.Connect(pinState, currentPin.PinState); // connect on the pinstate level
                    }
                    else
                    {
                        pinStateDict.Add(pinStateId, currentPin.PinState);
                    }
                }

                circuitChip.AddChip(chip);
            }

            return circuitChip;
        }

        // attempts to parse a string in the form [fieldName]:[int value] or [int value]
        // returns true if a field name was found
        private static bool ParseField(string line, out string fieldName, out int value)
        {
            var stringValue = line;
            var hasFieldName = false;
            fieldName = "";
            if (line.Contains(':'))
            {
                var lines = line.Split(':');
                fieldName = lines[0];
                stringValue = lines[1];
                hasFieldName = true;
            }

            if (!int.TryParse(stringValue, out int val))
            {
                throw new InvalidDataException($"Unable to parse field value on line '{line}'");
            }

            value = val;
            return hasFieldName;
        }

        private static bool ParseField(string line, string requiredFieldName, out string fieldName, out int value)
        {
            var hasField = ParseField(line, out fieldName, out value);

            if (!hasField || fieldName != requiredFieldName)
            {
                throw new InvalidDataException($"Expected field name '{requiredFieldName}' but got '{fieldName}'");
            }
            return hasField;
        }

        private static string NameToPath(string name)
        {
            return ChipFolder + "/" + name.Replace(" ", "") + ChipFileExtension;
        }

        private static string PinToPinField(Pin pin)
        {
            var str = pin.Name;
            if (str != "") str += ":";
            return str + pin.PinState.Id;
        }

    }
}