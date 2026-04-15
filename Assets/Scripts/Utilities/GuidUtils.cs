using System;

public static class GuidUtils
{
    public static string GetGuidOfLength(int length)
    {
        string guid = Guid.NewGuid().ToString().Replace("-", "").Substring(0, length);
        return guid;
    }
}