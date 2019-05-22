using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Randomizer<T>
{
    public List<T> items = new List<T>();

    private int nextItemIndex = 0;
    
    public Randomizer(List<T> items)
    {
        this.items = items;
        Shuffle();
    }
    
    public Randomizer(T[] items)
    {
        this.items.AddRange(items);
        Shuffle();
    }
    
    public void Shuffle()
    {
        HelperUtilities.Rearrange(items);

        nextItemIndex = 0;
    }
    
    public T GetRandomItem()
    {
        if (items.Count == 0)
        {
            return default(T);
        }

        if (nextItemIndex < 0)
        {
            Shuffle();
        }

        if (nextItemIndex >= items.Count)
        {
            Shuffle();
        }

        return items[nextItemIndex++];
    }
}