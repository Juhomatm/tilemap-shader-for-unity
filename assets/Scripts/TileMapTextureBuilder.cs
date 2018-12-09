using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class TileMapTextureBuilder : MonoBehaviour
{
    public Material Material;
    public int[] SequenceStartIndices;
    public float TileMapDelta;

    void Start()
    {
        Material oldMaterial = Material;
        Material = Instantiate(Material);

        Texture2D oldTexture = (Texture2D)oldMaterial.GetTexture("_MainTex");
        ColorMap oldMap = new ColorMap(oldTexture.GetPixels(), oldTexture.width);
        Color[] oldPixels = oldTexture.GetPixels();

        int tileCount = oldMaterial.GetInt("_TilesetHorizontalCount") * oldMaterial.GetInt("_TilesetVerticalCount");
        TileMapDelta = 255 * (1f / tileCount);

        int textureWidth = oldTexture.width;
        int textureHeight = oldTexture.height;
        Color[] newPixels = new Color[textureWidth * textureHeight];
        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                Color currentColor = oldPixels[x + textureWidth * y];
                int index = ColorToIndex(currentColor, tileCount);
                if (SequenceStartIndices != null && SequenceStartIndices.Contains(index))
                {
                    int indexShift = SelectTile(oldMap, x, y);
                    //Debug.LogFormat("({0},{1}): {2} + {3} = {4}", x, y, index, indexShift, index + indexShift);
                    if (index == 3)
                    {
                        index = 18;
                    }
                    else
                    {
                        index += indexShift;
                    }
                    currentColor = IndexToColor(index, tileCount);
                }
                newPixels[x + textureWidth * y] = currentColor;
            }
        }
        Texture2D newTexture = new Texture2D(textureWidth, textureHeight);
        newTexture.filterMode = FilterMode.Point;
        newTexture.SetPixels(newPixels);
        newTexture.Apply();
        Material.SetTexture("_MainTex", newTexture);

        foreach (MeshRenderer meshRenderer in FindObjectsOfType<MeshRenderer>())
        {
            if (meshRenderer.sharedMaterial == oldMaterial)
            {
                meshRenderer.sharedMaterial = Material;
            }
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            WritePaletteFile(Material.GetInt("_TilesetHorizontalCount"), Material.GetInt("_TilesetVerticalCount"));
        }
    }

    private static void WritePaletteFile(int tilesetHorizontalCount, int tilesetVerticalCount)
    {
        int tileCount = tilesetHorizontalCount * tilesetVerticalCount;
        Color[] pixels = new Color[tileCount];
        for (int x = 0; x < tilesetHorizontalCount; x++)
        {
            for (int y = 0; y <tilesetVerticalCount; y++)
            {
                int currentY = (tilesetVerticalCount - 1) - y;
                pixels[x + (tilesetHorizontalCount * y)] = IndexToColor(x + (tilesetHorizontalCount * currentY), tileCount);
            }
        }

        Texture2D texture = new Texture2D(tilesetHorizontalCount, tilesetVerticalCount);
        texture.SetPixels(pixels);
        texture.Apply();
        WritePng.WriteTexture(texture, "tilemapPalette");
        Debug.Log("Palette file generated");
    }
    
    private static int[] ParseIntPairString(string pairString)
    {
        pairString = pairString
            .Replace("(", "")
            .Replace(")", "")
            .Replace("{", "")
            .Replace("}", "")
            .Replace("[", "")
            .Replace("]", "");

        string[] numberStrings = pairString.Split(',', ';');
        if (numberStrings.Length != 0)
        {
            return null;
        }
        int[] numbers = new int[2];
        numbers[0] = int.Parse(numberStrings[0]);
        numbers[1] = int.Parse(numberStrings[1]);
        return numbers;
    }

    private static int SelectTile(ColorMap map, int x, int y)
    {
        Color? color = map.Get(x, y);
        int bitMask = 0;

        if (map.Get(x, y + 1) == color)
        {
            bitMask = bitMask | 1;
        }
        if (map.Get(x + 1, y) == color)
        {
            bitMask = bitMask | 1 << 1;
        }
        if (map.Get(x, y - 1) == color)
        {
            bitMask = bitMask | 1 << 2;
        }
        if (map.Get(x - 1, y) == color)
        {
            bitMask = bitMask | 1 << 3;
        }
        return bitMask;
    }

    private static int ColorToIndex(Color color, int tileCount)
    {
        return Mathf.FloorToInt(color.r * tileCount);
    }

    private static Color IndexToColor(int index, int tileCount)
    {
        bool evenIndex = index % 2 == 0;
        float floatIndex = index + 0.5f;
        float encodedIndex = floatIndex / tileCount;
        if (evenIndex)
        {
            return new Color(encodedIndex, 0, 0, 1f);
        }
        else
        {
            return new Color(encodedIndex, 0, encodedIndex, 1f);
        }
    }
}

class ColorMap
{
    int _mapWidth;
    Color[] _colorArray;
    public ColorMap(Color[] colorArray, int mapWidth)
    {
        _colorArray = colorArray;
        _mapWidth = mapWidth;
    }

    public Color? Get(int x, int y)
    {
        int index = x + _mapWidth * y;
        if (index < 0 || index >= _colorArray.Length)
        {
            return null;
        }
        return _colorArray[index];
    }

    public void Set(int x, int y, Color value)
    {
        int index = x + _mapWidth * y;
        if (index < 0 || index >= _colorArray.Length)
        {
            return;
        }
        _colorArray[index] = value;
    }
}