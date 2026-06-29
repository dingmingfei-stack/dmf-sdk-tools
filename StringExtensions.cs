using dmfExtension.Core.Models;
using System.Text.RegularExpressions;

namespace dmfExtension.Core.Extensions;

/// <summary>
/// String 类型的扩展方法，提供安全的字符串分割功能
/// </summary>
public static class StringExtensions
{
    #region 分割为 List<string>

    /// <summary>
    /// 安全地将字符串按指定分隔符分割为 List&lt;string&gt;
    /// </summary>
    /// <param name="str">要分割的字符串</param>
    /// <param name="separator">分隔符，默认为逗号</param>
    /// <param name="options">分割选项</param>
    /// <returns>分割后的 List&lt;string&gt;，输入为 null 或空时返回空列表</returns>
    public static List<string> ToSafeList(this string? str, 
        string separator = ",", 
        StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
    {
        if (string.IsNullOrEmpty(str))
        {
            return new List<string>();
        }

        var result = str.Split(separator, options);
        return result.ToList();
    }

    /// <summary>
    /// 安全地将字符串按多个分隔符分割为 List&lt;string&gt;
    /// </summary>
    public static List<string> ToSafeList(this string? str, 
        string[] separators, 
        StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
    {
        if (string.IsNullOrEmpty(str) || separators == null || separators.Length == 0)
        {
            return new List<string>();
        }

        var result = str.Split(separators, options);
        return result.ToList();
    }

    /// <summary>
    /// 安全地将字符串按分隔符分割为 List&lt;string&gt;，支持自定义字符过滤
    /// </summary>
    /// <param name="str">要分割的字符串</param>
    /// <param name="separator">分隔符</param>
    /// <param name="filter">过滤函数，返回 true 表示保留该元素</param>
    /// <param name="options">分割选项</param>
    /// <returns>过滤后的 List&lt;string&gt;</returns>
    public static List<string> ToSafeList(this string? str, 
        string separator, 
        Func<string, bool>? filter,
        StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
    {
        var list = ToSafeList(str, separator, options);
        return filter == null ? list : list.Where(filter).ToList();
    }

    #endregion

    #region 分割为 List<T>

    /// <summary>
    /// 安全地将字符串按分隔符分割并转换为指定类型的 List（值类型）
    /// </summary>
    /// <typeparam name="T">目标类型（值类型，如 int, bool, decimal, double, float, long, DateTime）</typeparam>
    /// <param name="str">要分割的字符串</param>
    /// <param name="separator">分隔符，默认为逗号</param>
    /// <param name="options">分割选项</param>
    /// <returns>转换后的 List&lt;T&gt;，转换失败的项会被跳过</returns>
    public static List<T> ToSafeList<T>(this string? str, 
        string separator = ",", 
        StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
        where T : struct
    {
        var result = new List<T>();
        if (string.IsNullOrEmpty(str))
        {
            return result;
        }

        var items = str.Split(separator, options);
        foreach (var item in items)
        {
            // ✅ 使用 Convert.ChangeType 直接转换，避免泛型可空问题
            try
            {
                var converted = Convert.ChangeType(item, typeof(T));
                if (converted != null)
                {
                    result.Add((T)converted);
                }
            }
            catch
            {
                // 转换失败，跳过该项
                continue;
            }
        }

        return result;
    }

    /// <summary>
    /// 安全地将字符串按分隔符分割并转换为指定类型的 List（使用自定义转换函数）
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="str">要分割的字符串</param>
    /// <param name="converter">自定义转换函数</param>
    /// <param name="separator">分隔符，默认为逗号</param>
    /// <param name="options">分割选项</param>
    /// <returns>转换后的 List&lt;T&gt;，转换失败的项会被跳过</returns>
    public static List<T> ToSafeList<T>(this string? str,
        Func<string, T> converter,
        string separator = ",",
        StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
    {
        var result = new List<T>();
        if (string.IsNullOrEmpty(str) || converter == null)
        {
            return result;
        }

        var items = str.Split(separator, options);
        foreach (var item in items)
        {
            try
            {
                var converted = converter(item);
                result.Add(converted);
            }
            catch
            {
                // 转换失败，跳过该项
                continue;
            }
        }

        return result;
    }

    #endregion

    #region 高级分割

    /// <summary>
    /// 安全地将字符串按分隔符分割，并返回包含转换结果的详细对象
    /// </summary>
    public class SplitResult<T>
    {
        public List<T> SuccessItems { get; set; } = new();
        public List<string> FailedItems { get; set; } = new();
        public bool HasFailures => FailedItems.Any();
        public int SuccessCount => SuccessItems.Count;
        public int FailedCount => FailedItems.Count;
    }

    /// <summary>
    /// 安全地将字符串按分隔符分割并转换，返回详细结果（包含成功和失败的项）
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="str">要分割的字符串</param>
    /// <param name="converter">自定义转换函数</param>
    /// <param name="separator">分隔符</param>
    /// <param name="options">分割选项</param>
    /// <returns>包含成功项和失败项的详细结果</returns>
    public static SplitResult<T> ToSplitResult<T>(this string? str,
        Func<string, T> converter,
        string separator = ",",
        StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
    {
        var result = new SplitResult<T>();
        if (string.IsNullOrEmpty(str) || converter == null)
        {
            return result;
        }

        var items = str.Split(separator, options);
        foreach (var item in items)
        {
            try
            {
                var converted = converter(item);
                result.SuccessItems.Add(converted);
            }
            catch
            {
                result.FailedItems.Add(item);
            }
        }

        return result;
    }

    /// <summary>
    /// 使用正则表达式分割字符串
    /// </summary>
    public static List<string> ToSafeListRegex(this string? str, 
        string pattern, 
        RegexOptions options = RegexOptions.None)
    {
        if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(pattern))
        {
            return new List<string>();
        }

        var regex = new Regex(pattern, options);
        var matches = regex.Matches(str);
        var result = new List<string>();
        foreach (Match match in matches)
        {
            if (match.Success && !string.IsNullOrEmpty(match.Value))
            {
                result.Add(match.Value);
            }
        }
        return result;
    }

    #endregion
}