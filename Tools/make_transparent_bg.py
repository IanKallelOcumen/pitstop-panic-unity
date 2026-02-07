#!/usr/bin/env python3
"""
Make white and/or black backgrounds transparent in PNGs.
Use for Gemini-generated assets that have white or black BG instead of alpha.
Run from project root: python Tools/make_transparent_bg.py
Or: python Tools/make_transparent_bg.py "Assets/Art"
"""
import os
import sys

try:
    from PIL import Image
except ImportError:
    print("Install Pillow: pip install Pillow")
    sys.exit(1)

# Pixels with R,G,B all >= WHITE_THRESHOLD become transparent (white BG)
WHITE_THRESHOLD = 250
# Pixels with R,G,B all <= BLACK_THRESHOLD become transparent (black BG, e.g. tools)
BLACK_THRESHOLD = 10

def make_transparent(path: str, white: bool = True, black: bool = True) -> bool:
    img = Image.open(path)
    if img.mode != "RGBA":
        img = img.convert("RGBA")
    data = list(img.getdata())  # (r,g,b,a) per pixel
    w, h = img.size
    new_data = []
    for i, pixel in enumerate(data):
        r, g, b, a = pixel[:4]
        if white and r >= WHITE_THRESHOLD and g >= WHITE_THRESHOLD and b >= WHITE_THRESHOLD:
            new_data.append((r, g, b, 0))
        elif black and r <= BLACK_THRESHOLD and g <= BLACK_THRESHOLD and b <= BLACK_THRESHOLD:
            new_data.append((r, g, b, 0))
        else:
            new_data.append((r, g, b, a))
    img.putdata(new_data)
    img.save(path, "PNG")
    return True

def main():
    root = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
    if len(sys.argv) > 1:
        search_dir = os.path.join(root, sys.argv[1].strip())
    else:
        search_dir = os.path.join(root, "Assets")
    if not os.path.isdir(search_dir):
        print("Directory not found:", search_dir)
        sys.exit(1)
    count = 0
    for dirpath, _dirnames, filenames in os.walk(search_dir):
        for name in filenames:
            if name.lower().endswith(".png"):
                path = os.path.join(dirpath, name)
                try:
                    make_transparent(path)
                    count += 1
                    print("OK (transparent BG):", os.path.relpath(path, root))
                except Exception as e:
                    print("SKIP", path, e)
    print("Done. Processed", count, "PNG(s). Re-import in Unity if needed.")

if __name__ == "__main__":
    main()
