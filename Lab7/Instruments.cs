public class Instruments
{
    private static int validCount = 0;
    private static int totalCount = 0;
    private static int maxAllowedVol = 100;
    private string name;
    private Brand brand;
    private int keyCount;
    private int stringCount;
    private bool isElectric;
    private bool isConnected;
    private Family family;
    private bool isTuned;
    private int currentVolume;

    public static int ValidCount //static властивість для підрахунку кількості створених інструментів
    {
        get { return validCount; }
    }
    public static int MaxAllowedVolume //static властивість для обмеження максимальної гучності
    {
        get { return maxAllowedVol; }
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException("Max volume must be between 0 and 100.");
            maxAllowedVol = value;
        }
    }

    private void RegisterObj()
    {
        totalCount++;
        validCount++;
        RegCode = $"REG-{totalCount:D2}";
    }

    public Instruments() //default конструктор
    {
        RegisterObj();
        Console.WriteLine($"(Default constructor was called for {RegCode})");
    }

    public string RegCode { get; private set; } = "REG_00"; 

    private Instruments(string name, Family family, Brand brand, bool isInternal) //validation x set
    {
        Name = name;
        Family = family;
        Brand = brand;
    }
    public Instruments(string name, Family family, Brand brand) : this(name, family, brand, true) //delegate validation, registration with 3 parameters
    {
        RegisterObj();
        Console.WriteLine($"(Constructor with 3 parameters was called for {RegCode})");
    }

    public Instruments(string name, Family family, Brand brand, bool isElectric, int keyCount, int stringCount) : this(name, family, brand, true)
    {
        IsElectric = isElectric;
        KeyCount = keyCount;
        StringCount = stringCount;
        RegisterObj();
        Console.WriteLine($"(Constructor with 6 parameters was called for {RegCode})");
    }

    public string Name
    { get { return name; }
        set
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Instrument name cannot be null or empty");
            name = value.Trim();
        } 
    }

    public Brand Brand 
    {
        get { return brand; }
        set { if (!Enum.IsDefined(typeof(Brand), value))
                throw new ArgumentException("Invalid brand specified");
            brand = value; }
    }

    public int KeyCount
    {
        get { return keyCount; }
        set
        {
            if (value < 0)
                throw new ArgumentException("Key count cannot be negative");
            keyCount = value;
        }
    }

    public int StringCount
    {
        get { return stringCount; }
        set
        {
            if (value < 0)
                throw new ArgumentException("String count cannot be negative");
            stringCount = value;
        }
    }

    public bool IsElectric
    {
        get { return isElectric; }
        set
        {
            isElectric = value;
        }
    }

    public bool IsConnected
    {
        get { return isConnected; }
        set
        {
            isConnected = value;
        }
    }

    public Family Family
    {
        get { return family; }
        set
        {
            if (!Enum.IsDefined(typeof(Family), value))
                throw new ArgumentException("Invalid family specified");
            family = value;
        }
    }

  
    public bool IsTuned
    {
        get { return isTuned; }
        private set
        {
            isTuned = value;
        }
    }

    public int CurrentVolume
    {
        get { return currentVolume; }
        set
        {
            if (value < 0)
                throw new ArgumentException($"Volume cannot be negative");
            currentVolume = Math.Min(value, maxAllowedVol);
        }
    }

    public string SoundType
    {
        get
        { 
            if (!IsElectric) return "Acoustic";
            if (CurrentVolume == 0 || CurrentVolume < 50) return "Silent"; 
            return CurrentVolume > 70 ? "Loud" : "Normal";
        }
    }

    private bool CanTune(out string message)
    {
        if (isElectric && !isConnected)
        {
            message = $"Can not tune [{RegCode}]. Instrument {name} is not connected to power.";
            return false;
        }
        string instName = name.ToLower();
        if (family == Family.Strings)
        {
            if ((instName.Contains("ukulele") || instName.Contains("violin")) && stringCount != 4)
            {
                message = $"Instrument {name} [{RegCode}] can not be tuned: string amount is not 4. Your input: {stringCount}";
                return false;
            }
            else if ((instName.Contains("guitar") && stringCount != 6 && stringCount != 7 && stringCount != 12))
            {
                message = $"Instrument {name} [{RegCode}]can not be tuned: string amount should be 6 / 7 / 12. Your input: {stringCount}";
                return false;
            }
            else if (stringCount < 4)
            {
                message = $"Instrument {name} [{RegCode}] can not be tuned: string amount can not be < 4. Your input: {stringCount}";
                return false;
            }
        }
        if (family == Family.Keyboard)
        {
            if (!isElectric && keyCount != 88)
            {
                message = $"Instrument {name} [{RegCode}] can not be tuned: key amount is not 88. Your input: {keyCount}";
                return false;
            }
            else if (keyCount != 61 && keyCount != 76 && keyCount != 88)
            {
                message = $"Instrument {name} [{RegCode}] can not be tuned: key amount should be (61 / 76 / 88). Your input: {keyCount}";
                return false;
            }
        }
        message = $"{name} [{RegCode}] was successfuly tuned.";
        return true;
    }

    //загальнодоступні методи

    public static int CalcRecommendVolume(double roomArea) //static метод для розрахунку рекомендованої гучності інструменту в залежності від площі приміщення
    {
        if (roomArea <= 0) throw new ArgumentException("Room area must be more than 0.");
        int recommended;
        if (roomArea < 20) recommended = 30;
        else if (roomArea < 50) recommended = 50;
        else recommended = 90;
        return Math.Min(recommended, maxAllowedVol);
    }

    public static Instruments Parse (string input) //parse метод для створення об'єкту інструменту з рядка
    {
        if(string.IsNullOrEmpty(input)) throw new ArgumentException("Input string cannot be null or empty.");
        string[] parts = input.Split(',');
        if (parts.Length < 6) throw new ArgumentException("Invalid input format. Missing value. Expected format: Name, Family, Brand, IsElectric, KeyCount, StringCount\n");
        string name = parts[0].Trim();
        string lowName = name.ToLower();

        if (!Enum.TryParse(parts[1].Trim(), true, out Family family))
            throw new FormatException($"Invalid family value: '{parts[1]}'. Allowed values: Strings, Woodwinds, Brass, Percussion, Keyboard.");
        if ((lowName.Contains("guitar") || lowName.Contains("violin") || lowName.Contains("ukulele")) && family != Family.Strings)
            throw new ArgumentException($"Instrument '{name}' must belong to the STRINGS family.");

        if ((lowName.Contains("piano") || lowName.Contains("keyboard")) && family != Family.Keyboard)
            throw new ArgumentException($"Instrument '{name}' must belong to the KEYBOARD family.");

        if ((lowName.Contains("flute") || lowName.Contains("clarinet")) && family != Family.Woodwinds)
            throw new ArgumentException($"Instrument '{name}' must belong to the WOODWINDS family.");

        if (lowName.Contains("trumpet") && family != Family.Brass)
            throw new ArgumentException($"Instrument '{name}' must belong to the BRASS family.");

        if (lowName.Contains("drum") && family != Family.Percussion)
            throw new ArgumentException($"Instrument '{name}' must belong to the PERCUSSION family.");

        if (!Enum.TryParse(parts[2].Trim(), true, out Brand brand))
            throw new FormatException($"Invalid brand value: '{parts[2]}'. Allowed values: Yamaha, Gibson, Pearl, Roland.");

        if (!bool.TryParse(parts[3].Trim(), out bool isElectric))
            throw new FormatException($"Invalid boolean value for IsElectric: '{parts[3]}'. Expected 'true' or 'false'.");

        if(!int.TryParse(parts[4].Trim(), out int keyCount) || keyCount < 0)
        throw new FormatException($"Invalid integer value for KeyCount: '{parts[4]}'");

        if (family == Family.Keyboard)
        {
            if (!isElectric && keyCount != 88)
                throw new ArgumentException($"Acoustic keyboard instruments ({name}) must have exactly 88 keys.");

            if (isElectric && keyCount != 61 && keyCount != 76 && keyCount != 88)
                throw new ArgumentException($"Electric keyboard instruments must have 61, 76, or 88 keys. Entered: {keyCount}");
        }
        else if (keyCount != 0)
        {
            throw new ArgumentException($"Non-keyboard instruments must have 0 keys. Entered: {keyCount}");
        }

        if (!int.TryParse(parts[5].Trim(), out int stringCount) || stringCount < 0)
            throw new FormatException($"Invalid integer value for StringCount: '{parts[5]}'.");

        if (family == Family.Strings)
        {
            if ((lowName.Contains("ukulele") || lowName.Contains("violin")) && stringCount != 4)
                throw new ArgumentException($"Instrument '{name}' must have exactly 4 strings. Entered: {stringCount}");

            if (lowName.Contains("guitar") && stringCount != 6 && stringCount != 7 && stringCount != 12)
                throw new ArgumentException($"Guitar must have 6, 7, or 12 strings. Entered: {stringCount}");

            if (stringCount < 4)
                throw new ArgumentException($"String instruments must have at least 4 strings. Entered: {stringCount}");
        }
        else if (stringCount != 0)
        {
            throw new ArgumentException($"Non-string instruments must have 0 strings. Entered: {stringCount}");
        }
        return new Instruments(name, family, brand, isElectric, keyCount, stringCount);
    }

    public static bool TryParse(string input, out Instruments instrument) //tryparse метод для створення об'єкту 
    {
        instrument = null;
        try
        {
            instrument = Parse(input);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public override string ToString() //ToString метод для виведення інформації про інструмент 
    {
        return $"{name}, {family}, {brand}, {isElectric}, {keyCount}, {stringCount}";
    }

    public string Tune() //настроювання інструменту
    {
        if (!CanTune(out string message))
        {
            IsTuned = false;
            return message;
        }

        IsTuned = true;
        return $"{name} was successfuly tuned.";
    }
    public string Play() 
    {
        if (!IsTuned) return $"Can not play {name}. The instrument [{RegCode}] is not tuned.";
        if (IsElectric) return $"Instrument {name} [{RegCode}] is playing at {CurrentVolume}% volume.";
        else return $"Instrument {name} [{RegCode}] is playing.";

    }
    public string Connect() //підключення інструменту до електроживлення
    {
        if (!isElectric) return $"Instrument {name} [{RegCode}] is not electric and does not require power connection.";

        isConnected = true;
        return $"Instrument {name} [{RegCode}] was successfuly connected to power.";
    }

    //public string SetVolume(int volume) //встановлення гучності інструменту
    //{
    //    if (!isElectric) return $"Can not set volume for this instrument. Instrument {name} [{RegCode}] is acoustic instrument.";
    //    if (volume < 0 || volume > 100) return $"Can not set this volume. Volume should be between 0 and 100.";
    //    currentVolume = volume;
    //    return $"Volume for instrument {name} [{RegCode}] is set to {currentVolume}%.";
    //}

    public void SetVolume(int volume) 
    {
        CurrentVolume = volume;
        Console.WriteLine($"Volume was changed to {CurrentVolume} %");
    }
    public void SetVolume(double sec, bool isMuted) //slightly fade out
    {
        if (!isMuted)
        {
            SetVolume((int)sec); return;
        }
        Console.WriteLine($"Volume of {name} [{RegCode}] is started to fade out for {sec} seconds");
        int steps = CurrentVolume;
        if (steps == 0) return;

        int delayMs = (int)(sec * 1000 / steps);

        while (CurrentVolume > 0)
        {
            CurrentVolume--;
            System.Threading.Thread.Sleep(delayMs);
            if (CurrentVolume % 10 == 0)
            {
                Console.WriteLine($"Volume of {name} [{RegCode}] is now at {CurrentVolume}%");
            }
        }
        Console.WriteLine($"Volume of {name} [{RegCode}] has reached 0%");
    }

    public void SetVolume(int volume, string roomType) //adjust volume based on room type
    {
        double coeff = roomType.ToLower() switch
        {
            "concert hall" => 1.0,
            "studio" => 0.8,
            "home" => 0.5,
            _ => 0.5
        };

        int baseVolume = (volume > 0) ? volume : 100;
        int adjustedVol = (int)(baseVolume * coeff);
        SetVolume(adjustedVol);
        Console.WriteLine($"Volume adjusted for {name} [{RegCode}] in {roomType}: {adjustedVol}%");
    }
}

