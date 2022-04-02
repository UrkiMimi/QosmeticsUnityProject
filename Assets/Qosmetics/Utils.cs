using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

static public class Utils
{
    public static void AddOrSetTextComponent(this GameObject obj, string text)
    {
        var txt = obj.GetComponent<Text>();
        if (!txt)
            txt = obj.AddComponent<Text>();
        txt.text = text;
        txt.fontSize = 0;
    }
}
