using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

namespace CrashSample
{
    class Program
    {
        static void Main(string[] args)
        {
            ExecuteAssembly();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void ExecuteAssembly()
        {
            var context = new CollectibleAssemblyLoadContext();
            var path = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..", "Module", "bin", "Debug", "netstandard2.1", "Module.dll");

            using(var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var assembly = context.LoadFromStream(fs);
                var type = assembly.GetType("Module.Foo");
                // This will crash when  Foo creates an instance of XmlSerializer
                var instance = Activator.CreateInstance(type);
            }

            context.Unload();
        }
    }

    public class CollectibleAssemblyLoadContext : AssemblyLoadContext
    {
        public CollectibleAssemblyLoadContext() : base(isCollectible: true) {}

        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}