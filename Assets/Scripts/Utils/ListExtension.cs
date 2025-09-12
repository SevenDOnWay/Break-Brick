using System.Collections.Generic;
using System.Linq;
using System;

public static class ListExtension
{
    private static Random rng = new Random();

    public static List<T> GetRandomElements<T>(this List<T> list, int n)
    {
        return list.OrderBy(x => rng.Next()).Take(n).ToList();
    }
}

