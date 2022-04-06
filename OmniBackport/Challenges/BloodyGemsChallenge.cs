using System.Collections.Generic;
using DiskCardGame;
using InscryptionAPI.Ascension;
using InscryptionAPI.Card;
using TDLib.Attributes;
using TDLib.GameContent;
using UnityEngine;

namespace OmniBackport.Challenges {
	[AutoInit]
	public static class BloodyGemsChallenge {
		public static AscensionChallenge Id { get; private set; } = AscensionChallenge.None;
		static void Init() {
			AscensionChallengeInfo bloodyGems = ScriptableObject.CreateInstance<AscensionChallengeInfo>();
			bloodyGems.name = nameof(BloodyGemsChallenge);
			bloodyGems.title = "Bloody Gems";
			bloodyGems.description = "Gem cards can be sacrificed.";
			bloodyGems.pointValue = -5;
			Id = ChallengeManager.Add(MainPlugin.GUID, bloodyGems).challengeType;
			CardManager.ModifyCardList += UpdateAllGemsCards;
		}

		static List<CardInfo> UpdateAllGemsCards(List<CardInfo> cards) {
			if(Id == AscensionChallenge.None) return cards;
			if(!AscensionSaveData.Data.ChallengeIsActive(Id)) return cards;

			MainPlugin.logger.LogDebug("Modifying gems cards");

			foreach(var card in cards) {
				if(card.IsGem() || card.HasTrait(Trait.Gem)) {
					card.traits.Remove(Trait.Terrain);
					card.appearanceBehaviour.Remove(CardAppearanceBehaviour.Appearance.TerrainLayout);
				}
			}

			return cards;
		}
	}
}
