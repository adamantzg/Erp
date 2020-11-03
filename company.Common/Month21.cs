using System;

namespace company.Common
{
    public class Month21
    {
        private int value;

        public int Value
        {
            get { return value; }
        }

        public DateTime Date
        {
            get { return GetDate(Value); }
        }

        public Month21(DateTime date)
        {
            value = Utilities.GetMonth21FromDate(date);
        }

        public Month21(int value)
        {
            this.value = value;
        }
        
        public static Month21 Now
        {
            get { return new Month21(DateTime.Today); }
        }

        public static Month21 FromDate(DateTime date)
        {
            return new Month21(date);
        }

        public static Month21 FromYearMonth(int year, int month)
        {
            return new Month21(new DateTime(year, month, 1));
        }

        public static DateTime GetDate(int value)
        {
            return Utilities.GetDateFromMonth21(value);
        }

        public static Month21 operator +(Month21 m, int offset)
        {
            return new Month21(Utilities.GetDateFromMonth21(m.Value).AddMonths(offset));
        }

        public static Month21 operator -(Month21 m, int offset)
        {
            return new Month21(Utilities.GetDateFromMonth21(m.Value).AddMonths(-1*offset));
        }

        public static int operator -(Month21 m, Month21 mToSubtract)
        {
            var date1 = Utilities.GetDateFromMonth21(m.Value);
            var date2 = Utilities.GetDateFromMonth21(mToSubtract.Value);
            return (date1.Year - date2.Year)*12 + date1.Month - date2.Month;
        }

        public static bool operator <(Month21 first, Month21 second)
        {
            return first.Value < second.Value;
        }

        public static bool operator >(Month21 first, Month21 second)
        {
            return first.Value > second.Value;
        }

        public static bool operator <=(Month21 first, Month21 second)
        {
            return first.Value <= second.Value;
        }

        public static bool operator >=(Month21 first, Month21 second)
        {
            return first.Value >= second.Value;
        }

        public static bool operator == (Month21 first, int second)
        {
            return first.Value == second;
        }

        public static bool operator !=(Month21 first, int second)
        {
            return first.Value != second;
        }

        public static bool operator ==(int first, Month21 second)
        {
            return first == second.Value;
        }

		public static bool operator == (Month21 first, Month21 second)
		{
			return first.Value == second.Value;
		}

		public static bool operator !=(Month21 first, Month21 second)
		{
			return first.Value != second.Value;
		}

		public static bool operator !=(int first, Month21 second)
        {
            return first != second.Value;
        }

		public override bool Equals(object obj)
		{
			if (!(obj is Month21))
				return false;
			return Value == (obj as Month21).Value;
		}

		public override int GetHashCode()
		{
			return Value;
		}

	}
}
