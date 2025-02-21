using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpWebManager
{
    public class TypeManager
    {
        /// <summary>
        /// Convert normal type naar Generic Type and initialiseren
        /// </summary>
        /// <param name="_Type">normal type</param>
        /// <returns>een nieuwe Generic Collection</returns>
        internal static object ConvertToGenericCollectionType(Type _Type)
        {
            return Activator.CreateInstance(typeof(List<>).MakeGenericType(_Type));
        }
    }
}
