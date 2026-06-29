namespace dmfExtension.Core.Models;

/// <summary>
/// 转换结果封装类，用于返回转换是否成功以及转换后的值
/// </summary>
/// <typeparam name="T">目标类型</typeparam>
public class ConversionResult<T>
{
    /// <summary>
    /// 是否转换成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 转换后的值（仅在成功时有值）
    /// </summary>
    public T? Value { get; set; }

    /// <summary>
    /// 转换失败时的错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 原始值
    /// </summary>
    public object? OriginalValue { get; set; }

    /// <summary>
    /// 创建一个成功的转换结果
    /// </summary>
    public static ConversionResult<T> Success(T value, object? originalValue = null)
    {
        return new ConversionResult<T>
        {
            IsSuccess = true,
            Value = value,
            OriginalValue = originalValue,
            ErrorMessage = null
        };
    }

    /// <summary>
    /// 创建一个失败的转换结果
    /// </summary>
    public static ConversionResult<T> Fail(string errorMessage, object? originalValue = null)
    {
        return new ConversionResult<T>
        {
            IsSuccess = false,
            Value = default,
            OriginalValue = originalValue,
            ErrorMessage = errorMessage
        };
    }

    /// <summary>
    /// 隐式转换为目标类型（直接取值，转换失败时返回默认值）
    /// </summary>
    public static implicit operator T?(ConversionResult<T> result)
    {
        return result.IsSuccess ? result.Value : default;
    }
}