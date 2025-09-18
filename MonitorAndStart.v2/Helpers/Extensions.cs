using System;
using System.Collections.Generic;

namespace MonitorAndStart.v2.Helpers;

public static class ListExtensions
{
    public static void Move<T>(this List<T> list, T item, int newIndex)
    {
        if (item == null) return;

        int oldIndex = list.IndexOf(item);
        if (oldIndex == -1 || oldIndex == newIndex) return;

        // Clamp the new index to valid range
        newIndex = Math.Max(0, Math.Min(newIndex, list.Count - 1));

        // Remove the item before re-inserting
        list.RemoveAt(oldIndex);

        // No adjustment to newIndex needed — just insert
        list.Insert(newIndex, item);
    }

    public static void MoveUp<T>(this List<T> list, T item)
    {
        int index = list.IndexOf(item);
        if (index > 0)
        {
            list.Move(item, index - 1);
        }
    }

    public static void MoveDown<T>(this List<T> list, T item)
    {
        int index = list.IndexOf(item);
        if (index > -1 && index < list.Count - 1)
        {
            list.Move(item, index + 1);
        }
    }
}