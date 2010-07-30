using System.Collections.Generic;
using QLNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestSuite.Calendars
{
	[TestClass]
	public class BrazilCalendarTest
	{
		private readonly Calendar _exchange;
		private readonly Calendar _settlement;

		public BrazilCalendarTest()
		{
			_exchange = new Brazil(Brazil.Market.Exchange);
			_settlement = new Brazil(Brazil.Market.Settlement);
		}

		// 2004 - leap-year in the past
		[TestMethod]
		public void testBrazilBovespaYear2004()
		{
			int year = 2004;

			List<Date> expectedHol = new List<Date>();

			expectedHol.Add(new Date(1, Month.January, year));
			expectedHol.Add(new Date(23, Month.February, year));
			expectedHol.Add(new Date(24, Month.February, year));
			expectedHol.Add(new Date(9, Month.April, year));
			expectedHol.Add(new Date(21, Month.April, year));
			expectedHol.Add(new Date(10, Month.June, year));
			expectedHol.Add(new Date(9, Month.July, year));
			expectedHol.Add(new Date(7, Month.September, year));
			expectedHol.Add(new Date(12, Month.October, year));
			expectedHol.Add(new Date(2, Month.November, year));
			expectedHol.Add(new Date(15, Month.November, year));
			expectedHol.Add(new Date(24, Month.December, year));
			expectedHol.Add(new Date(31, Month.December, year));

			// Call the Holiday Check
			CalendarUtil.CheckHolidayList(expectedHol, _exchange, year);
		}

		// 2004 - leap-year in the past
		[TestMethod]
		public void testBrazilBovespaYear2005()
		{
			int year = 2005;

			List<Date> expectedHol = new List<Date>();

			expectedHol.Add(new Date(25, Month.January, year));
			expectedHol.Add(new Date(7, Month.February, year));
			expectedHol.Add(new Date(8, Month.February, year));
			expectedHol.Add(new Date(25, Month.March, year));
			expectedHol.Add(new Date(21, Month.April, year));
			expectedHol.Add(new Date(26, Month.May, year));
			expectedHol.Add(new Date(7, Month.September, year));
			expectedHol.Add(new Date(12, Month.October, year));
			expectedHol.Add(new Date(2, Month.November, year));
			expectedHol.Add(new Date(15, Month.November, year));
			expectedHol.Add(new Date(30, Month.December, year));

			// Call the Holiday Check
			CalendarUtil.CheckHolidayList(expectedHol, _exchange, year);
		}

		// 2004 - leap-year in the past
		[TestMethod]
		public void testBrazilBovespaYear2006()
		{
			int year = 2006;

			List<Date> expectedHol = new List<Date>();

			expectedHol.Add(new Date(25, Month.January, year));
			expectedHol.Add(new Date(27, Month.February, year));
			expectedHol.Add(new Date(28, Month.February, year));
			expectedHol.Add(new Date(14, Month.April, year));
			expectedHol.Add(new Date(21, Month.April, year));
			expectedHol.Add(new Date(1, Month.May, year));
			expectedHol.Add(new Date(15, Month.June, year));
			expectedHol.Add(new Date(7, Month.September, year));
			expectedHol.Add(new Date(12, Month.October, year));
			expectedHol.Add(new Date(2, Month.November, year));
			expectedHol.Add(new Date(15, Month.November, year));
			expectedHol.Add(new Date(25, Month.December, year));
			expectedHol.Add(new Date(29, Month.December, year));

			// Call the Holiday Check
			CalendarUtil.CheckHolidayList(expectedHol, _exchange, year);
		}

		// 2007 - regular year in the past
		[TestMethod]
		public void testBrazilBovespaYear2007()
		{
			int year = 2007;

			List<Date> expectedHol = new List<Date>();

			expectedHol.Add(new Date(1, Month.January, year));
			expectedHol.Add(new Date(25, Month.January, year));
			expectedHol.Add(new Date(19, Month.February, year));
			expectedHol.Add(new Date(20, Month.February, year));
			expectedHol.Add(new Date(6, Month.April, year));
			expectedHol.Add(new Date(1, Month.May, year));
			expectedHol.Add(new Date(7, Month.June, year));
			expectedHol.Add(new Date(9, Month.July, year));
			expectedHol.Add(new Date(7, Month.September, year));
			expectedHol.Add(new Date(12, Month.October, year));
			expectedHol.Add(new Date(2, Month.November, year));
			expectedHol.Add(new Date(15, Month.November, year));
			expectedHol.Add(new Date(20, Month.November, year));
			expectedHol.Add(new Date(24, Month.December, year));
			expectedHol.Add(new Date(25, Month.December, year));
			expectedHol.Add(new Date(31, Month.December, year));

			// Call the Holiday Check
			CalendarUtil.CheckHolidayList(expectedHol, _exchange, year);
		}

		// 2008 - current year
		[TestMethod]
		public void testBrazilBovespaYear2008()
		{
			int year = 2008;

			List<Date> expectedHol = new List<Date>();

			expectedHol.Add(new Date(1, Month.January, year));
			expectedHol.Add(new Date(25, Month.January, year));
			expectedHol.Add(new Date(4, Month.February, year));
			expectedHol.Add(new Date(5, Month.February, year));
			expectedHol.Add(new Date(21, Month.March, year));
			expectedHol.Add(new Date(21, Month.April, year));
			expectedHol.Add(new Date(1, Month.May, year));
			expectedHol.Add(new Date(22, Month.May, year));
			expectedHol.Add(new Date(9, Month.July, year));
			expectedHol.Add(new Date(20, Month.November, year));
			expectedHol.Add(new Date(24, Month.December, year));
			expectedHol.Add(new Date(25, Month.December, year));
			expectedHol.Add(new Date(31, Month.December, year));

			// Call the Holiday Check
			CalendarUtil.CheckHolidayList(expectedHol, _exchange, year);
		}

		// 2009 - current year in the future
		[TestMethod]
		public void testBrazilBovespaYear2009()
		{
			int year = 2009;

			List<Date> expectedHol = new List<Date>();

			expectedHol.Add(new Date(1, Month.January, year));
			expectedHol.Add(new Date(23, Month.February, year));
			expectedHol.Add(new Date(24, Month.February, year));
			expectedHol.Add(new Date(10, Month.April, year));
			expectedHol.Add(new Date(21, Month.April, year));
			expectedHol.Add(new Date(1, Month.May, year));
			expectedHol.Add(new Date(11, Month.June, year));
			expectedHol.Add(new Date(9, Month.July, year));
			expectedHol.Add(new Date(7, Month.September, year));
			expectedHol.Add(new Date(12, Month.October, year));
			expectedHol.Add(new Date(2, Month.November, year));
			expectedHol.Add(new Date(20, Month.November, year));
			expectedHol.Add(new Date(24, Month.December, year));
			expectedHol.Add(new Date(25, Month.December, year));
			expectedHol.Add(new Date(31, Month.December, year));

			// Call the Holiday Check
			CalendarUtil.CheckHolidayList(expectedHol, _exchange, year);
		}

		// 2004 - leap-year in the past
		[TestMethod]
		public void testBrazilBovespaYear2010()
		{
			int year = 2010;

			List<Date> expectedHol = new List<Date>();

			expectedHol.Add(new Date(1, Month.January, year));
			expectedHol.Add(new Date(25, Month.January, year));
			expectedHol.Add(new Date(15, Month.February, year));
			expectedHol.Add(new Date(16, Month.February, year));
			expectedHol.Add(new Date(2, Month.April, year));
			expectedHol.Add(new Date(21, Month.April, year));
			expectedHol.Add(new Date(3, Month.June, year));
			expectedHol.Add(new Date(9, Month.July, year));
			expectedHol.Add(new Date(7, Month.September, year));
			expectedHol.Add(new Date(12, Month.October, year));
			expectedHol.Add(new Date(2, Month.November, year));
			expectedHol.Add(new Date(15, Month.November, year));
			expectedHol.Add(new Date(24, Month.December, year));
			expectedHol.Add(new Date(31, Month.December, year));

			// Call the Holiday Check
			CalendarUtil.CheckHolidayList(expectedHol, _exchange, year);
		}

		// 2004 - leap-year in the past
		[TestMethod]
		public void testBrazilBovespaYear2011()
		{
			int year = 2011;

			List<Date> expectedHol = new List<Date>();

			expectedHol.Add(new Date(25, Month.January, year));
			expectedHol.Add(new Date(7, Month.March, year));
			expectedHol.Add(new Date(8, Month.March, year));
			expectedHol.Add(new Date(21, Month.April, year));
			expectedHol.Add(new Date(22, Month.April, year));
			expectedHol.Add(new Date(23, Month.June, year));
			expectedHol.Add(new Date(7, Month.September, year));
			expectedHol.Add(new Date(12, Month.October, year));
			expectedHol.Add(new Date(2, Month.November, year));
			expectedHol.Add(new Date(15, Month.November, year));
			expectedHol.Add(new Date(30, Month.December, year));

			// Call the Holiday Check
			CalendarUtil.CheckHolidayList(expectedHol, _exchange, year);
		}

		// 2012 - next leap-year in the future
		[TestMethod]
		public void testBrazilBovespaYear2012()
		{
			int year = 2012;

			List<Date> expectedHol = new List<Date>();

			expectedHol.Add(new Date(25, Month.January, year));
			expectedHol.Add(new Date(20, Month.February, year));
			expectedHol.Add(new Date(21, Month.February, year));
			expectedHol.Add(new Date(6, Month.April, year));
			expectedHol.Add(new Date(1, Month.May, year));
			expectedHol.Add(new Date(7, Month.June, year));
			expectedHol.Add(new Date(9, Month.July, year));
			expectedHol.Add(new Date(7, Month.September, year));
			expectedHol.Add(new Date(12, Month.October, year));
			expectedHol.Add(new Date(2, Month.November, year));
			expectedHol.Add(new Date(15, Month.November, year));
			expectedHol.Add(new Date(20, Month.November, year));
			expectedHol.Add(new Date(24, Month.December, year));
			expectedHol.Add(new Date(25, Month.December, year));
			expectedHol.Add(new Date(31, Month.December, year));

			// Call the Holiday Check
			CalendarUtil.CheckHolidayList(expectedHol, _exchange, year);
		}

		[TestMethod]
		public void testBrazilSettlementYear2004()
		{
			int year = 2004;

			List<Date> expectedHol = new List<Date>();

			expectedHol.Add(new Date(1, Month.January, year));
			expectedHol.Add(new Date(23, Month.February, year));
			expectedHol.Add(new Date(24, Month.February, year));
			expectedHol.Add(new Date(9, Month.April, year));
			expectedHol.Add(new Date(21, Month.April, year));
			expectedHol.Add(new Date(10, Month.June, year));
			expectedHol.Add(new Date(7, Month.September, year));
			expectedHol.Add(new Date(12, Month.October, year));
			expectedHol.Add(new Date(2, Month.November, year));
			expectedHol.Add(new Date(15, Month.November, year));

			// Call the Holiday Check
			CalendarUtil.CheckHolidayList(expectedHol, _settlement, year);
		}

		[TestMethod]
		public void testBrazilSettlementYear2005()
		{
			int year = 2005;

			List<Date> expectedHol = new List<Date>();

			expectedHol.Add(new Date(7, Month.February, year));
			expectedHol.Add(new Date(8, Month.February, year));
			expectedHol.Add(new Date(25, Month.March, year));
			expectedHol.Add(new Date(21, Month.April, year));
			expectedHol.Add(new Date(26, Month.May, year));
			expectedHol.Add(new Date(7, Month.September, year));
			expectedHol.Add(new Date(12, Month.October, year));
			expectedHol.Add(new Date(2, Month.November, year));
			expectedHol.Add(new Date(15, Month.November, year));

			// Call the Holiday Check
			CalendarUtil.CheckHolidayList(expectedHol, _settlement, year);
		}

		[TestMethod]
		public void testBrazilSettlementYear2006()
		{
			int year = 2006;

			List<Date> expectedHol = new List<Date>();

			expectedHol.Add(new Date(27, Month.February, year));
			expectedHol.Add(new Date(28, Month.February, year));
			expectedHol.Add(new Date(14, Month.April, year));
			expectedHol.Add(new Date(21, Month.April, year));
			expectedHol.Add(new Date(1, Month.May, year));
			expectedHol.Add(new Date(15, Month.June, year));
			expectedHol.Add(new Date(7, Month.September, year));
			expectedHol.Add(new Date(12, Month.October, year));
			expectedHol.Add(new Date(2, Month.November, year));
			expectedHol.Add(new Date(15, Month.November, year));
			expectedHol.Add(new Date(25, Month.December, year));

			// Call the Holiday Check
			CalendarUtil.CheckHolidayList(expectedHol, _settlement, year);
		}

		[TestMethod]
		public void testBrazilSettlementYear2007()
		{
			int year = 2007;

			List<Date> expectedHol = new List<Date>();

			expectedHol.Add(new Date(1, Month.January, year));
			expectedHol.Add(new Date(19, Month.February, year));
			expectedHol.Add(new Date(20, Month.February, year));
			expectedHol.Add(new Date(6, Month.April, year));
			expectedHol.Add(new Date(1, Month.May, year));
			expectedHol.Add(new Date(7, Month.June, year));
			expectedHol.Add(new Date(7, Month.September, year));
			expectedHol.Add(new Date(12, Month.October, year));
			expectedHol.Add(new Date(2, Month.November, year));
			expectedHol.Add(new Date(15, Month.November, year));
			expectedHol.Add(new Date(25, Month.December, year));

			// Call the Holiday Check
			CalendarUtil.CheckHolidayList(expectedHol, _settlement, year);
		}

		[TestMethod]
		public void testBrazilSettlementYear2008()
		{
			int year = 2008;

			List<Date> expectedHol = new List<Date>();

			expectedHol.Add(new Date(1, Month.January, year));
			expectedHol.Add(new Date(4, Month.February, year));
			expectedHol.Add(new Date(5, Month.February, year));
			expectedHol.Add(new Date(21, Month.March, year));
			expectedHol.Add(new Date(21, Month.April, year));
			expectedHol.Add(new Date(1, Month.May, year));
			expectedHol.Add(new Date(22, Month.May, year));
			expectedHol.Add(new Date(25, Month.December, year));

			// Call the Holiday Check
			CalendarUtil.CheckHolidayList(expectedHol, _settlement, year);
		}

		[TestMethod]
		public void testBrazilSettlementYear2009()
		{
			int year = 2009;

			List<Date> expectedHol = new List<Date>();

			expectedHol.Add(new Date(1, Month.January, year));
			expectedHol.Add(new Date(23, Month.February, year));
			expectedHol.Add(new Date(24, Month.February, year));
			expectedHol.Add(new Date(10, Month.April, year));
			expectedHol.Add(new Date(21, Month.April, year));
			expectedHol.Add(new Date(1, Month.May, year));
			expectedHol.Add(new Date(11, Month.June, year));
			expectedHol.Add(new Date(7, Month.September, year));
			expectedHol.Add(new Date(12, Month.October, year));
			expectedHol.Add(new Date(2, Month.November, year));
			expectedHol.Add(new Date(25, Month.December, year));

			// Call the Holiday Check
			CalendarUtil.CheckHolidayList(expectedHol, _settlement, year);
		}

		[TestMethod]
		public void testBrazilSettlementYear2010()
		{
			int year = 2010;

			List<Date> expectedHol = new List<Date>();

			expectedHol.Add(new Date(1, Month.January, year));
			expectedHol.Add(new Date(15, Month.February, year));
			expectedHol.Add(new Date(16, Month.February, year));
			expectedHol.Add(new Date(2, Month.April, year));
			expectedHol.Add(new Date(21, Month.April, year));
			expectedHol.Add(new Date(3, Month.June, year));
			expectedHol.Add(new Date(7, Month.September, year));
			expectedHol.Add(new Date(12, Month.October, year));
			expectedHol.Add(new Date(2, Month.November, year));
			expectedHol.Add(new Date(15, Month.November, year));

			// Call the Holiday Check
			CalendarUtil.CheckHolidayList(expectedHol, _settlement, year);
		}

		[TestMethod]
		public void testBrazilSettlementYear2011()
		{
			int year = 2011;

			List<Date> expectedHol = new List<Date>();

			expectedHol.Add(new Date(7, Month.March, year));
			expectedHol.Add(new Date(8, Month.March, year));
			expectedHol.Add(new Date(21, Month.April, year));
			expectedHol.Add(new Date(22, Month.April, year));
			expectedHol.Add(new Date(23, Month.June, year));
			expectedHol.Add(new Date(7, Month.September, year));
			expectedHol.Add(new Date(12, Month.October, year));
			expectedHol.Add(new Date(2, Month.November, year));
			expectedHol.Add(new Date(15, Month.November, year));

			// Call the Holiday Check
			CalendarUtil.CheckHolidayList(expectedHol, _settlement, year);
		}

		[TestMethod]
		public void testBrazilSettlementYear2012()
		{
			int year = 2012;

			List<Date> expectedHol = new List<Date>();

			expectedHol.Add(new Date(20, Month.February, year));
			expectedHol.Add(new Date(21, Month.February, year));
			expectedHol.Add(new Date(6, Month.April, year));
			expectedHol.Add(new Date(1, Month.May, year));
			expectedHol.Add(new Date(7, Month.June, year));
			expectedHol.Add(new Date(7, Month.September, year));
			expectedHol.Add(new Date(12, Month.October, year));
			expectedHol.Add(new Date(2, Month.November, year));
			expectedHol.Add(new Date(15, Month.November, year));
			expectedHol.Add(new Date(25, Month.December, year));
			
			// Call the Holiday Check
			CalendarUtil.CheckHolidayList(expectedHol, _settlement, year);
		}
	}
}
