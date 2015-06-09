using System;
using System.Reflection;
using FuwaTea.Lib.Exceptions;

namespace LayerFramework.Exceptions
{
    public class ProcessorArchitectureMismatchException : Exception, IDepthElement
    {
        public AssemblyName OtherAssembly { get; private set; }
        public ProcessorArchitecture CurrentArchitecture { get; private set; }
        public ProcessorArchitecture OtherArchitecture { get; private set; }

        public ProcessorArchitectureMismatchException(AssemblyName otherAssembly, ProcessorArchitecture current, ProcessorArchitecture other)
        {
            OtherAssembly = otherAssembly;
            CurrentArchitecture = current;
            OtherArchitecture = other;
        }

        public override string Message
        {
            get
            {
                return string.Format("The target platform of the assembly {0} is {1}, which is incompatible with the target platform of the current assembly, which is {2}!",
                                     OtherAssembly.FullName, Enum.GetName(typeof(ProcessorArchitecture), OtherArchitecture),
                                     Enum.GetName(typeof(ProcessorArchitecture), CurrentArchitecture));
            }
        }

        public int DisplayDepth
        {
            get { return 0; }
        }
    }
}
