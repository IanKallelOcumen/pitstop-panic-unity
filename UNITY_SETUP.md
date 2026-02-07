# Unity Setup – Step by Step

## 1. Create project and copy files

1. Open Unity Hub → Create new project → **2D (Core)** or **2D (URP)**.
2. Copy the entire **Assets** folder from this repo into your project’s **Assets** folder (merge with existing).

## 2. Add scenes to Build Settings

1. You already have **Main_Menu** (from Bayanihan) in `Assets/Scenes/`. Create the other 3 scenes: **LevelSelect**, **Garage**, **Victory** (right‑click in Assets → Create → Scene).
2. **File → Build Settings** → drag these 4 scenes into "Scenes In Build" in this order:  
   **Main_Menu** (0), **LevelSelect** (1), **Garage** (2), **Victory** (3).

## 3. Main menu (Main_Menu scene – already set up)

The **Main_Menu** scene from Bayanihan is already in the project. It uses **MenuManager** (not MainMenuController):
- **Play** loads the scene named in `MenuManager.gameSceneName` (default: **LevelSelect**).
- **Settings** opens the settings panel (slide transition).
- **Quit** exits the game.
- Assign **Main_Menu_Panel** and **Setting_Panel** to the MenuManager component if they are missing.
- To start the repair game directly from Play, set `gameSceneName` to **Garage** in the Inspector.

## 4. Garage scene (gameplay)

1. **Canvas** with:
   - **Text** (top‑left): "Score: 0" → assign to `RepairGameManager.scoreText`.
   - **Text** (top‑left): "Time: 60" → assign to `RepairGameManager.timeText`.
   - **Text** (instruction): "Drag the correct tool to fix" → assign to `RepairGameManager.instructionText`.
2. **Car:** Create **UI → Image**, name it "Car". Assign your **car broken** sprite. Assign to `RepairGameManager.carImage` and set `carBrokenSprite` / `carFixedSprite`.
3. **Repair zones:** For each part to fix (e.g. front tire, rear tire, engine):
   - Add **UI → Image** as child of Car (or over it), no sprite or transparent. Add script **RepairTarget**.
   - Set `requiredToolType` (e.g. "Jack" for tire, "Wrench" for engine).
4. **Tools:** For each tool (Wrench, Jack, etc.):
   - Add **UI → Image**, assign tool sprite. Add script **DraggableTool**, set `toolType` to match (e.g. "Jack", "Wrench").
5. Empty GameObject **GameManager** → add **RepairGameManager**. Assign all **RepairTarget**s to the `repairTargets` array (or leave empty to auto‑find), and all UI references.

## 5. Victory scene

1. Canvas with text: "Car Repaired! Test drive successful."
2. Buttons: "Next Level", "Back To Menu".
3. Empty GameObject **VictoryController** → add script **VictoryController**. Wire button clicks to `OnNextLevel` and `OnBackToMenu`.

## 6. LevelSelect scene (optional)

1. Canvas with "SELECT LEVEL", "LEVEL 1", "LEVEL 2", "LEVEL 3", "BACK".
2. Empty GameObject **LevelSelectController** → add **LevelSelectController**. Wire BACK → `OnBack`, LEVEL 1/2/3 → `OnLevel1` / `OnLevel2` / `OnLevel3`.

---

**Tool ↔ target matching:**  
Use the same string for both:
- `DraggableTool.toolType` = "Wrench"
- `RepairTarget.requiredToolType` = "Wrench"  
Then dragging that tool onto that target counts as a correct repair.
