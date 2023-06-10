using Automation.Config;
using Automation.Origin;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Automation.Helper;
/// <summary>
/// 大漠插件 5.1423 破解
/// </summary>
public static class CrackHelper
{
    [DllImport("kernel32.dll", EntryPoint = "LoadLibraryA")]
    private static extern int LoadLibrary(string dllToLoad);

    /// <summary>
    /// 插件免注册
    /// </summary>
    /// <param name="path">插件dll路径</param>
    /// <param name="mode">填0即可</param>
    [DllImport("DmReg5.dll", EntryPoint = "SetDllPathA")]
    private static extern void SetDllPathA(string path, int mode);


    [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleA")]
    private static extern IntPtr GetModuleHandleA(string path);

    /// <summary>
    /// 写入内存
    /// </summary>
    /// <param name="hProcess">要修改的进程内存的句柄</param>
    /// <param name="lpBaseAddress">指向写入数据的指定进程中的基地址的指针</param>
    /// <param name="lpBuffer">指向包含要写入指定进程地址空间的数据的缓冲区的指针</param>
    /// <param name="nSize">要写入指定进程的字节数</param>
    /// <param name="lpNumberOfBytesWritten">指向变量的指针, 该变量接收传输到指定进程的字节数. 此参数是可选的. 如果为NULL则忽略该参数. </param>
    /// <returns>如果函数成功, 则返回值为非零</returns>
    [DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory")]
    private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, int[] lpBuffer, int nSize,
        IntPtr lpNumberOfBytesWritten);

    /// <summary>
    /// 破解大漠插件
    /// </summary>
    public static bool CrackPlugin()
    {
        // 加载免注册dll
        LoadLibrary(Dm5Config.AutoPlugin5RegDllPath);
        // 免注册dll加载插件
        SetDllPathA(Dm5Config.AutoPlugin5DllPath, 0);

        // 必须创建一个插件实例，否则无法破解
        var dm5 = new Dm5();
        // 拿到插件的内存模块句柄
        var moduleHandleA = GetModuleHandleA("dm5.dll").ToInt32();

        // 开始破解
        var crackMemory = Dm5Config.AutoPlugin5CrackMemory.Split("\r\n");
        foreach (var crackTxt in crackMemory)
        {
            // 字符串转ToInt64再转int, 因为部分数值超出int范围, 直接转会报错
            var memoryArea = Convert.ToInt32(crackTxt.Split("=").First(), 16) + moduleHandleA;
            var memoryValue = (int)Convert.ToInt64(crackTxt.Split("=").Last());
            // 写入内存进行破解
            WriteProcessMemory(Process.GetCurrentProcess().Handle, (IntPtr)memoryArea, new int[] { memoryValue }, 4,
                IntPtr.Zero);
        }
        // 判断是否成功
        return dm5.GetDmCount() >= 1;
    }
}