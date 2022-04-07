using System;
using System.IO;
using System.Text;
using BepInEx;
using BepInEx.Logging;
using DiskCardGame;
using InscryptionAPI.Card;
using OmniBackport.ML;
using TDLib.FileManagement;

namespace MLDatasetGenerator {
	[BepInPlugin(GUID, Name, Version)]
	[BepInDependency("cyantist.inscryption.api")]
	[BepInDependency("io.github.TeamDoodz.TDLib")]
	[BepInDependency("io.github.TeamDoodz.OmniBackport")]
	public class MainPlugin : BaseUnityPlugin {

		internal const string GUID = "io.github.TeamDoodz." + Name;
		internal const string Name = "MLDatasetGenerator";
		internal const string Version = "1.0.0";

		internal static ManualLogSource logger;

		private void Awake() {
			logger = Logger;
			logger.LogMessage($"{Name} v{Version} Loaded!");
			string path = new AssetManager(Info).PathFor("cardData", "tsv");
			using(StreamWriter sw = File.CreateText(path)) {
				foreach(var card in CardManager.BaseGameCards) {
					if((card.temple == CardTemple.Tech && card.metaCategories.Contains(CardMetaCategory.ChoiceNode)) || card.metaCategories.Contains(CardMetaCategory.Part3Random)) {
						logger.LogInfo($"Adding data from card {card.name} to dataset");
						sw.WriteLine(DatasetEntry.FromCard(card).ToString());
					}
				}
			}
			logger.LogMessage($"Saved file as {path}");
		}

	}
}
