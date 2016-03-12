using System;
using System.Collections.Generic;
using NSubstitute;

internal class Args
{
    public static bool Boolean
    {
        get { return Arg.Any<bool>(); }
    }

    public static Uri Uri
    {
        get { return Arg.Any<Uri>(); }
    }

    public static object Object
    {
        get { return Arg.Any<object>(); }
    }

    public static string String
    {
        get { return Arg.Any<string>(); }
    }

    public static Dictionary<string, string> EmptyDictionary
    {
        get { return Arg.Is<Dictionary<string, string>>(d => d.Count == 0); }
    }
}