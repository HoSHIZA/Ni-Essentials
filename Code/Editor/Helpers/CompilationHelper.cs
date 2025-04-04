using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Build;

namespace NiGames.Essentials.Editor
{
    [PublicAPI]
    public static class CompilationHelper
    {
        private static readonly Regex CscRspDefineRegex = new(@"-define:([\w_]*)", RegexOptions.Compiled | RegexOptions.Singleline);
        
        #region Defines
            
        public static void AddDefine(string define)
        {
            var buildTargets = GetBuildTargets();
            
            foreach (var buildTarget in buildTargets)
            {
                var defines = PlayerSettings.GetScriptingDefineSymbols(buildTarget).Split(';').ToList();

                if (!defines.Any(d => string.Equals(define, d, StringComparison.InvariantCultureIgnoreCase)))
                {
                    defines.Add(define);

                    var definesString = string.Join(";", defines.ToArray());
                    
                    PlayerSettings.SetScriptingDefineSymbols(buildTarget, definesString);
                }
            }
        }

        public static void RemoveDefine(string define)
        {
            var buildTargets = GetBuildTargets();
            
            foreach (var buildTarget in buildTargets)
            {
                var defines = PlayerSettings.GetScriptingDefineSymbols(buildTarget).Split(';').ToList();

                if (defines.Any(d => string.Equals(define, d, StringComparison.InvariantCultureIgnoreCase)))
                {
                    defines.RemoveAt(defines.IndexOf(define));

                    var result = string.Join(";", defines.ToArray());
                    
                    PlayerSettings.SetScriptingDefineSymbols(buildTarget, result);
                }
            }
        }

        private static IEnumerable<NamedBuildTarget> GetBuildTargets()
        {
            var type = typeof(BuildTargetGroup);
            var values = Enum.GetValues(type);

            for (var i = 0; i < values.Length; i++)
            {
                var value = NamedBuildTarget.FromBuildTargetGroup((BuildTargetGroup)values.GetValue(i));

                if (value == NamedBuildTarget.Unknown) continue;
                
                yield return value;
            }
        }
        
        #endregion
        
        #region Csc.rsp defines

        public static bool AddDefineToCscRsp(string path, string define, bool assetDatabaseRefresh = true)
        {
            if (!File.Exists(path) ||
                string.Equals(Path.GetFileName(path), "csc.rsp", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            
            define = define.ToUpper();
            
            var defines = ReadDefinesFromCscRsp(path);

            if (defines.Contains(define))
            {
                return false;
            }

            Array.Resize(ref defines, defines.Length + 1);

            defines[defines.Length - 1] = define;

            WriteDefinesToCscRsp(path, defines, assetDatabaseRefresh);

            return true;
        }

        public static bool RemoveDefineFromCscRsp(string path, string define, bool assetDatabaseRefresh = true)
        {
            if (!File.Exists(path) ||
                string.Equals(Path.GetFileName(path), "csc.rsp", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            define = define.ToUpper();
            
            var defines = ReadDefinesFromCscRsp(path);

            if (defines.Contains(define))
            {
                return false;
            }

            var index = Array.IndexOf(defines, define);

            Array.Clear(defines, index, 1);

            WriteDefinesToCscRsp(path, defines, assetDatabaseRefresh);

            return true;
        }
        
        internal static string[] ReadDefinesFromCscRsp(string path)
        {
            var input = File.ReadAllText(path);
            
            var matches = CscRspDefineRegex.Matches(input);
            
            var result = new string[matches.Count];
            for (var i = 0; i < matches.Count; i++)
            {
                result[i] = matches[i].Groups[1].Value.ToUpper();
            }

            return result;
        }
        
        internal static void WriteDefinesToCscRsp(string path, string[] defines, bool assetDatabaseRefresh = true)
        {
            var content = new StringBuilder();
            
            Array.Sort(defines);

            foreach (var def in defines)
            {
                content.AppendFormat("-define:{0}\n", def.ToUpper());
            }

            File.WriteAllText(path, content.ToString());

            if (assetDatabaseRefresh)
            {
                AssetDatabase.Refresh();
            }
        }

        #endregion
    }
}