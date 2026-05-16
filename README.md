# SoueiKakuPopFontMod

A ResoniteModLoader mod that forces Resonite UI text to use **HG Souei Kaku Pop** (`HGRPP1.TTC`) when that font is available on the user's PC.

## English

### Requirements

- ResoniteModLoader
- A legally obtained copy of **HG Souei Kaku Pop** / **HG Soei Kakupoptai**

This mod does **not** include the font file.

HG Souei Kaku Pop is not a free font. It is a RICOH commercial/proprietary font that may be bundled with some Microsoft Office or Japanese Windows environments, and it is also sold by font distributors.

Do not redistribute `HGRPP1.TTC` with this mod unless your font license explicitly permits redistribution.

### Installation

1. Install ResoniteModLoader.
2. Download `SoueiKakuPopFontMod.dll` from the release page.
3. Put the DLL into your Resonite `rml_mods` folder.
4. Make sure HG Souei Kaku Pop is installed or configured.
5. Start Resonite.

Typical `rml_mods` locations:

```text
C:\Program Files (x86)\Steam\steamapps\common\Resonite\rml_mods
C:\Users\<you>\AppData\Local\RESO Launcher\profiles\<profile>\Game\rml_mods
```

### Font Setup

The mod checks for the font in this order:

1. `font_path` in the ResoniteModLoader config.
2. `SoueiKakuPopFontMod.HGRPP1.TTC` next to `SoueiKakuPopFontMod.dll`.
3. `C:\Windows\Fonts\HGRPP1.TTC`.

If the font is missing, the mod leaves Resonite's default fonts unchanged and writes a log message.

To get the font legally, check whether your Windows or Microsoft Office installation already includes `HGRPP1.TTC`, or buy/license the font from a legitimate distributor. Search for `HG Soei Kakupoptai`, `HG Souei Kaku Pop`, or `HGRPP1.TTC`.

References:

- RICOH font licensing: https://industry.ricoh.com/en/font/license_po/
- RICOH PC font page: https://industry.ricoh.com/en/font/pc/
- MyFonts HG Soei Kakupoptai: https://www.myfonts.com/collections/hg-soei-kakupoptai-font-ricoh

## 日本語

### 必要なもの

- ResoniteModLoader
- 正規に入手した **HG Souei Kaku Pop** / **HG Soei Kakupoptai** / `HGRPP1.TTC`

このModにはフォントファイルは含まれていません。

HG Souei Kaku Pop、いわゆる創英角ポップ体はフリーフォントではありません。RICOH系の商用/プロプライエタリなフォントで、一部のMicrosoft Officeや日本語Windows環境に含まれている場合があります。また、フォント販売サイトで購入できる場合もあります。

フォントのライセンスで明示的に再配布が許可されていない限り、`HGRPP1.TTC` をこのModに同梱して配布しないでください。

### 導入方法

1. ResoniteModLoaderを導入します。
2. リリースページから `SoueiKakuPopFontMod.dll` をダウンロードします。
3. DLLをResoniteの `rml_mods` フォルダに入れます。
4. HG Souei Kaku Pop / `HGRPP1.TTC` がPCに存在するか、Mod設定で指定されていることを確認します。
5. Resoniteを起動します。

代表的な `rml_mods` の場所:

```text
C:\Program Files (x86)\Steam\steamapps\common\Resonite\rml_mods
C:\Users\<you>\AppData\Local\RESO Launcher\profiles\<profile>\Game\rml_mods
```

### フォントの設定

Modは次の順番でフォントを探します。

1. ResoniteModLoaderの設定にある `font_path`
2. `SoueiKakuPopFontMod.dll` と同じ場所にある `SoueiKakuPopFontMod.HGRPP1.TTC`
3. `C:\Windows\Fonts\HGRPP1.TTC`

フォントが見つからない場合、Resonite標準フォントのままになり、ログにメッセージを出します。UIが消えたりクラッシュしたりしないようにしています。

手動で入手する場合は、まず自分のWindowsやMicrosoft Officeに `HGRPP1.TTC` が含まれているか確認してください。無い場合は、正規のフォント販売サイトなどで `HG Soei Kakupoptai`、`HG Souei Kaku Pop`、`HGRPP1.TTC` などの名前で探してください。
