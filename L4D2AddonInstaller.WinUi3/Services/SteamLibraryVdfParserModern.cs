using System.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace L4D2AddonInstaller.WinUi3.Services;

public static class SteamLibraryVdfParserModern
{
    public static string? GetLibraryPathByGameId(string libraryVdfFilePath, string gameId)
    {
        if (!File.Exists(libraryVdfFilePath))
            return null;

        var root = ParseVdfContentIterative(File.ReadAllText(libraryVdfFilePath, TryGetGB18030Encoding()));
        if (!root.TryGetValue("libraryfolders", out var foldersObj) || foldersObj is not Dictionary<string, object> folders)
            return null;

        foreach (var entry in folders.Values.OfType<Dictionary<string, object>>())
        {
            var path = entry.TryGetValue("path", out var p) ? p?.ToString()?.Replace("\\\\", "\\") : null;
            if (string.IsNullOrWhiteSpace(path))
                continue;

            if (entry.TryGetValue("apps", out var appsObj) && appsObj is Dictionary<string, object> apps && apps.ContainsKey(gameId))
                return path;
        }

        return null;
    }

    public static Dictionary<string, object>? GetAddonConfigByCode(string downloadListContent, string code)
    {
        var root = ParseVdfContentIterative(downloadListContent);
        return root.TryGetValue(code, out var configObj) ? configObj as Dictionary<string, object> : null;
    }

    public static List<string> GetAddonPathsFromConfig(Dictionary<string, object> config)
    {
        var paths = new List<string>();
        if (config.TryGetValue("addons", out var addonsObj) && addonsObj is Dictionary<string, object> addons)
        {
            foreach (var value in addons.Values)
            {
                var path = value?.ToString();
                if (!string.IsNullOrWhiteSpace(path))
                    paths.Add(path);
            }
        }

        return paths;
    }

    private static Dictionary<string, object> ParseVdfContentIterative(string content)
    {
        var root = new Dictionary<string, object>();
        var stack = new Stack<Dictionary<string, object>>();
        var current = root;
        var idx = 0;

        while (idx < content.Length)
        {
            SkipWhitespaceAndNotes(content, ref idx);
            if (idx >= content.Length)
                break;

            if (content[idx] == '"')
            {
                var token = ReadQuotedValue(content, ref idx);
                SkipWhitespaceAndNotes(content, ref idx);
                if (idx >= content.Length)
                    break;

                if (content[idx] == '{')
                {
                    idx++;
                    var child = new Dictionary<string, object>();
                    current[token] = child;
                    stack.Push(current);
                    current = child;
                    continue;
                }

                if (content[idx] == '"')
                {
                    var value = ReadQuotedValue(content, ref idx);
                    current[token] = value;
                    continue;
                }

                current[current.Count.ToString()] = token;
                continue;
            }

            if (content[idx] == '}')
            {
                idx++;
                if (stack.Count > 0)
                    current = stack.Pop();
                continue;
            }

            idx++;
        }

        return root;
    }

    private static string ReadQuotedValue(string content, ref int idx)
    {
        idx++;
        var start = idx;
        while (idx < content.Length && content[idx] != '"')
            idx++;

        var value = content[start..idx];
        if (idx < content.Length)
            idx++;

        return value;
    }

    private static void SkipWhitespaceAndNotes(string content, ref int idx)
    {
        while (idx < content.Length)
        {
            if (char.IsWhiteSpace(content[idx]))
            {
                idx++;
                continue;
            }

            if (idx + 1 < content.Length && content[idx] == '/' && content[idx + 1] == '/')
            {
                idx += 2;
                while (idx < content.Length && content[idx] != '\n')
                    idx++;
                continue;
            }

            if (idx + 1 < content.Length && content[idx] == '/' && content[idx + 1] == '*')
            {
                idx += 2;
                while (idx + 1 < content.Length && !(content[idx] == '*' && content[idx + 1] == '/'))
                    idx++;
                idx = Math.Min(idx + 2, content.Length);
                continue;
            }

            break;
        }
    }

    private static Encoding TryGetGB18030Encoding()
    {
        try
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Encoding.GetEncoding("GB18030");
        }
        catch
        {
            return Encoding.UTF8;
        }
    }
}
