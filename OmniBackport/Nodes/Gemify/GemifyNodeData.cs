using System.Collections.Generic;
using System.Linq;
using BepInEx.Bootstrap;
using DiskCardGame;
using InscryptionAPI.Encounters;
using InscryptionAPI.Saves;
using TDLib.Attributes;
using TDLib.Config;
using TDLib.GameContent;
using UnityEngine;

namespace OmniBackport.Nodes.Gemify {
	[AutoInit]
	public class GemifyNodeData : CustomNodeData {
		private static BasicConfigHelper<bool> ForceSpawnGemify = new BasicConfigHelper<bool>(MainPlugin.cfg, nameof(ForceSpawnGemify), "Allow the Gemify node to spawn regardless of how many gem cards the player has in their deck/side deck.", false, "Nodes.Gemify");
		public override void Initialize() {
			AddGenerationPrerequisite(() => {
				if(ForceSpawnGemify.GetValue()) {
					MainPlugin.logger.LogDebug($"{nameof(ForceSpawnGemify)} is on");
					return true;
				}

				if(RunState.Run.playerDeck.Cards.Any((x) => x.IsGem())) {
					MainPlugin.logger.LogDebug($"Player has at least one gem in deck");
					return true;
				}

				CardInfo cardToCheck = null;
				if(Chainloader.PluginInfos.ContainsKey("zorro.inscryption.infiniscryption.sidedecks")) {
					cardToCheck = CardLoader.GetCardByName(ModdedSaveManager.SaveData.GetValue("zorro.inscryption.infiniscryption.sidedecks", "SideDeck.SelectedDeck"));
				} else {
					cardToCheck = AscensionSaveData.Data.ChallengeIsActive(AscensionChallenge.SubmergeSquirrels) ? CardLoader.GetCardByName("AquaSquirrel") : CardLoader.GetCardByName("Squirrel");
				}
				if(cardToCheck.IsGem() || cardToCheck.traits.Contains(Trait.Gem)) {
					MainPlugin.logger.LogDebug($"Side deck card ({cardToCheck.name}) is a gem");
					return true;
				}

				MainPlugin.logger.LogDebug($"Will not be spawning the gemify node");
				return false;
			});
		}
		private static void Init() {
			NodeManager.Add<GemifySequencer>(Animation.ToArray(), NodeManager.NodePosition.SpecialEventRandom);
			// init conifg
			_ = ForceSpawnGemify.GetValue();
		}
		public static List<Texture2D> Animation => new List<Texture2D>() {
			MainPlugin.assets.LoadPNG("animated_gemifynode_1"),
			MainPlugin.assets.LoadPNG("animated_gemifynode_2"),
			MainPlugin.assets.LoadPNG("animated_gemifynode_3"),
			MainPlugin.assets.LoadPNG("animated_gemifynode_4"),
		};
	}
}
