using UnityEngine;
using System.Collections;

public class CreateTexture : MonoBehaviour
{
    public Texture2D Texture;
    public MeshRenderer Target;

    void Start()
    {
        Texture2D newTexture = new Texture2D(101, 101);
        newTexture.SetPixels(1, 1, 100, 100, Texture.GetPixels(640 - 101, 480 - 101, 100, 100));
        newTexture.Apply();
        Target.material.mainTexture = newTexture;
    }

    void Update()
    {

    }
}
