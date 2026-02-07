# Tools

## Make PNG backgrounds transparent

Assets with **white** or **black** backgrounds (e.g. from Gemini) can be given real transparency so they look correct in Unity.

### 1. Run the script

From the **project root** (the folder that contains `Assets` and `Tools`):

```bash
pip install -r Tools/requirements.txt
python Tools/make_transparent_bg.py
```

This processes all PNGs under `Assets/` (Art, Sprites, etc.).

To limit to one folder:

```bash
python Tools/make_transparent_bg.py "Assets/Art"
python Tools/make_transparent_bg.py "Assets/Sprites"
```

- **White** pixels (R,G,B ≥ 250) → transparent  
- **Black** pixels (R,G,B ≤ 10) → transparent  

The script overwrites the PNGs. Back them up first if needed.

### 2. In Unity

- **Re-import**: right‑click the texture in the Project window → **Reimport**, or let Unity detect the file change.
- **Transparency** is enforced for PNGs in `Assets/Art/` and `Assets/Sprites/` by **TextureImportTransparent** (see `Assets/Editor/TextureImportTransparent.cs`): `alphaIsTransparency` is set so alpha is used in the scene.

### 3. In the scene

- **UI Image**: use **Canvas Group** or leave default; the Image uses the sprite’s alpha.
- If the sprite still shows a solid background, the PNG may not have had white/black in that area—run the script again after adding new assets, or adjust the thresholds in `make_transparent_bg.py` (e.g. `WHITE_THRESHOLD = 240` for more aggressive transparency).
