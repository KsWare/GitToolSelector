using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace KsWare.MergeToolSelector
{
    class Program
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(Program));
        internal static string ConfigFile { get; set; }
        internal static bool DisableExternalParserExistCheck { get; set; }

        static int Main(string[] args)
        {
#if(!DEBUG)
	        try
	        {
#endif
		        Log.Info("Startup");
		        Log.Debug($"args: {string.Join(" ", args)}");
		        var psi = Run(args);
		        if (psi == null) return -1;
		        Log.Info($"Process.Start: {psi.FileName} {psi.Arguments}");
		        var process = Process.Start(psi);
		        process.WaitForExit();
		        Log.Info($"Process ExitCode: {process.ExitCode}");
		        return process.ExitCode;
#if(!DEBUG)
	        }
	        catch (Exception ex)
	        {
		        Log.Fatal("Oops, something really went wrong.", ex);
				return -1;
	        }
#endif
        }

        internal static ProcessStartInfo Run(string[] args)
        {
	        var parameter = CreateParameterDictionary(args);
	        var configuration=new ConfFile(ConfigFile);
	        return CreateProcessStartInfo(parameter, configuration);
        }

        internal static ProcessStartInfo CreateProcessStartInfo(Dictionary<string,string> parameter, ConfFile configuration)
        {
	        if (!parameter.TryGetValue("tool", out var toolType)) // Diff|Merge
	        {
		        Log.Error("Missing parameter! \"-tool\"");
		        return null;
	        }

	        var ext = GetExtensionFromAnyFileParameter(parameter, configuration);
	        var externalParser = configuration.GetValue(ext, "ExternalParser");
	        if (!string.IsNullOrEmpty(externalParser))
	        {
		        if (!DisableExternalParserExistCheck && !File.Exists(externalParser))
		        {
					Log.Warn($"External parser not found! Continue with default settings. Path={externalParser}");
					ext = "*";
		        }
		        else
		        {
			        parameter.Add("externalparser", externalParser);
		        }
	        }

	        var toolSection = GetToolSection(ext, toolType, configuration);

	        var cmd = configuration.GetValue($"tool {toolSection}","cmd").Trim();
	        var psiExe = ParseFileName(cmd);
	        var psiParameter = cmd.Substring(psiExe.Length).Trim();
	        psiExe=psiExe.Trim('"');
	        psiParameter = Regex.Replace(psiParameter, @"\$[a-zA-Z]+", m=>ReplaceParameter(m, parameter));


	        ProcessStartInfo psi = new ProcessStartInfo(psiExe, psiParameter);
	        return psi;
        }

        internal static Dictionary<string, string> CreateParameterDictionary(string[] args)
        {
	        var parameter = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
	        var arguments = args.ToList();
	        while (arguments.Count > 0)
	        {
		        parameter.Add(arguments[0].Substring(1), arguments[1]);
		        arguments.RemoveAt(0);
		        arguments.RemoveAt(0);
	        }

	        return parameter;
        }

        private static string GetToolSection(string ext, string toolType, ConfFile configuration)
        {
	        var toolSection = configuration.GetValue(ext, $"{toolType}Tool");
	        if (string.IsNullOrEmpty(toolSection))
	        {
		        Log.Warn($"Configuration for extension not found! Continue with default settings. Extension={ext}");
		        toolSection=configuration.GetValue("*", $"{toolType}Tool");
	        }
	        if (string.IsNullOrEmpty(toolSection))
	        {
		        Log.Error($"Configuration for default not found! Make sure there's a [*] section in configuration file. Path={configuration.FullPath}");
	        }

	        return toolSection;
        }

        internal static string GetExtensionFromAnyFileParameter(Dictionary<string, string> parameter, ConfFile configuration)
        {
            foreach (var s in parameter)
            {
                if(s.Key.Equals("tool",StringComparison.OrdinalIgnoreCase)) continue;
                if(!s.Value.Contains("\\")) continue;
				//TODO check exists to be sure its a filename
                var ext=configuration.GetExtensionFromOverrides(s.Value);
                if (ext != null) return ext;

                var f = Path.GetFileName(s.Value);
                if (f.Contains("."))
                {
                    return Path.GetExtension(s.Value);
                }
                else
                {
                    return f; // support for files w/o extension
                }
            }

            return null;
        }

        public static string ReplaceParameter(Match m, Dictionary<string,string> parameter)
        {
            var name = m.Value.Substring(1);
            if (!parameter.TryGetValue(name, out var value))
            {
                return name;
            }
            else
            {
                return value;
            }
        }

        private static string ParseFileName(string cmd)
        {
            if (cmd.StartsWith("\""))
            {
                var p = cmd.IndexOf('\"', 1);
                return cmd.Substring(0, p+1);
            }
            else
            {
                var p = cmd.IndexOf(' ', 1);
                return cmd.Substring(0, p);
            }
        }

        
    }
}
/*
[difftool "semanticdiff"]
	cmd = \"C:\\Program Files\\SemanticMerge\\semanticmergetool.exe\" -s \"$LOCAL\" -d \"$REMOTE\"
	keepBackup = false
[mergetool "semanticmerge"]
  cmd = \"C:\\Program Files\\SemanticMerge\\semanticmergetool.exe\" -s \"$REMOTE\" -d \"$LOCAL\" -b \"$BASE\" -r \"$MERGED\"
  trustExitCode = true
 */
