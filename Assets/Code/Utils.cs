using UnityEngine;


public static class Utils
{
    public static void DeleteChildren(this Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Object.Destroy(transform.GetChild(i).gameObject);
        }
    }
}
