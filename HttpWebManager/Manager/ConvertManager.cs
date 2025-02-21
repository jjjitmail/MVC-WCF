using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpWebManager
{
    public class ConvertManager
    {
        internal static T InitThisObject<T>(object _Obj)
        {
            Object _TObj = Activator.CreateInstance(typeof(T));
            _TObj.GetType().GetProperties().ToList().ForEach(x => 
            {
                //
            });
            return (T)Convert.ChangeType(_TObj, typeof(T));
        }

        internal static object CreateGenericType(Type genericType)
        {
            return Activator.CreateInstance(typeof(List<>).MakeGenericType(genericType));
        }

        /// <summary>
        /// Convert de collection met andere type naar Generic collection
        /// </summary>
        /// <param name="_Type">type object</param>
        /// <param name="_Collection">collection voor convert</param>
        /// <returns>nieuwe collection</returns>
        internal static object ConvertToGenericCollection(Type _Type, IEnumerable<object> _Collection)
        {
            object list = TypeManager.ConvertToGenericCollectionType(_Type);

            _Collection.ToList().ForEach(a =>
            {
                list.GetAddMethod().Invoke(list, new object[] { a });
            });

            return list;
        }
    }
}
