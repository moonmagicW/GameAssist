using Automation.Helper;
using Automation.Origin;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using static System.String;

namespace Automation;

/// <summary>
/// 基于大漠插件 5.1423 的封装
/// </summary>
public class AutoPlugin : Dm5
{
    /// <summary>
    /// 默认点击延时
    /// </summary>
    private const int DefaultClickDelay = 250;

    static AutoPlugin()
    {
        if (!CrackHelper.CrackPlugin())
            Serilog.Log.Fatal("大漠插件破解失败");
    }

    public AutoPlugin()
    {
        _defaultClickAction = (x, y) =>
        {
            MoveTo(x, y);
            LeftClick();
        };
        _defaultClickAsync = (x, y) =>
        {
            MoveTo(x, y);
            LeftClick();
            return Task.CompletedTask;
        };
        // 关闭错误消息
        // SetShowErrorMsg(0);
    }

    /// <summary>
    /// 默认点击操作
    /// </summary>
    private readonly Action<int, int> _defaultClickAction;

    /// <summary>
    /// 默认异步点击操作
    /// </summary>
    private readonly Func<int, int, Task> _defaultClickAsync;


    #region 基础函数

    /// <summary>
    /// 输出日志
    /// </summary>
    /// <returns></returns>
    private void DebugLog([CallerMemberName] string methodName = "", string searchResults = "", int x = 0, int y = 0)
    {
        // 过滤某些寻找目标
        Serilog.Log.Debug("{MethodName}:  {FindTarget},  {X}, {Y}", methodName, searchResults, x, y);
    }

    #endregion


    #region 找图

    private const string DefaultPicDeltaColor = "020202";
    private const double DefaultSim = 0.9;

    // 全参数的寻图并点击  ======================================================
    /// <summary>
    /// 寻找图片并点击, 点击无偏移, 默认延时 250 ms
    /// </summary>
    public async Task<(string retPic, int x, int y)> FindClickPicGetCoordAsync(string target, int x1, int y1, int x2,
        int y2,
        Func<int, int, Task>? callBack = null,
        string deltaColor = DefaultPicDeltaColor, double sim = DefaultSim, int dir = 0,
        int delay = DefaultClickDelay, CancellationToken ct = default)
    {
        var result = FindPic(target, x1, y1, x2, y2, out int x, out int y, deltaColor, sim, dir);
        if (IsNullOrEmpty(result))
            return (Empty, x, y);
        callBack ??= _defaultClickAsync;
        await callBack(x, y);
        await Task.Delay(delay, ct);
        return (result, x, y);
    }

    // 使用找图结果的坐标执行指定的回调操作, 但不需要返回坐标给调用者  ======================================================
    public async Task<string> FindClickPicAsync(string target, int x1, int y1, int x2, int y2,
        Func<int, int, Task>? callBack = null,
        string deltaColor = DefaultPicDeltaColor, double sim = DefaultSim, int dir = 0,
        int delay = DefaultClickDelay, CancellationToken ct = default)
    {
        var result = FindPic(target, x1, y1, x2, y2, out int x, out int y, deltaColor, sim, dir);
        if (IsNullOrEmpty(result))
            return Empty;
        callBack ??= _defaultClickAsync;
        await callBack(x, y);
        await Task.Delay(delay, ct);
        return result;
    }

    // 不使用找图结果的坐标, 执行指定的回调操作 ======================================================
    public async Task<string> FindClickPicAsync(string target, int x1, int y1, int x2, int y2, Func<Task> callBack,
        string deltaColor = DefaultPicDeltaColor, double sim = DefaultSim, int dir = 0,
        int delay = DefaultClickDelay, CancellationToken ct = default)
    {
        var result = FindPic(target, x1, y1, x2, y2, out _, out _, deltaColor, sim, dir);
        if (IsNullOrEmpty(result))
            return Empty;
        await callBack();
        await Task.Delay(delay, ct);
        return result;
    }

    // 简单找图 ======================================================
    public string FindPic(string target, int x1, int y1, int x2, int y2) =>
        FindPic(target, x1, y1, x2, y2, DefaultPicDeltaColor, DefaultSim);

    public string FindPic(string target, int x1, int y1, int x2, int y2, string deltaColor, double sim, int dir = 0) =>
        FindPic(target, x1, y1, x2, y2, out _, out _, deltaColor, sim, dir);


    /// <summary>
    /// 查找指定区域内的图片
    /// </summary>
    /// <param name="target">寻找的图片</param>
    /// <param name="x">out 返回图片左上角的X坐标</param>
    /// <param name="y">out 返回图片左上角的Y坐标</param>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <param name="deltaColor">颜色色偏</param>
    /// <param name="sim">相似度,取值范围0.1-1.0</param>
    /// <param name="dir">查找方向 0: 从左到右,从上到下 1: 从左到右,从下到上 2: 从右到左,从上到下 3: 从右到左, 从下到上</param>
    /// <returns></returns>
    public string FindPic(string target, int x1, int y1, int x2, int y2, out int x, out int y, string deltaColor,
        double sim, int dir)
    {
        // 先分割, 后加上 .bmp 后缀
        // var picNames = findTarget.Split("|");
        // findTarget = Join("|", picNames.Select(x => x + ".bmp"));
        var picNames = target.Split("|");
        var sb = new StringBuilder();
        foreach (var picName in picNames)
        {
            sb.Append(picName);
            sb.Append(".bmp|");
        }

        target = sb.Remove(sb.Length - 1, 1).ToString(); // 去掉最后一个 "|"
        // 指定参数为引用类型
        ParameterModifier[] mods = new ParameterModifier[1];
        mods[0] = new ParameterModifier(10)
        {
            [8] = true,
            [9] = true
        };

        object[] args = new object[10];
        // // 直接复制图片坐标会出现找不到的情况, 所以这里修正一下
        args[0] = x1 > 0 ? x1 - 1 : 0;
        args[1] = y1 > 0 ? y1 - 1 : 0;
        args[2] = x2 + 1;
        args[3] = y2 + 1;
        args[4] = target;
        args[5] = deltaColor;
        args[6] = sim;
        args[7] = dir;

        int result = obj.InvokeMember("FindPic", BindingFlags.InvokeMethod, null,
            obj_object, args, mods, null, null) as int? ?? 0;
        x = (int)args[8];
        y = (int)args[9];

        if (result == -1) return Empty;
        DebugLog(searchResults: picNames[result], x: x, y: y);
        return picNames[result];
    }

    #endregion


    #region 找字

    // 全参数的找字并点击  ======================================================
    /// <summary>
    /// 寻找字库的字并点击, 点击无偏移, 默认延时 250 ms
    /// </summary>
    public async Task<(string retStr, int x, int y)> FindClickStrFastGetCoordAsync(string target, string color, int x1,
        int y1, int x2, int y2,
        Func<int, int, Task>? callBack = null, double sim = DefaultSim, int delay = DefaultClickDelay,
        CancellationToken ct = default)
    {
        var result = FindStrFast(target, color, x1, y1, x2, y2, out int x, out int y, sim);
        if (IsNullOrEmpty(result))
            return (Empty, x, y);
        callBack ??= _defaultClickAsync;
        await callBack(x, y);
        await Task.Delay(delay, ct);
        return (result, x, y);
    }


    // 使用找字结果的坐标执行指定的回调操作, 但不需要返回坐标给调用者  ======================================================
    public async Task<string> FindClickStrFastAsync(string target, string color, int x1, int y1, int x2, int y2,
        Func<int, int, Task>? callBack = null, double sim = DefaultSim, int delay = DefaultClickDelay,
        CancellationToken ct = default)
    {
        var result = FindStrFast(target, color, x1, y1, x2, y2, out int x, out int y, sim);
        if (IsNullOrEmpty(result))
            return result;
        callBack ??= _defaultClickAsync;
        await callBack(x, y);
        await Task.Delay(delay, ct);
        return result;
    }

    // 不使用找字结果的坐标, 执行指定的回调操作 ======================================================
    public async Task<string> FindClickStrFastAsync(string target, string color, int x1, int y1, int x2, int y2,
        Func<Task> callBack,
        double sim = DefaultSim, int delay = DefaultClickDelay, CancellationToken ct = default)
    {
        var result = FindStrFast(target, color, x1, y1, x2, y2, out _, out _, sim);
        if (IsNullOrEmpty(result))
            return result;
        await callBack();
        await Task.Delay(delay, ct);
        return result;
    }

    // 简单找字 ======================================================
    public string FindStrFast(string target, string color, int x1, int y1, int x2, int y2, double sim = DefaultSim)
        => FindStrFast(target, color, x1, y1, x2, y2, out _, out _, sim);

    /// <summary>
    /// 寻找指定区域内的文字
    /// </summary>
    /// <param name="target">待查找的字符串</param>
    /// <param name="color">颜色格式串</param>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="sim">相似度</param>
    /// <returns></returns>
    public string FindStrFast(string target, string color, int x1, int y1, int x2, int y2, out int x, out int y,
        double sim = DefaultSim)
    {
        var words = target.Split("|");

        object[] args = new object[9];
        ParameterModifier[] mods = new ParameterModifier[1];

        mods[0] = new ParameterModifier(9)
        {
            [7] = true,
            [8] = true
        };
        args[0] = x1 > 0 ? x1 - 1 : 0;
        args[1] = y1 > 0 ? y1 - 1 : 0;
        args[2] = x2 + 1;
        args[3] = y2 + 1;
        args[4] = target;
        args[5] = color;
        args[6] = sim;
        int result = obj.InvokeMember("FindStrFast", BindingFlags.InvokeMethod, null,
            obj_object, args, mods, null, null) as int? ?? -1;
        x = (int)args[7];
        y = (int)args[8];
        if (result == -1) return Empty;
        DebugLog(searchResults: words[result], x: x, y: y);
        return words[result];
    }

    #endregion
}