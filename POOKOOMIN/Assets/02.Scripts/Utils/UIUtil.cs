using System;
using UnityEngine;

public static class UIUtil
{
    public static Texture2D Base64ToTexture2D(string base64)
    {
        if (string.IsNullOrEmpty(base64))
        {
            Debug.Log($"base64 : null");
            return null;
        }

        try
        {
            byte[] imageBytes = Convert.FromBase64String(base64);
            Texture2D tex = new Texture2D(2, 2);
            if (tex.LoadImage(imageBytes))
                return tex;
        }
        catch (Exception e)
        {
            Debug.LogError("Base64 decode error: " + e.Message);
        }
        return null;
    }
}
