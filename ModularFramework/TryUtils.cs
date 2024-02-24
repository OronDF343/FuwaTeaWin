using System;
using ModularFramework.Exceptions;

namespace ModularFramework
{
    public delegate void ErrorCallback(Exception arg1);

    public static class TryUtils
    {
        public static T TryCreateInstance<T>(Type t, params object[] parameters)
        {
            return (T)TryCreateInstance(t, parameters);
        }

        public static T TryCreateInstance<T>(Type t)
        {
            return (T)TryCreateInstance(t);
        }

        public static object TryCreateInstance(Type t)
        {
            object o;
            try
            {
                o = Activator.CreateInstance(t);
            }
            catch (Exception ex)
            {
                throw new ActivatorException(t, ex);
            }
            return o;
        }

        public static object TryCreateInstance(Type t, params object[] parameters)
        {
            object o;
            try
            {
                o = Activator.CreateInstance(t, parameters);
            }
            catch (Exception ex)
            {
                throw new ActivatorException(t, ex);
            }
            return o;
        }

        public static TResult TryGetResult<TResult>(this Func<TResult> op, ErrorCallback ec)
        {
            try
            {
                return op();
            }
            catch (Exception ex)
            {
                ec(ex);
                return default(TResult);
            }
        }
    }
}
