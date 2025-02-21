using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace HttpWebManager
{
    public class ObjectManager
    {
        internal static object InitLookupViewModel(Type _TObj)
        {
            MemberInfo info = _TObj;
            object _NewObject = info.GetCustomAttributes(true).Where(x => x.GetType().Name.Equals("LookupViewModel")).FirstOrDefault();
            if (_NewObject != null)
            {
                string _ObjectName = _NewObject.GetType().GetProperty("ViewModel").GetValue(_NewObject, null).ToString();
                return AssemblyManager.CreateObjectInstance(_ObjectName);
            }

            return null;
        }
    }
}
