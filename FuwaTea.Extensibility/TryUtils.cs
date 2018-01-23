using System;

namespace FuwaTea.Extensibility
{
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
                throw new Exception($"Failed to create an instance of the type {t.FullName}!", ex);
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
                throw new Exception($"Failed to create an instance of the type {t.FullName}!", ex);
            }
            return o;
        }

        public static TResult TryGetResult<TResult>(this Func<TResult> op, Action<Exception> ec)
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
