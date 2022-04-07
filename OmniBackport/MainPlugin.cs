using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using DiskCardGame;
using HarmonyLib;
using Infiniscryption.PackManagement;
using InscryptionAPI.Ascension;
using InscryptionAPI.Guid;
using TDLib.Attributes;
using TDLib.FileManagement;
using UnityEngine;

namespace OmniBackport {
	[BepInPlugin(GUID, Name, Version)]
	[BepInDependency("cyantist.inscryption.api")]
	[BepInDependency("io.github.TeamDoodz.TDLib")]
	[BepInDependency("zorro.inscryption.infiniscryption.packmanager", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("extraVoid.inscryption.LifeCost", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("MADH.inscryption.JSONLoader", BepInDependency.DependencyFlags.SoftDependency)]
	public class MainPlugin : BaseUnityPlugin {

		internal const string GUID = "io.github.TeamDoodz." + Name;
		internal const string Name = "OmniBackport";
		internal const string Version = "0.2.2";

		internal static ManualLogSource logger;
		internal static AssetManager assets;
		internal static ConfigFile cfg;

		internal static readonly CardMetaCategory SIDE_DECK_CATEGORY = GuidManager.GetEnumValue<CardMetaCategory>("zorro.inscryption.infiniscryption.sidedecks", "SideDeck");

		private void Awake() {
			logger = Logger;
			assets = new AssetManager(Info);
			cfg = Config;
			var harmony = new Harmony(GUID);
			harmony.PatchAll();
			AutoInitAttribute.CallAllInit(Assembly.GetExecutingAssembly());
			AddCards.AddAllCards(false);
			AddCards.AddAllSideDecks();
			if(Chainloader.PluginInfos.ContainsKey("zorro.inscryption.infiniscryption.packmanager")) {
				AddPacks();
			}
			AddDecks();
			logger.LogMessage($"{Name} v{Version} Loaded!");
			logger.LogDebug(GameColors.Instance.nearWhite);
		}

		private void AddDecks() {
			CreateStarterDeck(GUID, "BonePurity", "Bone Purity", "starterdeck_icon_bones", new List<CardInfo>() {
					CardLoader.GetCardByName("UndeadBackport_Gravedigger"),
					CardLoader.GetCardByName("UndeadBackport_Zombie"),
					CardLoader.GetCardByName("UndeadBackport_FrankNStein"),
				}
			);
			CreateStarterDeck(GUID, "Reanimation", "Reanimation", "starterdeck_icon_reanimation", new List<CardInfo>() {
					CardLoader.GetCardByName("UndeadBackport_TombRobber"),
					CardLoader.GetCardByName("UndeadBackport_Gravedigger"),
					CardLoader.GetCardByName("UndeadBackport_HeadlessHorseman"),
				}
			);
			CreateStarterDeck(GUID, "GreenMox", "Green Mox", "starterdeck_icon_greenmox", new List<CardInfo>() {
					CardLoader.GetCardByName("WizardBackport_JuniorSage"),
					CardLoader.GetCardByName("WizardBackport_JuniorSage"),
					CardLoader.GetCardByName("WizardBackport_StimMage"),
				}
			);
			CreateStarterDeck(GUID, "BlueMox", "Blue Mox", "starterdeck_icon_bluemox", new List<CardInfo>() {
					CardLoader.GetCardByName("WizardBackport_FlyingMage"),
					CardLoader.GetCardByName("WizardBackport_GemFiend"),
					CardLoader.GetCardByName("WizardBackport_Pupil"),
				}
			);
			CreateStarterDeck(GUID, "OrangeMox", "Orange Mox", "starterdeck_icon_orangemox", new List<CardInfo>() {
					CardLoader.GetCardByName("WizardBackport_OrangeMage"),
					CardLoader.GetCardByName("WizardBackport_RubyGolem"),
					CardLoader.GetCardByName("WizardBackport_MageKnight"),
				}
			);
		}

		private static void CreateStarterDeck(string GUID, string name, string title, string PNGPath, List<CardInfo> cards) {
			try {
				StarterDeckInfo conduits = ScriptableObject.CreateInstance<StarterDeckInfo>();
				conduits.name = name;
				conduits.title = title;
				Texture2D texture2D = assets.LoadPNG(PNGPath);
				conduits.iconSprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
				conduits.cards = cards;
				StarterDeckManager.Add(GUID, conduits);
			} catch(Exception ex) {
				logger.LogError($"Error creating deck: {ex}");
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void AddPacks() {
			{
				PackInfo nature = PackManager.GetPackInfo("NatureBackport");
				nature.Title = "Nature";
				nature.Description = "All act 2 exclusive nature cards backported to act 1.";
				nature.ValidFor.Add(PackInfo.PackMetacategory.LeshyPack);
			}
			{
				PackInfo nature = PackManager.GetPackInfo("TechBackport");
				nature.Title = "Tech";
				nature.Description = "All act 2/3 exclusive tech cards backported to act 1.";
				nature.ValidFor.Add(PackInfo.PackMetacategory.LeshyPack);
			}
			{
				PackInfo nature = PackManager.GetPackInfo("UndeadBackport");
				nature.Title = "Undead";
				nature.Description = "All act 2 exclusive undead cards backported to act 1.";
				nature.ValidFor.Add(PackInfo.PackMetacategory.LeshyPack);
			}
			{
				PackInfo nature = PackManager.GetPackInfo("WizardBackport");
				nature.Title = "Wizard";
				nature.Description = "All act 2 exclusive wizard cards backported to act 1.";
				nature.ValidFor.Add(PackInfo.PackMetacategory.LeshyPack);
			}
		}
	}
}
