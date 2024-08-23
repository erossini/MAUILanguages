using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiLanguages.Extensions
{
    /// <summary>
    /// Extensions methods to ease dealing with collections.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Synchronizes the items of the source list with the items of the target list. The order of the items is ignored.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="target">The list to synchronize.</param>
        /// <param name="source">The items that should be in the target list.</param>
        public static void SynchronizeWith<T>(this ICollection<T> target, ICollection<T> source)
        {
            SynchronizeWith(target, source, null);
        }

        /// <summary>
        /// Synchronizes the items of the source list with the items of the target list. The order of the items is ignored.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="target">The list to synchronize.</param>
        /// <param name="source">The items that should be in the target list.</param>
        /// <param name="comparer">The comparer used to compare the items. If comparer is <c>null</c>, the default equality comparer is used to compare values.</param>
        public static void SynchronizeWith<T>(this ICollection<T> target, ICollection<T> source, IEqualityComparer<T>? comparer)
        {
            var removedItems = target.Except(source, comparer).ToArray();
            var addedItems = source.Except(target, comparer).ToArray();

            target.RemoveRange(removedItems);
            target.AddRange(addedItems);
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="target">The target list.</param>
        /// <param name="items">The collection whose elements should be added to the end of the list. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> items)
        {
            foreach (var i in items)
            {
                target.Add(i);
            }
        }

        /// <summary>
        /// Removes a range of elements from the list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="items">The items to remove.</param>
        public static void RemoveRange<T>(this ICollection<T> target, IEnumerable<T> items)
        {
            foreach (var i in items)
            {
                target.Remove(i);
            }
        }

        /// <summary>
        /// Gets the value from the dictionary, or the default value if no item with the specified key exists.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key to lookup.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// The value from the dictionary, or the default value if no item with the specified key exists.
        /// </returns>
        [return: NotNullIfNotNull(nameof(defaultValue))]
        public static TValue? GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue? defaultValue)
        {
            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }

        /// <summary>
        /// Gets the value from the dictionary, or the default value if no item with the specified key exists.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key to lookup.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// The value from the dictionary, or the default value if no item with the specified key exists.
        /// </returns>
        [return: NotNullIfNotNull(nameof(defaultValue))]
        public static TValue? GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue? defaultValue)
            where TKey : notnull
        {
            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }

        /// <summary>
        /// Gets the value from the dictionary, or the default value if no item with the specified key exists.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key to lookup.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// The value from the dictionary, or the default value if no item with the specified key exists.
        /// </returns>
        [return: NotNullIfNotNull(nameof(defaultValue))]
        public static TValue? GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue? defaultValue)
        {
            return dictionary.TryGetValue(key, out var value) ? value : defaultValue;
        }

        /// <summary>
        /// Gets the value associated with the specified key from the <paramref name="dictionary"/>, or creates a new entry if the dictionary does not contain a value associated with the key.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="generator">The generator function called when a new value needs to be created.</param>
        /// <returns>The element with the specified key.</returns>
        public static TValue ForceValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> generator)
        {
            if (dictionary.TryGetValue(key, out var result))
                return result;

            result = generator(key);

            dictionary.Add(key, result);

            return result;
        }
    }
}