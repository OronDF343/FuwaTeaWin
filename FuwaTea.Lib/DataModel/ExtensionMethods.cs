using System.Linq;

namespace FuwaTea.Lib.DataModel
{
    public static class ExtensionMethods
    {
        public static TInterface FirstCanHandleOrDefault<TInterface, TInput, TOutput>(
            this IImplementationPriorityManager<TInterface, TInput, TOutput> iipm, TInput ti)
            where TInterface : ICanHandle<TInput, TOutput>
        {
            return iipm.GetImplementations(ti).FirstOrDefault(t => t.CanHandle(ti));
        }
    }
}
