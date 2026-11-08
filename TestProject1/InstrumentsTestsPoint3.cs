using System;
using System.Collections.Generic;
using System.Text;

namespace TestProject1
{
    [TestClass]
    public class InstrumentsTestsPoint3
    {
        private List<Instruments> _instrumentsList;
        private Instruments _defaultAcoustic;
        private Instruments _defaultElectric;


        [TestInitialize]
        public void SetUp()
        {
            _instrumentsList = new List<Instruments>();
            _defaultAcoustic = new Instruments("guitar", Family.Strings, Brand.Yamaha, false, 0, 6);
            _defaultElectric = new Instruments("piano", Family.Keyboard, Brand.Gibson, true, 61, 0);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _instrumentsList = null;
            _defaultAcoustic = null;
            _defaultElectric = null;
        }

        [TestMethod]
        public void List_AddObj_IncreaseListCount()
        {
            _instrumentsList.Add(_defaultAcoustic);
            Assert.AreEqual(1, _instrumentsList.Count);
        }

        [TestMethod]
        public void List_RemoveAt_Remove_Correctly()
        {
            _instrumentsList.Add(_defaultAcoustic);
            _instrumentsList.Add(_defaultElectric);
            _instrumentsList.RemoveAt(0);
            Assert.AreEqual(1, _instrumentsList.Count);
            Assert.AreEqual(_defaultElectric, _instrumentsList[0]);
        }

        [TestMethod]
        public void List_RemoveAt_Remove_NegativeIndex()
        {
            _instrumentsList.Add(_defaultAcoustic);
            _instrumentsList.Add(_defaultElectric);
            Assert.Throws<ArgumentOutOfRangeException>(() => _instrumentsList.RemoveAt(-1));
        }
        [TestMethod]
        public void List_RemoveAt_Remove_IndexEqualCount()
        {
            _instrumentsList.Add(_defaultAcoustic);
            _instrumentsList.Add(_defaultElectric);
            Assert.Throws<ArgumentOutOfRangeException>(() => _instrumentsList.RemoveAt(2));
        }
        [TestMethod]
        public void List_RemoveAt_Remove_IndexGreaterThanCount()
        {
            _instrumentsList.Add(_defaultAcoustic);
            _instrumentsList.Add(_defaultElectric);
            Assert.Throws<ArgumentOutOfRangeException>(() => _instrumentsList.RemoveAt(3));
        }

        [TestMethod]
        public void List_Search_EmptyList()
        {
            List<Instruments> searchResult = _instrumentsList.FindAll(x => x.Name == "guitar");
            Assert.IsNotNull(searchResult);
            Assert.AreEqual(0, searchResult.Count);
        }

        [TestMethod]
        public void List_Search_NonExistObj()
        {
            _instrumentsList.Add(_defaultAcoustic);
            _instrumentsList.Add(_defaultElectric);
            List<Instruments> searchResult = _instrumentsList.FindAll(x => x.Name == "violin");
            Assert.AreEqual(0, searchResult.Count);
        }

        [TestMethod]
        public void List_SearchByName_Correct()
        {
            _instrumentsList.Add(_defaultAcoustic);            
            _instrumentsList.Add(_defaultAcoustic);            
            _instrumentsList.Add(_defaultElectric);
            _instrumentsList.Add(new Instruments("violin", Family.Strings, Brand.Yamaha, false, 0, 4));

            List<Instruments> searchResult = _instrumentsList.FindAll(x => x.Name == "guitar");
            Assert.AreEqual(2, searchResult.Count);
        }
        [TestMethod]
        public void List_SearchByFamily_Correct()
        {
            _instrumentsList.Add(_defaultAcoustic);            
            _instrumentsList.Add(_defaultElectric);
            _instrumentsList.Add(new Instruments("violin", Family.Strings, Brand.Yamaha, false, 0, 4));

            List<Instruments> searchResult = _instrumentsList.FindAll(x => x.Family == Family.Strings);
            Assert.AreEqual(2, searchResult.Count);
        }
        [TestMethod]
        public void List_SearchByBrand_Correct()
        {
            _instrumentsList.Add(_defaultAcoustic);            
            _instrumentsList.Add(_defaultAcoustic);            
            _instrumentsList.Add(_defaultElectric);
            _instrumentsList.Add(new Instruments("violin", Family.Strings, Brand.Yamaha, false, 0, 4));

            List<Instruments> searchResult = _instrumentsList.FindAll(x => x.Brand == Brand.Yamaha);
            Assert.AreEqual(3, searchResult.Count);
        }
        [TestMethod]
        public void List_SearchByIsElectric_Correct()
        {
            _instrumentsList.Add(_defaultAcoustic);            
            _instrumentsList.Add(_defaultAcoustic);            
            _instrumentsList.Add(_defaultElectric);
            _instrumentsList.Add(new Instruments("violin", Family.Strings, Brand.Yamaha, false, 0, 4));

            List<Instruments> searchResult = _instrumentsList.FindAll(x => x.IsElectric == true);
            Assert.AreEqual(1, searchResult.Count);
        }
    }
}
