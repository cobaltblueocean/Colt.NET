using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class PremitiveExtension
    {
        public static int FloatToIntBits(this float value)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        }

        public static int ToSafeInt(this double value)
        {
            int result;

            if (!int.TryParse(value.ToString(), out result))
            {
                if (value <= (double)Int32.MinValue)
                    return Int32.MinValue;
                else if (value >= (double)Int32.MaxValue)
                    return Int32.MaxValue;
            }

            return result;
        }

        public static int ToInt(this double value)
        {
            return Convert.ToInt32(value);
        }
    }
}
