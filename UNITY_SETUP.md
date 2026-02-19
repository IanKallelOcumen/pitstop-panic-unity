# Unity Setup – Step by Step

## 0. Automatic setup (recommended)

After copying Assets into your Unity project (step 1 below), use the **one-click setup** instead of doing steps 2–6 manually:

1. In Unity: **Pitstop Panic → Setup All Scenes** (top menu bar).
2. This automatically:
   - Creates **LevelSelect**, **Garage**, **Victory** scenes with full UI, scripts, and button events already wired.
   - Configures **Build Settings** (Main_Menu 0 → LevelSelect 1 → Garage 2 → Victory 3).
   - Sets up the **Garage** with 4 repair zones (TireFront, TireRear, Engine, Oil), 6 draggable tools (Wrench, Jack, OilCan, Screwdriver, Funnel, Multimeter), HUD (score + timer), and a GameManager with everything pre-assigned.
   - Sets up **Victory** with score display, Next Level, and Back To Menu buttons.
   - Sets up **LevelSelect** with 3 level buttons and a Back button.
3. Open **Main_Menu**, press **Play** — the full game flow works out of the box.
4. Replace the placeholder colors with your actual sprites (car, tools) in the **Garage** scene whenever you have them.

> If you only need to fix Build Settings without recreating scenes, use **Pitstop Panic → Setup Build Settings Only**.

---

## Manual setup (steps 1–6 below)

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
