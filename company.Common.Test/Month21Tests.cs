using Microsoft.VisualStudio.TestTools.UnitTesting;
using company.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace company.Common.Tests
{
	[TestClass()]
	public class Month21Tests
	{
		[TestMethod, TestCategory("Month21 tests")]
		public void Month21Test()
		{
			var month21 = new Month21(1808);
			Assert.AreEqual(1808, month21.Value);

			month21 = new Month21(new DateTime(2018,8,20));
			Assert.AreEqual(1808, month21.Value);

			month21 = new Month21(1808);
			Assert.AreEqual(new DateTime(2018,8,1),  month21.Date);

			month21 = new Month21(DateTime.Today);
			Assert.AreEqual(month21.Value, Month21.Now.Value);
		}
		

		[TestMethod, TestCategory("Month21 tests")]
		public void FromDateTest()
		{
			var month21 = Month21.FromDate(new DateTime(2018, 8, 20));
			Assert.AreEqual(1808,month21.Value);
		}

		[TestMethod, TestCategory("Month21 tests")]
		public void FromYearMonthTest()
		{
			var month21 = Month21.FromYearMonth(2018,8);
			Assert.AreEqual(1808,month21.Value);
		}

		[TestMethod, TestCategory("Month21 tests")]
		public void GetDateTest()
		{
			Assert.AreEqual(new DateTime(2018,8,1),Month21.GetDate(1808));
		}

		[TestMethod, TestCategory("Month21 tests")]
		public void EqualsTest()
		{
			var m1 = new Month21(1808);
			var m2 = new Month21(new DateTime(2018,8,20));
			Assert.IsTrue(m1.Equals(m2));
			Assert.IsTrue(m1 == m2);
			Assert.IsFalse(m1.Equals(300));
		}

		[TestMethod, TestCategory("Month21 tests")]
		public void GetHashCodeTest()
		{
			var m1 = new Month21(1808);
			Assert.AreEqual(m1.Value, m1.GetHashCode());
		}

		[TestMethod, TestCategory("Month21 tests")]
		public void OperatorsTest()
		{
			var m1 = new Month21(1808);
			var m2 = new Month21(1809);

			Assert.IsTrue(m1 == 1808);
			Assert.IsTrue(1808 == m1);

			Assert.IsTrue(m1 != 1810);
			Assert.IsTrue(1810 != m1);
			Assert.IsTrue(m1 != m2);

			Assert.AreEqual(m2, m1 + 1);
			Assert.AreEqual(1807, (m1 -1).Value);

			Assert.IsTrue(m2 >= m1);
			Assert.IsTrue(m2 > m1);
			Assert.IsFalse(m2 <= m1);
			Assert.IsFalse(m2 < m1);

			Assert.AreEqual(1, m2 - m1);
			Assert.AreEqual(m1, m2 - 1);
		}
	}
}