///////////////////////////////////////////////////////////////////////////////////
//Omschrijving: WebServiceManger is een Main Controller voor alle acties         //
//              van de web service. De Controller zorgt ervoor                   //
//              automatisch binding van object naar juiste ViewModel             //
//              met juiste event.                                                //
//Author: Jun Tam                                                                //
///////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;

namespace HttpWebManager
{
    public class WebServiceController<T>
        where T : class, new()
    {
        //protected event Func<Type, object> _ModelObject;
        private T _Content;
        public T Content
        {
            get { return _Content ?? new T(); }
            set { _Content = value; }
        }

        public T Execute()
        {
            // Lookup object van "T" type aanmaken
            object _ModelObject = ObjectManager.InitLookupViewModel(typeof(T));

            if (_ModelObject != null)
            {
                // object ophalen via method "Execute"
                object _NewObj = _ModelObject.InvokeExecuteMethod();

                // nieuwe object omzetten in T type
                this.Content = (T)Convert.ChangeType(_NewObj, typeof(T));
            }

            return this.Content;
        }
    }

    public class WebServiceController<T, U>
        where T : class, new()
        where U : class, new()
    {
        private T _Content;
        public T Content
        {
            get { return _Content ?? new T(); }
            set { _Content = value; }
        }

        public T Get()
        {
            // object met "U" type aanmaken
            Object _TObj = typeof(U).CreateInstance();

            // object ophalen via method "Get"
            object _NewObj = _TObj.InvokeGetMethod();

            // nieuwe object omzetten in T type
            this.Content = (T)Convert.ChangeType(_NewObj, typeof(T));

            return this.Content;
        }

        public T Get(string MobielNr, string SimOfRekeningNr)
        {
            // object met "U" type aanmaken
            Object _TObj = typeof(U).CreateInstance();

            // object ophalen via method "Get"
            object _NewObj = _TObj.InvokeGetMethod(MobielNr, SimOfRekeningNr);

            // nieuwe object omzetten in T type
            this.Content = (T)Convert.ChangeType(_NewObj, typeof(T));

            return this.Content;
        }

        public T Set()
        {
            // object met "U" type aanmaken
            Object _TObj = typeof(U).CreateInstance();

            // object ophalen via method "Set"
            object _NewObj = _TObj.InvokeSetMethod();

            // nieuwe object omzetten in T type
            this.Content = (T)Convert.ChangeType(_NewObj, typeof(T));

            return this.Content;
        }

        public T Set(object _Obj)
        {
            // object met "U" type aanmaken
            Object _TObj = typeof(U).CreateInstance();

            // object ophalen via method "Set"
            object _NewObj = _TObj.InvokeSetMethod(_Obj);

            // nieuwe object omzetten in T type
            this.Content = (T)Convert.ChangeType(_NewObj, typeof(T));

            return this.Content;
        }
    }
}