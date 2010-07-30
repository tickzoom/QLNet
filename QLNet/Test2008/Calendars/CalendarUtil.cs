using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using QLNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestSuite.Calendars
{
	/// <summary>
	/// This is the general test base class for Calendars including generic methods.
	/// </summary>
	internal static class CalendarUtil
	{
		[DebuggerHidden]
		public static void CheckHolidayList(IEnumerable<Date> expected, Calendar calendar, int year)
		{
			IEnumerable<Date> calculated = Calendar.holidayList(calendar, new Date(1, Month.January, year), new Date(31, Month.December, year), false);

			int error = 0;

			StringBuilder sb = new StringBuilder();
			sb.Append("Holidays do not match\n");

			foreach (Date date in expected)
			{
				if (!calculated.Contains(date))
				{
					sb.Append("  >> Holiday expected but not calculated: ")
						.Append(date.DayOfWeek)
						.Append(", ")
						.Append(date)
						.Append('\n');

					error++;
				}
			}

			foreach (Date date in calculated)
			{
				if (!expected.Contains(date))
				{
					sb.Append("  >> Holiday calculated but not expected: ").Append(date.DayOfWeek).Append(", ").Append(date).Append('\n');
					error++;
				}
			}

			Assert.IsFalse(error > 0, sb.ToString());
		}
	}
}