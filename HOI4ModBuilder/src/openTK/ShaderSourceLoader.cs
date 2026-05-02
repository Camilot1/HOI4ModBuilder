using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace HOI4ModBuilder.src.openTK
{
    public static class ShaderSourceLoader
    {
        private static readonly Regex IncludeRegex = new Regex(
            "^\\s*#include\\s+\"([^\"]+)\"\\s*$",
            RegexOptions.Compiled
        );

        public static string Load(string shaderPath)
            => Load(shaderPath, new HashSet<string>(StringComparer.OrdinalIgnoreCase), new Stack<string>());

        private static string Load(string shaderPath, HashSet<string> activeFiles, Stack<string> includeStack)
        {
            string fullPath = Path.GetFullPath(shaderPath);
            if (!activeFiles.Add(fullPath))
                throw new InvalidOperationException(
                    $"Cycle detected while loading shader includes: {string.Join(" -> ", includeStack)} -> {fullPath}"
                );

            includeStack.Push(fullPath);

            try
            {
                var result = new StringBuilder();
                string directoryPath = Path.GetDirectoryName(fullPath);

                foreach (string line in File.ReadLines(fullPath))
                {
                    Match includeMatch = IncludeRegex.Match(line);
                    if (!includeMatch.Success)
                    {
                        result.AppendLine(line);
                        continue;
                    }

                    string includePath = Path.Combine(directoryPath, includeMatch.Groups[1].Value);
                    result.AppendLine(Load(includePath, activeFiles, includeStack));
                }

                return result.ToString();
            }
            finally
            {
                includeStack.Pop();
                activeFiles.Remove(fullPath);
            }
        }
    }
}
