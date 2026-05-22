using UnityEngine;

public static class PlaceholderGraphics
{
    public static Sprite CreateSquare(Color color, int size = 32)
    {
        var tex = new Texture2D(size, size) { filterMode = FilterMode.Point };
        var pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), Vector2.one * 0.5f, 32f);
    }

    public static Sprite CreateCircle(Color color, int size = 32)
    {
        var tex = new Texture2D(size, size) { filterMode = FilterMode.Bilinear };
        tex.alphaIsTransparency = true;
        var pixels = new Color[size * size];
        float c = size * 0.5f, r = c - 0.5f;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dx = x - c + 0.5f, dy = y - c + 0.5f;
                pixels[y * size + x] = dx * dx + dy * dy <= r * r ? color : Color.clear;
            }
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), Vector2.one * 0.5f, 32f);
    }

    public static Sprite CreateDiamond(Color color, int size = 32)
    {
        var tex = new Texture2D(size, size) { filterMode = FilterMode.Point };
        tex.alphaIsTransparency = true;
        var pixels = new Color[size * size];
        float c = size * 0.5f;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dx = Mathf.Abs(x - c + 0.5f), dy = Mathf.Abs(y - c + 0.5f);
                pixels[y * size + x] = dx + dy <= c ? color : Color.clear;
            }
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), Vector2.one * 0.5f, 32f);
    }
}
