using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InstallerForL4D2AddonInstaller.Parser
{
    /// <summary>
    /// 从libraryfolders.vdf文件中解析指定游戏ID对应的Steam库根路径（path字段）。
    /// 采用迭代实现以避免深度递归导致的 StackOverflowException。
    /// 也支持解析只包含值的数组结构（如 addons { "file1" "file2" }）；或类似的文件结构（即便不是.vdf文件，但是是纯文本文件）。
    /// </summary>
    public static class SteamLibraryVdfParser
    {
        /// <summary>
        /// 从libraryfolders.vdf文件中解析指定游戏ID对应的Steam库根路径（path字段）。
        /// 采用迭代实现以避免深度递归导致的 StackOverflowException。
        /// </summary>
        /// <param name="libraryVdfFilePath">传入的 libraryfolders.vdf 文件路径。</param>
        /// <param name="gameId">要查找的游戏ID。</param>
        /// <returns>返回该游戏ID对应的Steam库根路径，如果找不到则返回 null。</returns>
        /// <exception cref="ArgumentException">VDF 文件路径为空。</exception>
        /// <exception cref="FileNotFoundException">libraryfolders.vdf 文件不存在。</exception>
        /// <exception cref="IOException">读取 VDF 文件失败。</exception>
        public static string GetLibraryPathByGameId(string libraryVdfFilePath, string gameId)
        {
            // 检查VDF文件路径是否为空或只包含空白字符，如果为空则抛出异常
            if (string.IsNullOrWhiteSpace(libraryVdfFilePath))
                throw new ArgumentException("VDF文件路径不能为空", nameof(libraryVdfFilePath));
            
            // 检查游戏ID是否为空或只包含空白字符，如果为空则抛出异常
            if (string.IsNullOrWhiteSpace(gameId))
                throw new ArgumentException("游戏ID不能为空", nameof(gameId));
            
            // 检查指定的VDF文件是否存在，如果不存在则抛出文件未找到异常
            if (!File.Exists(libraryVdfFilePath))
                throw new FileNotFoundException("libraryfolders.vdf文件不存在", libraryVdfFilePath);

            // 读取到的VDF文件内容，初始化为空字符串
            string vdfContent;
            try
            {
                // 尝试以UTF-8编码读取整个VDF文件内容到vdfContent变量中
                vdfContent = File.ReadAllText(libraryVdfFilePath, Universal.TryGetGB18030Encoding());
            }
            catch (Exception ex)
            {
                // 如果读取过程中发生任何异常，抛出输入输出异常，并附带原始异常信息
                throw new IOException("读取VDF文件失败", ex);
            }

            // 调用ParseVdfContentIterative方法解析VDF文件内容，返回一个字典对象作为解析结果
            var rootDict = ParseVdfContentIterative(vdfContent);

            // 尝试从解析后的字典中获取"libraryfolders"键对应的值，并检查该值是否为字典类型
            if (rootDict.TryGetValue("libraryfolders", out object libraryFoldersObj) &&
                libraryFoldersObj is Dictionary<string, object> libraryFolders)
            {
                // 总思路：遍历 libraryfolders 字典，查找每个库文件夹条目，并检查该条目是否包含指定的游戏ID。
                // 遍历每个库文件夹条目（以键值对形式存储）
                foreach (var libraryEntry in libraryFolders)
                {
                    // 检查当前库文件夹条目的值是否为字典类型，如果不是则跳过当前条目
                    if (!(libraryEntry.Value is Dictionary<string, object> libraryDict)) continue;

                    // 尝试从库文件夹字典中获取"path"键对应的值，并转换为字符串形式
                    if (!libraryDict.TryGetValue("path", out object pathObj)) continue;
                    string libraryPath = pathObj?.ToString().Replace("\\\\", "\\") ?? string.Empty;

                    // 检查当前库文件夹中是否存在指定的游戏ID，如果存在则返回该库文件夹路径
                    if (libraryDict.TryGetValue("apps", out object appsObj) &&
                        appsObj is Dictionary<string, object> appsDict &&
                        appsDict.ContainsKey(gameId))
                    {
                        return libraryPath;
                    }
                }
            }

            // 如果遍历所有库文件夹后仍未找到指定的游戏ID，则返回null表示未找到该游戏
            return null;
        }

        public class VersionDetails
        {
            public string Version { get; set; }      // 来自 key
            public string Name { get; set; }
            public string Type { get; set; }
            public string Description { get; set; }
            public string Author { get; set; }
            public string WebServer { get; set; }
            public string WebPort { get; set; }
            public string Prefix { get; set; }
            public string Protocol { get; set; }
            public string RelativePath { get; set; }
            public string SevenZipExePath { get; set; }
            public string SevenZipDllPath { get; set; }
            public string ExeName { get; set; }
        }


        private static string GetString(
            Dictionary<string, object> dict,
            string key)
        {
            return dict.TryGetValue(key, out var value)
                ? value?.ToString() ?? string.Empty
                : string.Empty;
        }

        /// <summary>
        /// 读取程序的版本内容
        /// </summary>
        /// <returns>返回程序的版本内容</returns>
        public static Dictionary<string, VersionDetails> GetVersionDetails(string versionListText)
        {
            var root = ParseVdfContentIterative(versionListText);

            if (!root.TryGetValue("versions", out var versionsObj) ||
                !(versionsObj is Dictionary<string, object> versionsDict))
            {
                throw new Exception("Invalid version list format.");
            }

            var result = new Dictionary<string, VersionDetails>();

            foreach (var versionEntry in versionsDict)
            {
                string versionKey = versionEntry.Key;

                if (!(versionEntry.Value is Dictionary<string, object> detailDict))
                    continue;

                var details = new VersionDetails
                {
                    Version = versionKey,
                    Name = GetString(detailDict, "Name"),
                    Type = GetString(detailDict, "Type"),
                    Author = GetString(detailDict, "Author"),
                    Description = GetString(detailDict, "Description"),
                    WebServer = GetString(detailDict, "WebServer"),
                    WebPort = GetString(detailDict, "WebPort"),
                    Prefix = GetString(detailDict, "Prefix"),
                    Protocol = GetString(detailDict, "Protocol"),
                    RelativePath = GetString(detailDict, "RelativePath"),
                    SevenZipExePath = GetString(detailDict, "SevenZipExePath"),
                    SevenZipDllPath = GetString(detailDict, "SevenZipDllPath"),
                    ExeName = GetString(detailDict, "ExeName"),
                };

                result[versionKey] = details;
            }

            return result;
        }



        /// <summary>
        /// 定义一个私有的静态方法 ParseVdfContentIterative，用于解析 VDF（Valve Data Format）格式的内容，并将其解析为嵌套的字典结构。
        /// </summary>
        /// <param name="content">要解析的 VDF 文件内容（先读取为字符串）。</param>
        /// <returns>返回解析后的字典。</returns>
        private static Dictionary<string, object> ParseVdfContentIterative(string content)
        {
            // 创建一个根字典，用于存储解析后的所有键值对
            var root = new Dictionary<string, object>();
            // 创建一个字典堆栈，用于管理当前解析的嵌套字典层级
            var dictStack = new Stack<Dictionary<string, object>>();
            // 初始化当前字典为根字典，开始解析
            Dictionary<string, object> current = root;

            // 定义索引变量 index，用于跟踪当前解析位置
            int index = 0;
            // 获取内容字符串的总长度
            int length = content.Length;

            // 循环遍历内容字符串，直到解析到字符串末尾
            while (index < length)
            {
                // 跳过当前索引位置的空白字符（如空格、制表符等）
                SkipWhitespace(content, ref index);

                // 跳过注释
                SkipNotes(content, ref index);

                // 如果跳过空白字符/注释后索引超出内容长度，退出循环
                if (index >= length) break;

                // 获取当前索引位置的字符
                char c = content[index];

                

                // 如果当前字符是双引号 '"'
                if (c == '"')
                {
                    // 调用 ReadQuotedValue 方法读取下一个被双引号括起来的 token（可能是键或独立的值）
                    string token = ReadQuotedValue(content, ref index);
                    // 读取 token 后再次跳过空白字符
                    SkipWhitespace(content, ref index);
                    // 跳过注释
                    SkipNotes(content, ref index);

                    // 如果跳过空白字符后索引超出内容长度，退出循环
                    if (index >= length) break;

                    // 获取下一个字符，用于判断 token 是键还是值
                    char next = content[index];

                    // 如果下一个字符是 '{'，表示 token 是一个键，后面跟着嵌套字典
                    if (next == '{')
                    {
                        // 索引加一，跳过 '{'
                        index++;
                        // 创建一个新的子字典
                        var child = new Dictionary<string, object>();
                        // 将子字典作为当前键的值存储在 current 字典中
                        current[token] = child;
                        // 将当前字典 current 压入堆栈，以便后续返回上一层级
                        dictStack.Push(current);
                        // 将当前字典 current 更新为子字典，继续解析下一层级
                        current = child;
                        continue;
                    }
                    // 如果下一个字符也是双引号 '"'
                    else if (next == '"')
                    {
                        // 调用 ReadQuotedValue 方法读取下一个被双引号括起来的值
                        string value = ReadQuotedValue(content, ref index);
                        // 将读取到的值存储在 current 字典中，键为前面读取到的 token
                        current[token] = value;
                        continue;
                    }
                    else
                    {
                        // 如果下一个字符既不是 '{' 也不是 '"'
                        // 表示 token 是一个独立的值（类似数组的条目）。使用当前字典的 Count 作为键来存储。
                        // 确定一个不重复的键：使用 current.Count 作为索引字符串。
                        string autoKey = current.Count.ToString();
                        // 如果键已经存在，则递增键值以确保唯一性
                        while (current.ContainsKey(autoKey))
                        {
                            autoKey = (int.Parse(autoKey) + 1).ToString();
                        }
                        // 将 token 存储在 current 字典中，键为 autoKey
                        current[autoKey] = token;
                        // 不需要在这里增加索引，因为下一个字符可能是 '}' 或另一个 token
                        // 继续解析
                        continue;
                    }
                }
                // 如果当前字符是 '}'，表示结束当前嵌套字典
                else if (c == '}')
                {
                    // 索引加一，跳过 '}'
                    index++;
                    // 如果堆栈中有记录的上一层级字典
                    if (dictStack.Count > 0)
                    {
                        // 将当前字典 current 更新为堆栈中的上一层级字典，以便继续解析
                        current = dictStack.Pop();
                    }
                    continue;
                }
                else
                {
                    // 如果当前字符既不是双引号也不是右大括号
                    // 跳过该字符，继续解析
                    index++;
                }
            }

            // 返回解析后的根字典
            return root;
        }


        /// <summary>
        /// 私有静态方法，用于从字符串中读取被双引号包围的值
        /// </summary>
        /// <param name="content">参数 content 是包含双引号包围值的源字符串</param>
        /// <param name="index">参数 index 是一个引用参数，用于跟踪当前读取位置，以便在方法调用后继续读取</param>
        /// <returns>返回被双引号包围的值（字符串）</returns>
        private static string ReadQuotedValue(string content, ref int index)
        {
            // 检查 index 是否超出字符串长度或者当前字符是否不是双引号
            // 如果任一条件为真，则返回空字符串
            if (index >= content.Length || content[index] != '"') return string.Empty;
            
            // 跳过起始的双引号
            index++;
            
            // 创建一个 StringBuilder 对象，用于高效地构建字符串
            var sb = new StringBuilder();
            
            // 使用 while 循环读取字符，直到遇到结束的双引号
            while (index < content.Length)
            {
                // 读取当前字符，并将 index 增加以指向下一个字符
                char c = content[index++];
                
                // 如果当前字符是转义字符 '\'，并且 index 尚未超出字符串长度
                if (c == '\\' && index < content.Length)
                {
                    // 读取转义字符后的下一个字符
                    char next = content[index++];
                    
                    // 根据下一个字符进行相应的处理
                    switch (next)
                    {
                        case '\\': sb.Append('\\'); break; // 如果下一个字符也是 '\'，则添加一个 '\' 到 sb
                        case '"': sb.Append('"'); break;  // 如果下一个字符是 '"'，则添加一个 '"' 到 sb
                        case 'n': sb.Append('\n'); break; // 如果下一个字符是 'n'，则添加一个换行符 '\n' 到 sb
                        case 'r': sb.Append('\r'); break; // 如果下一个字符是 'r'，则添加一个回车符 '\r' 到 sb
                        case 't': sb.Append('\t'); break; // 如果下一个字符是 't'，则添加一个制表符 '\t' 到 sb
                        default: sb.Append(next); break;    // 如果是其他字符，则直接添加该字符到 sb
                    }
                    // 跳过本次循环的剩余部分，继续下一次循环
                    continue;
                }
                
                // 如果当前字符是结束的双引号，则停止读取
                if (c == '"')
                {
                    break;
                }
                
                // 如果当前字符不是转义字符，也不是结束的双引号，则将其添加到 sb
                sb.Append(c);
            }
            
            // 返回构建好的字符串
            return sb.ToString();
        }


        /// <summary>
        /// 此方法会跳过字符串中的空白字符，更新索引位置到下一个非空白字符处。
        /// <remark>index由外部传入（使用ref修饰），以便在递归调用中更新。</remark>
        /// </summary>
        /// <param name="content">包含空白的字符串</param>
        /// <param name="index">字符串中当前位置的索引（从该位置开始检查并跳过空白字符）</param>
        private static void SkipWhitespace(string content, ref int index)
        {
            while (index < content.Length && char.IsWhiteSpace(content[index])) index++;
        }

        /// <summary>
        /// 此方法会跳过字符串中的注释，更新索引位置到下一个非注释字符处。
        /// </summary>
        /// <param name="content">包含空白的字符串</param>
        /// <param name="index">字符串中当前位置的索引</param>
        private static void SkipNotes(string content, ref int index)
        {
            // 跳过单行注释
            if (index + 1 < content.Length && content[index] == '/' && content[index + 1] == '/')
            {
                index += 2; // 跳过 "//"
                while (index < content.Length && content[index] != '\n') index++;
            }
            // 跳过多行注释
            else if (index + 1 < content.Length && content[index] == '/' && content[index + 1] == '*')
            {
                index += 2; // 跳过 "/*"
                while (index + 1 < content.Length)
                {
                    if (content[index] == '*' && content[index + 1] == '/')
                    {
                        index += 2; // 跳过 "*/"
                        break;
                    }
                    index++;
                }
            }
        }

        // ------------------------------------------------------------------------------------
        /// <summary>
        /// 解析download.txt，获取指定代号的配置（webServer/port/addons等）
        /// </summary>
        /// <param name="downloadListContent">download.txt内容</param>
        /// <param name="code">代号（作为字典的key）</param>
        /// <returns>返回指定代号对应的配置字典；找不到则返回null。</returns>
        public static Dictionary<string, object> GetAddonConfigByCode(string downloadListContent, string code)
        {
            var rootDict = ParseVdfContentIterative(downloadListContent);
            // 直接获取代号对应的配置字典
            if (rootDict.TryGetValue(code, out object configObj) && configObj is Dictionary<string, object> config)
            {
                return config;
            }
            return null;
        }

        /// <summary>
        /// 从配置中提取addons列表
        /// </summary>
        public static List<string> GetAddonPathsFromConfig(Dictionary<string, object> config)
        {
            var addonPaths = new List<string>();
            if (config.TryGetValue("addons", out object addonsObj) && addonsObj is Dictionary<string, object> addons)
            {
                // addons下的key是索引（0/1/2），value是文件路径（如l4d2/coldfront_full.vpk）
                foreach (var addon in addons.Values)
                {
                    var path = addon?.ToString();
                    if (!string.IsNullOrEmpty(path))
                    {
                        addonPaths.Add(path);
                    }
                }
            }
            return addonPaths;
        }
    }
}
