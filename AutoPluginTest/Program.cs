// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Automation;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    // .WriteTo.Async(w=>w.Console())
    .WriteTo.Console()
    .CreateLogger();

var ap = new AutoPlugin();
ap.SetPath(@"D:\code\game");
ap.SetDict(0, "font.txt");
var bindResult = ap.BindWindow(134206, "gdi", "windows3","windows", 0);
if (bindResult != 1)
{
    Log.Error("绑定窗口失败, {Error}", ap.GetLastError());
}
else
{
    Log.Information("绑定窗口成功");
}
// var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
// var ocrInFile = ap.OcrInFile(0, 0, 101, 80, @"e:/01.bmp", "e9e9ea-404040", 0.9);
ap.FindStrFastS(0,0,1600,900, "金铲铲之战", "e9e9ea-404040", 0.9, out var x, out var y);
var timestamp2 = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - timestamp;
Log.Information("timestamp: {Timestamp}", timestamp2);
Log.Information("ocr: {X} {Y}", x, y);
// 保存图片
ap.Capture(0, 0, 1600, 900, "e:/02.bmp");