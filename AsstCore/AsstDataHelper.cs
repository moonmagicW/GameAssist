using DynamicData;

namespace AsstCore;

public static class AsstDataHelper
{
    /// <summary>
    /// 核心数据源
    /// </summary>
    public static readonly SourceCache<Asst, int> AsstData = new(x => x.Id);

    /// <summary>
    /// 在日志中指定的属性名
    /// </summary>
    public const string LogProperty = "Asst";
}