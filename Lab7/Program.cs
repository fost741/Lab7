using System.Collections.Generic;

namespace Lab_7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();

            int maxArg = Menu("Enter the max amount of elements (N > 0): ", 1, 10);
            List<Instruments> instruments = new List<Instruments>();

            while (true)
            {
                Console.WriteLine("\n------MENU------" +
                                    "\n1. Add new\n" +
                                    "2. Show all objects\n" +
                                    "3. Search for object\n" +
                                    "4. Show behaviour\n" +
                                    "5. Delete object\n" +
                                    "6. Show static methods\n" +
                                    "0. Exit");
                int choice = Menu("Choose option: ", 0, 6);
                switch (choice)
                {
                    case 1: AddObj(instruments, maxArg); break;
                    case 2: ShowAll(instruments); break;
                    case 3: Search(instruments); break;
                    case 4: ShowBehaviour(instruments); break;
                    case 5: DeleteObj(instruments); break;
                    case 6: ShowStaticMethods(); break;
                    case 0:
                        Console.WriteLine("Program is ended. Goodbye!");
                        return;
                }
            }
        }

        static int Menu(string message, int min, int max)
        {
            int a;
            while (true)
            {
                Console.Write(message);
                if (int.TryParse(Console.ReadLine(), out a) && a >= min && a <= max) return a;
                Console.WriteLine($"Invalid input. Enter number from {min} to {max} ");
            }
        }

        static void AddObj(List<Instruments> list, int max)
        {
            if (list.Count >= max)
            {
                Console.WriteLine($"Error. Max permitted number of objects is {max}. ");
                return;
            }

            Console.WriteLine("\nAdding options" +
                                "\n1. Add manually\n" +
                                "2. Add randomly\n" + 
                                "3. Add in one string foemat\n");
            int mode = Menu("Choose option: ", 1, 3);

            Instruments inst = null;
            if (mode == 1)
            {
                string name = "";
                while (true)
                {
                    try
                    {
                        Console.Write("Enter name(f.e. piano): ");
                        name = Console.ReadLine()?.Trim();
                        if (!string.IsNullOrEmpty(name)) break;
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                Family selectedFamily = Family.Strings;
                while (true)
                {
                    try
                    {
                        Console.Write("Enter family(strings / woodwinds / brass / percussion / keyboard): ");
                        string inp = Console.ReadLine();

                        string lowName = name.ToLower();
                        if (!string.IsNullOrEmpty(inp) && Enum.TryParse<Family>(inp, true, out selectedFamily))
                        {
                            if ((lowName.Contains("guitar") || lowName.Contains("violin") || lowName.Contains("ukulele")) && selectedFamily != Family.Strings)
                                throw new ArgumentException($"Wrong family. Instrument {name} belongs to STRINGS family.");
                            else if ((lowName.Contains("piano") || lowName.Contains("keyboard")) && selectedFamily != Family.Keyboard)
                                throw new ArgumentException($"Wrong family. Instrument {name} belongs to KEYBOARD family.");
                            else if ((lowName.Contains("flute") || lowName.Contains("clarinet")) && selectedFamily != Family.Woodwinds)
                                throw new ArgumentException($"Wrong family. Instrument {name} belongs to WOODWINDS family.");
                            else if ((lowName.Contains("trumpet")) && selectedFamily != Family.Brass)
                                throw new ArgumentException($"Wrong family. Instrument {name} belongs to BRASS family.");
                            else if ((lowName.Contains("drum")) && selectedFamily != Family.Percussion)
                                throw new ArgumentException($"Wrong family. Instrument {name} belongs to PERCUSSION family.");

                            break;
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }


                Brand selectedBrand = Brand.Yamaha;
                while (true)
                {
                    try
                    {
                        Console.Write("Enter brand(f.e. yamaha): ");
                        string inp = Console.ReadLine();
                        if (!string.IsNullOrEmpty(inp) && Enum.TryParse<Brand>(inp, true, out selectedBrand)) break;
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                bool isElectric = false;
                while (true)
                {
                    Console.Write("Need power or not (1 - yes, 0 - no): ");
                    string inp = Console.ReadLine();

                    if (inp == "1") { isElectric = true; break; }

                    else if (inp == "0") { isElectric = false; break; }
                    else Console.WriteLine("Invalid input. Try again.");

                }

                int stringCount = StringCount(name, selectedFamily);
                int keyCount = KeyCount(selectedFamily, isElectric);

                inst = new Instruments(name, selectedFamily, selectedBrand, isElectric, keyCount, stringCount);

                inst.IsConnected = false;
                Console.WriteLine(inst.Tune());
                inst.CurrentVolume = inst.IsElectric ? Menu("Enter start volume (0-100%): ", 0, 100) : 0;
            }
            else if (mode == 2)
            {
                Random rand = new Random();
                string[] names = { "guitar", "violin", "piano", "keyboard", "drums", "ukulele", "flute", "clarinet", "trumpet" };
                string randName = names[rand.Next(names.Length)];

                string nameToLow = randName.ToLower();
                Family randFamily;
                if (nameToLow.Contains("guitar") || nameToLow.Contains("violin") || nameToLow.Contains("ukulele")) randFamily= Family.Strings;
                else if (nameToLow.Contains("piano") || nameToLow.Contains("keyboard")) randFamily = Family.Keyboard;
                else if (nameToLow.Contains("flute") || nameToLow.Contains("clarinet")) randFamily = Family.Woodwinds;
                else if (nameToLow.Contains("trumpet")) randFamily = Family.Brass;
                else randFamily = Family.Percussion;

                Array brands = Enum.GetValues(typeof(Brand));
                Brand randBrand = (Brand)brands.GetValue(rand.Next(brands.Length));

                inst = new Instruments(randName, randFamily, randBrand);

                inst.IsElectric = rand.Next(0, 2) == 1;

                if (inst.Family == Family.Strings)
                {
                    inst.StringCount = inst.Name.Contains("guitar") ? 6 : 4;
                    inst.KeyCount = 0;
                }
                else if (inst.Family == Family.Keyboard)
                {
                    inst.KeyCount = inst.IsElectric ? 61 : 88;
                    inst.StringCount = 0;
                }
                else
                {
                    inst.StringCount = 0;
                    inst.KeyCount = 0;
                }

                inst.IsConnected = false;
                Console.WriteLine(inst.Tune());
                inst.CurrentVolume = inst.IsElectric ? 50 : 0;
                Console.WriteLine("Object has been generated successfuly! ");
            }
            else if (mode == 3)
            {
                while (true)
                {
                    Console.WriteLine("Enter instrument data in one string format (name, family, brand, isElectric, keyCount, stringCount): ");
                    Console.WriteLine("Example: guitar, Strings, Yamaha, false, 0, 6");
                    string instData = Console.ReadLine();
                    try
                    {
                        inst = Instruments.Parse(instData);
                        Console.WriteLine("Object has been generated successfuly! ");
                            if (inst.IsElectric)
                            {
                                inst.CurrentVolume = Menu("Enter start volume (0-100%): ", 0, 100);
                            }
                            break;
                    } catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}.  Please try again.");
                    }
                }
            }

            list.Add(inst);
            Console.WriteLine($"Instrument {inst.Name} is added ({list.Count} / {max})");
        }

        static void ShowAll(List<Instruments> list)
        {
            if (list.Count == 0)
            {
                Console.WriteLine("\nList is empty!"); return;
            }

            Console.WriteLine($"Current valid instruments count: {Instruments.ValidCount}");
            Console.WriteLine(new string('-', 107));
            Console.WriteLine($"| {"№",-3} | {"Name",-15} | {"Family",-10} | {"Brand",-8} | {"Electric",-8} | {"Keys",-5} | {"Strings",-7} | {"Tuned",-6} | {"Vol",-4} | {"Sound Type",-8} |");
            Console.WriteLine(new string('-', 107));

            for (int i = 0; i < list.Count; i++)
            {
                PrintRow(i + 1, list[i]);
            }
            Console.WriteLine(new string('-', 107));
        }

        static void PrintRow(int num, Instruments inst)
        {
            string keys = inst.KeyCount == 0 ? "-" : inst.KeyCount.ToString();
            string strings = inst.StringCount == 0 ? "-" : inst.StringCount.ToString();
            string volume = inst.IsElectric == false ? "-" : inst.CurrentVolume.ToString();

            Console.WriteLine($"| {num,-3} | {inst.Name,-15} | {inst.Family,-10} | {inst.Brand,-8} | {inst.IsElectric,-8} | {keys,-5} | {strings,-7} | {inst.IsTuned,-6} | {volume,-4} | {inst.SoundType,-8} |");
        }

        static void Search(List<Instruments> list)
        {
            if (list.Count == 0)
            {
                Console.WriteLine("\nList is empty!"); return;
            }

            int choice = Menu("\nChoose option to search: \n1-name \n2-family \n3-brand \n4-is electric \n0-back to main menu\n", 0, 4);
            int count = 0;

            List<Instruments> searchResult = new List<Instruments>();
            switch (choice)
            {
                case 1:
                    while (true)
                    {
                        Console.WriteLine("Enter name to find: ");
                        string searchName = Console.ReadLine().ToLower().Trim();

                        if (!string.IsNullOrEmpty(searchName))
                        {
                            searchResult = list.FindAll(x => x.Name.ToLower().Contains(searchName));
                        }
                        break;
                    }
                    break;
                case 2:
                    Console.Write("Enter family (Strings, Woodwinds, Brass, Percussion, Keyboard): ");
                    string familyInp = Console.ReadLine();

                    if (Enum.TryParse(familyInp, true, out Family searchFamily) && Enum.IsDefined(typeof(Family), searchFamily))
                    {
                        searchResult = list.FindAll(x => x.Family == searchFamily);
                    }
                    else
                    {
                        Console.WriteLine("Invalid family name.");
                        return;
                    }
                    break;
                case 3:
                    Console.Write("Enter brand (Yamaha, Gibson, Pearl, Roland): ");
                    string brandInp = Console.ReadLine();

                    if (Enum.TryParse(brandInp, true, out Brand searchBrand) && Enum.IsDefined(typeof(Brand), searchBrand))
                    {
                        searchResult = list.FindAll(x => x.Brand == searchBrand);
                    }
                    else
                    {
                        Console.WriteLine("Invalid brand name.");
                    }
                    break;
                case 4:
                    int electric = Menu("Search for: 1 - Electric, 0 - Acoustic: ", 0, 1);
                    bool isElec = electric == 1;
                    searchResult = list.FindAll(x => x.IsElectric == isElec);
                    break;
                case 0:
                    return;
            }
            if (searchResult.Count == 0)
            {
                Console.WriteLine("Such elements were not found. ");
            }
            else
            {
                Console.WriteLine($"\nFound {searchResult.Count} matching instruments:");
                ShowAll(searchResult);
            }
        }

        static void ShowBehaviour(List<Instruments> list)
        {
            if (list.Count == 0) { Console.WriteLine("\nNo instruments to display."); return; }

            ShowAll(list);
            int index = Menu("Choose instrument number to show behaviour (0 to cancel): ", 0, list.Count);
            if (index == 0) return;

            while (true)
            {
                Console.WriteLine("\n------BEHAVIOUR MENU------" +
                                    "\n1. Tune\n" +
                                    "2. Play\n" +
                                    "3. Connect\n" +
                                    "4. Set volume\n" +
                                    "0. Back to main menu");
                int choice = Menu("Choose option: ", 0, 4);

                switch (choice)
                {
                    case 1:
                        Console.WriteLine(list[index - 1].Tune());
                        break;
                    case 2:
                        Console.WriteLine(list[index - 1].Play());
                        break;
                    case 3:
                        Console.WriteLine(list[index - 1].Connect());
                        break;
                    case 4:
                        if (!list[index - 1].IsElectric)
                        {
                            Console.WriteLine("Error: This instrument is not electric and does not have volume control.");
                            break;
                        }

                        Console.WriteLine("\n Volume Modes " + "\n1.Standart set" + "\n2.Fading out" + "\n3.Based on room type");
                        int volumeMode = Menu("Choose volume mode: ", 1, 3);

                        if (volumeMode == 1)
                        {
                            int newVolume = Menu("Enter new volume (0-100%): ", 0, 100);
                            list[index - 1].SetVolume(newVolume);
                            Console.WriteLine("Volume set to standard level.");
                        }
                        else if (volumeMode == 2)
                        {
                            int seconds = Menu("Enter duration for fading out (seconds): ", 0, 60);
                            bool isMuted = Menu("Mute after fading out completely? (1 - yes, 0 - no): ", 0, 1) == 1;
                            list[index-1].SetVolume(seconds, isMuted);
                            Console.WriteLine("Volume fading out.");
                        }
                        else if (volumeMode == 3)
                        {
                            string room = Menu("Choose room type (1-concert hall, 2-studio, 3-home): ", 1, 3) switch
                            {
                                1 => "concert hall",
                                2 => "studio",
                                3 => "home",
                                _ => throw new ArgumentException("Invalid room type.")
                            };
                            list[index-1].SetVolume(list[index - 1].CurrentVolume, room);
                            Console.WriteLine("Volume adjusted based on room type.");
                        }

                        break;
                    case 0:
                        return;
                }
            }
        }

        static void DeleteObj(List<Instruments> list)
        {
            if (list.Count == 0) { Console.WriteLine("\nNo instruments to delete."); return; }
            ShowAll(list);
            int index = Menu("Choose instrument number to delete (0 to cancel): ", 0, list.Count);
            if (index == 0) return;
            Instruments instToDel = list[index - 1];
            list.RemoveAt(index - 1);
            Console.WriteLine($"Instrument {instToDel.Name} has been deleted.");
        }

        static void ShowStaticMethods()
        {
            Console.WriteLine("\n------STATIC METHODS MENU------" +
                                "\n1. Change max allowed volume\n" +
                                "2. Get recommended volume by room area\n" +
                                "0. Back to main menu");
            int choice = Menu("Choose option: ", 0, 2);
            switch (choice)
            {
                case 1:
                    int newMaxVolume = Menu("Enter new max allowed volume (0-100%): ", 0, 100);
                    Instruments.MaxAllowedVolume = newMaxVolume;
                    Console.WriteLine($"Max allowed volume changed to {newMaxVolume}%.");
                    break;
                case 2:
                    Console.WriteLine("Enter room area in square meters: ");
                    if (double.TryParse(Console.ReadLine(), out double area) && area > 0)
                    {
                        int recommendedVolume = Instruments.CalcRecommendVolume(area);
                        Console.WriteLine($"Recommended volume for room area {area} m**2 is {recommendedVolume}%.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Room area must be a positive number.");
                    }
                    break;
                case 0:
                    return;
            }
        }

        static int StringCount(string name, Family family)
        {
            if (family != Family.Strings) return 0;
            int strings;
            string nameLow = name.ToLower();

            while (true)
            {
                try
                {
                    Console.Write("Enter string count: ");
                    string inp = Console.ReadLine();
                    if (int.TryParse(inp, out strings))
                    {
                        if ((nameLow.Contains("ukulele") || nameLow.Contains("violin")) && strings != 4)
                            throw new ArgumentException($"Error: {name} must have exactly 4 strings.");

                        if (nameLow.Contains("guitar") && strings != 6 && strings != 7 && strings != 12)
                            throw new ArgumentException("Error. Guitar must have 6, 7, or 12 strings.");

                        if (strings < 4)
                            throw new ArgumentException("Error: String instrument must have at least 4 strings.");

                        return strings;
                    }
                }
                catch (ArgumentException ex) { Console.WriteLine(ex.Message); }
            }
        }

        static int KeyCount(Family family, bool isElectric)
        {
            if (family != Family.Keyboard) return 0;
            int keys;

            while (true)
            {
                try
                {
                    Console.Write("Enter key count: ");
                    string inp = Console.ReadLine();
                    if (int.TryParse(inp, out keys))
                    {
                        if (!isElectric && keys != 88)
                            throw new ArgumentException("Error: Acoustic keyboard instruments (like piano) must have 88 keys!");
                        if (isElectric && keys != 61 && keys != 76 && keys != 88)
                            throw new ArgumentException("Error: Electric keyboard instruments must have 61, 76 or 88 keys!");
                        return keys;
                    }
                }
                catch (ArgumentException ex) { Console.WriteLine(ex.Message); }
                Console.WriteLine("Invalid input. Try again.");

            }
        }
    }
}