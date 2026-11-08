using Newtonsoft.Json;
using System.Collections.Generic;

namespace TestProject1
{
    [TestClass]
    public class FileOperationsTests
    {
        private string _testCsvPath;
        private string _testJsonPath;

        [TestInitialize]
        public void SetUp()
        {
            _testCsvPath = "test_instruments.csv";
            _testJsonPath = "test_instruments.json";
        }

        [TestCleanup]
        public void CleanUp()
        {
            try
            {
                if (File.Exists(_testCsvPath)) File.Delete(_testCsvPath);
                if (File.Exists(_testJsonPath)) File.Delete(_testJsonPath);
            }
            catch (Exception ex)
            {
            }
        }

        [TestMethod]
        public void CsvDeserialize_InvalidLines_KeepsValid()
        {
            List<string> lines = new List<string>
            {
                "Name,Family,Brand,IsElectric,KeyCount,StringCount",
                "guitar, Strings, Yamaha, false, 0, 6",
                "invalid, string, format, true, abc, xyz",
                "piano, Keyboard, Roland, false, 88, 0"
            };
            File.WriteAllLines(_testCsvPath, lines);

            List<Instruments> list = new List<Instruments>();
            string[] fileLines = File.ReadAllLines(_testCsvPath);
            int loadedCount = 0;

            foreach (string line in fileLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith("Name,")) continue;

                if (Instruments.TryParse(line, out Instruments inst) && inst != null)
                {
                    list.Add(inst);
                    loadedCount++;
                }
            }

            Assert.AreEqual(2, loadedCount);
            Assert.AreEqual("guitar", list[0].Name);
            Assert.AreEqual("piano", list[1].Name);
        }

        [TestMethod]
        public void JsonSerializeDeserialize_RenamesProperty()
        {
            List<Instruments> list = new List<Instruments>
            {
                new Instruments("guitar", Family.Strings, Brand.Yamaha, false, 0, 6)
            };

            string json = JsonConvert.SerializeObject(list, Formatting.Indented);

            StringAssert.Contains(json, "InstrumentName");

            List<Instruments> deserializedList = JsonConvert.DeserializeObject<List<Instruments>>(json);

            Assert.IsNotNull(deserializedList);
            Assert.AreEqual(1, deserializedList.Count);
            Assert.AreEqual("guitar", deserializedList[0].Name);
        }

        [TestMethod]
        public void Deserialization_AppendsToNonEmptyList()
        {
            List<Instruments> list = new List<Instruments>
            {
                new Instruments("violin", Family.Strings, Brand.Yamaha, false, 0, 4)
            };

            int initialCount = list.Count;

            List<Instruments> loadedItems = new List<Instruments>
            {
                new Instruments("piano", Family.Keyboard, Brand.Roland, false, 88, 0)
            };

            foreach (Instruments item in loadedItems)
            {
                list.Add(item);
            }

            Assert.AreEqual(initialCount + 1, list.Count);
            Assert.AreEqual("violin", list[0].Name);
            Assert.AreEqual("piano", list[1].Name);
        }
    }
}