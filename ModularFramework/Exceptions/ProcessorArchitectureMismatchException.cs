using System;
using System.Reflection;

namespace ModularFramework.Exceptions
{
    public class ProcessorArchitectureMismatchException : Exception
    {
        public AssemblyName OtherAssembly { get; }
        public ProcessorArchitecture CurrentArchitecture { get; }
        public ProcessorArchitecture OtherArchitecture { get; }

        public ProcessorArchitectureMismatchException(AssemblyName otherAssembly, ProcessorArchitecture current, ProcessorArchitecture other)
        {
            OtherAssembly = otherAssembly;
            CurrentArchitecture = current;
            OtherArchitecture = other;
        }

        public override string Message =>
            $"The target platform of the assembly {OtherAssembly.FullName} is {Enum.GetName(typeof(ProcessorArchitecture), OtherArchitecture)}, which is incompatible with the target platform of the current assembly, which is {Enum.GetName(typeof(ProcessorArchitecture), CurrentArchitecture)}!";
    }
}
