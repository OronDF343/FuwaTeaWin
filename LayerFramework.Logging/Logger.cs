using System.Runtime.CompilerServices;

namespace LayerFramework.Logging
{
    public static class Logger
    {
        static Logger()
        {
            log4net.Config.BasicConfigurator.Configure();
        }

        public static void DebugMessage(string message, object location, [CallerMemberName] string member = "%nowhere%")
        {
            var layer = LayerFactory.GetFactory(location.GetType());
            log4net.LogManager.GetLogger("Layer:" + layer.LayerName).Debug(string.Format("[{0}.{1}] {2}", location.GetType().FullName, member, message));
        }
    }
}
