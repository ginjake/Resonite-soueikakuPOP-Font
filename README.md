# SoueiKakuPopFontMod

A ResoniteModLoader mod that forces Resonite UI text to use **HG Souei Kaku Pop** (`HGRPP1.TTC`) when that font is available on the user's PC.

## Requirements

- ResoniteModLoader
- A legally obtained copy of **HG Souei Kaku Pop** / **HG Soei Kakupoptai**

This repository and release packages do **not** include the font file.

## Font License

HG Souei Kaku Pop is not a free font. It is a RICOH commercial/proprietary font that may be bundled with some Microsoft Office or Japanese Windows environments, and it is also sold by font distributors.

The mod source code is MIT licensed. The font itself is not.

Do not redistribute `HGRPP1.TTC` with this mod unless your font license explicitly permits redistribution.

## Installation

1. Install ResoniteModLoader.
2. Download `SoueiKakuPopFontMod.dll` from the release page.
3. Put the DLL into your Resonite `rml_mods` folder.

Typical locations:

```text
C:\Program Files (x86)\Steam\steamapps\common\Resonite\rml_mods
```

or, for RESO Launcher profiles:

```text
C:\Users\<you>\AppData\Local\RESO Launcher\profiles\<profile>\Game\rml_mods
```

4. Make sure HG Souei Kaku Pop is installed or configured. See below.
5. Start Resonite.

## How to Provide the Font

The mod checks for the font in this order:

1. `font_path` in the ResoniteModLoader config.
2. `SoueiKakuPopFontMod.HGRPP1.TTC` next to `SoueiKakuPopFontMod.dll`.
3. `C:\Windows\Fonts\HGRPP1.TTC`.

Recommended options:

- If you already own the font and it is installed in Windows, no extra setup may be needed.
- If the font is in a different location, set `font_path` in the mod config to the full local path.
- For private local use, you can place your own licensed copy named `SoueiKakuPopFontMod.HGRPP1.TTC` next to the DLL. Do not upload or redistribute that file.

## How to Get the Font Legally

Check whether your existing Windows or Microsoft Office installation already includes `HGRPP1.TTC`.

You can also buy/license the font from legitimate distributors. Search for:

- `HG Soei Kakupoptai`
- `HG Souei Kaku Pop`
- `HGRPP1.TTC`

Known references:

- RICOH font licensing: https://industry.ricoh.com/en/font/license_po/
- RICOH PC font page: https://industry.ricoh.com/en/font/pc/
- MyFonts HG Soei Kakupoptai: https://www.myfonts.com/collections/hg-soei-kakupoptai-font-ricoh

## If the Font Is Missing

The mod will leave Resonite's default fonts unchanged and write a log message. It should not crash or blank the UI.

This means many users outside Japan may see no change until they install or configure a licensed local copy of the font.

## Build

Update the reference paths in `SoueiKakuPopFontMod.csproj` if your Resonite profile is not:

```text
C:\Users\ottoi\AppData\Local\RESO Launcher\profiles\reshade\Game
```

Build:

```powershell
dotnet build -c Release
```

The current project file copies the built DLL to:

```text
C:\Users\ottoi\AppData\Local\RESO Launcher\profiles\reshade\Game\rml_mods
```

Only distribute the DLL and source code. Do not distribute `HGRPP1.TTC`.
