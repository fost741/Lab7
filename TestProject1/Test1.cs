using System.Diagnostics.Metrics;
using System.Xml.Linq;

namespace TestProject1
{
    [TestClass]
    public sealed class InstrumentsTests
    {
        private Instruments _defaultInstrument;

        [TestInitialize]
        public void SetUp()
        {
            _defaultInstrument = new Instruments("guitar", Family.Strings, Brand.Yamaha, true, 0, 6);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _defaultInstrument = null;
        }

        [TestMethod]
        [DataRow("guitar, strings, yamaha, false, 0, 6", "guitar", Family.Strings, Brand.Yamaha, false, 0, 6)]
        [DataRow("piano, keyboard, roland, false, 88, 0", "piano", Family.Keyboard, Brand.Roland, false, 88, 0)]

        public void ParseTest_input_valid(string input, string expectedName, Family expectedFamily, Brand expectedBrand, bool expectedElectric, int expectedKeys, int expectedStrings)
        {
            Instruments inst = Instruments.Parse(input);
            Assert.IsNotNull(inst);
            Assert.AreEqual(expectedName, inst.Name);
            Assert.AreEqual(expectedFamily, inst.Family);
            Assert.AreEqual(expectedBrand, inst.Brand);
            Assert.AreEqual(expectedElectric, inst.IsElectric);
            Assert.AreEqual(expectedKeys, inst.KeyCount);
            Assert.AreEqual(expectedStrings, inst.StringCount);
        }

        [TestMethod]
        [DataRow("invalid, string, format")]
        [DataRow("piano, keyboard, roland, false, 88, 6")]
        [DataRow("piano, brass, roland, false, 88, 0")]

        public void ParseTest_input_invalid(string input)
        {
            Assert.Throws<Exception>(() => Instruments.Parse(input));
        }


        [TestMethod]
        [DataRow("invalid, string, format", false)]
        [DataRow("piano, keyboard, roland, false, 88, 0", true)]
        public void TryParseTest_input_valid_Invalid(string input, bool expectedResult)
        {
            bool result = Instruments.TryParse(input, out Instruments inst);
            Assert.AreEqual(expectedResult, result);
            if (result)
            {
                Assert.IsNotNull(inst);
            }
            else
            {
                Assert.IsNull(inst);
            }
        }

        [TestMethod]
        [DataRow(15.5, 30)]
        [DataRow(45.0, 50)]
        [DataRow(60.0, 90)]

        public void CalcRecommendVolumeTest_input_valid(double roomArea, int expectedVolume)
        {
            int volume = Instruments.CalcRecommendVolume(roomArea);
            Assert.AreEqual(expectedVolume, volume);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-8)]

        public void CalcRecommendVolumeTest_input_lessOrEqualZero(double roomArea)
        {
            Assert.Throws<ArgumentException>(() => Instruments.CalcRecommendVolume(roomArea));
        }

        [TestMethod]
        public void TuneTest_instrument_valid()
        {
            Instruments inst = new Instruments("guitar", Family.Strings, Brand.Yamaha, false, 0, 6);
            inst.Tune();

            Assert.IsTrue(inst.IsTuned);
        }

        [TestMethod]
        public void CanTuneTest_instrument_invalidStringCount()
        {
            Instruments inst = new Instruments("guitar", Family.Strings, Brand.Yamaha, false, 0, 5);
            string result = inst.Tune();
            Assert.IsFalse(inst.IsTuned);
            StringAssert.Contains(result, "string amount should be 6 / 7 / 12");
        }

        [TestMethod]
        [DataRow("guitar, strings, yamaha, true, 0, 6")]
        public void CanTuneTest_instrument_isNotConnected(string instData)
        {
            Instruments inst = Instruments.Parse(instData);

            inst.Tune();
            Assert.IsFalse(inst.IsTuned);
        }

        [TestMethod]
        [DataRow("guitar, strings, yamaha, true, 0, 6")]
        public void ConnectTest_ValidElectricInstrument(string instData)
        {
            Instruments inst = Instruments.Parse(instData);
            inst.Connect();

            Assert.IsTrue(inst.IsConnected);
        }

        [TestMethod]
        [DataRow("guitar, strings, yamaha, false, 0, 6")]
        public void ConnectTest_AccousticInstrument_CannotConnect(string instData)
        {
            Instruments inst = Instruments.Parse(instData);
            inst.Connect();

            Assert.IsFalse(inst.IsConnected);
        }

        [TestMethod]
        [DataRow("guitar, strings, yamaha, true, 0, 6")]
        [DataRow("guitar, strings, yamaha, false, 0, 6")]
        public void PlayTest_validAll(string instData)
        {
            Instruments inst = Instruments.Parse(instData);
            inst.Connect();
            inst.Tune();
            string result = inst.Play();
            StringAssert.Contains(result, "is playing");
        }

        [TestMethod]
        [DataRow("guitar, strings, yamaha, true, 0, 6")]
        public void PlayTest_Inst_Not_Tuned(string instData)
        {
            Instruments inst = Instruments.Parse(instData);
            inst.Connect();
            string result = inst.Play();
            StringAssert.Contains(result, "is not tuned");
        }

        [TestMethod]
        [DataRow(50, 50)]
        [DataRow(120, 100)]
        public void SetVolumeTest_volume_valid(int volume, int expectedVolume)
        {
            _defaultInstrument.SetVolume(volume);
            Assert.AreEqual(expectedVolume, _defaultInstrument.CurrentVolume);
        }

        [TestMethod]
        public void SetVolumeTest_volume_less_0()
        {
            Instruments inst = new Instruments("guitar", Family.Strings, Brand.Yamaha, true, 0, 6);
            Assert.Throws<ArgumentException>(() => inst.SetVolume(-5));
        }

        [TestMethod]
        [DataRow(2, true, 0)]
        [DataRow(25, false, 25)]
        public void SetVolumeFadeTest_valid_Reaches0(int seconds, bool isMuted, int expectedVolume)
        {
            _defaultInstrument.SetVolume(50);
            _defaultInstrument.SetVolume(seconds, isMuted);
            Assert.AreEqual(expectedVolume, _defaultInstrument.CurrentVolume);
        }

        [TestMethod]
        [DataRow(80, "concert hall", 80)]
        [DataRow(80, "studio", 64)]
        [DataRow(80, "home", 40)]
        public void SetVolumeRoomTypeTest_roomType_valid(int volume, string roomType, int expectedVolume)
        {
            _defaultInstrument.SetVolume(50);
            _defaultInstrument.SetVolume(volume, roomType);
            Assert.AreEqual(expectedVolume, _defaultInstrument.CurrentVolume);
        }

        [TestMethod]
        [DataRow(80, "flat", 40)]
        public void SetVolumeRoomTypeTest_roomType_default(int volume, string roomType, int expectedVolume)
        {
            _defaultInstrument.SetVolume(50);
            _defaultInstrument.SetVolume(volume, roomType);
            Assert.AreEqual(expectedVolume, _defaultInstrument.CurrentVolume);
        }

        [TestMethod]
        [DataRow(0, "concert hall", 100)]
        [DataRow(-5, "studio", 80)]
        [DataRow(0, "home", 50)]
        public void SetVolumeRoomTypeTest_volume_less_or_0(int volume, string roomType, int expectedVolume)
        {
            _defaultInstrument.SetVolume(50);
            _defaultInstrument.SetVolume(volume, roomType);
            Assert.AreEqual(expectedVolume, _defaultInstrument.CurrentVolume);
        }

        [TestMethod]
        [DataRow("guitar", "guitar")]
        [DataRow("piano  ", "piano")]
        [DataRow("  violin", "violin")]
        public void NameTest_Valid(string name, string expectedName)
        {
            Instruments inst = new Instruments(name, Family.Strings, Brand.Yamaha, true, 0, 0);
            Assert.AreEqual(expectedName, inst.Name);
        }
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void NameTest_null_or_empty(string name)
        {
            Assert.Throws<Exception> (() => new Instruments(name, Family.Strings, Brand.Yamaha, true, 0, 0));
        }
        [TestMethod]
        [DataRow(Brand.Yamaha)]
        [DataRow(Brand.Roland)]
        public void BrandTest_valid(Brand expectedBrand)
        {
            Instruments inst = new Instruments("piano", Family.Strings, expectedBrand, true, 88, 0);
            Instruments inst2 = new Instruments("piano", Family.Strings, Brand.Gibson, true, 88, 0);
            inst2.Brand = expectedBrand;
            Assert.AreEqual(expectedBrand, inst.Brand);
            Assert.AreEqual(expectedBrand, inst2.Brand);
        }
        [TestMethod]
        public void BrandTest_doNotExist()
        {
            Assert.Throws<Exception>(() => new Instruments("piano", Family.Strings, (Brand)88, true, 88, 0));
        }
        [TestMethod]
        [DataRow(6, 6)]
        [DataRow(0, 0)]
        [DataRow(12, 12)]
        public void StringCountTest_Valid(int stringCount, int expectedStringCount)
        {
            Instruments inst = new Instruments("piano", Family.Strings, Brand.Yamaha, true, 0, stringCount);
            Assert.AreEqual(expectedStringCount, inst.StringCount);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(-5)]
        public void StringCountTest_less_than_0(int stringCount)
        {
            Assert.Throws<ArgumentException>(() => new Instruments("piano", Family.Strings, Brand.Yamaha, true, 0, stringCount));   
        }

    }
}

