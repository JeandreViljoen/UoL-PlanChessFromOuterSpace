using System.Collections.Generic;

/// <summary>
/// Extensions for iterating through linked list nodes.
/// </summary>
public static class LinkedListExtensions {
    /// <summary>
    /// Enumerate a linked list forwards from a specific node.
    /// </summary>
    /// <typeparam name="T">The type of the content of the list.</typeparam>
    /// <param name="node">The linked list node to start on.</param>
    /// <returns>A forward enumerable on the list.</returns>
    public static IEnumerable<T> Forward<T>(this LinkedListNode<T> node) {
        while (node != null) {
            yield return node.Value;
            node = node.Next;
        }
    }
    /// <summary>
    /// Enumerate a linked list backwards from a specific node.
    /// </summary>
    /// <typeparam name="T">The type of the content of the list.</typeparam>
    /// <param name="node">The linked list node to start on.</param>
    /// <returns>A backward enumerable on the list.</returns>
    public static IEnumerable<T> Backward<T>(this LinkedListNode<T> node) {
        while (node != null) {
            yield return node.Value;
            node = node.Previous;
        }
    }
}