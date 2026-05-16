using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Elements.Core;
using FrooxEngine;
using FrooxEngine.UIX;
using HarmonyLib;
using ResoniteModLoader;

namespace SoueiKakuPopFontMod
{
	public class SoueiKakuPopFontMod : ResoniteMod
	{
		private const string HarmonyId = "net.ginjake.soueikakupopfont";
		private const string RegisteredFontKey = "SoueiKakuPopFontMod.ForcedFont";
		private const string FontSlotName = "SoueiKakuPopFontMod Font";
		internal const string BundledFontFileName = "SoueiKakuPopFontMod.HGRPP1.TTC";
		private static readonly Uri DefaultFontUri = new Uri(@"C:\Windows\Fonts\HGRPP1.TTC");
		private static readonly Dictionary<World, SoueiKakuPopProceduralFont> FontProviders = new Dictionary<World, SoueiKakuPopProceduralFont>();
		private static readonly HashSet<World> SweepingWorlds = new HashSet<World>();
		private static readonly object StateLock = new object();
		private static bool MissingFontLogged;

		public override string Name => "SoueiKakuPopFontMod";
		public override string Author => "ginjake";
		public override string Version => "1.0.0";
		public override string Link => "";

		public static ModConfiguration Config;

		[AutoRegisterConfigKey]
		public static readonly ModConfigurationKey<bool> Enabled =
			new ModConfigurationKey<bool>("enabled", "Force all Resonite text to use Souei Kaku Pop font", () => true);

		[AutoRegisterConfigKey]
		public static readonly ModConfigurationKey<string> FontPath =
			new ModConfigurationKey<string>("font_path", "Font file path used for forced text rendering", () => @"C:\Windows\Fonts\HGRPP1.TTC");

		public override void OnEngineInit()
		{
			Config = GetConfiguration();
			Config.Save(true);

			var harmony = new Harmony(HarmonyId);
			harmony.PatchAll();
			Msg("Souei Kaku Pop font force patch initialized.");
		}

		public static bool IsEnabled()
		{
			return Config == null || Config.GetValue(Enabled);
		}

		public static IAssetProvider<FontSet> GetForcedFontProvider(World world)
		{
			if (!IsEnabled() || world == null)
				return null;

			lock (StateLock)
			{
				if (FontProviders.TryGetValue(world, out var existing) && existing != null && !existing.IsRemoved)
				{
					EnsureWorldSweep(world);
					return GetAvailableProvider(existing);
				}
			}

			EnsureWorldSweep(world);
			return null;
		}

		public static void ForceTextFont(Text text)
		{
			if (text == null)
				return;

			var provider = GetForcedFontProvider(text.World);
			if (provider != null && text.Font.Target != provider)
				text.Font.Target = provider;
		}

		public static void ForceTextRendererFont(TextRenderer textRenderer)
		{
			if (textRenderer == null)
				return;

			var provider = GetForcedFontProvider(textRenderer.World);
			if (provider != null && textRenderer.Font.Target != provider)
				textRenderer.Font.Target = provider;
		}

		private static void ConfigureFont(SoueiKakuPopProceduralFont font)
		{
		}

		internal static string GetConfiguredFontPath()
		{
			var path = Config?.GetValue(FontPath);
			if (TryNormalizeExistingPath(path, out var normalizedPath))
				return normalizedPath;

			var bundledPath = GetBundledFontPath();
			if (File.Exists(bundledPath))
				return bundledPath;

			if (File.Exists(DefaultFontUri.LocalPath))
				return DefaultFontUri.LocalPath;

			if (!MissingFontLogged)
			{
				MissingFontLogged = true;
				Msg("HG Souei Kaku Pop font was not found. The mod will leave Resonite fonts unchanged. Set font_path in the config to a licensed local font file.");
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

		private static IAssetProvider<FontSet> GetAvailableProvider(SoueiKakuPopProceduralFont provider)
		{
			return provider as IAssetProvider<FontSet>;
		}

		private static string GetBundledFontPath()
		{
			var modPath = Assembly.GetExecutingAssembly().Location;
			var modDirectory = Path.GetDirectoryName(modPath);
			return Path.Combine(modDirectory ?? string.Empty, BundledFontFileName);
		}

		private static SoueiKakuPopProceduralFont CreateWorldFontProvider(World world)
		{
			try
			{
				if (GetConfiguredFontPath() == null)
					return null;

				var root = world.RootSlot;
				if (root == null)
					return null;

				var slot = root.AddSlot(FontSlotName, persistent: false);
				slot.OrderOffset = long.MinValue;

				var font = slot.AttachComponent<SoueiKakuPopProceduralFont>();
				ConfigureFont(font);
				return font;
			}
			catch (Exception ex)
			{
				Error($"Failed to create forced font provider in world {world?.Name}: {ex}");
				return null;
			}
		}

		private static void EnsureWorldSweep(World world)
		{
			if (world == null)
				return;

			lock (StateLock)
			{
				if (!SweepingWorlds.Add(world))
					return;
			}

			world.RunInSeconds(2f, () => SweepWorld(world));
		}

		private static void SweepWorld(World world)
		{
			try
			{
				if (!IsEnabled() || world?.RootSlot == null)
					return;

				var provider = GetOrCreateForcedFontProvider(world);
				if (provider == null)
					return;

				foreach (var text in world.RootSlot.GetComponentsInChildren<Text>())
				{
					if (text != null && !text.IsRemoved && text.Font.Target != provider)
						text.Font.Target = provider;
				}

				foreach (var textRenderer in world.RootSlot.GetComponentsInChildren<TextRenderer>())
				{
					if (textRenderer != null && !textRenderer.IsRemoved && textRenderer.Font.Target != provider)
						textRenderer.Font.Target = provider;
				}
			}
			catch (Exception ex)
			{
				Error($"Failed to sweep world fonts: {ex}");
			}
			finally
			{
				if (IsEnabled() && world != null)
					world.RunInSeconds(2f, () => SweepWorld(world));
			}
		}

		private static IAssetProvider<FontSet> GetOrCreateForcedFontProvider(World world)
		{
			if (!IsEnabled() || world == null)
				return null;

			lock (StateLock)
			{
				if (FontProviders.TryGetValue(world, out var existing) && existing != null && !existing.IsRemoved)
					return GetAvailableProvider(existing);
			}

			var provider = CreateWorldFontProvider(world);
			if (provider == null)
				return null;

			lock (StateLock)
				FontProviders[world] = provider;

			return GetAvailableProvider(provider);
		}
	}

	public class SoueiKakuPopProceduralFont : ProceduralFont
	{
		protected override bool AlwaysLoad => true;

		protected override void AssetCreated(Font asset)
		{
			base.AssetCreated(asset);
			asset.RenderMethod = Elements.Assets.GlyphRenderMethod.MSDF;
		}

		protected override Elements.Assets.FontX CreateFont()
		{
			var path = SoueiKakuPopFontMod.GetConfiguredFontPath();
			if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
				throw new FileNotFoundException("HG Souei Kaku Pop font file was not found. Set font_path to a licensed local font file.");

			var extension = Path.GetExtension(path).TrimStart('.').ToLowerInvariant();
			return Elements.Assets.FontX.Load(path, extension);
		}

		protected override void UpdateFontData(Elements.Assets.FontX font)
		{
			ApplyFontSettings();
		}

		protected override async Task UpdateFontDataAsync(Elements.Assets.FontX font)
		{
			await default(ToBackground);
			ApplyFontSettings();
		}

		private void ApplyFontSettings()
		{
			Asset.GlyphEmSize = 64;
			Asset.PixelRange = 4;
			Asset.Padding = 2;
			Asset.MipMaps = true;
			Asset.MipMapFiltering = Elements.Assets.Filtering.Box;
			Asset.LODBias = -1f;
		}
	}

	[HarmonyPatch(typeof(Text), "OnAttach")]
	internal static class TextOnAttachPatch
	{
		public static void Postfix(Text __instance)
		{
			try
			{
				SoueiKakuPopFontMod.ForceTextFont(__instance);
			}
			catch (Exception ex)
			{
				SoueiKakuPopFontMod.Error($"Failed to force UIX.Text font: {ex}");
			}
		}
	}

	[HarmonyPatch(typeof(TextRenderer), "OnAttach")]
	internal static class TextRendererOnAttachPatch
	{
		public static void Postfix(TextRenderer __instance)
		{
			try
			{
				SoueiKakuPopFontMod.ForceTextRendererFont(__instance);
			}
			catch (Exception ex)
			{
				SoueiKakuPopFontMod.Error($"Failed to force TextRenderer font: {ex}");
			}
		}
	}

	[HarmonyPatch(typeof(TextRenderHelper), nameof(TextRenderHelper.GetDefaultFont))]
	internal static class GetDefaultFontPatch
	{
		public static void Postfix(World world, ref IAssetProvider<FontSet> __result)
		{
			try
			{
				var provider = SoueiKakuPopFontMod.GetForcedFontProvider(world);
				if (provider != null)
					__result = provider;
			}
			catch (Exception ex)
			{
				SoueiKakuPopFontMod.Error($"Failed to replace default font: {ex}");
			}
		}
	}

	[HarmonyPatch(typeof(TextRenderHelper), nameof(TextRenderHelper.GetBolderFont))]
	internal static class GetBolderFontPatch
	{
		public static void Postfix(World world, ref IAssetProvider<FontSet> __result)
		{
			try
			{
				var provider = SoueiKakuPopFontMod.GetForcedFontProvider(world);
				if (provider != null)
					__result = provider;
			}
			catch (Exception ex)
			{
				SoueiKakuPopFontMod.Error($"Failed to replace bolder font: {ex}");
			}
		}
	}

	[HarmonyPatch(typeof(TextRenderHelper), nameof(TextRenderHelper.GetMonospaceFont))]
	internal static class GetMonospaceFontPatch
	{
		public static void Postfix(World world, ref IAssetProvider<FontSet> __result)
		{
			try
			{
				var provider = SoueiKakuPopFontMod.GetForcedFontProvider(world);
				if (provider != null)
					__result = provider;
			}
			catch (Exception ex)
			{
				SoueiKakuPopFontMod.Error($"Failed to replace monospace font: {ex}");
			}
		}
	}

	[HarmonyPatch(typeof(AssetRef<FontSet>), "Target", MethodType.Setter)]
	internal static class FontSetAssetRefTargetPatch
	{
		public static void Prefix(AssetRef<FontSet> __instance, ref IAssetProvider<FontSet> value)
		{
			try
			{
				if (!SoueiKakuPopFontMod.IsEnabled())
					return;

				if (__instance?.Worker is Text text && __instance.Name == "Font")
				{
					var provider = SoueiKakuPopFontMod.GetForcedFontProvider(text.World);
					if (provider != null)
						value = provider;
				}
				else if (__instance?.Worker is TextRenderer textRenderer && __instance.Name == "Font")
				{
					var provider = SoueiKakuPopFontMod.GetForcedFontProvider(textRenderer.World);
					if (provider != null)
						value = provider;
				}
			}
			catch (Exception ex)
			{
				SoueiKakuPopFontMod.Error($"Failed to redirect font reference: {ex}");
			}
		}
	}
}
