using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpWebManager
{
    public interface IBaseContyroller
    {
        T Using<T>();
    }
    public class BaseController
    {
        protected T Using<T>() where T : class
        {
            return null;// serviceLocator.GetInstance<T>();
        }
    }
}
