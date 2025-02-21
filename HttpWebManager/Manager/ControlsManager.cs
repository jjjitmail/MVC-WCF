using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace HttpWebManager
{
    internal class ControlsManager
    {
        /// <summary>
        /// Bind de alle child controls met bijhorende event method
        /// </summary>
        /// <param name="_Control">Huidige control</param>
        /// <param name="_ControlType">control type</param>
        internal static void InitWinFormControls(ContainerControl _Control, Type _ControlType)
        {
            // alle betreffende control van de main control ophalen
            GetControls(_Control, _ControlType)
                .ToList().ForEach(x =>
                {
                    // haal de object op via property "tag" value van betreffende control
                    var property = AssemblyManager.GetPropertyTagValue(x);

                    //  property "tag" value -> if not null/empty
                    if (property != null)
                    {
                        // maak een instance van betreffende objet
                        Object _Obj = AssemblyManager.CreateObjectInstance(property.ToString());

                        // "Obj" property vullen met huidige control
                        _Obj.GetType().GetProperty("Obj").SetValue(_Obj, _Control, null);
                        
                        // click event van de control ophalen
                        EventInfo _EventInfo = AssemblyManager.ClickEvent(x);

                        // click event -> if not null/empty
                        if (_EventInfo != null)
                        {
                            // "Execute" method ophalen
                            MethodInfo _MethodInfo = AssemblyManager.GetNonPublicExecuteMethod(_Obj);

                            // "Execute" method -> if not null/empty
                            if (_MethodInfo != null)
                            {
                                // "Execute" method toevoegen aan click event van betreffende control
                                _MethodInfo.InvokeMethod(x, _EventInfo, _Obj);
                            }
                        }
                    }
                });
        }

        /// <summary>
        /// Bind alle controls naar betreffende object.
        /// </summary>
        /// <param name="_Control">Huidige control</param>
        /// <param name="_ObjToMap">Object wordt gevuld/gemap</param>
        /// <param name="_Type">Control type</param>
        internal static void BindAllControlsToObject(object _Control, object _ObjToMap, Type _Type)
        {
            // betreffende control van bepaalde type ophalen
            GetControls((ContainerControl)_Control, _Type)
                .ToList().ForEach(x =>
                {
                    // haal de property naam op bij de control via tag
                    var tp = AssemblyManager.GetPropertyTagValue(x);

                    // if not null/empty
                    if (tp != null)
                    {
                        // haal de property naam op bij de object
                        var property = _ObjToMap.GetType().GetProperty(tp.ToString());

                        // betreffende waarde naar betreffende property mappen, if not null/empty
                        if (property != null)
                            property.SetValue(_ObjToMap, x.Text, null);
                    }
                });
        }

        /// <summary>
        /// Alle child controls van main controls ophalen
        /// </summary>
        /// <param name="control">Main control</param>
        /// <param name="type">child control type</param>
        /// <returns></returns>
        internal static IEnumerable<Control> GetControls(Control control, Type type)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetControls(ctrl, type)).Concat(controls).Where(c => c.GetType() == type);
        }
        //

    }
}
