using Microsoft.VisualStudio.TestTools.UnitTesting;
using QLNet;

namespace TestSuite
{
	[TestClass]
	public class CurrencyTest
	{
		[TestMethod]
		public void testCurrencies()
		{
			CHFCurrency chf = new CHFCurrency();
			EURCurrency euro = new EURCurrency();
			CHFCurrency chf2 = new CHFCurrency();

			Assert.IsTrue(chf.Name == "Swiss franc");
			Assert.IsTrue(chf.code == "CHF");
			Assert.AreEqual(chf.numericCode, 756);
			Assert.AreEqual(chf.symbol, "SwF");
			Assert.AreEqual(chf.fractionSymbol, string.Empty);
			Assert.AreEqual(chf.fractionsPerUnit, 100);
			Assert.AreEqual(chf.rounding.getType, Rounding.Type.None);
			Assert.IsInstanceOfType(chf.triangulationCurrency, typeof(Currency));
			Assert.IsTrue(chf.triangulationCurrency.IsEmpty);
			Assert.IsFalse(euro == chf);
			Assert.IsTrue(euro != chf);
			Assert.IsFalse(chf2 != chf);
			Assert.IsTrue(chf2 == chf);
		}
	}
}
