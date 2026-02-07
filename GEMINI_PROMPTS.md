# Gemini prompts – one consistent style (white background)

Use the **same style line** for every image so car, scooter, wheel, garage, outside, and tools all match.

**Shared style line:**  
`Simple 2D game sprite, cartoon style, bold black outlines, flat colors, no gradients, plain white background, centered on canvas, no text or labels, single object only.`

---

## Vehicles (no wheels – you use one wheel asset for both)

Wheels are generated separately so you can animate them when driving. Use **one wheel sprite** for front and rear on both car and scooter.

### Car – broken
```
Simple 2D game sprite, cartoon style, bold black outlines, flat colors, no gradients, plain white background, centered on canvas, no text or labels, single object only. Side view of a small cartoon hatchback car with NO WHEELS (empty wheel wells), hood open, gray smoke or steam from engine, looks broken and needs repair.
```

### Car – fixed
```
Simple 2D game sprite, cartoon style, bold black outlines, flat colors, no gradients, plain white background, centered on canvas, no text or labels, single object only. Side view of a small cartoon hatchback car with NO WHEELS (empty wheel wells), hood closed, clean and fully repaired.
```

### Scooter – broken
```
Simple 2D game sprite, cartoon style, bold black outlines, flat colors, no gradients, plain white background, centered on canvas, no text or labels, single object only. Side view of a cartoon moped or scooter with NO WHEELS (empty space where wheels go), rust or damage, fenders and frame only, needs repair.
```

### Scooter – fixed
```
Simple 2D game sprite, cartoon style, bold black outlines, flat colors, no gradients, plain white background, centered on canvas, no text or labels, single object only. Side view of a cartoon moped or scooter with NO WHEELS (empty space where wheels go), clean frame and fenders, fully repaired.
```

---

## One wheel (use for both vehicles, front and rear)

Generate **once** and use the same sprite for all wheel positions in Unity.

```
Simple 2D game sprite, cartoon style, bold black outlines, flat colors, no gradients, plain white background, centered on canvas, no text or labels, single object only. One car or scooter wheel: dark gray tire with simple tread, light gray rim with five spokes, view straight on (perpendicular to axle).
```

---

## Backgrounds

### Garage (interior)
```
Simple 2D game background, cartoon style, bold black outlines, flat colors, no gradients, plain white or light gray background. Interior of a garage: red brick walls, gray concrete floor with a few oil stains, brown pegboard with wrenches and screwdrivers on the back wall, green storage cabinet on one side, wooden workbench with red toolbox on the other. No vehicles. Wide horizontal or rectangular layout for game background.
```

### Outside (road / test drive)
```
Simple 2D game background, cartoon style, bold black outlines, flat colors, no gradients. Sunny outside: bright blue sky, yellow sun, a few white clouds, green grass strips top and bottom, dark gray two-lane road in the middle with white dashed center line, two or three simple trees and bushes. Wide horizontal layout for driving or victory screen.
```

---

## Tools

Use either **white** or **black** background to match the rest of your assets. Your tools image (2x3 grid) uses black – use the prompts below to get more tools in that style, or slice the existing image in Unity (see end of this section).

### Tools – white background (match car/scooter)

### Wrench
```
Simple 2D game sprite, cartoon style, bold black outlines, flat colors, no gradients, plain white background, centered on canvas, no text or labels, single object only. One adjustable wrench or spanner, metal silver/grey, cartoon mechanic tool.
```

### Screwdriver
```
Simple 2D game sprite, cartoon style, bold black outlines, flat colors, no gradients, plain white background, centered on canvas, no text or labels, single object only. One Phillips or flathead screwdriver, red or yellow handle, silver shaft, cartoon mechanic tool.
```

### Car jack
```
Simple 2D game sprite, cartoon style, bold black outlines, flat colors, no gradients, plain white background, centered on canvas, no text or labels, single object only. One red hydraulic car jack or bottle jack for lifting a car, cartoon mechanic tool.
```

### Oil can
```
Simple 2D game sprite, cartoon style, bold black outlines, flat colors, no gradients, plain white background, centered on canvas, no text or labels, single object only. One red or yellow oil can with long spout, cartoon mechanic style.
```

### Funnel
```
Simple 2D game sprite, cartoon style, bold black outlines, flat colors, no gradients, plain white background, centered on canvas, no text or labels, single object only. One metal funnel for oil or fluid, silver/grey, cartoon mechanic tool.
```

### Multimeter
```
Simple 2D game sprite, cartoon style, bold black outlines, flat colors, no gradients, plain white background, centered on canvas, no text or labels, single object only. One digital multimeter with two probe leads, yellow and black, cartoon mechanic tool.
```

---

### Tools – black background (match your 2×3 tools image)

Same tools, plain **black** background so they match your existing tools sheet.

**Wrench:** `Simple 2D game sprite, cartoon style, bold outlines, flat colors, plain black background, centered, no text. One adjustable wrench, silver/grey metal, cartoon mechanic tool.`

**Screwdriver:** `Simple 2D game sprite, cartoon style, bold outlines, flat colors, plain black background, centered, no text. One flathead screwdriver, red handle, silver shaft, cartoon mechanic tool.`

**Oil can:** `Simple 2D game sprite, cartoon style, bold outlines, flat colors, plain black background, centered, no text. One red oil can with yellow label and long silver spout, cartoon mechanic style.`

**Jack:** `Simple 2D game sprite, cartoon style, bold outlines, flat colors, plain black background, centered, no text. One red hydraulic bottle jack with grey base and silver cap, cartoon mechanic tool.`

**Funnel:** `Simple 2D game sprite, cartoon style, bold outlines, flat colors, plain black background, centered, no text. One silver metal funnel, wide cone to narrow spout, cartoon mechanic tool.`

**Multimeter:** `Simple 2D game sprite, cartoon style, bold outlines, flat colors, plain black background, centered, no text. One yellow digital multimeter with grey face, dial, and red and black probe leads, cartoon mechanic tool.`

---

### Using the tools image in Unity

If you have one image with all 6 tools in a **2×3 grid** (e.g. wrench, screwdriver, oil can top row; jack, funnel, multimeter bottom row):

1. Import the image into Unity (e.g. `Assets/Sprites/Tools.png`).
2. Select it → Inspector → **Texture Type** = **Sprite (2D and UI)**.
3. **Sprite Mode** = **Multiple**.
4. Click **Sprite Editor** → **Slice** → **Grid By Cell Count** → Columns **3**, Rows **2** → **Slice**.
5. Name each slice (e.g. `Tools_0` = Wrench, `Tools_1` = Screwdriver, …) and **Apply**.
6. In your Garage scene, create 6 UI Images; assign each slice as the sprite for one **DraggableTool** and set **toolType** (e.g. Wrench, Screwdriver, OilCan, Jack, Funnel, Multimeter).

---

## Using one wheel in Unity

- Import the **wheel** sprite once (e.g. `Wheel.png`).
- For **car**: create two child sprites (front wheel, rear wheel), both using the same `Wheel` sprite. Position them in the wheel wells; rotate them in code for drive animation.
- For **scooter**: same idea – two child sprites using the same `Wheel` sprite.
- One asset, multiple instances keeps the look consistent and file size down.
