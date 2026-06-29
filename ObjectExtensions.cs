using dmfExtension.Core.Models;
using System.Globalization;

namespace dmfExtension.Core.Extensions;

/// <summary>
/// Object 类型的扩展方法，提供安全的类型转换
/// </summary>
public static class ObjectExtensions
{
    #region 转换为字符串

    /// <summary>
    /// 安全地将 object 转换为 string，转换失败时返回 null
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>转换后的字符串，失败时返回 null</returns>
    public static string? ToSafeString(this object? value)
    {
        return value?.ToString();
    }

    /// <summary>
    /// 安全地将 object 转换为 string，转换失败时返回指定的默认值
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <param name="defaultValue">转换失败时的默认值</param>
    /// <returns>转换后的字符串，失败时返回默认值</returns>
    public static string ToSafeString(this object? value, string defaultValue)
    {
        var result = value?.ToString();
        return string.IsNullOrEmpty(result) ? defaultValue : result;
    }

    /// <summary>
    /// 安全地将 object 转换为 string，并返回详细的转换结果
    /// </summary>
    public static ConversionResult<string> ToStringResult(this object? value)
    {
        try
        {
            if (value == null)
            {
                return ConversionResult<string>.Fail("原始值为 null", value);
            }

            var result = value.ToString() ?? string.Empty;
            return ConversionResult<string>.Success(result, value);
        }
        catch (Exception ex)
        {
            return ConversionResult<string>.Fail($"转换失败: {ex.Message}", value);
        }
    }

    #endregion

    #region 转换为布尔值

    /// <summary>
    /// 安全地将 object 转换为 bool
    /// 支持：true/false, 1/0, yes/no, y/n, on/off
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>转换后的 bool 值，失败时返回 false</returns>
    public static bool ToSafeBool(this object? value)
    {
        return ToSafeBool(value, false);
    }

    /// <summary>
    /// 安全地将 object 转换为 bool，转换失败时返回指定的默认值
    /// 支持：true/false, 1/0, yes/no, y/n, on/off
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <param name="defaultValue">转换失败时的默认值</param>
    /// <returns>转换后的 bool 值，失败时返回默认值</returns>
    public static bool ToSafeBool(this object? value, bool defaultValue)
    {
        var result = ToBoolResult(value);
        return result.IsSuccess ? result.Value : defaultValue;  // ✅ 修复：直接使用 result.Value
    }

    /// <summary>
    /// 安全地将 object 转换为 bool，并返回详细的转换结果
    /// 支持：true/false, 1/0, yes/no, y/n, on/off
    /// </summary>
    public static ConversionResult<bool> ToBoolResult(this object? value)
    {
        try
        {
            if (value == null)
            {
                return ConversionResult<bool>.Fail("原始值为 null", value);
            }

            // 如果是 bool 类型，直接返回
            if (value is bool boolValue)
            {
                return ConversionResult<bool>.Success(boolValue, value);
            }

            var str = value.ToString()?.Trim().ToLowerInvariant();

            if (string.IsNullOrEmpty(str))
            {
                return ConversionResult<bool>.Fail("字符串为空", value);
            }

            // 支持的 true 值
            var trueValues = new[] { "true", "1", "yes", "y", "on", "ok", "success" };
            var falseValues = new[] { "false", "0", "no", "n", "off", "fail", "error" };

            if (trueValues.Contains(str))
            {
                return ConversionResult<bool>.Success(true, value);
            }

            if (falseValues.Contains(str))
            {
                return ConversionResult<bool>.Success(false, value);
            }

            // 尝试用标准方法转换
            if (bool.TryParse(str, out var result))
            {
                return ConversionResult<bool>.Success(result, value);
            }

            return ConversionResult<bool>.Fail($"无法将 '{str}' 转换为 bool", value);
        }
        catch (Exception ex)
        {
            return ConversionResult<bool>.Fail($"转换失败: {ex.Message}", value);
        }
    }

    #endregion

    #region 转换为整数

    /// <summary>
    /// 安全地将 object 转换为 int
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <returns>转换后的 int 值，失败时返回 0</returns>
    public static int ToSafeInt(this object? value)
    {
        return ToSafeInt(value, 0);
    }

    /// <summary>
    /// 安全地将 object 转换为 int，转换失败时返回指定的默认值
    /// </summary>
    /// <param name="value">要转换的值</param>
    /// <param name="defaultValue">转换失败时的默认值</param>
    /// <returns>转换后的 int 值，失败时返回默认值</returns>
    public static int ToSafeInt(this object? value, int defaultValue)
    {
        var result = ToIntResult(value);
        return result.IsSuccess ? result.Value : defaultValue;  // ✅ 修复：直接使用 result.Value
    }

    /// <summary>
    /// 安全地将 object 转换为 int，并返回详细的转换结果
    /// </summary>
    public static ConversionResult<int> ToIntResult(this object? value)
    {
        try
        {
            if (value == null)
            {
                return ConversionResult<int>.Fail("原始值为 null", value);
            }

            if (value is int intValue)
            {
                return ConversionResult<int>.Success(intValue, value);
            }

            if (value is decimal decimalValue)
            {
                return ConversionResult<int>.Success(Convert.ToInt32(decimalValue), value);
            }

            if (value is double doubleValue)
            {
                return ConversionResult<int>.Success(Convert.ToInt32(doubleValue), value);
            }

            if (value is float floatValue)
            {
                return ConversionResult<int>.Success(Convert.ToInt32(floatValue), value);
            }

            var str = value.ToString()?.Trim();
            if (string.IsNullOrEmpty(str))
            {
                return ConversionResult<int>.Fail("字符串为空", value);
            }

            if (int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
            {
                return ConversionResult<int>.Success(result, value);
            }

            // 尝试解析带单位的值（如 "100px"）
            var numericStr = new string(str.TakeWhile(c => char.IsDigit(c) || c == '-').ToArray());
            if (!string.IsNullOrEmpty(numericStr) && int.TryParse(numericStr, out var numericResult))
            {
                return ConversionResult<int>.Success(numericResult, value);
            }

            return ConversionResult<int>.Fail($"无法将 '{str}' 转换为 int", value);
        }
        catch (Exception ex)
        {
            return ConversionResult<int>.Fail($"转换失败: {ex.Message}", value);
        }
    }

    #endregion

    #region 转换为可空整数

    /// <summary>
    /// 安全地将 object 转换为 int?，转换失败时返回 null
    /// </summary>
    public static int? ToSafeNullableInt(this object? value)
    {
        var result = ToIntResult(value);
        return result.IsSuccess ? result.Value : null;
    }

    #endregion

    #region 通用转换

    /// <summary>
    /// 安全地将 object 转换为指定类型
    /// </summary>
    /// <typeparam name="T">目标类型（支持 string, bool, int, long, decimal, double, float, DateTime）</typeparam>
    /// <param name="value">要转换的值</param>
    /// <param name="defaultValue">转换失败时的默认值</param>
    /// <returns>转换后的值，失败时返回默认值</returns>
    public static T? ToSafe<T>(this object? value, T? defaultValue = default)
    {
        try
        {
            if (value == null)
            {
                return defaultValue;
            }

            var targetType = typeof(T);

            // 如果是目标类型，直接返回
            if (value is T tValue)
            {
                return tValue;
            }

            // 处理可空类型
            var underlyingType = Nullable.GetUnderlyingType(targetType);
            if (underlyingType != null)
            {
                targetType = underlyingType;
            }

            // 特殊处理常见类型
            if (targetType == typeof(string))
            {
                return (T)(object)(value.ToString() ?? string.Empty);
            }

            if (targetType == typeof(bool))
            {
                return (T)(object)ToSafeBool(value);
            }

            if (targetType == typeof(int))
            {
                return (T)(object)ToSafeInt(value);
            }

            if (targetType == typeof(long))
            {
                return (T)(object)Convert.ToInt64(value);
            }

            if (targetType == typeof(decimal))
            {
                return (T)(object)Convert.ToDecimal(value);
            }

            if (targetType == typeof(double))
            {
                return (T)(object)Convert.ToDouble(value);
            }

            if (targetType == typeof(float))
            {
                return (T)(object)Convert.ToSingle(value);
            }

            if (targetType == typeof(DateTime))
            {
                return (T)(object)Convert.ToDateTime(value);
            }

            // 使用 Convert.ChangeType 作为后备
            var converted = Convert.ChangeType(value, targetType);
            return (T)converted;
        }
        catch
        {
            return defaultValue;
        }
    }

    #endregion
}