
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;


namespace erp.Model
{

	public class Web_product_component_details
	{
        public const double TOINCHES= 0.03937;
        public int web_unique { get; set; }
		public double? pack_GW { get; set; }
		public double? prod_nw { get; set; }
		public int web_site_id { get; set; }
		public string cprod_code1 { get; set; }
		public string cprod_name { get; set; }
		public string product_type_desc { get; set; }
		public double? pack_length { get; set; }
		public double? pack_width { get; set; }
		public double? pack_height { get; set; }
		public string prod_finish { get; set; }
		public double? prod_length { get; set; }
		public double? prod_width { get; set; }
		public double? prod_height { get; set; }
		public double? cprod_retail_web_override { get; set; }
		public double? cprod_retail { get; set; }
		public int? product_type_seq { get; set; }
		public double? flow02 { get; set; }
		public double? flow05 { get; set; }
		public double? flow10 { get; set; }
		public double? flow20 { get; set; }
		public double? flow30 { get; set; }
		public double? aerator02 { get; set; }
		public double? aerator05 { get; set; }
		public double? aerator10 { get; set; }
		public double? aerator20 { get; set; }
		public double? aerator30 { get; set; }
		public string web_code { get; set; }
		public string web_name { get; set; }
		public string prod_material { get; set; }
		public string whitebook_cprod_name { get; set; }
		public string cprod_status { get; set; }
		public int cprod_id { get; set; }
		public string glass_thickness { get; set; }
		public double? sale_retail { get; set; }
		public int? qty { get; set; }
		public string whitebook_cprod_code1 { get; set; }
		public int? mast_id { get; set; }
		public int? product_type { get; set; }
		public int? category1 { get; set; }
        public int? whitebook_template_id { get; set; }
        public string tech_tap_holes { get; set; }
        public double? bath_volume { get; set; }
		public string cprod_instructions { get; set; }

        [NotMapped]
        public string whitebook_notes { get; set; }
        
        [NotMapped]
        public string component_size_imperial { get; set; }

        public string dimensions_note { get; set; }

        public string ComponentName
        {
            get { return !string.IsNullOrEmpty(whitebook_cprod_name) ? whitebook_cprod_name:cprod_name; }
        }
        public string ComponentSize
        {
            get
            {
                var size = FormatMeasures(prod_length,prod_width, prod_height);
	            if (!string.IsNullOrEmpty(size))
		            size += "mm";
                return size;
            }
        }

       
        public StringBuilder ComponentSizeImperial
        {
            get
            {



                var size = new StringBuilder();
                size.Append(FormatMeasures(Math.Round((double)prod_length * 0.039370,2,MidpointRounding.AwayFromZero), Math.Round((double)prod_width * 0.039370,2,MidpointRounding.AwayFromZero),Math.Round((double)prod_height * 0.039370,2,MidpointRounding.AwayFromZero)));
                //size.Append(!string.IsNullOrEmpty(size.ToString()) ? "" : "");

                return size;
            }
        }
        public String ComponentSizeImperialLength
        {
            get
            {
                var size = "";
                int w, n, d;
                RoundToMixedFraction((double)prod_length * TOINCHES, 8, out w, out n, out d);
                size = FormatImperialMesures(w, n, d);
                return size;
            }
        }
        public String ComponentSizeImperialWidth
        {
            get
            {
                var size = "";
                int w, n, d;
                RoundToMixedFraction((double)prod_width * TOINCHES, 8, out w, out n, out d);
                size = FormatImperialMesures(w, n, d);
                return size;
            }
        }
        public String ComponentSizeImperialHeight
        {
            get
            {
                var size = "";
                int w, n, d;
                RoundToMixedFraction((double)prod_height * TOINCHES, 8, out w, out n, out d);
                size = FormatImperialMesures(w,n,d); 
                return size;
            }
        }
        public string PackedDimension
        {
            get
            {
                var dimension = new StringBuilder();
                dimension.Append(FormatMeasures((double)pack_length,(double)pack_width, (double)pack_height));

                var volume =string.Format("{0:0.####}", pack_length / 1000 * pack_width / 1000 * pack_height / 1000);
                dimension.Append(!string.IsNullOrEmpty(dimension.ToString()) ? $" = {volume} m3"  : "");


                return dimension.ToString();

            }
        }
        public string PackedDimensionImperial
        {
            get
            {
                var dimension = new StringBuilder();
                dimension.Append(FormatMeasures(Math.Round((double)pack_length*TOINCHES, 2, MidpointRounding.AwayFromZero), Math.Round((double)pack_width * TOINCHES , 2, MidpointRounding.AwayFromZero), Math.Round((double)pack_height * TOINCHES, 2, MidpointRounding.AwayFromZero)));

                var volume = string.Format("{0:0.####}", (double)((pack_length / 1000 * pack_width / 1000 * pack_height / 1000)*1.3080)*TOINCHES);
                dimension.Append(!string.IsNullOrEmpty(dimension.ToString()) ? $" = {volume} yd3" : "");


                return dimension.ToString();

            }
        }

        public string PackedImperialVolume
        {
            get
            {
                var size = "";
                var fraction = new Fraction();
                var volume = (double)((pack_length / 1000 * pack_width / 1000 * pack_height / 1000) * 1.3080) * TOINCHES;
               
                int w, n, d;
                //size= Convert((double)volume * TOINCHES);
                // size = FormatImperialMesures(w, n, d);
                if (volume < 1)
                {
                    fraction = RealToFraction((double)volume , 0.9999);
                    size = $"{fraction.N}/{fraction.D}";

                }
                else                    
                   RoundToMixedFraction((double)pack_length * TOINCHES, 8, out w, out n, out d);

                return size;
            }
        }
        public string PckedImperialOnlyVolume
        {
            get
            {
                var volume = (double)((pack_length / 1000 * pack_width / 1000 * pack_height / 1000) * 1.3080) * TOINCHES;
                return string.Format("{0:0.####}",volume);

            }
        }

        public string PackedImperialLength
        {
            get
            {
                var size = "";
                int w, n, d;
                RoundToMixedFraction((double)pack_length * TOINCHES, 8, out w, out n, out d);
                size = FormatImperialMesures(w, n, d);

                return size;
            }
        }

        public string PackedImperialWidth
        {
            get
            {
                var size = "";
                int w, n, d;
                RoundToMixedFraction((double)pack_width * TOINCHES, 8, out w, out n, out d);
                size = FormatImperialMesures(w, n, d);

                return size;
            }
        }

        public string PackedImperialHeight
        {
            get
            {
                var size = "";
                int w, n, d;
                RoundToMixedFraction((double)pack_height * TOINCHES, 8, out w, out n, out d);
                size = FormatImperialMesures(w, n, d);

                return size;
            }
        }

        private string FormatImperialMesures(double w, double n, double d)
        {
             var size = string.Format("{0} {1}", w == 0 ? string.Empty : w.ToString(), n == 0 || d == 1 ? string.Empty : n.ToString() + "/" + d.ToString());
            return size;
        }
        private string FormatMeasures(double? length, double? width, double? height)
        {
            /*var measures = new StringBuilder();


            measures.Append(length != null && length > 0.0 ? length.ToString() : "");

            var separator = length != null && length > 0.0 ? "x" : "";
            measures.Append(width != null && width > 0.0 ? $" {separator} {width.ToString()}" : "");

            var lastSeparator = !string.IsNullOrEmpty(measures.ToString()) ? "x" : "";
            measures.Append(height != null && height > 0.0 ? $" {lastSeparator} {height}" : "");*/
	        var dimensions = new List<double?> {length, width, height};

	        return string.Join(" x ", dimensions.Where(d => d > 0));
        }
        //[NotMapped]
        //public List<Web_product_new> RelatedProducts { get; set; }
        //[NotMapped]
        //public List<Web_product_info> ProductInfo { get; set; }
        public Web_product_new Product { get; set; }
        //public List<Web_product_pressure> Pressures { get; set; }
        //public List<Web_product_part> Parts { get; set; }

        /* convert 1*/
        void RoundToMixedFraction(double input, int accuracy, out int whole, out int numerator, out int denominator)
        {
            double dblAccuracy = (double)accuracy;
            whole = (int)(Math.Truncate(input));
            var fraction = Math.Abs(input - whole);
            if (fraction == 0)
            {
                numerator = 0;
                denominator = 1;
                return;
            }
            var n = Enumerable.Range(0, accuracy + 1).SkipWhile(e => (e / dblAccuracy) < fraction).First();
            var hi = n / dblAccuracy;
            var lo = (n - 1) / dblAccuracy;
            if ((fraction - lo) < (hi - fraction)) n--;
            if (n == accuracy)
            {
                whole++;
                numerator = 0;
                denominator = 1;
                return;
            }
            var gcd = GCD(n, accuracy);
            numerator = n / gcd;
            denominator = accuracy / gcd;
        }

        int GCD(int a, int b)
        {
            if (b == 0) return a;
            else return GCD(b, a % b);
        }
        /* end convert 1*/

        /* CONVERT 2/
        /* from url:
         * https://social.msdn.microsoft.com/Forums/en-US/e4df16cf-4207-4b76-8116-e02f689135ec/converting-a-decimal-to-a-fraction-in-c?forum=csharplanguage 
         */
        public static string Convert(decimal value)
        {
            // get the whole value of the fraction
            decimal mWhole = Math.Truncate(value);

            // get the fractional value
            decimal mFraction = value - mWhole;

            // initialize a numerator and denomintar
            uint mNumerator = 0;
            uint mDenomenator = 1;

            // ensure that there is actual a fraction
            if (mFraction > 0m)
            {
                // convert the value to a string so that you can count the number of decimal places there are
                string strFraction = mFraction.ToString().Remove(0, 2);

                // store teh number of decimal places
                uint intFractLength = (uint)strFraction.Length;

                // set the numerator to have the proper amount of zeros
                mNumerator = (uint)Math.Pow(10, intFractLength);

                // parse the fraction value to an integer that equals [fraction value] * 10^[number of decimal places]
                uint.TryParse(strFraction, out mDenomenator);

                // get the greatest common divisor for both numbers
                uint gcd = GreatestCommonDivisor(mDenomenator, mNumerator);

                // divide the numerator and the denominator by the gratest common divisor
                mNumerator = mNumerator / gcd;
                mDenomenator = mDenomenator / gcd;
            }

            // create a string builder
            StringBuilder mBuilder = new StringBuilder();

            // add the whole number if it's greater than 0
            if (mWhole > 0m)
            {
                mBuilder.Append(mWhole);
            }

            // add the fraction if it's greater than 0m
            if (mFraction > 0m)
            {
                if (mBuilder.Length > 0)
                {
                    mBuilder.Append(" ");
                }

                mBuilder.Append(mDenomenator);
                mBuilder.Append("/");
                mBuilder.Append(mNumerator);
            }

            return mBuilder.ToString();
        }

        public static decimal Convert(string value)
        {
            return 0m;
        }

        private static uint GreatestCommonDivisor(uint valA, uint valB)
        {
            // return 0 if both values are 0 (no GSD)
            if (valA == 0 &&
              valB == 0)
            {
                return 0;
            }
            // return value b if only a == 0
            else if (valA == 0 &&
                  valB != 0)
            {
                return valB;
            }
            // return value a if only b == 0
            else if (valA != 0 && valB == 0)
            {
                return valA;
            }
            // actually find the GSD
            else
            {
                uint first = valA;
                uint second = valB;

                while (first != second)
                {
                    if (first > second)
                    {
                        first = first - second;
                    }
                    else
                    {
                        second = second - first;
                    }
                }

                return first;
            }

        }
        /* END CONVERT 2*/

        /* CONVERT 3*/
        /* from url: */
        private Fraction RealToFraction(double value, double accuracy)
        {
            if (accuracy <= 0.0 || accuracy >= 1.0)
            {
                throw new ArgumentOutOfRangeException("accuracy", "Must be > 0 and < 1.");
            }

            int sign = Math.Sign(value);

            if (sign == -1)
            {
                value = Math.Abs(value);
            }

            // Accuracy is the maximum relative error; convert to absolute maxError
            double maxError = sign == 0 ? accuracy : value * accuracy;

            int n = (int)Math.Floor(value);
            value -= n;

            if (value < maxError)
            {
                return new Fraction(sign * n, 1);
            }

            if (1 - maxError < value)
            {
                return new Fraction(sign * (n + 1), 1);
            }

            // The lower fraction is 0/1
            int lower_n = 0;
            int lower_d = 1;

            // The upper fraction is 1/1
            int upper_n = 1;
            int upper_d = 1;

            while (true)
            {
                // The middle fraction is (lower_n + upper_n) / (lower_d + upper_d)
                int middle_n = lower_n + upper_n;
                int middle_d = lower_d + upper_d;

                if (middle_d * (value + maxError) < middle_n)
                {
                    // real + error < middle : middle is our new upper
                    upper_n = middle_n;
                    upper_d = middle_d;
                }
                else if (middle_n < (value - maxError) * middle_d)
                {
                    // middle < real - error : middle is our new lower
                    lower_n = middle_n;
                    lower_d = middle_d;
                }
                else
                {
                    // Middle is our best fraction
                    return new Fraction((n * middle_d + middle_n) * sign, middle_d);
                }
            }
        }

            /* END CONVERT 3*/
        }
    public struct Fraction
    {
        public Fraction(int n, int d)
        {
            N = n;
            D = d;
        }

        public int N { get; private set; }
        public int D { get; private set; }
    }

}
