using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Reflection.Emit;

namespace HttpWebManager
{
    public class AssemblyManager
    {
        public AssemblyManager() { }

        internal string GetAssemblyQualifiedName()
        {
            return this.GetType().AssemblyQualifiedName;
        }

        private string GetAssemblyFullName()
        {
            return this.GetType().Assembly.FullName;
        }

        private void N()
        {
            AssemblyName name = new AssemblyName("MyRuntimeTypes");
            AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            ModuleBuilder module = assembly.DefineDynamicModule("MyRuntimeTypes");

            TypeBuilder type = module.DefineType("Mindscape.RuntimeTypes.Profile");
            
            Type compiledType = type.CreateType();

            TypeBuilder type2 = module.DefineType("Mindscape.RuntimeTypes.Profile", TypeAttributes.Public, typeof(Telfort_Objects.BaseObject));
        }

        //private static void ImplementProperty(TypeBuilder type, string propertyName, Type propertyType)
        //{
        //    FieldBuilder field = type.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

        //    PropertyBuilder property = type.DefineProperty(propertyName, PropertyAttributes.None, propertyType, Type.EmptyTypes);

        //    MethodBuilder getter = type.DefineMethod("get_" + propertyName, MethodAttributes.Public, propertyType, Type.EmptyTypes);
        //    ILGenerator getterIL = getter.GetILGenerator();
        //    getterIL.Emit(OpCodes.Ldarg_0);
        //    getterIL.Emit(OpCodes.Ldfld, field);
        //    getterIL.Emit(OpCodes.Ret);

        //    MethodBuilder setter = type.DefineMethod("set_" + propertyName, MethodAttributes.Public, typeof(void), new Type[] { propertyType });
        //    ILGenerator setterIL = setter.GetILGenerator();
        //    setterIL.Emit(OpCodes.Ldarg_0);
        //    setterIL.Emit(OpCodes.Ldarg_0);
        //    setterIL.Emit(OpCodes.Ldflda, field);
        //    setterIL.Emit(OpCodes.Ldarg_1);
        //    setterIL.Emit(OpCodes.Ldstr, propertyName);
        //    setterIL.Emit(OpCodes.Call, typeof(ViewModelBase).GetMethod("Set", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(propertyType));
        //    setterIL.Emit(OpCodes.Ret);

        //    property.SetGetMethod(getter);
        //    property.SetSetMethod(setter);
        //}

        /// <summary>
        /// Hiermee wordt een object geinitialiseerd
        /// </summary>
        /// <param name="TypeName">object naam</param>
        /// <returns>gewenste object</returns>
        internal static object CreateObjectInstance(string TypeName)
        {
            AssemblyManager _AssemblyManager = new AssemblyManager();

            return Activator.CreateInstance(Type.GetType(_AssemblyManager.GetAssemblyQualifiedName()
                .Replace(_AssemblyManager.GetType().Name, TypeName)));
        }

        internal static object CreateObjectInstance(string TypeName, params object[] args)
        {
            AssemblyManager _AssemblyManager = new AssemblyManager();

            return Activator.CreateInstance(Type.GetType(_AssemblyManager.GetAssemblyQualifiedName()
                .Replace(_AssemblyManager.GetType().Name, TypeName)), args);
        }

        /// <summary>
        /// Convert ene object naar andere type object
        /// </summary>
        /// <param name="_objOut">object out</param>
        /// <param name="_ObjIn">object in</param>
        internal static void ConvertObject(object _objOut, object _ObjIn)
        {
            _ObjIn.GetType().GetProperties().ToList().ForEach(z => 
                {
                    var x_Query = _objOut.GetType().GetProperties()
                    .Where(x => x.Name.ToLower().Equals(z.Name.ToLower())).FirstOrDefault();

                    if (x_Query != null)
                    {
                        try
                        {
                            x_Query.SetValue(_objOut, Convert.ChangeType(z.GetValue(_ObjIn, null), x_Query.PropertyType), null);
                        }
                        catch (Exception err) { }
                    }
                });
        }

        internal static string GetExecutionPath()
        {
            string codeBase = Assembly.GetExecutingAssembly().GetName().CodeBase;
            return new Uri(Path.GetDirectoryName(codeBase)).LocalPath;
        }

        internal static object GetPropertyTagValue(object _Obj)
        {            
            return _Obj.GetType().GetProperty("Tag").GetValue(_Obj, null);
        }

        internal static void InvokeMethod(object _Obj)
        {
            _Obj.GetType().GetMethod("Execute").Invoke(_Obj, null);
        }

        internal static void InvokeMethod(System.Windows.Forms.Control x, EventInfo _EventInfo, object _Obj, MethodInfo _MethodInfo)
        {
            _EventInfo.GetAddMethod().Invoke(x, new object[] { GetDelegate(_EventInfo, _Obj, _MethodInfo) });
        }

        internal static Delegate GetDelegate(EventInfo _EventInfo, object _Obj, MethodInfo _MethodInfo)
        {
            return Delegate.CreateDelegate(_EventInfo.EventHandlerType, _Obj, _MethodInfo);
        }

        internal static MethodInfo ExecuteMethod(object _Obj)
        {
            return _Obj.GetType().GetMethod("Execute");
        }

        internal static MethodInfo GetNonPublicExecuteMethod(object _Obj)
        {
            return _Obj.GetType().GetMethod("Execute", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        internal static EventInfo ClickEvent(object _Obj)
        {
            return _Obj.GetType().GetEvent("Click");
        }

        internal static string Telfort_Objects_LookupManager()
        {
            return new Telfort_Objects.LookupManager().GetType().AssemblyQualifiedName;
        }

        internal static string Telfort_XPO_Objects_LookupManager()
        {
            return new Telfort_XPO_Objects.LookupManager().GetType().AssemblyQualifiedName;
        }

        internal static Type Telfort_Objects_Type(string nameToreplace, string objectName)
        {
            return Type.GetType(new Telfort_Objects.LookupManager().GetType().AssemblyQualifiedName.Replace(nameToreplace, objectName));
        }

        internal static Type Telfort_XPO_Objects_Type(string nameToreplace, string objectName)
        {
            return Type.GetType(new Telfort_XPO_Objects.LookupManager().GetType().AssemblyQualifiedName.Replace(nameToreplace, objectName));
        }

        internal static Type Telfort_XPO_Objects_Type(string nameToreplace, Type objectName)
        {
            return Type.GetType(new Telfort_XPO_Objects.LookupManager().GetType().AssemblyQualifiedName.Replace(nameToreplace, objectName.Name));
        }
    }
}
