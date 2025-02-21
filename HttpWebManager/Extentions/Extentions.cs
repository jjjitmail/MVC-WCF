using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace HttpWebManager
{
    public static class Extentions
    {
        public static DateTime ToDateTime(this string str)
        {
            return Convert.ToDateTime(str);
        }
        public static Boolean ToBoolean(this string str)
        {
            return Convert.ToBoolean(str);
        }
        public static Boolean IntToBoolean(this string str)
        {
            return Convert.ToBoolean(str.ToInt16());
        }
        public static Int16 ToInt16(this string str)
        {
            try
            {
                return Convert.ToInt16(str);
            }
            catch (Exception err)
            {
                return 0;
            }            
        }
        public static Int16 ToInt16(this object str)
        {
            try
            {
                return Convert.ToInt16(str);
            }
            catch (Exception err)
            {
                return 0;
            }
        }
        public static Decimal ToDecimal(this string str)
        {
            return Convert.ToDecimal(str);
        }
        public static string RemoveSpaceAndBreak(this string str)
        {
            return Regex.Replace(str, @"\s*(<[^>]+>)\s*", "$1", RegexOptions.Singleline);
        }

        public static bool IsEquals(this IEnumerable<object> _Collection, IEnumerable<object> _CollectionToCompare)
        {
            return _Collection.OrderBy(i => i).SequenceEqual(_CollectionToCompare.OrderBy(i => i));
        }
        
        /// <summary>
        /// Bind de control naar TelfortViewModel
        /// </summary>
        /// <param name="_Control">Huidige control</param>
        public static void TelfortDataContext(this ContainerControl _Control)
        {
            new TelfortViewModel(_Control);
        }

        /// <summary>
        /// Bind alle controls naar betreffende object.
        /// </summary>
        /// <param name="_Control">Huidige control</param>
        /// <param name="_ObjToMap">Object wordt gevuld/gemap</param>
        /// <param name="_Type">Control type</param>
        public static void BindAllControlsToObject(this object _Control, object _ObjToMap, Type _Type)
        {
            if (_Control != null)
                ControlsManager.BindAllControlsToObject(_Control, _ObjToMap, _Type);
        }

        //
        public static byte[] ConvertToByteArray(this StreamReader responseReader)
        {
            var bytes = default(byte[]);
            using (var memstream = new MemoryStream())
            {
                responseReader.BaseStream.CopyTo(memstream);
                bytes = memstream.ToArray();
            }
            return bytes;
        }

        public static void SaveAsPdf(this StreamReader responseReader, string FileName)
        {
            MemoryStream ms = new MemoryStream(responseReader.ConvertToByteArray());
            FileStream outStream = File.OpenWrite(String.Format(@"{0}\{1}.pdf", AssemblyManager.GetExecutionPath(), FileName));
            ms.WriteTo(outStream);
            outStream.Flush();
            outStream.Close();
            ms.Close();
        }

        public static void RaiseSafeEvent(this EventHandler eventHandler, object sender, System.EventArgs e)
        {
            if (eventHandler != null)
                eventHandler(sender, e);
        }

        public static void RaiseSafeEvent<T>(this EventHandler<T> eventHandler, object sender, T e) where T : System.EventArgs
        {
            if (eventHandler != null)
                eventHandler(sender, e);
        }

        public static IEnumerable<T> OrderBy<T, TResult>(this IEnumerable<T> source, Func<T, TResult> f, bool asscending)
        {
            return (asscending)? source.OrderBy(f) : source.OrderByDescending(f);
        }

        public static void InvokeMethod(this MethodInfo _MethodInfo, System.Windows.Forms.Control _Control, EventInfo _EventInfo, object _Obj)
        {
            AssemblyManager.InvokeMethod(_Control, _EventInfo, _Obj, _MethodInfo);
        }

        public static object ConvertToGenericCollection(this IEnumerable<object> _Collection, Type _Type)
        {
            return ConvertManager.ConvertToGenericCollection(_Type, _Collection);
        }

        #region "Get Method"
        public static MethodInfo GetGetMethod(this Type _Type)
        {
            return _Type.GetMethod("Get");
        }
        public static MethodInfo GetGetMethod(this object _Obj)
        {
            return _Obj.GetType().GetGetMethod();
        }

        public static object InvokeGetMethod(this object _Obj)
        {
            return _Obj.GetGetMethod().Invoke(_Obj, null);
        }
        public static object InvokeGetMethod(this object _Obj, string MobielNr, string SimOfRekeningNr)
        {
            return _Obj.GetGetMethod().Invoke(_Obj, new object[] { MobielNr, SimOfRekeningNr });
        }
        #endregion

        #region "Set Method"
        public static MethodInfo GetSetMethod(this Type _Type)
        {
            return _Type.GetMethod("Set");
        }
        public static MethodInfo GetSetMethod(this object _Obj)
        {
            return _Obj.GetType().GetSetMethod();
        }

        public static object InvokeSetMethod(this object _Obj)
        {
            return _Obj.GetSetMethod().Invoke(_Obj, null);
        }
        public static object InvokeSetMethod(this object _Obj, object _ObjInput)
        {
            return _Obj.GetSetMethod().Invoke(_Obj, new object[] { _ObjInput });
        }
        #endregion

        #region "Execute Method"
        public static MethodInfo GetExecuteMethod(this Type _Type)
        {
            return _Type.GetMethod("Execute");
        }
        public static MethodInfo GetExecuteMethod(this object _Obj)
        {
            return _Obj.GetType().GetExecuteMethod();
        }

        public static object InvokeExecuteMethod(this object _Obj)
        {
            return _Obj.GetExecuteMethod().Invoke(_Obj, null);
        }
        #endregion

        #region "Add Method"
        public static MethodInfo GetAddMethod(this Type _Type)
        {
            return _Type.GetMethod("Add");
        }        
        public static MethodInfo GetAddMethod(this object _Obj)
        {
            return _Obj.GetType().GetAddMethod();
        }
        #endregion

        #region "Activator"
        public static object CreateInstance(this Type _Type)
        {
            return Activator.CreateInstance(_Type);
        }

        public static object CreateInstance(this Type _Type, params object[] _Objects)
        {
            return Activator.CreateInstance(_Type, _Objects);
        }
        #endregion
    }
}
