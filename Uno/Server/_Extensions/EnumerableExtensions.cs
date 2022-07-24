namespace Uno.Server;

/// <summary>
/// Extension methods for <see cref="IEnumerable{T}"/>.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Returns the first element that is valid according to <paramref name="selector"/>, or <see langword="null"/> if none of the elements are valid.
    /// </summary>
    /// <typeparam name="T">The type of elements.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="selector">The selector.</param>
    /// <returns>The first element matched by <paramref name="selector"/> or <see langword="null"/> if none is matched.</returns>
    public static T? FirstOrNull<T>(this IEnumerable<T> source, Func<T, bool> selector)
        where T: unmanaged
    {
        foreach (var element in source)
        {
            if (selector(element))
            {
                return element;
            }
        }

        return null;
    }
}
