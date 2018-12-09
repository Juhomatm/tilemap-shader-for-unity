using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SpriteCollection : MonoBehaviour
{
    public Texture2D[] Tiles;
    int startingX;
    int startingY;
    public int TileWidth;
    public int TileHeight;
    public int tileCount;
    public Texture2D SourceTexture;
    public MeshRenderer Target;

    void Start()
    {
        List<Texture2D> textures = new List<Texture2D>();
        for (int i = 0; i < tileCount; i++)
        {
            textures.Add(ExtractTile(SourceTexture, i));
        }
        Tiles = textures.ToArray();
        Target.material.mainTexture = Tiles.Last();
    }

    private Texture2D ExtractTile(Texture2D sourceTexture, int tileIndex)
    {
        int textureWidth = sourceTexture.width;
        int textureHeight = sourceTexture.height;
        int pixelsToSkip = tileIndex * TileWidth;
        int x = startingX + pixelsToSkip + 1;
        int y = startingY + 1;

        int i = 0;
        Texture2D newTexture = new Texture2D(TileWidth, TileHeight);
        Color[] pixels = sourceTexture.GetPixels(1 + (tileIndex * (TileWidth + 1)), textureHeight - (TileHeight + 1), TileWidth,  TileHeight);
        newTexture.SetPixels(0, 0, TileWidth, TileHeight, pixels);
        newTexture.Apply();
        return newTexture;
    }
}
