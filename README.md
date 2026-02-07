# Pitstop Panic – Car Repair Game (Unity)

**Repository:** [pitstop-panic-unity](https://github.com/IanKallelOcumen/pitstop-panic-unity) — push with `git push -u origin master` (create the repo on GitHub first if needed).

A 2D car repair / automotive maintenance game for Unity, inspired by drag-and-drop “fix the car” games. Drag the correct tool to the right part of the car (tire, engine, etc.) to repair it.

---

## Quick Start

1. **Create a new Unity project** (2D, any LTS e.g. 2022.3+).
2. Copy the contents of **`Assets`** from this folder into your project’s **`Assets`** folder (merge with existing).
3. **File → Build Settings**: add **Main_Menu**, **LevelSelect**, **Garage**, **Victory** (in that order). Create LevelSelect, Garage, Victory as new scenes if needed.
4. Open **Scenes/Main_Menu** as the first scene. Assign sprites in the Garage scene (car, tools, UI) – use placeholders until you add your assets.

**Transparent backgrounds:** PNGs with white or black backgrounds (e.g. from Gemini) can be made transparent so they look correct in the scene. Run `python Tools/make_transparent_bg.py` from the project root (see **Tools/README.md**). Unity will use alpha for any PNG in `Assets/Art/` and `Assets/Sprites/` (see `Assets/Editor/TextureImportTransparent.cs`).

---

## Asset Guide – Where to Find “Fixed Car” and Tools

Use these for the **fixed version of the car** and **the tools** in your game.

### Cars (fixed / repaired look)

| Asset | Type | Link | Notes |
|-------|------|------|--------|
| **Pixel Cars** | 2D sprites, free | [Unity Asset Store – Pixel Cars](https://assetstore.unity.com/packages/2d/pixel-cars-178447) | Pixel cars; use as “repaired” car or base for states. |
| **2D Race Cars** | 2D sprites, free | [Unity Asset Store – 2D Race Cars](https://assetstore.unity.com/packages/2d/characters/2d-race-cars-18207) | Multiple car sprites. |
| **2D Car Sprite** | Single 2D sprite | [OpenGameArt – 2D car sprite](https://opengameart.org/content/2d-car-sprite-14) | Kart-style; good for “fixed” state. |
| **Hill Climb Racing – Car Sprites** | 2D, free | [GameArt2D – Hill Climb Racing](https://www.gameart2d.com/hill-climb-racing---free-car-sprites.html) | PNG/SVG; use one as “repaired” car. |
| **FREE Cartoon Car Pack** | 3D, free | [Unity – Free Cartoon Car Pack](https://assetstore.unity.com/packages/3d/vehicles/land/free-cartoon-car-pack-simple-vehicles-282425) | If you want 3D; use as “fixed” model. |

**Tip:** Use one sprite as “broken” (e.g. flat tire, open hood) and another as “fixed” (hood closed, tires normal), or swap sprite on the same GameObject when repair is done.

### Tools (wrench, jack, screwdriver, etc.)

| Asset | Type | Link | Notes |
|-------|------|------|--------|
| **Rotating Wrench** | 2D animated | [OpenGameArt – Rotating Wrench](https://opengameart.org/content/rotating-wrench) | CC0; good for “correct tool” feedback. |
| **16x16 Weapon/Tool Spritesheet** | Pixel tools | [OpenGameArt – 16x16 Tool Spritesheet](https://opengameart.org/content/16x16-weapontool-spritesheet) | Small tools; scale up if needed. |
| **Car Mechanic Garage & Workshop Tools** | 3D bundle | [Unity – Car Mechanic Tools Bundle](https://marketplace.unity.com/packages/package/car-mechanic-garage-workshop-tools-bundle-320532) | Jack, compressor, etc.; render to sprite or use in 3D. |
| **Garage Workbench** | 3D props | [Unity – Garage Workbench](https://assetstore.unity.com/packages/3d/props/tools/garage-workbench-131418) | Wrench, drill, toolbox. |

**Extra:** Search **itch.io** and **OpenGameArt** for “wrench”, “screwdriver”, “car jack”, “oil can”, “multimeter” for more 2D tool sprites.

### Garage / Environment (optional)

| Asset | Link |
|-------|------|
| **Car Repair Shop HQ** | [Unity – Car Repair Shop HQ](https://marketplace.unity.com/packages/3d/environments/car-repair-shop-hq-232807) |
| **Modular Car Service Garage** | [Unity – Modular Car Service Garage](https://assetstore.unity.com/packages/3d/environments/modular-car-service-garage-parking-interior-exterior-239799) |

### Learning reference (repair-style gameplay)

| Asset | Link |
|-------|------|
| **Unity Learn – 2D Robot Repair** | [Unity – 2D Adventure Robot Repair](https://assetstore.unity.com/packages/essentials/tutorial-projects/unity-learn-2d-adventure-game-robot-repair-271239) |

---

## Project Structure

```
Assets/
├── Scripts/           # MenuManager, RepairGameManager, DraggableTool, RepairTarget, etc.
├── Scenes/            # Main_Menu (from Bayanihan), LevelSelect, Garage, Victory
├── Font/              # pixel-art-font (used by Main_Menu)
├── Art/               # Menu background image (Main_Menu)
├── Sprites/           # Put your car + tool sprites here
├── Prefabs/           # Car, tools, UI buttons as prefabs
└── Resources/         # Optional: load by name
```

---

## Scene Setup (summary)

1. **Main_Menu:** (from Bayanihan) Play, Settings, Quit; uses `MenuManager.cs`. Play loads `gameSceneName` (default: LevelSelect).

2. **LevelSelect:** Level 1/2/3 buttons and Back. Use `LevelSelectController.cs`. Back loads **Main_Menu**.
3. **Garage (game):** Canvas with score/time, instruction text, car image with `RepairTarget` zones (tire, engine), and draggable tool images with `DraggableTool`. Use `RepairGameManager.cs`.
4. **Victory:** “Car Repaired! Test drive successful.” + Next Level + Back To Menu. Use `VictoryController.cs`.

See the scripts for public fields to assign in the Inspector (e.g. which tool fixes which target).

---

## License

Scripts in this repo are provided as-is. Asset licenses depend on each asset (Unity Asset Store, CC0, etc.) – check the page for each link above.
