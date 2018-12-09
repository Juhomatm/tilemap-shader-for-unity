using UnityEngine;
using System.IO;

public static class WritePng
{
    public static void WriteTexture(Texture2D texture, string fileName)
    {
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + string.Format("/../{0}.png", fileName), bytes);
    }
}
