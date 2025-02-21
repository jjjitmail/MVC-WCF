using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace HttpWebManager
{
    public abstract class ManagerBase<T> where T : class, new()
    {
        private T _Content;
        public T Content
        {
            get { return _Content ?? new T(); }
            set { _Content = value; }
        }
    }
}
