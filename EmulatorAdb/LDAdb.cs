using System.Diagnostics.CodeAnalysis;
using EmulatorAdb.Config;
using EmulatorAdb.Model;
using EmulatorAdb.Utils;
using Microsoft.Win32;
#pragma warning disable CS8600

namespace EmulatorAdb;

// TODO 重构, 拆分成: 模拟器本身操作(启动/关闭), 模拟器内部操作(发送文本/截图/点击等)
// 将通用代码放到虚方法中, 由基类实现

/// <summary>
/// 雷电模拟器 Adb 操作
/// </summary>
[SuppressMessage("Interoperability", "CA1416:验证平台兼容性")]
public class LDAdb : IEmulatorAdb
{
    /// <summary>
    /// 雷电模拟器 Adb 实例
    /// </summary>
    /// <param name="emulatorInstance">模拟器信息</param>
    /// <param name="emulatorPath">模拟器路径</param>
    public LDAdb(EmulatorInfo emulatorInstance, string emulatorPath = "")
    {
        EmulatorInfo = emulatorInstance;
        if (string.IsNullOrEmpty(emulatorPath))
            emulatorPath = GetInstallPath();
        SetEmulatorPath(emulatorPath);
    }

    public EmulatorInfo EmulatorInfo { get; set; }
    public string InstallPath { get; set; } = string.Empty;
    public string AdbPath { get; set; } = string.Empty;

    /// <summary>
    /// 执行命令, 自动指定adb路径与索引
    /// </summary>
    /// <param name="subCommand">子命令</param>
    /// <param name="arguments">命令参数</param>
    /// <returns></returns>
    private async Task<string> ExecuteAuto(string subCommand, params string[] arguments)
    {
        string[] args = new string[arguments.Length + 3];
        Array.Copy(arguments, 0, args, 3, arguments.Length);
        args[0] = subCommand;
        args[1] = "--index";
        args[2] = EmulatorInfo.Index.ToString();
        var result = await Executable.ExecProcess(AdbPath, arg: args);
        // 如果有错误输出, 则返回错误
        return !string.IsNullOrEmpty(result.Item2) ? result.Item2 : result.Item1;
    }

    /// <summary>
    /// 执行命令, 自动指定 adb 路径与索引, 返回值为 (标准输出, 错误输出)
    /// </summary>
    /// <param name="subCommand">子命令</param>
    /// <param name="arguments">命令参数</param>
    /// <returns></returns>
    private async Task<(string, string)> ExecuteAutoReturnErr(string subCommand, params string[] arguments)
    {
        string[] args = new string[arguments.Length + 3];
        Array.Copy(arguments, 0, args, 3, arguments.Length);
        args[0] = subCommand;
        args[1] = "--index";
        args[2] = EmulatorInfo.Index.ToString();

        return await Executable.ExecProcess(AdbPath, arg: args);
    }

    /// <summary>
    /// 执行模拟器的 Action 子命令.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private async Task<string> ExecuteAction(string key, string value)
    {
        return await ExecuteAuto("action", "--key", key, "--value", value);
    }

    public string GetInstallPath()
    {
        string[] registryPaths = { LD.Registry9, LD.Registry5, LD.Registry5_64 };
        string[] keyNames = { "leidian", "XuanZhi" };

        foreach (string keyName in keyNames)
        {
            foreach (string path in registryPaths)
            {
                string fullPath = path.Replace("leidian", keyName);
                using var leiDian = Registry.CurrentUser.OpenSubKey(fullPath);
                if (leiDian != null)
                {
                    var installDir = leiDian.GetValue(LD.InstallDir);
                    if (installDir != null)
                    {
                        return installDir.ToString();
                    }
                }
            }
        }
        return string.Empty;
    }

    public string GetAdbPath()
    {
        // 判断 dnconsole.exe 是否存在
        var path = Path.Combine(InstallPath, "dnconsole.exe");
        if (File.Exists(path))
            return path;
        return string.Empty;
    }

    public void SetAdbPath(string adbName)
    {
        AdbPath = Path.Combine(InstallPath, adbName);
    }

    public void SetEmulatorPath(string installPath)
    {
        InstallPath = installPath;
        SetAdbPath(GetAdbPath());
    }

    public async Task<List<EmulatorInfo>> GetAllEmulatorInfo()
    {
        (string success, string _) = await Executable.ExecProcess(AdbPath, arg: new[] { "list2" });
        return success.Trim()
            .Split(Environment.NewLine)
            .Where(line => !string.IsNullOrEmpty(line))
            .Select(line =>
            {
                string[] info = line.Split(",");
                return new EmulatorInfo()
                {
                    Index = int.Parse(info[0]),
                    Title = info[1],
                    MainWindowHandle = int.Parse(info[2]),
                    RenderWindowHandle = int.Parse(info[3]),
                    Running = int.Parse(info[4]),
                    Pid = int.Parse(info[5]),
                    VBoxPid = int.Parse(info[6]),
                };
            })
            .ToList();
    }


    public async Task<EmulatorInfo> GetEmulatorInfo()
    {
        (string success, string _) = await Executable.ExecProcess(AdbPath, arg: new[] { "list2" });
        var infoString = success.Trim()
            .Split(Environment.NewLine)
            .Where(line => !string.IsNullOrEmpty(line))
            .First(line =>
            {
                string[] info = line.Split(",");
                return info[0] == EmulatorInfo.Index.ToString();
            });
        string[] info = infoString.Split(",");
        return new EmulatorInfo()
        {
            Index = int.Parse(info[0]),
            Title = info[1],
            MainWindowHandle = int.Parse(info[2]),
            RenderWindowHandle = int.Parse(info[3]),
            Running = int.Parse(info[4]),
            Pid = int.Parse(info[5]),
            VBoxPid = int.Parse(info[6]),
        };
    }

    public async Task UpdateEmulatorInfo()
    {
        (string success, string _) = await Executable.ExecProcess(AdbPath, arg: new[] { "list2" });
        var infoString = success.Trim()
            .Split(Environment.NewLine)
            .Where(line => !string.IsNullOrEmpty(line))
            .First(line =>
            {
                string[] info = line.Split(",");
                return info[0] == EmulatorInfo.Index.ToString();
            });
        string[] info = infoString.Split(",");
        EmulatorInfo.Index = int.Parse(info[0]);
        EmulatorInfo.Title = info[1];
        EmulatorInfo.MainWindowHandle = int.Parse(info[2]);
        EmulatorInfo.RenderWindowHandle = int.Parse(info[3]);
        EmulatorInfo.Running = int.Parse(info[4]);
        EmulatorInfo.Pid = int.Parse(info[5]);
        EmulatorInfo.VBoxPid = int.Parse(info[6]);
    }

    public async Task RunApp(string packageName)
    {
        await ExecuteAuto("runapp", "--packagename", packageName);
    }

    public async Task CloseApp(string packageName)
    {
        await ExecuteAuto("killapp", "--packagename", packageName);
    }

    public async Task<bool> IsRunApp(string packageName)
    {
        string success = await ExecuteAuto("adb", "--command", $"shell dumpsys window windows | grep {packageName}");
        return success.Contains("Window #");
    }

    public async Task StartEmulator()
    {
        await ExecuteAuto("launch");
    }

    public async Task CloseEmulator()
    {
        await ExecuteAuto("quit");
    }

    public async Task CloseAllEmulator()
    {
        await Executable.ExecProcess(AdbPath, arg: new[] { "quitall" });
    }

    public async Task RebootEmulator()
    {
        await ExecuteAuto("reboot");
    }

    public async Task RemoveEmulator()
    {
        await ExecuteAuto("remove");
    }

    public async Task CopyEmulator(int index, string newTitle)
    {
        var args = "copy " + (string.IsNullOrEmpty(newTitle) ? "" : "--name " + newTitle + " ") + "--from " + index;
        await Executable.ExecProcess(AdbPath, false, arg: args);
    }

    public async Task Input(string content)
    {
        await ExecuteAction("call.input", content);
    }

    public async Task<string> ModifyConfig(string width, string height, string dpi)
    {
        // 判断是否是空
        if (string.IsNullOrEmpty(width) || string.IsNullOrEmpty(height) || string.IsNullOrEmpty(dpi))
            return "参数不能为空";
        return await ExecuteAuto("modify", "--resolution", $"{width},{height},{dpi}");
    }



    #region 鼠标

    public async Task MouseSwipe(int x1, int y1, int x2, int y2, int time)
    {
        await ExecuteAuto("adb", $"--command", $"shell input swipe {x1} {y1} {x2} {y2} {time}");
    }

    #endregion
}