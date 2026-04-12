using System.IO;
using UnityEditor;
using UnityEngine;

public static class LoginAssetGenerator
{
    [MenuItem("PlayMeow/Generate Login Sprites")]
    public static void GenerateSprites()
    {
        int w = 128, h = 64, radius = 28;

        string[] names = { "pill-dark-border", "pill-dark", "pill-pink", "pill-white" };
        Color[] fills = {
            new Color(0x3B/255f, 0x3B/255f, 0x3B/255f, 1f),
            new Color(0x3B/255f, 0x3B/255f, 0x3B/255f, 1f),
            new Color(1.00f,     0x67/255f, 0x8F/255f, 1f),
            Color.white
        };
        Color[] borders = {
            new Color(1.00f, 0x67/255f, 0x8F/255f, 1f),
            Color.clear, Color.clear, Color.clear
        };
        int[] bWidths = { 4, 0, 0, 0 };

        string folder = Path.Combine(Application.dataPath, "Arts/Images/Login");
        Directory.CreateDirectory(folder);

        for (int s = 0; s < names.Length; s++)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            var px = new Color[w * h];
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float cx = Mathf.Clamp(x, radius, w - 1 - radius);
                    float cy = Mathf.Clamp(y, radius, h - 1 - radius);
                    float dist = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));
                    if (dist > radius) { px[y * w + x] = Color.clear; continue; }
                    if (bWidths[s] > 0 && dist > radius - bWidths[s]) { px[y * w + x] = borders[s]; continue; }
                    px[y * w + x] = fills[s];
                }
            }

            tex.SetPixels(px);
            tex.Apply();

            string file = Path.Combine(folder, names[s] + ".png");
            File.WriteAllBytes(file, tex.EncodeToPNG());
            UnityEngine.Object.DestroyImmediate(tex);

            string assetPath = "Assets/Arts/Images/Login/" + names[s] + ".png";
            AssetDatabase.ImportAsset(assetPath);
            var imp = (TextureImporter)AssetImporter.GetAtPath(assetPath);
            if (imp != null)
            {
                imp.textureType = TextureImporterType.Sprite;
                imp.spriteImportMode = SpriteImportMode.Single;
                imp.spriteBorder = new Vector4(28, 28, 28, 28);
                imp.mipmapEnabled = false;
                imp.filterMode = FilterMode.Bilinear;
                imp.SaveAndReimport();
            }
        }
        AssetDatabase.Refresh();
        Debug.Log("[LoginAssetGenerator] 4 pill sprites generated.");
    }
}