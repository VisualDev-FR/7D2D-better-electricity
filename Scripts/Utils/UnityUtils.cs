using System.Collections.Generic;
using UnityEngine;

public static class UnityUtils
{
    public static IEnumerable<Transform> FindByTag(Transform transform, string tag)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);

            if (child.CompareTag(tag))
                yield return child;

            foreach (var rChild in FindByTag(child, tag))
                yield return rChild;
        }
    }

    public static Transform FindByName(Transform transform, string name)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);

            if (child.name.ToLower() == name.ToLower())
                return child;
        }

        throw new System.Exception($"No transform found with name '{name}'");
    }
}