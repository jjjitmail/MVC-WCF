using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HttpWebManager
{
    public class TelfortViewModel
    {
        public TelfortViewModel(object _Obj)
        {
            InitTelfortWinFormControls(_Obj);
        }

        public static Telfort_XPO_Objects.LookupManager GetLookupManager()
        {
            Telfort_XPO_Objects.LookupManager _LookupManager = new Telfort_XPO_Objects.LookupManager();
            _LookupManager.Lookup_AfgegeveninList.Add(null);

            return _LookupManager;
        }

        public static T GetServiceContract<T>(object _Obj)
        {
            return ConvertManager.InitThisObject<T>(_Obj);
        }

        private static void InitTelfortWinFormControls(object _Control)
        {
            if (_Control is ContainerControl)
                ControlsManager.InitWinFormControls((ContainerControl)_Control, typeof(Button));
        }
    }
}
