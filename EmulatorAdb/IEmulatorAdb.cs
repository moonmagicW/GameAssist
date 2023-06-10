using EmulatorAdb.Model;

namespace EmulatorAdb;

/// <summary>
/// 模拟器提供的模拟器特有ADB命令
/// </summary>
public interface IEmulatorAdb
{
    /// <summary>
    /// 模拟器信息
    /// </summary>
    EmulatorInfo EmulatorInfo { get; set; }

    /// <summary>
    /// 模拟器安装路径
    /// </summary>
    string InstallPath { get; set; }

    /// <summary>
    /// 模拟器特有ADB执行程序全路径
    /// </summary>
    string AdbPath { get; set; }

    /// <summary>
    /// 取模拟器安装目录
    /// </summary>
    /// <returns></returns>
    string GetInstallPath();

    /// <summary>
    /// 取ADB路径
    /// </summary>
    /// <returns></returns>
    string GetAdbPath();

    /// <summary>
    /// 设置ADB路径
    /// </summary>
    /// <param name="adbName">adb名称, 不带路径</param>
    void SetAdbPath(string adbName);

    /// <summary>
    /// 设置模拟器目录, 会自动设置ADB路径
    /// </summary>
    /// <param name="path">模拟器路径</param>
    void SetEmulatorPath(string path);

    /// <summary>
    /// 取全部模拟器信息
    /// </summary>
    /// <returns></returns>
    Task<List<EmulatorInfo>> GetAllEmulatorInfo();
    

    /// <summary>
    /// 取当前模拟器信息
    /// </summary>
    /// <returns></returns>
    Task<EmulatorInfo> GetEmulatorInfo();


    /// <summary>
    /// 启动应用
    /// </summary>
    /// <param name="packageName">应用包名</param>
    /// <returns></returns>
    Task RunApp(string packageName);

    /// <summary>
    /// 关闭应用
    /// </summary>
    /// <param name="packageName">应用包名</param>
    /// <returns></returns>
    Task CloseApp(string packageName);

    /// <summary>
    /// 应用是否已启动
    /// </summary>
    /// <param name="packageName">应用包名</param>
    /// <returns></returns>
    Task<bool> IsRunApp(string packageName);

    /// <summary>
    /// 启动模拟器
    /// </summary>
    /// <returns></returns>
    Task StartEmulator();

    /// <summary>
    /// 关闭模拟器
    /// </summary>
    /// <returns></returns>
    Task CloseEmulator();

    /// <summary>
    /// 更新模拟器信息
    /// </summary>
    /// <returns></returns>
    Task UpdateEmulatorInfo();

    /// <summary>
    /// 关闭所有模拟器
    /// </summary>
    /// <returns></returns>
    Task CloseAllEmulator();

    /// <summary>
    /// 重启模拟器
    /// </summary>
    /// <returns></returns>
    Task RebootEmulator();

    /// <summary>
    /// 删除模拟器
    /// </summary>
    /// <returns></returns>
    Task RemoveEmulator();

    /// <summary>
    /// 往模拟器发送文本
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    Task Input(string content);

    /// <summary>
    /// 复制模拟器
    /// </summary>
    /// <param name="index">模拟器索引</param>
    /// <param name="newTitle">指定复制出的模拟器的标题, 可空参数</param>
    /// <returns></returns>
    Task CopyEmulator(int index, string newTitle);


    /// <summary>
    /// 修改模拟器配置
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="dpi"></param>
    /// <returns></returns>
    Task<string> ModifyConfig(string width, string height, string dpi);


    #region 鼠标

    /// <summary>
    /// 鼠标滑动
    /// </summary>
    public Task MouseSwipe(int x1, int y1, int x2, int y2, int time);

    #endregion
}