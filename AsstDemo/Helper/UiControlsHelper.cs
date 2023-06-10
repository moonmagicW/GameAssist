using System.Collections.Generic;
using System.Windows;

namespace AsstDemo.Helper;

public class UiControlsHelper
{
    /// <summary>
    /// 拿到指定类型的子控件
    /// </summary>
    /// <param name="parent"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<T> Descendants<T>(DependencyObject? parent) where T : DependencyObject
    {
        if (parent == null) yield break;
        foreach (object child in LogicalTreeHelper.GetChildren(parent))
        {
            if (child is T tChild)
            {
                yield return tChild;
            }
            if (child is DependencyObject dChild)
            {
                foreach (T grandChild in Descendants<T>(dChild))
                {
                    yield return grandChild;
                }
            }
        }
    }
}