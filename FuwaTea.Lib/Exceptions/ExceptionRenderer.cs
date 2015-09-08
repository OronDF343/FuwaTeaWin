using System;
using System.IO;
using FuwaTea.Annotations;
using log4net.ObjectRenderer;

namespace FuwaTea.Lib.Exceptions
{
    [UsedImplicitly]
    public class ExceptionRenderer : IObjectRenderer
    {
        public void RenderObject(RendererMap rendererMap, object obj, TextWriter writer)
        {
            var ex = obj as Exception;
            if (ex == null) return;
            writer.WriteLine("    " + ex.GetType().FullName + ": " + ex.Message);
            if (!string.IsNullOrWhiteSpace(ex.StackTrace)) writer.WriteLine("    " + ex.StackTrace.Replace(Environment.NewLine, Environment.NewLine + "    "));
            ex = ex.InnerException;
            while (ex != null)
            {
                writer.WriteLine("    Caused by " + ex.GetType().FullName + ": " + ex.Message);
                if (!string.IsNullOrWhiteSpace(ex.StackTrace)) writer.WriteLine("    " + ex.StackTrace.Replace(Environment.NewLine, Environment.NewLine + "    "));
                ex = ex.InnerException;
            }
        }
    }
}
