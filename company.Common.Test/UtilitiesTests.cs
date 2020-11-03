using company.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace company.Common.Test
{
	[TestClass]
	public class UtilitiesTests
	{
		[TestMethod, TestCategory("Common utilities")]
		public void OrdinalToLetters()
		{
			var number = 3;

			Assert.AreEqual("C", Utilities.OrdinalToLetters(number));

			number = 29;
			Assert.AreEqual("AC", Utilities.OrdinalToLetters(number));

			number = 0;
			Assert.AreEqual(string.Empty, Utilities.OrdinalToLetters(number));
		}

		[TestMethod, TestCategory("Common utilities")]
		public void CapitalizeFirstLetter()
		{
			string text = null;
			Assert.IsNull(Utilities.CapitalizeFirstLetter(text));

			text = string.Empty;
			Assert.IsTrue(string.IsNullOrEmpty(Utilities.CapitalizeFirstLetter(text)));

			text = "abc def";
			Assert.AreEqual("Abc def", Utilities.CapitalizeFirstLetter(text));
		}

		[TestMethod, TestCategory("Common utilities")]
		public void LettersToOrdinal()
		{
			string text = null;
			Assert.ThrowsException<ArgumentNullException>(() => Utilities.LettersToOrdinal(text));
			text = string.Empty;
			Assert.ThrowsException<ArgumentNullException>(() => Utilities.LettersToOrdinal(text));
			text = "C";
			Assert.AreEqual(3, Utilities.LettersToOrdinal(text));
			text = "AC";
			Assert.AreEqual(29, Utilities.LettersToOrdinal(text));
		}

		[TestMethod, TestCategory("Common utilities")]
		public void GetMonth21FromDate()
		{
			var date = new DateTime(2018,6,12);
			Assert.AreEqual(1806, Utilities.GetMonth21FromDate(date));
		}

		[TestMethod, TestCategory("Common utilities")]
		public void FirstDateOfWeek()
		{
			var year = 2018;
			var week = 3;
			var rule = System.Globalization.CalendarWeekRule.FirstDay;
			var date = Utilities.FirstDateOfWeek(year, week, rule);
			Assert.IsTrue(date.Month == 1 && date.Day == 15);

			year = 2017;
			date = Utilities.FirstDateOfWeek(year, week, rule);
			Assert.IsTrue(date.Month == 1 && date.Day == 9);
			
			rule = CalendarWeekRule.FirstFourDayWeek;
			date = Utilities.FirstDateOfWeek(year, week, rule);
			Assert.IsTrue(date.Month == 1 && date.Day == 16);
		}

		[TestMethod, TestCategory("Common utilities")]
		public void GetFirstDayInWeek()
		{
			var date = Utilities.GetFirstDayInWeek(new DateTime(2018,7,15));
			Assert.IsTrue(date.Month == 7 && date.Day == 9);
		}

		[TestMethod, TestCategory("Common utilities")]
		public void WeekNumberFromDate()
		{
			var rule = CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
			var info = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
			info.DateTimeFormat.CalendarWeekRule = CalendarWeekRule.FirstDay;
			CultureInfo.CurrentCulture = info;
			var date = new DateTime(2017,1,16);
			var week = Utilities.WeekNumberFromDate(date);
			Assert.AreEqual(4, week);
			
			CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule = CalendarWeekRule.FirstFourDayWeek;
			week = Utilities.WeekNumberFromDate(date);
			Assert.AreEqual(3, week);

			CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule = rule;
		}

		[TestMethod, TestCategory("Common utilities")]
		public void AppendSuffixToFileName()
		{
			var filename = "file.xls";
			var suffix = "_1";
			Assert.AreEqual("file_1.xls", Utilities.AppendSuffixToFileName(filename, suffix));
		}

		[TestMethod, TestCategory("Common utilities")]
		public void ArrayToString()
		{
			var arr = new[] {"1", "3", " ", "5"};
			Assert.AreEqual("1,3,5", Utilities.ArrayToString(arr,","));

			Assert.AreEqual("1,3, ,5", Utilities.ArrayToString(arr,",",false));
		}

		[TestMethod, TestCategory("Common utilities")]
		public void ShortenText()
		{
			var text = "";

			Assert.AreEqual("", Utilities.ShortenText(text, 0));

			text = "abcd";
			Assert.AreEqual("a...", Utilities.ShortenText(text, 1));

			Assert.AreEqual("...", Utilities.ShortenText(text, 0));
		}

		[TestMethod, TestCategory("Common utilities")]
		public void GetIdsFromStringTest()
		{
			var ids = "";
			Assert.IsNull(Utilities.GetIdsFromString(ids));

			ids = "3,2";
			var list = Utilities.GetIdsFromString(ids);
			Assert.AreEqual(2, list.Count);
			Assert.IsInstanceOfType(list[0], typeof(int));

			ids = "4,a,3";
			list = Utilities.GetIdsFromString(ids);
			Assert.AreEqual(2, list.Count);
			Assert.IsInstanceOfType(list[0], typeof(int));
		}

		[TestMethod, TestCategory("Common utilities")]
		public void GetDoublesFromStringTest()
		{
			var ids = "";
			Assert.IsNull(Utilities.GetDoublesFromString(ids));

			ids = "3,2";
			var list = Utilities.GetDoublesFromString(ids);
			Assert.AreEqual(2, list.Count);
			Assert.IsInstanceOfType(list[0], typeof(double));

			ids = "4,a,3";
			list = Utilities.GetDoublesFromString(ids);
			Assert.AreEqual(2, list.Count);
			Assert.IsInstanceOfType(list[0], typeof(double));
		}

		[TestMethod, TestCategory("Common utilities")]
		public void GetQuotedStringsFromStringTest()
		{
			var ids = "";
			Assert.IsNull(Utilities.GetQuotedStringsFromString(ids));

			ids = "3,2";
			var list = Utilities.GetQuotedStringsFromString(ids);
			Assert.AreEqual(2, list.Count);
			Assert.IsInstanceOfType(list[0], typeof(string));
			Assert.AreEqual("'3'", list[0]);
		}

		[TestMethod, TestCategory("Common utilities")]
		public void GetNullableIntsFromStringTest()
		{
			var ids = "";
			Assert.IsNull(Utilities.GetNullableIntsFromString(ids));

			ids = "3,2";
			var list = Utilities.GetNullableIntsFromString(ids);
			Assert.AreEqual(2, list.Count);
			Assert.IsInstanceOfType(list[0], typeof(int?));
		}

		[TestMethod, TestCategory("Common utilities")]
		public void IpToLongTest()
		{
			var ip = "a.1.1.1";
			Assert.IsNull(Utilities.IpToLong(ip));

			ip = "100.100.100.100";
			var result = Utilities.IpToLong(ip);
			Assert.IsTrue(result > 0);

			Assert.AreEqual(100, result & 0xFF);
			Assert.AreEqual(100, result & 0xFF00 >> 8);
			Assert.AreEqual(100, result & 0xFF0000 >> 16);
			Assert.AreEqual(100, result & 0xFF000000 >> 24);
		}

		[TestMethod, TestCategory("Common utilities")]
		public void GetMonthFromNowTest()
		{
			var date = DateTime.Today;
			date = date.AddMonths(-2);
			var month = date.Month;
			var year = date.Year;
			Assert.AreEqual((year-2000) * 100 + month, Utilities.GetMonthFromNow(-2));
		}

		[TestMethod, TestCategory("Common utilities")]
		public void GetMonthFromDateTest()
		{
			var date = new DateTime(2018,8,20);
			Assert.AreEqual(1808, Utilities.GetMonthFromDate(date));

			Assert.AreEqual(1901, Utilities.GetMonthFromDate(date,5));
		}
		

		[TestMethod, TestCategory("Common utilities")]
		public void GetMonthStartTest()
		{
			var date = new DateTime(2018,8,20);
			Assert.AreEqual(new DateTime(2018,8,1), Utilities.GetMonthStart(date) );
		}

		[TestMethod, TestCategory("Common utilities")]
		public void GetMonthEndTest()
		{
			var date = new DateTime(2018,8,20);
			Assert.AreEqual(new DateTime(2018,9,1).AddMilliseconds(-1), Utilities.GetMonthEnd(date));

			DateTime? date2 = null;
			Assert.IsNull(Utilities.GetMonthEnd(date2));
		}

		[TestMethod, TestCategory("Common utilities")]
		public void GetDayEndTest()
		{
			var date = new DateTime(2018,8,20);
			Assert.AreEqual(date.AddDays(1).AddMilliseconds(-1), Utilities.GetDayEnd(date));
		}

		[TestMethod, TestCategory("Common utilities")]
		public void GetYearStartTest()
		{
			var date = new DateTime(2018,8,20);
			Assert.AreEqual(new DateTime(date.Year,1,1), Utilities.GetYearStart(date) );
		}

		[TestMethod, TestCategory("Common utilities")]
		public void GetYearEndTest()
		{
			var date = new DateTime(2018,8,20);
			Assert.AreEqual(new DateTime(date.Year+1,1,1).AddMilliseconds(-1), Utilities.GetYearEnd(date) );
		}

		[TestMethod, TestCategory("Common utilities")]
		public void GetDateFromMonth21Test()
		{
			var month21 = 1808;
			Assert.AreEqual(new DateTime(2018,8,1), Utilities.GetDateFromMonth21(month21));
		}

		public const string TestsDir = "filetests";

		[TestMethod, TestCategory("Common utilities")]
		public void GetFilePathTest()
		{
			var fileName = "test.txt";
			var filePath = Path.Combine(TestsDir, fileName);
			CreateTestDirectory();
			Assert.AreEqual(filePath, Utilities.GetFilePath(fileName, TestsDir));
			CreateTestFile(fileName, "test");
			Assert.AreNotEqual(filePath, Utilities.GetFilePath(fileName, TestsDir));
			Assert.AreEqual(Path.Combine(TestsDir,"test_1.txt"), Utilities.GetFilePath(fileName, TestsDir));
			DeleteTestDirectory();
			
		}

		private void CreateTestFile(string name, string contents)
		{
			using (var f = new StreamWriter(Path.Combine(TestsDir, name)))
			{
				f.WriteLine(contents);
				f.Close();
			}
		}

		private void DeleteTestDirectory()
		{
			var exists = Directory.Exists(TestsDir);
			if (exists)
			{
				var files = Directory.GetFiles(TestsDir);
				foreach(var f in files)
					File.Delete(f);
				Directory.Delete(TestsDir);
			}
		}

		private void CreateTestDirectory()
		{
			if(Directory.Exists(TestsDir))
				DeleteTestDirectory();
			Directory.CreateDirectory(TestsDir);
		}

		[TestMethod, TestCategory("Common utilities")]
		public void WriteFileTest()
		{
			CreateTestDirectory();

			var contents = new byte[] {0, 1, 2};
			Utilities.WriteFile("test.txt", TestsDir, contents);
			Assert.IsTrue(Directory.GetFiles(TestsDir).Length > 0);

			DeleteTestDirectory();
		}

		[TestMethod, TestCategory("Common utilities")]
		public void SafeGetElementTest()
		{
			var list = new List<int> {1, 2, 4};
			Assert.AreEqual(2, Utilities.SafeGetElement(list, 1));

			Assert.AreEqual(default(int), Utilities.SafeGetElement(list, 3));
		}

		[TestMethod, TestCategory("Common utilities")]
		public void MinTest()
		{
			var date1 = new DateTime(2018,8,1);
			var date2 = new DateTime(2018,7,31);
			Assert.AreEqual(date2, Utilities.Min(date1, date2));

			double? first = null, second = -2;
			Assert.IsNull(Utilities.Min(first, second));

			first = -22222222;
			Assert.AreEqual(first, Utilities.Min(first, second));
		}

		[TestMethod, TestCategory("Common utilities")]
		public void FileStreamToBytesTest()
		{
			CreateTestDirectory();
			var fileName = "test.txt";
			CreateTestFile(fileName, "test");
			using (var file = File.Open(Path.Combine(TestsDir, fileName), FileMode.Open))
			{
				var result = Utilities.FileStreamToBytes(file);
				Assert.IsInstanceOfType(result, typeof(byte[]));
				Assert.IsTrue(result.Length > 0); 
				file.Close();
			}
			DeleteTestDirectory();
		}

		[TestMethod, TestCategory("Common utilities")]
		public void BrochureId()
		{
			Assert.AreEqual(10, Utilities.DeCryptBrochureId(90));

			Assert.AreEqual(90, Utilities.EncryptBrochureId(10));
		}


		[TestMethod, TestCategory("Regex utilities")]
		public void IsValidEmail()
		{
			var email = "test";
			Assert.IsFalse(RegexUtilities.IsValidEmail(email));

			email = "test@test.com";
			Assert.IsTrue(RegexUtilities.IsValidEmail(email));

			email = "test@";
			Assert.IsFalse(RegexUtilities.IsValidEmail(email));
			
		}


		[TestMethod, TestCategory("Extension methods")]
		public void InTest()
		{
			Assert.ThrowsException<ArgumentNullException>(() => 1.In(null));
			var list = new[] {1, 3, 5};
			Assert.IsTrue(3.In(list));
		}

		/*[TestMethod, TestCategory("Extension methods")]
		public void IfNotNullTest()
		{
			throw new NotImplementedException();
		}

		[TestMethod, TestCategory("Extension methods")]
		public void SplitCamelCaseTest()
		{
			throw new NotImplementedException();
		}*/

		[TestMethod, TestCategory("Extension methods")]
		public void RightTest()
		{
			var s = "abcd";
			Assert.AreEqual("cd", s.Right(2));

			Assert.AreEqual(s, s.Right(5));

			Assert.IsNull(s.Right(-1));
		}

		[TestMethod, TestCategory("Extension methods")]
		public void LeftTest()
		{
			var s = "abcd";
			Assert.AreEqual("ab", s.Left(2));

			Assert.AreEqual(s, s.Left(5));

			Assert.IsNull(s.Left(-1));
		}

		[TestMethod, TestCategory("Extension methods")]
		public void ToMonth21Test()
		{
			var date = new DateTime(2018,8,20);
			var result = date.ToMonth21();
			Assert.IsNotNull(result);
			Assert.AreEqual(1808,result.Value);
		}

		[TestMethod, TestCategory("Extension methods")]
		public void ToMonth21ValueTest()
		{
			var date = new DateTime(2018,8,20);
			var result = date.ToMonth21Value();
			Assert.AreEqual(1808,result);
		}

		[TestMethod, TestCategory("Extension methods")]
		public void NotInTest()
		{
			var list1 = new List<Tuple<int, int>>
			{
				new Tuple<int, int>(1,1), 
				new Tuple<int, int>(2,2), 
				new Tuple<int, int>(3,3), 
			};

			var list2 = new List<Tuple<int, int>>
			{
				new Tuple<int, int>(1,1), 
				new Tuple<int, int>(2,2)
			};

			var result = list1.NotIn(list2, "Item1");
			Assert.AreEqual(1, result.Count);

			Assert.AreEqual(3, result[0].Item1);

		}

		[TestMethod, TestCategory("Extension methods")]
		public void ToIsoDateTest()
		{
			DateTime? date = new DateTime(2018,8,20);
			Assert.AreEqual("2018-08-20 00:00", date.ToIsoDate());

			date = null;
			Assert.IsTrue(string.IsNullOrEmpty(date.ToIsoDate()));
		}
		
		[TestMethod(), TestCategory("Common classes")]
		public void ResolveAddressTest()
		{
			var result = Geocoder.ResolveAddress("Plehanov put 16", "Zagreb", "","10000", "Hr");
			Assert.IsNotNull(result);
		}

		[TestMethod(), TestCategory("Common classes")]
		public void Geolocation()
		{
			decimal lat = 30, lon = 30;
			var geoLocation = new Geolocation(lat, lon);
			Assert.AreEqual($"Latitude: {lat} Longitude: {lon}", geoLocation.ToString());

			Assert.AreEqual($"+to:{lat}%2B{lon}", geoLocation.ToQueryString());
		}


		[TestMethod(), TestCategory("Common classes")]
		public void ExtendedWebClientTest()
		{
			using (var client = new ExtendedWebClient())
			{
				var method = client.GetType().GetMethod("GetWebRequest", BindingFlags.NonPublic | BindingFlags.Instance);
				Assert.IsNotNull(method);
				var uri = new Uri("http://google.com");
				var result = method.Invoke(client, new [] {(object)uri});
				Assert.IsInstanceOfType(result, typeof(HttpWebRequest));
				Assert.AreEqual(500000, (result as HttpWebRequest).Timeout);
			}
		}

	}
}
