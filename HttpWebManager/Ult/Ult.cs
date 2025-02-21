using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.Xpo;
using System.Globalization;
using System.Threading;

namespace HttpWebManager
{
    public static class Ult
    {
        public static bool IsActivationDayValid(DateTime dt)
        {
            return (dt > DateTime.Today.AddDays(-1) && dt < DateTime.Today.AddDays(60));
        }

        public static bool IsWishDayValid(DateTime dt)
        {
            bool result = false;
            CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl-NL");

            result = dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday;

            return result;
        }

        public static DateTime GetNummerPorteringWishDay(DateTime _dt)
        {
            DateTime dt = _dt;

            while (!IsWeekDay(dt))
                dt = dt.AddDays(1);

            return dt;
        }

        public static DateTime GetNummerPorteringPrepaidActivationDay()
        {
            DateTime dt = DateTime.Today.AddDays(5);

            while (!IsWeekDay(dt))
                dt = dt.AddDays(1);
            
            return dt;
        }

        public static bool IsHoliday(DateTime dt)
        {
            bool result = false;
            // holiday check.........
            return result;
        }

        public static bool IsWeekDay(DateTime dt)
        {
            return dt.DayOfWeek != DayOfWeek.Saturday && 
                   dt.DayOfWeek != DayOfWeek.Sunday;
        }

        public static string CreateRandomString(int StringLength)
        {
            const string _allowedChars = "123456789";
            Random randNum = new Random();
            char[] chars = new char[StringLength];

            for (int i = 0; i < StringLength; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }

            return new string(chars);
        }

        public static string CreateRandomCharacter(int StringLength)
        {
            const string _allowedChars = "QWRTYPSDFGHJKLZXCVBNM";
            Random randNum = new Random();
            char[] chars = new char[StringLength];

            for (int i = 0; i < StringLength; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }

            return new string(chars);
        }

        public static string AdditionelePromotieTekst(string str)
        {
            string newStr = "";
            string Value = "";
            string oldStr = str;
            while (oldStr.Contains("<tr><td nowrap><nobr>"))
            {
                Value = HttpWebManager.ScrapeHelper.ExtractValue(oldStr, "<tr><td nowrap><nobr>", "</nobr>").Trim();
                newStr += Value + ";";
                oldStr = oldStr.Replace(string.Format("<tr><td nowrap><nobr>{0}</nobr>", Value), "");
            }
            return newStr;
        }

        /// <summary>
        /// Convert een collection naar List
        /// objecten hoeft niet zelfde zijn
        /// </summary>
        /// <typeparam name="T">Collection Type</typeparam>
        /// <param name="_list">input List</param>
        /// <returns>List met T type</returns>
        public static List<T> ConvertTList<T>(IEnumerable<object> _list)
        {
            object _obj = (T)Activator.CreateInstance(typeof(T));
            List<T> _listOut = new List<T>();
            _list.ToList().ForEach(x => 
                {
                    AssemblyManager.ConvertObject(_obj, x);
                    _listOut.GetType().GetMethod("Add").Invoke(_listOut, new object[] { _obj });
                });

            return _listOut;
        }

        /// <summary>
        /// Convert ene type collection naar andere type collection met verschillende type object
        /// </summary>
        /// <typeparam name="T">Collection Type</typeparam>
        /// <typeparam name="U">input type</typeparam>
        /// <param name="_list">collection</param>
        /// <returns>List met T type</returns>
        public static IEnumerable<T> ConvertTList<T, U>(IEnumerable<U> _list)
        {
            List<T> _listOut = new List<T>();
            _list.ToList().ForEach(x =>
            {
                object _obj = (T)Activator.CreateInstance(typeof(T));
                AssemblyManager.ConvertObject(_obj, x);
                _listOut.GetType().GetMethod("Add").Invoke(_listOut, new object[] { _obj });
            });

            return _listOut;
        }

        public static List<object> ConvertTList(XPCollection _list, Type _Type)
        {
            List<object> _listOut = new List<object>();

            foreach (object x in _list)
            {
                object _obj = Activator.CreateInstance(_Type);
                AssemblyManager.ConvertObject(_obj, x);
                _listOut.GetType().GetMethod("Add").Invoke(_listOut, new object[] { _obj });
            }
            return _listOut;
        }

        public static XPCollection<object> ConvertToXPCollection(XPCollection _list, Type _Type)
        {
            XPCollection<object> _listOut = new XPCollection<object>();

            foreach (object x in _list)
            {
                object _obj = Activator.CreateInstance(_Type);
                AssemblyManager.ConvertObject(_obj, x);
                _listOut.GetType().GetMethod("Add").Invoke(_listOut, new object[] { _obj });
            }
            return _listOut;
        }

        public static List<object> ConvertTList_X(XPCollection _list, Type _Type)
        {
            List<object> _listOut = new List<object>();

            foreach (object x in _list)
            {
                object _obj = Activator.CreateInstance(_Type);
                AssemblyManager.ConvertObject(_obj, x);
                _listOut.GetType().GetMethod("Add").Invoke(_listOut, new object[] { _obj });
            }
            return _listOut;
        }

    }
}
