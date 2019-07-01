using System;
using System.Reflection;
using Oxide.Core;
using Oxide.Core.Extensions;

namespace Oxide.Ext.OutputLogger
{
    public class OutputLoggerExtension : Extension
    {
        public OutputLoggerExtension(ExtensionManager manager) : base(manager)
        {            
        }

        public override string Name => "Output Logger";

        public override string Author => "k1lly0u";

        public override VersionNumber Version => new VersionNumber(1, 0, 0);

        public override void Load()
        {
            outputLogger = new OutputLogger();
        }

        public override void OnModLoad()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, exception) =>
            {
                Interface.Oxide.LogException("An exception was thrown!", exception.ExceptionObject as Exception);
            };
        }

        public override bool SupportsReloading => false;

        private OutputLogger outputLogger;

        internal static Assembly Assembly = Assembly.GetExecutingAssembly();

        internal static AssemblyName AssemblyName = Assembly.GetName();

        internal static VersionNumber AssemblyVersion = new VersionNumber(AssemblyName.Version.Major, AssemblyName.Version.Minor, AssemblyName.Version.Build);

        internal static string AssemblyAuthors = ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(Assembly, typeof(AssemblyCompanyAttribute), false)).Company;
    }
}
