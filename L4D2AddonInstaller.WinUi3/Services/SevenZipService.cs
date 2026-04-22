using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace L4D2AddonInstaller.WinUi3.Services;

public enum OverwriteMode
{
    OverwriteAll,
    SkipExisting,
    RenameNewer,
    RenameExisting
}

public sealed class SevenZipService
{
    private const string SevenZipExeUrl = "https://furina.dakang233.com:8443/www/tools/7z.exe";
    private const string SevenZipDllUrl = "https://furina.dakang233.com:8443/www/tools/7z.dll";

    public string? Detect7ZipPath()
    {
        var paths = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "tools", "7z.exe"),
            Path.Combine(Environment.CurrentDirectory, "7z.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "7-Zip", "7z.exe")
        };

        return paths.FirstOrDefault(File.Exists);
    }

    public async Task<string> Download7ZipAsync(CancellationToken token, IProgress<int>? progress = null)
    {
        var toolsDir = Path.Combine(AppContext.BaseDirectory, "tools");
        Directory.CreateDirectory(toolsDir);

        var exePath = Path.Combine(toolsDir, "7z.exe");
        var dllPath = Path.Combine(toolsDir, "7z.dll");

        await DownloadSingleAsync(SevenZipExeUrl, exePath, token, p => progress?.Report((int)(p * 0.4)));
        await DownloadSingleAsync(SevenZipDllUrl, dllPath, token, p => progress?.Report(40 + (int)(p * 0.6)));

        return exePath;
    }

    public async Task ExtractAsync(IEnumerable<string> archives, string outputDir, string sevenZipExePath, OverwriteMode overwriteMode, CancellationToken token, IProgress<int>? progress = null)
    {
        Directory.CreateDirectory(outputDir);
        var items = archives.Where(File.Exists).ToList();
        if (items.Count == 0)
            throw new InvalidOperationException("没有可解压的压缩包");

        var modeArg = overwriteMode switch
        {
            OverwriteMode.OverwriteAll => "-y",
            OverwriteMode.SkipExisting => "-aos",
            OverwriteMode.RenameNewer => "-aou",
            OverwriteMode.RenameExisting => "-aot",
            _ => "-y"
        };

        for (var i = 0; i < items.Count; i++)
        {
            token.ThrowIfCancellationRequested();
            var archive = items[i];
            var psi = new ProcessStartInfo
            {
                FileName = sevenZipExePath,
                Arguments = $"x \"{archive}\" -o\"{outputDir}\" {modeArg} -bsp1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            using var process = new Process { StartInfo = psi, EnableRaisingEvents = true };
            process.Start();
            while (!process.HasExited)
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(150, token);
            }

            if (process.ExitCode != 0)
            {
                var err = await process.StandardError.ReadToEndAsync(token);
                throw new InvalidOperationException($"解压失败：{Path.GetFileName(archive)} {err}".Trim());
            }

            var percent = (int)Math.Round((i + 1) * 100d / items.Count);
            progress?.Report(percent);
        }
    }

    private static async Task DownloadSingleAsync(string url, string path, CancellationToken token, Action<int>? report)
    {
        using var client = new HttpClient();
        using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);
        response.EnsureSuccessStatusCode();

        var total = response.Content.Headers.ContentLength ?? 0;
        var done = 0L;

        await using var input = await response.Content.ReadAsStreamAsync(token);
        await using var output = File.Create(path);
        var buffer = new byte[81920];
        int read;
        while ((read = await input.ReadAsync(buffer, token)) > 0)
        {
            await output.WriteAsync(buffer.AsMemory(0, read), token);
            done += read;
            if (total > 0)
                report?.Invoke((int)(done * 100d / total));
        }
    }
}
