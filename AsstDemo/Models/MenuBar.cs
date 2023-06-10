using System;

namespace AsstDemo.Models;
/// <summary>
/// 菜单栏
/// </summary>
/// <param name="Icon">菜单图标</param>
/// <param name="Title">菜单名称</param>
/// <param name="Target">目标视图</param>
public record MenuBar(string Icon, string Title, Type Target);