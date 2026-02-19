#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// One-click project setup for Android mobile: creates missing scenes (LevelSelect, Garage, Victory),
/// populates each with TMP UI + scripts, wires button events, and configures Build Settings.
/// Menu: Pitstop Panic → Setup All Scenes
/// </summary>
public static class PitstopSetup
{
    const string ScenesFolder = "Assets/Scenes";
    const string ArtFolder = "Assets/Art";
    const string AudioFolder = "Assets/Audio";
    const string PrefabsFolder = "Assets/Prefabs";

    // Mobile-first reference: 1080x1920 portrait (scales to landscape too)
    static readonly Vector2 RefResolution = new Vector2(1080, 1920);

    static readonly Color BgDark    = new Color(0.12f, 0.12f, 0.16f);
    static readonly Color BgPanel   = new Color(0.20f, 0.20f, 0.28f, 0.90f);
    static readonly Color Accent    = new Color(0.95f, 0.65f, 0.15f);
    static readonly Color AccentRed = new Color(0.85f, 0.25f, 0.25f);
    static readonly Color TextWhite = Color.white;
    static readonly Color TextGray  = new Color(0.70f, 0.70f, 0.70f);

    // ─────────────────────────────────────────────────────────────
    //  MENU ENTRIES
    // ─────────────────────────────────────────────────────────────

    [MenuItem("Pitstop Panic/Setup All Scenes", false, 1)]
    public static void SetupAll()
    {
        if (Application.isPlaying)
        {
            EditorUtility.DisplayDialog("Pitstop Panic", "Please exit Play Mode before running setup.", "OK");
            return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        // Ensure Art Assets exist (Regenerate placeholders if missing)
        if (!File.Exists(Path.Combine(ArtFolder, "CarBroken.png")))
        {
            Debug.Log("Art assets missing. Regenerating placeholders...");
            AssetGenerator.GenerateAll();
        }

        EnsureTMPResources();
        EnsureSpriteSettings(); 
        
        // Force refresh before loading assets
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        
        if (!AssetDatabase.IsValidFolder(ScenesFolder)) Directory.CreateDirectory(ScenesFolder);
        if (!AssetDatabase.IsValidFolder(PrefabsFolder)) Directory.CreateDirectory(PrefabsFolder);

        // Generate Vehicle Prefabs First
        GameObject carPrefab = CreateVehiclePrefab("Car", "CarBroken.png", "CarFixed.png", true);
        GameObject scooterPrefab = CreateVehiclePrefab("Scooter", "ScooterBroken.png", "ScooterFixed.png", false);

        CreateMainMenuScene();
        CreateLevelSelectScene();
        CreateGarageScene(carPrefab, scooterPrefab);
        CreateVictoryScene();
        SetBuildSettings();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Pitstop Panic",
            "Setup complete!\n\n" +
            "• Scenes regenerated (Main_Menu, LevelSelect, Garage, Victory)\n" +
            "• Prefabs created for Car and Scooter\n" +
            "• Level logic wired (Scooter=L1, Car=L2, Random=L3)\n" +
            "• Settings menu populated\n" +
            "• Animations & Audio linked\n\n" +
            "Ready to play!",
            "OK");
    }

    // ... (Existing TMP methods) ...
    static void EnsureTMPResources()
    {
        if (IsTMPImported()) return;
        string packagePath = Path.GetFullPath("Packages/com.unity.ugui");
        string essentials = Path.Combine(packagePath, "Package Resources", "TMP Essential Resources.unitypackage");
        if (File.Exists(essentials)) { AssetDatabase.ImportPackage(essentials, false); return; }
        string tmpPackagePath = Path.GetFullPath("Packages/com.unity.textmeshpro");
        string tmpEssentials = Path.Combine(tmpPackagePath, "Package Resources", "TMP Essential Resources.unitypackage");
        if (File.Exists(tmpEssentials)) { AssetDatabase.ImportPackage(tmpEssentials, false); return; }
    }
    static bool IsTMPImported() { return AssetDatabase.FindAssets("LiberationSans SDF t:TMP_FontAsset").Length > 0; }

    static void EnsureSpriteSettings()
    {
        if (!Directory.Exists(ArtFolder)) return;
        string[] files = Directory.GetFiles(ArtFolder, "*.png");
        bool changed = false;
        foreach (string file in files)
        {
            string assetPath = file.Replace("\\", "/");
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null && importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.SaveAndReimport();
                changed = true;
            }
        }
        if (changed) AssetDatabase.Refresh();
    }

    static void SetBuildSettings()
    {
        var scenes = new List<EditorBuildSettingsScene>();
        string[] order = { "Main_Menu", "LevelSelect", "Garage", "Victory" };
        foreach (string name in order)
        {
            string path = $"{ScenesFolder}/{name}.unity";
            if (File.Exists(path)) scenes.Add(new EditorBuildSettingsScene(path, true));
        }
        EditorBuildSettings.scenes = scenes.ToArray();
    }

    // ─────────────────────────────────────────────────────────────
    //  PREFAB GENERATION
    // ─────────────────────────────────────────────────────────────
    static GameObject CreateVehiclePrefab(string prefabName, string brokenImg, string fixedImg, bool isCar)
    {
        // Create temp object in scene to build structure
        GameObject root = new GameObject(prefabName);
        RectTransform rt = root.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(900, 600); // Standard size

        Image visual = root.AddComponent<Image>();
        Sprite bSprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtFolder}/{brokenImg}");
        if (bSprite != null) 
        {
            visual.sprite = bSprite;
        }
        else
        {
            // Fallback Color if Sprite Missing
            visual.color = isCar ? new Color(0.8f, 0.2f, 0.2f) : new Color(0.8f, 0.4f, 0.1f);
        }

        GameObject fixedObj = new GameObject("FixedVisual");
        fixedObj.transform.SetParent(root.transform, false);
        Image fixedVisual = fixedObj.AddComponent<Image>();
        Sprite fSprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtFolder}/{fixedImg}");
        if (fSprite != null) 
        {
            fixedVisual.sprite = fSprite;
        }
        else
        {
             fixedVisual.color = isCar ? new Color(0.2f, 0.8f, 0.2f) : new Color(0.2f, 0.8f, 0.4f);
        }
        fixedObj.SetActive(false);

        VehicleController vc = root.AddComponent<VehicleController>();
        // Use SerializedObject to assign private fields
        SerializedObject so = new SerializedObject(vc);
        so.FindProperty("brokenSprite").objectReferenceValue = bSprite;
        so.FindProperty("fixedSprite").objectReferenceValue = fSprite;
        so.FindProperty("visualImage").objectReferenceValue = visual;
        so.FindProperty("fixedVisualObj").objectReferenceValue = fixedObj;
        
        // Add Repair Targets
        List<RepairTarget> targets = new List<RepairTarget>();
        
        if (isCar)
        {
            targets.Add(CreateRepairZone(root.transform, "TireFront", new Vector2(-280, -200), ToolType.CarJack));
            targets.Add(CreateRepairZone(root.transform, "TireRear", new Vector2(280, -200), ToolType.CarJack));
            targets.Add(CreateRepairZone(root.transform, "Engine", new Vector2(-140, 120), ToolType.Wrench));
            targets.Add(CreateRepairZone(root.transform, "Oil", new Vector2(140, 120), ToolType.OilCan));
            
            // New Zones to reach 10
            targets.Add(CreateRepairZone(root.transform, "HeadlightL", new Vector2(-350, 50), ToolType.Screwdriver));
            targets.Add(CreateRepairZone(root.transform, "HeadlightR", new Vector2(350, 50), ToolType.Screwdriver));
            targets.Add(CreateRepairZone(root.transform, "DoorL", new Vector2(-200, -50), ToolType.Wrench));
            targets.Add(CreateRepairZone(root.transform, "DoorR", new Vector2(200, -50), ToolType.Wrench));
            targets.Add(CreateRepairZone(root.transform, "Radiator", new Vector2(0, 150), ToolType.Funnel));
            targets.Add(CreateRepairZone(root.transform, "Battery", new Vector2(-100, 180), ToolType.Multimeter));
        }
        else
        {
            // Scooter Layout
            targets.Add(CreateRepairZone(root.transform, "WheelFront", new Vector2(-200, -150), ToolType.CarJack)); // Or Wrench
            targets.Add(CreateRepairZone(root.transform, "WheelRear", new Vector2(200, -150), ToolType.CarJack));
            targets.Add(CreateRepairZone(root.transform, "Engine", new Vector2(0, 0), ToolType.Screwdriver));
            
            // New Zones
            targets.Add(CreateRepairZone(root.transform, "Seat", new Vector2(-50, 50), ToolType.Wrench));
            targets.Add(CreateRepairZone(root.transform, "Handlebars", new Vector2(-150, 100), ToolType.Wrench));
            targets.Add(CreateRepairZone(root.transform, "Kickstand", new Vector2(0, -200), ToolType.OilCan));
            targets.Add(CreateRepairZone(root.transform, "Exhaust", new Vector2(150, -100), ToolType.Screwdriver));
            targets.Add(CreateRepairZone(root.transform, "MirrorL", new Vector2(-180, 150), ToolType.Screwdriver));
            targets.Add(CreateRepairZone(root.transform, "Tank", new Vector2(50, 80), ToolType.Funnel));
            targets.Add(CreateRepairZone(root.transform, "Battery", new Vector2(0, 50), ToolType.Multimeter));
        }

        // Assign targets to list
        SerializedProperty listProp = so.FindProperty("allPossibleTargets");
        listProp.arraySize = targets.Count;
        for(int i=0; i<targets.Count; i++)
        {
            listProp.GetArrayElementAtIndex(i).objectReferenceValue = targets[i];
        }
        so.ApplyModifiedProperties();

        // Save as Prefab
        string path = $"{PrefabsFolder}/{prefabName}.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root); // Remove from current scene
        return prefab;
    }

    // ─────────────────────────────────────────────────────────────
    //  MAIN MENU
    // ─────────────────────────────────────────────────────────────

    static void CreateMainMenuScene()
    {
        string path = $"{ScenesFolder}/Main_Menu.unity";
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        CreateCamera(BgDark);
        Canvas canvas = CreateMobileCanvas();
        EnsureEventSystem();

        RectTransform mainPanel = CreatePanel(canvas.transform, "MainPanel", Vector2.zero, Vector2.one);
        var mainCg = mainPanel.gameObject.AddComponent<CanvasGroup>();
        mainCg.blocksRaycasts = true;

        CreateTMP(mainPanel, "TitleText", "PITSTOP PANIC", new Vector2(0, 400), new Vector2(800, 150), 80, Accent, TextAlignmentOptions.Center);

        var managerObj = new GameObject("MenuManager");
        var manager = managerObj.AddComponent<MenuManager>();

        CreateButton(mainPanel, "PlayBtn", "PLAY", new Vector2(0, 100), new Vector2(600, 120), 48, BgDark, Accent, managerObj, "OnPlayPressed");
        CreateButton(mainPanel, "SettingsBtn", "SETTINGS", new Vector2(0, -60), new Vector2(600, 120), 48, TextWhite, BgPanel, managerObj, "OnSettingsPressed");
        CreateButton(mainPanel, "QuitBtn", "QUIT", new Vector2(0, -220), new Vector2(400, 100), 36, TextWhite, AccentRed, managerObj, "OnQuitPressed");

        // Settings Panel
        RectTransform settingsPanel = CreatePanel(canvas.transform, "SettingsPanel", Vector2.zero, Vector2.one);
        var settingsCg = settingsPanel.gameObject.AddComponent<CanvasGroup>();
        settingsCg.alpha = 0;
        settingsCg.blocksRaycasts = false;
        settingsPanel.gameObject.SetActive(false);

        CreateTMP(settingsPanel, "SettingsTitle", "SETTINGS", new Vector2(0, 500), new Vector2(800, 120), 60, TextWhite, TextAlignmentOptions.Center);
        
        // Sliders
        Slider musicSlider = CreateSlider(settingsPanel, "MusicSlider", "Music Volume", new Vector2(0, 200));
        Slider sfxSlider = CreateSlider(settingsPanel, "SFXSlider", "SFX Volume", new Vector2(0, 0));

        CreateButton(settingsPanel, "BackBtn", "BACK", new Vector2(0, -300), new Vector2(400, 100), 36, TextWhite, AccentRed, managerObj, "OnBackPressed");

        // Wire Manager
        manager.mainMenuPanel = mainCg;
        manager.settingsPanel = settingsCg;
        manager.musicSlider = musicSlider;
        manager.sfxSlider = sfxSlider;
        manager.gameSceneName = "LevelSelect";

        EditorSceneManager.SaveScene(scene, path);
    }

    // ─────────────────────────────────────────────────────────────
    //  LEVEL SELECT
    // ─────────────────────────────────────────────────────────────

    static void CreateLevelSelectScene()
    {
        string path = $"{ScenesFolder}/LevelSelect.unity";
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        CreateCamera(BgDark);
        Canvas canvas = CreateMobileCanvas();
        EnsureEventSystem();

        CreateTMP(canvas.transform, "TitleText", "SELECT LEVEL", new Vector2(0, 400), new Vector2(800, 120), 72, Accent, TextAlignmentOptions.Center);
        var controller = new GameObject("LevelSelectController").AddComponent<LevelSelectController>();

        // Level 1: Scooter
        CreateButton(canvas.transform, "Level1Btn", "LEVEL 1\n(Scooter)", new Vector2(0, 140), new Vector2(600, 110), 40, Accent, BgPanel, controller.gameObject, "LoadGarageLevel", 1);
        
        // Level 2: Car
        CreateButton(canvas.transform, "Level2Btn", "LEVEL 2\n(Car)", new Vector2(0, 0), new Vector2(600, 110), 40, Accent, BgPanel, controller.gameObject, "LoadGarageLevel", 2);
        
        // Level 3: Random
        CreateButton(canvas.transform, "Level3Btn", "LEVEL 3\n(Random)", new Vector2(0, -140), new Vector2(600, 110), 40, Accent, BgPanel, controller.gameObject, "LoadGarageLevel", 3);

        CreateButton(canvas.transform, "BackBtn", "BACK", new Vector2(0, -360), new Vector2(420, 100), 36, TextWhite, AccentRed, controller.gameObject, "LoadMainMenu");

        EditorSceneManager.SaveScene(scene, path);
    }

    // ─────────────────────────────────────────────────────────────
    //  GARAGE (GAMEPLAY)
    // ─────────────────────────────────────────────────────────────

    static void CreateGarageScene(GameObject carPrefab, GameObject scooterPrefab)
    {
        string path = $"{ScenesFolder}/Garage.unity";
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        CreateCamera(new Color(0.16f, 0.16f, 0.20f));
        Canvas canvas = CreateMobileCanvas();
        EnsureEventSystem();

        // Background
        Sprite garageBg = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtFolder}/GarageBg.png");
        if (garageBg != null)
        {
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(canvas.transform, false);
            bgObj.transform.SetAsFirstSibling();
            Image bgImg = bgObj.AddComponent<Image>();
            bgImg.sprite = garageBg;
            bgObj.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            bgObj.GetComponent<RectTransform>().anchorMax = Vector2.one;
            bgObj.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        }

        // HUD
        RectTransform hud = CreatePanel(canvas.transform, "HUD", new Vector2(0, 1), new Vector2(1, 1));
        hud.pivot = new Vector2(0.5f, 1);
        hud.anchoredPosition = Vector2.zero;
        hud.sizeDelta = new Vector2(0, 90);
        
        hud.gameObject.AddComponent<Image>().color = new Color(0.10f, 0.10f, 0.14f, 0.85f);

        TMP_Text scoreText = CreateTMP(hud.transform, "ScoreText", "Score: 0", new Vector2(40, 0), new Vector2(300, 70), 40, Accent, TextAlignmentOptions.Left);
        scoreText.rectTransform.anchorMin = new Vector2(0, 0.5f); scoreText.rectTransform.anchorMax = new Vector2(0, 0.5f); scoreText.rectTransform.pivot = new Vector2(0, 0.5f);

        TMP_Text timeText = CreateTMP(hud.transform, "TimeText", "Time: 60", new Vector2(-40, 0), new Vector2(300, 70), 40, TextWhite, TextAlignmentOptions.Right);
        timeText.rectTransform.anchorMin = new Vector2(1, 0.5f); timeText.rectTransform.anchorMax = new Vector2(1, 0.5f); timeText.rectTransform.pivot = new Vector2(1, 0.5f);

        TMP_Text instructionText = CreateTMP(canvas.transform, "InstructionText", "Drag tool to fix!", new Vector2(0, -100), new Vector2(900, 60), 32, TextGray, TextAlignmentOptions.Center);
        instructionText.rectTransform.anchorMin = new Vector2(0.5f, 1); instructionText.rectTransform.anchorMax = new Vector2(0.5f, 1); instructionText.rectTransform.pivot = new Vector2(0.5f, 1);

        // Spawn Area
        RectTransform spawnArea = CreatePanel(canvas.transform, "SpawnArea", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        spawnArea.sizeDelta = Vector2.zero; // Center point

        // Tool Tray
        RectTransform toolTray = CreatePanel(canvas.transform, "ToolTray", new Vector2(0, 0), new Vector2(1, 0));
        toolTray.pivot = new Vector2(0.5f, 0);
        toolTray.anchoredPosition = Vector2.zero;
        toolTray.sizeDelta = new Vector2(0, 200);

        toolTray.gameObject.AddComponent<Image>().color = new Color(0.14f, 0.14f, 0.18f, 0.92f);
        HorizontalLayoutGroup layout = toolTray.gameObject.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 16; layout.childAlignment = TextAnchor.MiddleCenter; layout.padding = new RectOffset(20, 20, 12, 12);

        CreateDraggableTool(toolTray.transform, "Tool_Wrench", ToolType.Wrench, new Color(0.70f, 0.70f, 0.75f));
        CreateDraggableTool(toolTray.transform, "Tool_Jack", ToolType.CarJack, new Color(0.85f, 0.30f, 0.30f));
        CreateDraggableTool(toolTray.transform, "Tool_OilCan", ToolType.OilCan, new Color(0.90f, 0.75f, 0.20f));
        CreateDraggableTool(toolTray.transform, "Tool_Screwdriver", ToolType.Screwdriver, new Color(0.30f, 0.70f, 0.40f));
        CreateDraggableTool(toolTray.transform, "Tool_Funnel", ToolType.Funnel, new Color(0.60f, 0.60f, 0.65f));
        CreateDraggableTool(toolTray.transform, "Tool_Multimeter", ToolType.Multimeter, new Color(0.90f, 0.85f, 0.20f));

        // Game Manager
        var gmObj = new GameObject("GameManager");
        var gm = gmObj.AddComponent<RepairGameManager>();
        SerializedObject so = new SerializedObject(gm);
        
        so.FindProperty("carPrefab").objectReferenceValue = carPrefab;
        so.FindProperty("scooterPrefab").objectReferenceValue = scooterPrefab;
        so.FindProperty("spawnPoint").objectReferenceValue = spawnArea;
        
        so.FindProperty("scoreText").objectReferenceValue = scoreText;
        so.FindProperty("timerText").objectReferenceValue = timeText;
        so.FindProperty("instructionText").objectReferenceValue = instructionText;
        so.FindProperty("scorePerPart").intValue = 100;
        so.FindProperty("timeLimit").floatValue = 60f;
        so.FindProperty("countDown").boolValue = true;
        so.ApplyModifiedProperties();

        EditorSceneManager.SaveScene(scene, path);
    }

    // ─────────────────────────────────────────────────────────────
    //  VICTORY
    // ─────────────────────────────────────────────────────────────

    static void CreateVictoryScene()
    {
        string path = $"{ScenesFolder}/Victory.unity";
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        CreateCamera(new Color(0.08f, 0.16f, 0.08f));
        Canvas canvas = CreateMobileCanvas();
        EnsureEventSystem();

        Sprite victoryBg = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtFolder}/VictoryBg.png");
        if (victoryBg != null)
        {
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(canvas.transform, false);
            bgObj.transform.SetAsFirstSibling();
            Image bgImg = bgObj.AddComponent<Image>();
            bgImg.sprite = victoryBg;
            bgObj.GetComponent<RectTransform>().anchorMin = Vector2.zero;
            bgObj.GetComponent<RectTransform>().anchorMax = Vector2.one;
            bgObj.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        }

        CreateTMP(canvas.transform, "VictoryTitle", "CAR REPAIRED!", new Vector2(0, 380), new Vector2(800, 120), 72, Accent, TextAlignmentOptions.Center);
        CreateTMP(canvas.transform, "VictorySubtitle", "Test drive successful.", new Vector2(0, 260), new Vector2(800, 60), 36, TextGray, TextAlignmentOptions.Center);
        TMP_Text scoreText = CreateTMP(canvas.transform, "ScoreText", "Score: 0", new Vector2(0, 120), new Vector2(600, 80), 52, TextWhite, TextAlignmentOptions.Center);

        var controller = new GameObject("VictoryController").AddComponent<VictoryController>();
        SerializedObject so = new SerializedObject(controller);
        so.FindProperty("scoreText").objectReferenceValue = scoreText;
        so.ApplyModifiedProperties();

        CreateButton(canvas.transform, "NextLevelBtn", "NEXT LEVEL", new Vector2(0, -60), new Vector2(560, 110), 40, TextWhite, Accent, controller.gameObject, "OnNextLevel");
        CreateButton(canvas.transform, "BackMenuBtn", "BACK TO MENU", new Vector2(0, -200), new Vector2(560, 110), 40, TextWhite, AccentRed, controller.gameObject, "OnBackToMenu");

        EditorSceneManager.SaveScene(scene, path);
    }

    // ─────────────────────────────────────────────────────────────
    //  HELPERS & UTILS
    // ─────────────────────────────────────────────────────────────
    static Camera CreateCamera(Color bg)
    {
        var camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";
        var cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = bg;
        cam.orthographic = true;
        cam.orthographicSize = 5;
        camObj.AddComponent<AudioListener>();
        
        AudioClip music = AssetDatabase.LoadAssetAtPath<AudioClip>($"{AudioFolder}/Music.wav");
        if (music != null)
        {
            AudioSource audio = camObj.AddComponent<AudioSource>();
            audio.clip = music;
            audio.loop = true;
            audio.playOnAwake = true;
            audio.volume = 0.3f;
        }
        return cam;
    }

    static Canvas CreateMobileCanvas()
    {
        var obj = new GameObject("Canvas");
        var canvas = obj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = obj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = RefResolution;
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        obj.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    static void EnsureEventSystem()
    {
        if (Object.FindAnyObjectByType<EventSystem>() != null) return;
        var obj = new GameObject("EventSystem");
        obj.AddComponent<EventSystem>();
        obj.AddComponent<StandaloneInputModule>();
    }

    static TMP_Text CreateTMP(Transform parent, string name, string content, Vector2 pos, Vector2 size, float fontSize, Color color, TextAlignmentOptions alignment)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var rt = obj.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        var tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = content;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = alignment;
        tmp.enableAutoSizing = false;
        tmp.raycastTarget = false;
        return tmp;
    }

    static void CreateButton(Transform parent, string name, string label, Vector2 pos, Vector2 size, float fontSize, Color textColor, Color bgColor, GameObject targetObj, string methodName, int intArg = -999)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var rt = obj.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        var img = obj.AddComponent<Image>();
        img.color = bgColor;
        var btn = obj.AddComponent<Button>();
        btn.colors = new ColorBlock { normalColor = bgColor, highlightedColor = bgColor*1.15f, pressedColor = bgColor*0.75f, colorMultiplier = 1, fadeDuration = 0.1f };
        
        // Add Animation & Sound
        var anim = obj.AddComponent<SimpleButtonAnim>();
        SerializedObject animSo = new SerializedObject(anim);
        animSo.FindProperty("idlePulse").boolValue = true; // Make them pulse!
        animSo.ApplyModifiedProperties();

        AudioClip clickSound = AssetDatabase.LoadAssetAtPath<AudioClip>($"{AudioFolder}/Click.wav");
        if (clickSound != null)
        {
            var audio = obj.AddComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.clip = clickSound;
            btn.onClick.AddListener(() => audio.Play());
        }

        var tmp = CreateTMP(obj.transform, "Label", label, Vector2.zero, Vector2.zero, fontSize, textColor, TextAlignmentOptions.Center);
        var tmpRt = tmp.GetComponent<RectTransform>();
        tmpRt.anchorMin = Vector2.zero;
        tmpRt.anchorMax = Vector2.one;
        tmpRt.offsetMin = Vector2.zero;
        tmpRt.offsetMax = Vector2.zero;

        MonoBehaviour target = targetObj.GetComponents<MonoBehaviour>()[0];
        WireButtonEvent(btn, target, methodName, intArg);
    }

    static void WireButtonEvent(Button btn, Object target, string methodName, int intArg)
    {
        var so = new SerializedObject(btn);
        var calls = so.FindProperty("m_OnClick").FindPropertyRelative("m_PersistentCalls.m_Calls");
        calls.InsertArrayElementAtIndex(calls.arraySize);
        var entry = calls.GetArrayElementAtIndex(calls.arraySize - 1);
        entry.FindPropertyRelative("m_Target").objectReferenceValue = target;
        entry.FindPropertyRelative("m_MethodName").stringValue = methodName;
        entry.FindPropertyRelative("m_CallState").intValue = 2; // RuntimeOnly
        
        if (intArg != -999)
        {
            // Int argument call
            entry.FindPropertyRelative("m_Mode").intValue = 3; // Int Mode
            entry.FindPropertyRelative("m_Arguments").FindPropertyRelative("m_IntArgument").intValue = intArg;
             entry.FindPropertyRelative("m_Arguments")
                 .FindPropertyRelative("m_ObjectArgumentAssemblyTypeName")
                 .stringValue = "System.Int32, mscorlib"; 
        }
        else
        {
            // Void call
            entry.FindPropertyRelative("m_Mode").intValue = 1; // Void Mode
             entry.FindPropertyRelative("m_Arguments")
                 .FindPropertyRelative("m_ObjectArgumentAssemblyTypeName")
                 .stringValue = "UnityEngine.Object, UnityEngine";
        }
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static Slider CreateSlider(Transform parent, string name, string label, Vector2 pos)
    {
        // Simple Slider construction
        GameObject root = new GameObject(name);
        root.transform.SetParent(parent, false);
        RectTransform rt = root.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(500, 40);

        // Background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(root.transform, false);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.2f, 0.2f, 0.2f);
        RectTransform bgRt = bg.GetComponent<RectTransform>();
        bgRt.anchorMin = new Vector2(0, 0.25f);
        bgRt.anchorMax = new Vector2(1, 0.75f);
        bgRt.offsetMin = Vector2.zero; bgRt.offsetMax = Vector2.zero;

        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(root.transform, false);
        RectTransform fillAreaRt = fillArea.AddComponent<RectTransform>();
        fillAreaRt.anchorMin = new Vector2(0, 0.25f);
        fillAreaRt.anchorMax = new Vector2(1, 0.75f);
        fillAreaRt.offsetMin = new Vector2(5, 0); fillAreaRt.offsetMax = new Vector2(-5, 0);

        // Fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = Accent;
        RectTransform fillRt = fill.GetComponent<RectTransform>();
        fillRt.sizeDelta = Vector2.zero;

        // Handle
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(root.transform, false);
        RectTransform handleAreaRt = handleArea.AddComponent<RectTransform>();
        handleAreaRt.anchorMin = Vector2.zero; handleAreaRt.anchorMax = Vector2.one;
        handleAreaRt.offsetMin = new Vector2(10, 0); handleAreaRt.offsetMax = new Vector2(-10, 0);

        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        Image handleImg = handle.AddComponent<Image>();
        handleImg.color = Color.white;
        RectTransform handleRt = handle.GetComponent<RectTransform>();
        handleRt.sizeDelta = new Vector2(40, 0);
        handleRt.anchorMin = new Vector2(0, 0); handleRt.anchorMax = new Vector2(0, 1);

        Slider slider = root.AddComponent<Slider>();
        slider.fillRect = fillRt;
        slider.handleRect = handleRt;
        slider.targetGraphic = handleImg;
        slider.direction = Slider.Direction.LeftToRight;

        // Label
        CreateTMP(root.transform, "Label", label, new Vector2(0, 50), new Vector2(500, 40), 30, TextWhite, TextAlignmentOptions.Center);

        return slider;
    }

    static void CreateDraggableTool(Transform parent, string name, ToolType toolType, Color color)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var rt = obj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(100, 100);

        var img = obj.AddComponent<Image>();
        img.color = color;
        
        // Attempt to load specific tool sprite
        string spriteName = "Tool_" + toolType.ToString() + ".png";
        if (toolType == ToolType.CarJack) spriteName = "Tool_Jack.png";

        Sprite toolSprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtFolder}/{spriteName}");
        if (toolSprite != null) 
        {
            img.sprite = toolSprite;
            img.color = Color.white; // Reset color if sprite is found
        }

        // Add Label
        string labelText = toolType.ToString().ToUpper();
        if (toolType == ToolType.CarJack) labelText = "JACK";
        
        // Background for tool label
        var labelBg = new GameObject("LabelBg");
        labelBg.transform.SetParent(obj.transform, false);
        var lBgRt = labelBg.AddComponent<RectTransform>();
        lBgRt.sizeDelta = new Vector2(120, 35);
        lBgRt.anchoredPosition = new Vector2(0, -60);
        var lBgImg = labelBg.AddComponent<Image>();
        lBgImg.color = new Color(0,0,0,0.8f);

        var label = CreateTMP(labelBg.transform, "Label", labelText, Vector2.zero, new Vector2(120, 35), 20, Color.white, TextAlignmentOptions.Center);
        label.fontStyle = FontStyles.Bold;

        var draggable = obj.AddComponent<DraggableTool>();
        
        // DraggableTool has [RequireComponent(typeof(CanvasGroup))], so we retrieve it.
        var cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();
        
        cg.blocksRaycasts = true;

        SerializedObject so = new SerializedObject(draggable);
        so.FindProperty("toolType").enumValueIndex = (int)toolType;
        so.ApplyModifiedProperties();
    }

    static RectTransform CreatePanel(Transform parent, string name, Vector2 min, Vector2 max)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var rt = obj.AddComponent<RectTransform>();
        rt.anchorMin = min; rt.anchorMax = max;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        return rt;
    }
    
    // Copy of CreateRepairZone helper needed for prefabs
    static RepairTarget CreateRepairZone(Transform parent, string name, Vector2 pos, ToolType toolType)
    {
        // 1. Root Object (Holds Script & Audio)
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var rt = obj.AddComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(160, 160);

        // Invisible raycast target for dragging
        var img = obj.AddComponent<Image>();
        img.color = Color.clear; 
        img.raycastTarget = true;
        
        var target = obj.AddComponent<RepairTarget>();
        
        // 2. Broken Visual (Child) - This will be disabled on fix
        GameObject brokenObj = new GameObject("BrokenVisual");
        brokenObj.transform.SetParent(obj.transform, false);
        RectTransform brokenRt = brokenObj.AddComponent<RectTransform>();
        brokenRt.anchorMin = Vector2.zero;
        brokenRt.anchorMax = Vector2.one;
        brokenRt.sizeDelta = Vector2.zero; // Fill parent

        var brokenImg = brokenObj.AddComponent<Image>();
        brokenImg.color = new Color(1f, 0.4f, 0.4f, 0.20f);
        brokenImg.raycastTarget = false; // Root handles raycast

        // Add "Required Tool" Indicator to BrokenVisual
        string toolName = toolType.ToString().ToUpper();
        if (toolType == ToolType.CarJack) toolName = "JACK";
        
        // Background for label
        GameObject labelBg = new GameObject("LabelBg");
        labelBg.transform.SetParent(brokenObj.transform, false);
        var bgImg = labelBg.AddComponent<Image>();
        bgImg.color = new Color(0, 0, 0, 0.7f);
        bgImg.raycastTarget = false;
        bgImg.rectTransform.sizeDelta = new Vector2(140, 40);

        var label = CreateTMP(labelBg.transform, "ReqToolLabel", toolName, Vector2.zero, new Vector2(140, 40), 24, Color.yellow, TextAlignmentOptions.Center);
        label.fontStyle = FontStyles.Bold;
        label.raycastTarget = false; 

        SerializedObject so = new SerializedObject(target);
        so.FindProperty("requiredTool").enumValueIndex = (int)toolType;
        
        // 3. Fixed Visual (Child) - Enabled on fix
        GameObject fixedVis = new GameObject("FixedVisual");
        fixedVis.transform.SetParent(obj.transform, false);
        RectTransform fixedRt = fixedVis.AddComponent<RectTransform>();
        fixedRt.anchorMin = Vector2.zero;
        fixedRt.anchorMax = Vector2.one;
        fixedRt.sizeDelta = Vector2.zero;

        Image fixedImg = fixedVis.AddComponent<Image>();
        fixedImg.color = new Color(0, 1, 0, 0.5f);
        fixedImg.raycastTarget = false; // Don't block subsequent drags (though they shouldn't happen)

        Sprite wheel = AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtFolder}/Wheel.png");
        if (toolType == ToolType.CarJack && wheel != null) fixedImg.sprite = wheel;
        fixedVis.SetActive(false);
        
        // 4. Wiring
        // CRITICAL FIX: brokenVisual points to child object, NOT root object
        so.FindProperty("brokenVisual").objectReferenceValue = brokenObj;
        so.FindProperty("fixedVisual").objectReferenceValue = fixedVis;
        
        AudioClip repairSound = AssetDatabase.LoadAssetAtPath<AudioClip>($"{AudioFolder}/RepairSuccess.wav");
        if (repairSound != null)
        {
            var source = obj.AddComponent<AudioSource>();
            so.FindProperty("audioSource").objectReferenceValue = source;
            so.FindProperty("repairSound").objectReferenceValue = repairSound;
        }
        so.ApplyModifiedProperties();
        return target;
    }
}
#endif
