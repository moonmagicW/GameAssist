namespace Automation.Extensions;

public static class MouseExtensions
{
    const int DefaultDelay = 250;

    /// <summary>
    /// 移动并点击, 默认加入随机偏移量
    /// </summary>
    /// <param name="ap"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="delay">默认为250ms延时</param>
    /// <param name="randomMax">默认为10, 当为0时不随机偏移</param>
    /// <param name="ct"></param>
    public static async Task MoveToClick(this AutoPlugin ap, int x, int y, int delay = DefaultDelay, int randomMax = 10,
        CancellationToken ct = default)
    {
        var next = randomMax < 0 ? Random.Shared.Next(randomMax, 0) : Random.Shared.Next(randomMax);
        ap.MoveTo(x + next, y + next);
        ap.LeftClick();
        await Task.Delay(delay, ct);
    }

    /// <summary>
    /// 移动并点击, 默认加入随机偏移量, xy分开随机
    /// </summary>
    /// <param name="ap"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="delay">默认为250ms延时</param>
    /// <param name="randomMax">(x,y) 两个最大偏移量, 如果是负数, 那么最大值为0</param>
    /// <param name="ct"></param>
    public static async Task MoveToClick(this AutoPlugin ap, int x, int y, (int, int) randomMax,
        int delay = DefaultDelay, CancellationToken ct = default)
    {
        var next1 = randomMax.Item1 < 0 ? Random.Shared.Next(randomMax.Item1, 0) : Random.Shared.Next(randomMax.Item1);
        var next2 = randomMax.Item2 < 0 ? Random.Shared.Next(randomMax.Item2, 0) : Random.Shared.Next(randomMax.Item2);
        ap.MoveTo(x + next1, y + next2);
        ap.LeftClick();
        await Task.Delay(delay, ct);
    }
}