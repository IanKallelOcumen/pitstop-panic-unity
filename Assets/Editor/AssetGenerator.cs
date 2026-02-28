using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class AssetGenerator : EditorWindow
{
    const string ArtPath = "Assets/Art";
    const string ModelsPath = "Assets/Models";
    const string DownloadsPath = "d:\\downloads";

    [MenuItem("Pitstop Panic/Regenerate Placeholders")]
    public static void GenerateAll()
    {
        if (!Directory.Exists(ArtPath)) Directory.CreateDirectory(ArtPath);

        // Vehicles
        CreateTexture("CarBroken", 512, 256, new Color(0.8f, 0.2f, 0.2f)); // Red Car
        CreateTexture("CarFixed", 512, 256, new Color(0.2f, 0.8f, 0.2f)); // Green Car
        
        CreateTexture("ScooterBroken", 256, 256, new Color(0.8f, 0.4f, 0.1f)); // Orange Scooter
        CreateTexture("ScooterFixed", 256, 256, new Color(0.2f, 0.8f, 0.4f)); // Green Scooter

        // Tools (Different Colors for identification)
        CreateTexture("Tool_Wrench", 128, 128, new Color(0.6f, 0.6f, 0.7f)); // Grey
        CreateTexture("Tool_Jack", 128, 128, new Color(0.9f, 0.2f, 0.2f)); // Red
        CreateTexture("Tool_OilCan", 128, 128, new Color(0.9f, 0.8f, 0.1f)); // Yellow
        CreateTexture("Tool_Screwdriver", 128, 128, new Color(0.2f, 0.8f, 0.2f)); // Green
        CreateTexture("Tool_Funnel", 128, 128, Color.white);
        CreateTexture("Tool_Multimeter", 128, 128, new Color(1f, 0.5f, 0f)); // Orange

        // Backgrounds
        CreateTexture("GarageBg", 1080, 1920, new Color(0.15f, 0.15f, 0.18f)); // Dark Blue-Grey
        CreateTexture("VictoryBg", 1080, 1920, new Color(0.1f, 0.2f, 0.1f)); // Dark Green

        // Parts
        CreateTexture("Wheel", 128, 128, Color.black);

        AssetDatabase.Refresh();
        Debug.Log("Placeholder Assets Generated in Assets/Art!");
    }

    [MenuItem("Pitstop Panic/Import Models from Downloads")]
    public static void ImportModelsFromDownloads()
    {
        if (!Directory.Exists(ModelsPath)) Directory.CreateDirectory(ModelsPath);
        if (!Directory.Exists(DownloadsPath))
        {
            Debug.LogWarning("Downloads folder not found: " + DownloadsPath);
            return;
        }

        string[] exts = new[] { ".glb", ".fbx", ".obj" };
        var files = Directory.EnumerateFiles(DownloadsPath, "*.*", SearchOption.AllDirectories)
            .Where(f => exts.Contains(Path.GetExtension(f).ToLower()))
            .ToList();

        int copied = 0;
        foreach (var f in files)
        {
            string dest = Path.Combine(ModelsPath, Path.GetFileName(f));
            try
            {
                File.Copy(f, dest, true);
                copied++;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Copy failed: " + f + " -> " + dest + " :: " + e.Message);
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"Imported {copied} model files into {ModelsPath}");
    }

    [MenuItem("Pitstop Panic/Bake 3D Models to Sprites")]
    public static void BakeModelsToSprites()
    {
        if (!Directory.Exists(ArtPath)) Directory.CreateDirectory(ArtPath);
        if (!Directory.Exists(ModelsPath))
        {
            Debug.LogWarning("Models folder not found: " + ModelsPath + ". Import models first.");
            return;
        }

        var modelPaths = Directory.EnumerateFiles(ModelsPath, "*.*", SearchOption.TopDirectoryOnly)
            .Where(p => p.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase)
                     || p.EndsWith(".glb", System.StringComparison.OrdinalIgnoreCase)
                     || p.EndsWith(".obj", System.StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (modelPaths.Count == 0)
        {
            Debug.LogWarning("No models found to bake in " + ModelsPath);
            return;
        }

        // Setup temporary camera
        var camGO = new GameObject("BakeCamera");
        var cam = camGO.AddComponent<Camera>();
        cam.orthographic = true;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0);
        cam.cullingMask = ~0;

        int baked = 0;
        foreach (var path in modelPaths)
        {
            var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (obj == null)
            {
                Debug.LogWarning("Could not load model (requires importer for GLB): " + path);
                continue;
            }

            GameObject inst = PrefabUtility.InstantiatePrefab(obj) as GameObject;
            if (inst == null) continue;

            // Frame model
            Bounds bounds = CalculateBounds(inst);
            cam.transform.position = bounds.center + new Vector3(0, 0, -10);
            cam.transform.rotation = Quaternion.identity;

            // Decide output name and size
            string outName;
            int w, h;
            DecideOutput(Path.GetFileName(path), out outName, out w, out h);
            if (string.IsNullOrEmpty(outName))
            {
                Object.DestroyImmediate(inst);
                continue;
            }

            float aspect = (float)w / h;
            float sizeByHeight = bounds.extents.y * 1.2f;
            float sizeByWidth = (bounds.extents.x * 1.2f) / aspect;
            cam.orthographicSize = Mathf.Max(0.01f, Mathf.Max(sizeByHeight, sizeByWidth));

            RenderTexture rt = new RenderTexture(w, h, 24, RenderTextureFormat.ARGB32);
            rt.antiAliasing = 4;
            cam.targetTexture = rt;
            cam.Render();

            Texture2D tex = new Texture2D(w, h, TextureFormat.ARGB32, false);
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            tex.Apply();
            RenderTexture.active = null;
            cam.targetTexture = null;
            rt.Release();

            var bytes = tex.EncodeToPNG();
            var outPath = Path.Combine(ArtPath, outName);
            File.WriteAllBytes(outPath, bytes);
            AssetDatabase.ImportAsset(outPath);
            var importer = AssetImporter.GetAtPath(outPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }

            baked++;
            Object.DestroyImmediate(inst);
        }

        Object.DestroyImmediate(camGO);
        AssetDatabase.Refresh();
        Debug.Log($"Baked {baked} sprites into {ArtPath}");
    }

    static void CreateTexture(string name, int width, int height, Color color)
    {
        Texture2D tex = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        
        // Add a simple border
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x < 4 || x > width - 5 || y < 4 || y > height - 5)
                    tex.SetPixel(x, y, Color.black);
                else
                    tex.SetPixel(x, y, color);
            }
        }
        
        tex.Apply();
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(ArtPath, name + ".png"), bytes);

        // Import settings
        string assetPath = Path.Combine(ArtPath, name + ".png");
        AssetDatabase.ImportAsset(assetPath);
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.SaveAndReimport();
        }
    }

    static Bounds CalculateBounds(GameObject go)
    {
        var renderers = go.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(go.transform.position, Vector3.one);
        Bounds b = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            b.Encapsulate(renderers[i].bounds);
        return b;
    }

    static void DecideOutput(string fileName, out string outName, out int w, out int h)
    {
        string name = fileName.ToLowerInvariant();
        outName = null; w = 256; h = 256;

        // Vehicles
        if (name.Contains("car"))
        {
            w = 512; h = 256;
            if (name.Contains("fixed")) outName = "CarFixed.png";
            else if (name.Contains("broken")) outName = "CarBroken.png";
            else outName = "CarFixed.png";
            return;
        }
        if (name.Contains("scooter"))
        {
            w = 256; h = 256;
            if (name.Contains("fixed")) outName = "ScooterFixed.png";
            else if (name.Contains("broken")) outName = "ScooterBroken.png";
            else outName = "ScooterFixed.png";
            return;
        }

        // Tools
        w = 128; h = 128;
        if (name.Contains("wrench")) { outName = "Tool_Wrench.png"; return; }
        if (name.Contains("screwdriver")) { outName = "Tool_Screwdriver.png"; return; }
        if (name.Contains("jack")) { outName = "Tool_Jack.png"; return; }
        if (name.Contains("oilcan") || name.Contains("oil_can") || name.Contains("oil")) { outName = "Tool_OilCan.png"; return; }
        if (name.Contains("funnel")) { outName = "Tool_Funnel.png"; return; }
        if (name.Contains("multimeter") || name.Contains("multi") || name.Contains("meter")) { outName = "Tool_Multimeter.png"; return; }

        // Parts
        if (name.Contains("wheel")) { outName = "Wheel.png"; return; }
    }
}
