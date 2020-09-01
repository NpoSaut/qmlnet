using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using NetNativeLibLoader.Loader;

namespace Qml.Net.Runtimes
{
    public static partial class RuntimeManager
    {
        public static void ConfigureRuntimeDirectory(string directory)
        {
            var runtimeTarget = GetCurrentRuntimeTarget();
            if (runtimeTarget == RuntimeTarget.Unsupported)
                throw new Exception("Unsupported runtime target");

            ConfigureRuntimeDirectory(directory, RuntimeTargetToString(runtimeTarget));
        }

        public static void ConfigureRuntimeDirectory(string directory, string runtimeTargetString)
        {
            if (string.IsNullOrEmpty(directory)) throw new ArgumentNullException(nameof(directory));

            if (!Directory.Exists(directory)) throw new Exception("The directory doesn't exist.");

            var versionFile = Path.Combine(directory, "version.txt");

            if (!File.Exists(versionFile)) throw new Exception("The version.txt file doesn't exist in the directory.");

            var version         = File.ReadAllText(versionFile).TrimEnd(Environment.NewLine.ToCharArray());
            var expectedVersion = $"{QmlNetConfig.QtBuildVersion}-{runtimeTargetString}";

            if (version != expectedVersion)
                throw new Exception(
                    $"The version of the runtime directory was {versionFile}, but expected {expectedVersion}");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var pluginsDirectory = Path.Combine(directory, "qt", "plugins");
                if (!Directory.Exists(pluginsDirectory))
                    throw new Exception($"Plugins directory didn't exist: {pluginsDirectory}");
                Environment.SetEnvironmentVariable("QT_PLUGIN_PATH", pluginsDirectory);

                var qmlDirectory = Path.Combine(directory, "qt", "qml");
                if (!Directory.Exists(qmlDirectory)) throw new Exception($"QML directory didn't exist: {qmlDirectory}");
                Environment.SetEnvironmentVariable("QML2_IMPORT_PATH", qmlDirectory);

                var libDirectory = Path.Combine(directory, "qt", "lib");
                if (!Directory.Exists(libDirectory))
                    throw new Exception($"The lib directory didn't exist: {libDirectory}");

                var preloadPath = Path.Combine(libDirectory, "preload.txt");
                if (!File.Exists(preloadPath)) throw new Exception($"The preload.txt file didn't exist: {preloadPath}");

                var libsToPreload = File.ReadAllLines(preloadPath).Where(x => !string.IsNullOrEmpty(x))
                                        .Select(x => Path.Combine(libDirectory, x))
                                        .ToList();
                var platformLoader = PlatformLoaderBase.SelectPlatformLoader();
                foreach (var libToPreload in libsToPreload)
                {
                    var libHandler = platformLoader.LoadLibrary(libToPreload);
                    if (libHandler == IntPtr.Zero) throw new Exception($"Unabled to preload library: {libToPreload}");
                }

                return;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var pluginsDirectory = Path.Combine(directory, "qt", "plugins");
                if (!Directory.Exists(pluginsDirectory))
                    throw new Exception($"Plugins directory didn't exist: {pluginsDirectory}");
                Environment.SetEnvironmentVariable("QT_PLUGIN_PATH", pluginsDirectory);

                var qmlDirectory = Path.Combine(directory, "qt", "qml");
                if (!Directory.Exists(qmlDirectory)) throw new Exception($"QML directory didn't exist: {qmlDirectory}");
                Environment.SetEnvironmentVariable("QML2_IMPORT_PATH", qmlDirectory);

                var libDirectory = Path.Combine(directory, "qt", "lib");
                if (!Directory.Exists(libDirectory))
                    throw new Exception($"The lib directory didn't exist: {libDirectory}");

                var preloadPath = Path.Combine(libDirectory, "preload.txt");
                if (!File.Exists(preloadPath)) throw new Exception($"The preload.txt file didn't exist: {preloadPath}");

                var libsToPreload = File.ReadAllLines(preloadPath).Where(x => !string.IsNullOrEmpty(x))
                                        .Select(x => Path.Combine(libDirectory, x))
                                        .ToList();
                var platformLoader = PlatformLoaderBase.SelectPlatformLoader();
                foreach (var libToPreload in libsToPreload)
                {
                    var libHandler = platformLoader.LoadLibrary(libToPreload);
                    if (libHandler == IntPtr.Zero) throw new Exception($"Unabled to preload library: {libToPreload}");
                }

                return;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var pluginsDirectory = Path.Combine(directory, "qt", "plugins");
                if (!Directory.Exists(pluginsDirectory))
                    throw new Exception($"Plugins directory didn't exist: {pluginsDirectory}");
                Environment.SetEnvironmentVariable("QT_PLUGIN_PATH", pluginsDirectory);

                var qmlDirectory = Path.Combine(directory, "qt", "qml");
                if (!Directory.Exists(qmlDirectory)) throw new Exception($"QML directory didn't exist: {qmlDirectory}");
                Environment.SetEnvironmentVariable("QML2_IMPORT_PATH", qmlDirectory);

                var binDirectory = Path.Combine(directory, "qt", "bin");
                if (!Directory.Exists(binDirectory))
                    throw new Exception($"The bin directory didn't exist: {binDirectory}");

                Environment.SetEnvironmentVariable(
                    "PATH", $"{binDirectory};{Environment.GetEnvironmentVariable("PATH")}");

                return;
            }

            throw new Exception("Unknown platform, can't configure runtime directory");
        }

        /// <summary>Custom configurations for unsupported runtime targets</summary>
        /// <param name="libDirectory">Directory where QT library binaries are located</param>
        /// <param name="qmlDirectory">QML directory</param>
        /// <param name="pluginsDirectory">QT plugins directory</param>
        /// <example>
        ///     For Windows it would be like:
        ///     <code>RuntimeManager.ConfigureRuntimeDirectory("path_to_qt/bin", "path_to_qt/qml", "path_to_qt/plugins");</code>
        ///     For Linux: <code>RuntimeManager.ConfigureRuntimeDirectory("/usr/lib", "/usr/qml/", "/usr/lib/qt/plugins/");</code>
        /// </example>
        public static void ConfigureRuntimeDirectory(string libDirectory, string qmlDirectory, string pluginsDirectory)
        {
            if (!Directory.Exists(pluginsDirectory))
                throw new Exception($"Plugins directory didn't exist: {pluginsDirectory}");
            Environment.SetEnvironmentVariable("QT_PLUGIN_PATH", pluginsDirectory);

            if (!Directory.Exists(qmlDirectory)) throw new Exception($"QML directory didn't exist: {qmlDirectory}");
            Environment.SetEnvironmentVariable("QML2_IMPORT_PATH", qmlDirectory);

            if (!Directory.Exists(libDirectory)) throw new Exception($"The lib directory didn't exist: {libDirectory}");

            var preloadPath = "preload.txt";
            if (!File.Exists(preloadPath)) throw new Exception($"The preload.txt file didn't exist: {preloadPath}");

            var libsToPreload = File.ReadAllLines(preloadPath).Where(x => !string.IsNullOrEmpty(x))
                                    .Select(x => Path.Combine(libDirectory, x))
                                    .ToList();
            var platformLoader = PlatformLoaderBase.SelectPlatformLoader();
            foreach (var libToPreload in libsToPreload)
            {
                var libHandler = platformLoader.LoadLibrary(libToPreload);
                if (libHandler == IntPtr.Zero) throw new Exception($"Unabled to preload library: {libToPreload}");
            }
        }
    }
}