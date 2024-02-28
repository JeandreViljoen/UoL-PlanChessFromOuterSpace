using System.Collections.Generic;

public static class LinkedListExtensions {
    public static IEnumerable<T> Forward<T>(this LinkedListNode<T> node) {
        while (node != null) {
            yield return node.Value;
            node = node.Next;
        }
    }
    public static IEnumerable<T> Backward<T>(this LinkedListNode<T> node) {
        while (node != null) {
            yield return node.Value;
            node = node.Previous;
        }
    }
}