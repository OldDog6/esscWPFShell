using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlTypes;

namespace esscWPFShell.Supporting
{
    public class myConversions
    {
        public static DateTime? AsDateTimeorNull(Object value)
        {
            DateTime? rval = null;

            if (value is SqlDateTime)
            {
                SqlDateTime val = (SqlDateTime)value;

                if (!val.IsNull)
                {
                    String datestring = value.ToString();
                    rval = DateTime.Parse(datestring);
                }
            }

            if (value is DateTime)
            {
                rval = (DateTime?)value;
            }

            return rval;
        }

        public static Object PutDateOrDBNull(DateTime? value)
        {
            if (value == null)
                return (Object)DBNull.Value;

            return (Object)value;
        }

        public static Guid? PutGuidorDBNull(Guid? value)
        {
            Guid? rval = null;

            if (value != null)
                rval = value;

            return rval;
        }

        public static Object PutGuidOrNull(Guid? value)
        {
            if (value == null)
                return (Object)DBNull.Value;

            return (Object)value;
        }

        public static Guid AsGuidorEmpty(Object value)
        {
            if ((value == null) || (value == DBNull.Value))
                return Guid.Empty;
            else
                return (Guid)value;
        }

        /// <summary>
        /// Converts object to string, null returns string.empty
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String AsStringOrNull(Object value)
        {
            if (value == null)
                return String.Empty;

            return value.ToString();
        }

        /// <summary>
        /// Takes a datetime and returns a "Good Morning/Afternoon, etc string...
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String AsGoodMorning(DateTime value)
        {
            if (value.Hour < 12)
                return "Morning";
            else
                if ((value.Hour >= 12) && (value.Hour < 18))
                    return "Afternoon";
                else
                    return "Evening";
        }

    }
}
