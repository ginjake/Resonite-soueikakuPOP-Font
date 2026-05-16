# SoueiKakuPopFontMod

A ResoniteModLoader mod that tries to force Resonite UI text to use HG Souei Kaku Pop / HG創英角ポップ体 when the font is installed locally.

## Important font license note

This repository does not include HG創英角ポップ体 / `HGRPP1.TTC`.

HG創英角ポップ体 is a RICOH font commonly bundled with some Microsoft Office or Japanese Windows environments, and it is also sold through commercial font distributors. Treat it as a commercial/proprietary font, not a free font. Do not redistribute the font file with this mod unless you have a license that explicitly permits redistribution.

The mod code is MIT licensed. The font is not.

## What happens if the font is missing?

If the font file is not found, the mod leaves Resonite's default fonts unchanged and writes a log message. This is the expected behavior for users who do not own the font, including many overseas users.

## Font lookup order

1. `font_path` in the ResoniteModLoader config.
2. `SoueiKakuPopFontMod.HGRPP1.TTC` next to the mod DLL, for private local use only.
3. `C:\Windows\Fonts\HGRPP1.TTC`.

## Build

Install ResoniteModLoader, then update the reference paths in `SoueiKakuPopFontMod.csproj` if your Resonite profile is not:

`C:\Users\ottoi\AppData\Local\RESO Launcher\profiles\reshade\Game`

Build:

```powershell
dotnet build -c Release
```

The current project file copies the built DLL to:

`C:\Users\ottoi\AppData\Local\RESO Launcher\profiles\reshade\Game\rml_mods`

Only distribute the DLL and source code. Do not distribute `HGRPP1.TTC`.
