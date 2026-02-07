# Triple-check verification

## ✅ What was verified

### 1. Menu assets from Bayanihan (all present)
- **Assets/Scenes/Main_Menu.unity** + `.meta`
- **Assets/Scripts/MenuManager.cs** + `.meta` (guid preserved for scene reference)
- **Assets/Font/pixel-art-font.ttf** + `.meta` (guid `e352edd47e3ac8d438df0255f15f4b15`)
- **Assets/Art/Gemini_Generated_Image_be8lvlbe8lvlbe8l.png** + `.meta` (guid `deac6a4019e12914fbc06f115fc9cb17`)

### 2. Scene references (GUIDs match)
- **Main_Menu.unity** references:
  - Font: `e352edd47e3ac8d438df0255f15f4b15` → **pixel-art-font.ttf** ✓
  - Sprite: `deac6a4019e12914fbc06f115fc9cb17` → **Gemini_Generated_Image...png** ✓
  - Script: `d5c9cada1beabae48a504c02c99581e5` → **MenuManager.cs** ✓

### 3. Scene name consistency
- **SceneNames.MainMenu** = `"Main_Menu"` (matches Main_Menu.unity so Back from LevelSelect/Victory returns to the right menu).
- **MenuManager.gameSceneName** default = `"LevelSelect"` (Play loads level select; set to `"Garage"` in Inspector to skip level select).

### 4. Scripts
- No linter errors in **Assets/Scripts**.
- **LevelSelectController** OnBack → `SceneNames.MainMenu` → `"Main_Menu"` ✓
- **VictoryController** OnBackToMenu → `SceneNames.MainMenu` → `"Main_Menu"` ✓
- **RepairGameManager** loads `SceneNames.Victory` on win ✓

### 5. Docs
- **README.md** and **UNITY_SETUP.md** updated for Main_Menu, Build Settings order, and MenuManager.

---

## What you should do in Unity

1. **Open project** in Unity (2D).
2. **Build Settings:** File → Build Settings. Add (in order): **Main_Menu**, **LevelSelect**, **Garage**, **Victory**. Create LevelSelect, Garage, Victory as new empty scenes if they don’t exist.
3. **Main_Menu:** On the **MenuManager** component, assign **Main_Menu_Panel** and **Setting_Panel** (CanvasGroup) if the scene shows missing refs. Set **gameSceneName** to `LevelSelect` or `Garage` as desired.
4. **Garage scene:** Add UI and assign **RepairGameManager**, **RepairTarget**s, **DraggableTool**s (see UNITY_SETUP.md).
5. **Run:** Open Main_Menu, press Play. Play → LevelSelect (or Garage); Back from LevelSelect/Victory → Main_Menu.

If any asset shows as *Missing* in the Main_Menu scene, re-copy the corresponding file and `.meta` from Bayanihan so the GUID stays the same.
