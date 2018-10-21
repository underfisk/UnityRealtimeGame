using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Newtonsoft.Json.Linq;

public static class JsonHelper
{
    /// <summary>
    /// Returns the result of checking the string given
    /// </summary>
    /// <param name="js"></param>
    /// <returns></returns>
    public static bool isJson(string js)
    {
        //we just check if starts with { and ends with }
        if (string.IsNullOrWhiteSpace(js)) return false;

        var c = js.Trim();
        if ((c.StartsWith("{") && c.EndsWith("}")) || //For object
        (c.StartsWith("[") && c.EndsWith("]"))) //For array
            return true;

        return false;//false by default
    }
}
