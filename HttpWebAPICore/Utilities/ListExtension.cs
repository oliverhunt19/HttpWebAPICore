namespace HttpWebAPICore.Utilities;

/// <summary>
/// List Extensions.
/// </summary>
public static class ListExtension
{
    /// <summary>
    /// Adds a <see cref="KeyValuePair{TKey,TValue}"/> to generic <see cref="List{KeyValuePair}"/>.
    /// </summary>
    /// <param name="list">The <see cref="List{T}"/> to add a <see cref="KeyValuePair{TKey,TValue}"/> to.</param>
    /// <param name="key">The <see cref="string"/> key.</param>
    /// <param name="value">The <see cref="string"/> value.</param>
    public static void Add(this IList<KeyValuePair<string, string?>> list, string key, string? value = null)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));

        if (key == null)
            throw new ArgumentNullException(nameof(key));

        var parameter = new KeyValuePair<string, string?>(key, value);

        list.Add(parameter);
    }
    public static bool AddNullable(this IList<KeyValuePair<string, string?>> list, string key, string? value)
    {
        if (value == null)
        {
            return false;
        }
        list.Add(key, value);
        return true;
    }


    public static bool AddNullable(this IList<KeyValuePair<string, string?>> list, string key, Enum? value)
    {
        if (value == null)
        {
            return false;
        }
        list.AddNullable(key, value?.GetStringValue());
        return true;
    }
    public static bool AddNullableKey(this IList<KeyValuePair<string, string?>> list, string? key)
    {
        if (key == null)
        {
            return false;
        }
        list.Add(key, null);
        return true;
    }

    public static bool AddNullableKey(this IList<KeyValuePair<string, string?>> list, Enum? key)
    {
        if (key == null)
        {
            return false;
        }
        list.Add(key.GetStringValue()!, null);
        return true;
    }

    public static void AddBool(this IList<KeyValuePair<string, string?>> list, string key, string? value, bool addOk)
    {
        if (addOk)
        {
            list.Add(key, value);
        }
    }
}