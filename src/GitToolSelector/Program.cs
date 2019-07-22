using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace KsWare.ToolSelector
{
    class Program
    {
        static Dictionary<string,string> parameter=new Dictionary<string,string>();
        static void Main(string[] args)
        {
            var arguments = args.ToList();
            while (arguments.Count > 0)
            {
                parameter.Add(arguments[0].Substring(1), arguments[1]);
                arguments.RemoveAt(0);
                arguments.RemoveAt(0);
            }

            var ext = GetExtensionFromAnyFileParameter();
            var conf = new ConfFile();
            var toolId = parameter["tool"];
            var externalParser = conf.GetValue(ext, "ExternalParser");
            parameter.Add("EXTERNALPARSER", externalParser);
            var cmd = conf.GetValue($"tool {toolId}","cmd").Trim();
            var psiExe = ParseFileName(cmd);
            var psiParameter = cmd.Substring(psiExe.Length).Trim();
            psiExe=psiExe.Trim('"');
            psiParameter = Regex.Replace(psiParameter, @"\$[a-zA-Z]+", new MatchEvaluator(ReplaceParameter));


            ProcessStartInfo psi = new ProcessStartInfo(psiExe, psiParameter);
            Process.Start(psi);
        }

        private static string GetExtensionFromAnyFileParameter()
        {
            foreach (var s in parameter)
            {
                if(s.Key.Equals("tool",StringComparison.OrdinalIgnoreCase)) continue;
                if(!s.Value.Contains("\\")) continue;
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

        public static string ReplaceParameter(Match m)
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
