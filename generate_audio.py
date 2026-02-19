import wave
import math
import struct
import random
import os

os.makedirs("Assets/Audio", exist_ok=True)

def create_wave_file(filename, duration, freq, volume=0.5, wave_type="sine"):
    sample_rate = 44100
    n_samples = int(sample_rate * duration)
    
    try:
        with wave.open(filename, 'w') as wav_file:
            wav_file.setnchannels(1) # Mono
            wav_file.setsampwidth(2) # 2 bytes per sample (16-bit)
            wav_file.setframerate(sample_rate)
            
            for i in range(n_samples):
                t = float(i) / sample_rate
                
                if wave_type == "sine":
                    val = math.sin(2.0 * math.pi * freq * t)
                elif wave_type == "square":
                    val = 1.0 if math.sin(2.0 * math.pi * freq * t) > 0 else -1.0
                elif wave_type == "saw":
                    val = 2.0 * (t * freq - math.floor(t * freq + 0.5))
                elif wave_type == "noise":
                    val = random.uniform(-1, 1)
                else:
                    val = 0
                
                # Apply envelope (simple fade out)
                envelope = 1.0 - (i / n_samples)
                
                sample = int(val * volume * envelope * 32767.0)
                wav_file.writeframes(struct.pack('<h', sample))
        print(f"Generated {filename}")
    except Exception as e:
        print(f"Error generating {filename}: {e}")

def create_music_file(filename, duration):
    sample_rate = 44100
    bpm = 120
    beat_duration = 60 / bpm
    total_beats = int(duration / beat_duration)
    
    try:
        with wave.open(filename, 'w') as wav_file:
            wav_file.setnchannels(1)
            wav_file.setsampwidth(2)
            wav_file.setframerate(sample_rate)
            
            melody = [440, 523, 659, 587, 523, 440, 392, 440] # A C E D C A G A
            
            samples = []
            for beat in range(total_beats):
                freq = melody[beat % len(melody)]
                n_samples_beat = int(sample_rate * beat_duration)
                
                for i in range(n_samples_beat):
                    t = float(i) / sample_rate
                    val = math.sin(2.0 * math.pi * freq * t) * 0.3
                    sample = int(val * 32767.0)
                    wav_file.writeframes(struct.pack('<h', sample))
        print(f"Generated {filename}")
    except Exception as e:
        print(f"Error generating {filename}: {e}")

if __name__ == "__main__":
    print("Generating audio...")
    create_wave_file("Assets/Audio/Click.wav", 0.1, 800, 0.5, "sine")
    create_wave_file("Assets/Audio/RepairSuccess.wav", 0.3, 1200, 0.6, "square")
    create_wave_file("Assets/Audio/Win.wav", 1.0, 600, 0.7, "saw")
    create_music_file("Assets/Audio/Music.wav", 10.0)
    print("Done.")
