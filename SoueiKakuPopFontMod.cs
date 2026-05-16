using System;
using System.IO;
using System.Reflection;
using Elements.Assets;
using FrooxEngine;
using HarmonyLib;
using ResoniteModLoader;

namespace SoueiKakuPopFontMod
{
	public class SoueiKakuPopFontMod : ResoniteMod
	{
		private const string HarmonyId = "net.ginjake.soueikakupopfont";
		internal const string BundledFontFileName = "SoueiKakuPopFontMod.HGRPP1.TTC";
		private static readonly string DefaultFontPath = @"C:\Windows\Fonts\HGRPP1.TTC";
		private static readonly object FontLock = new object();
		private static FontX ForcedFont;
		private static string LoadedFontPath;
		private static bool MissingFontLogged;
		private static bool LoadErrorLogged;
		[ThreadStatic]
		private static int SuppressOverrideDepth;

		public override string Name => "SoueiKakuPopFontMod";
		public override string Author => "ginjake";
		public override string Version => "1.1.0";
		public override string Link => "https://github.com/ginjake/Resonite-soueikakuPOP-Font";

		public static ModConfiguration Config;

		[AutoRegisterConfigKey]
		public static readonly ModConfigurationKey<bool> Enabled =
			new ModConfigurationKey<bool>("enabled", "Force Resonite text rendering to use Souei Kaku Pop font locally", () => true);

		[AutoRegisterConfigKey]
		public static readonly ModConfigurationKey<string> FontPath =
			new ModConfigurationKey<string>("font_path", "Font file path used for local text rendering", () => DefaultFontPath);

		public override void OnEngineInit()
		{
			Config = GetConfiguration();
			Config.Save(true);

			var harmony = new Harmony(HarmonyId);
			harmony.PatchAll();
			Msg("Souei Kaku Pop local font render override initialized.");
		}

		public static bool IsEnabled()
		{
			return Config == null || Config.GetValue(Enabled);
		}

		internal static FontX GetForcedFontOrOriginal(FontX original)
		{
			if (SuppressOverrideDepth > 0 || !IsEnabled() || original == null)
				return original;

			if (original.NamedGlyphCount > 0)
				return original;

			var forcedFont = GetForcedFont();
			return forcedFont ?? original;
		}

		internal static void SuppressFontOverride()
		{
			SuppressOverrideDepth++;
		}

		internal static void RestoreFontOverride()
		{
			if (SuppressOverrideDepth > 0)
				SuppressOverrideDepth--;
		}

		private static FontX GetForcedFont()
		{
			var path = GetConfiguredFontPath();
			if (string.IsNullOrWhiteSpace(path))
				return null;

			lock (FontLock)
			{
				if (ForcedFont != null && string.Equals(LoadedFontPath, path, StringComparison.OrdinalIgnoreCase))
					return ForcedFont;

				try
				{
					ForcedFont?.Dispose();
					var extension = Path.GetExtension(path).TrimStart('.').ToLowerInvariant();
					ForcedFont = FontX.Load(path, extension);
					LoadedFontPath = path;
					LoadErrorLogged = false;
					Msg($"Loaded local font override: {path}");
				}
				catch (Exception ex)
				{
					ForcedFont = null;
					LoadedFontPath = null;
					if (!LoadErrorLogged)
					{
						LoadErrorLogged = true;
						Error($"Failed to load local font override from {path}: {ex}");
					}
				}

				return ForcedFont;
			}
		}

		private static string GetConfiguredFontPath()
		{
			var path = Config?.GetValue(FontPath);
			if (TryNormalizeExistingPath(path, out var normalizedPath))
				return normalizedPath;

			var bundledPath = GetBundledFontPath();
			if (File.Exists(bundledPath))
				return bundledPath;

			if (File.Exists(DefaultFontPath))
				return DefaultFontPath;

			if (!MissingFontLogged)
			{
				MissingFontLogged = true;
				Msg("HG Souei Kaku Pop font was not found. The mod will leave Resonite fonts unchanged.");
			}

			return null;
		}

		private static bool TryNormalizeExistingPath(string path, out string normalizedPath)
		{
			normalizedPath = null;
			if (string.IsNullOrWhiteSpace(path))
				return false;

			try
			{
				var expandedPath = Environment.ExpandEnvironmentVariables(path);
				if (!Path.IsPathRooted(expandedPath))
					expandedPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, expandedPath);

				if (!File.Exists(expandedPath))
					return false;

				normalizedPath = Path.GetFullPath(expandedPath);
				return true;
			}
			catch
			{
				return false;
			}
		}

		private static string GetBundledFontPath()
		{
			var modPath = Assembly.GetExecutingAssembly().Location;
			var modDirectory = Path.GetDirectoryName(modPath);
			return Path.Combine(modDirectory ?? string.Empty, BundledFontFileName);
		}
	}

	[HarmonyPatch(typeof(Font), "get_Data")]
	internal static class FontDataGetterPatch
	{
		public static void Postfix(ref FontX __result)
		{
			try
			{
				__result = SoueiKakuPopFontMod.GetForcedFontOrOriginal(__result);
			}
			catch (Exception ex)
			{
				SoueiKakuPopFontMod.Error($"Failed to override font data locally: {ex}");
			}
		}
	}

	[HarmonyPatch(typeof(Font), nameof(Font.Unload))]
	internal static class FontUnloadPatch
	{
		public static void Prefix()
		{
			SoueiKakuPopFontMod.SuppressFontOverride();
		}

		public static void Finalizer()
		{
			SoueiKakuPopFontMod.RestoreFontOverride();
		}
	}

	[HarmonyPatch(typeof(Font), nameof(Font.SetFontData))]
	internal static class FontSetFontDataPatch
	{
		public static void Prefix()
		{
			SoueiKakuPopFontMod.SuppressFontOverride();
		}

		public static void Finalizer()
		{
			SoueiKakuPopFontMod.RestoreFontOverride();
		}
	}
}
