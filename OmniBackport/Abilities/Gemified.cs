using System.Collections;
using System.Collections.Generic;
using DiskCardGame;
using InscryptionAPI.Card;
using TDLib.Attributes;
using TDLib.GameContent;
using UnityEngine;

namespace OmniBackport.Abilities {
	[AutoInit]
	public class Gemified : AbilityBehaviour {
		private static void Init() {
			AbilityInfo info = ScriptableObject.CreateInstance<AbilityInfo>();
			info.name = nameof(Gemified);
			info.rulebookName = nameof(Gemified);
			info.rulebookDescription =
				"If its owner controls a green gem, [creature] will gain 1 health.\n" +
				"Also: If its owner controls an orange gem, [creature] will gain 1 power.\n" +
				"Also: If its owner controls a blue gem, when [creature] is played, draw one card from your side deck.";
			info.powerLevel = 3;
			info.opponentUsable = true;
			info.metaCategories = new List<AbilityMetaCategory>() {
				AbilityMetaCategory.Part1Rulebook
			};
			ability = AbilityManager.Add(MainPlugin.GUID, info, typeof(Gemified), MainPlugin.assets.LoadPNG("ability_gemified")).Id;
		}

		public override Ability Ability => ability;
		public static Ability ability { get; private set; }

		private bool resolved = false;

		public override bool RespondsToResolveOnBoard() {
			return true;
		}
		public override IEnumerator OnResolveOnBoard() {
			yield return UpdateMod();
			resolved = true;
		}

		public override bool RespondsToOtherCardResolve(PlayableCard otherCard) {
			return resolved && otherCard.Info.IsGem();
		}
		public override IEnumerator OnOtherCardResolve(PlayableCard otherCard) {
			yield return UpdateMod();
		}

		public override bool RespondsToOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer) {
			return resolved && card.Info.IsGem();
		}
		public override IEnumerator OnOtherCardDie(PlayableCard card, CardSlot deathSlot, bool fromCombat, PlayableCard killer) {
			yield return UpdateMod();
		}

		private IEnumerator UpdateMod() {
			MainPlugin.logger.LogInfo("Updating mod");
			Card.Anim.StrongNegationEffect();

			CardModificationInfo gemMod = Card.TemporaryMods.Find((CardModificationInfo x) => x.singletonId == MOD_ID);
			if(gemMod == null) {
				gemMod = new CardModificationInfo();
				gemMod.singletonId = MOD_ID;
				Card.AddTemporaryMod(gemMod);
			}

			gemMod.healthAdjustment = ResourcesManager.Instance.gems.Contains(GemType.Green) ? 1 : 0;
			gemMod.attackAdjustment = ResourcesManager.Instance.gems.Contains(GemType.Orange) ? 1 : 0;
			Card.OnStatsChanged();

			if(!resolved) {
				yield return new WaitForSeconds(0.4f);
				if(ResourcesManager.Instance.gems.Contains(GemType.Blue) && CardDrawPiles3D.Instance.SidePile.NumCards > 0) {
					MainPlugin.logger.LogInfo("Drawing from side deck");
					if(Singleton<ViewManager>.Instance.CurrentView != View.Default) {
						yield return new WaitForSeconds(0.2f);
						Singleton<ViewManager>.Instance.SwitchToView(View.Default, false, false);
						yield return new WaitForSeconds(0.2f);
					}
					CardDrawPiles3D.Instance.SidePile.Draw();
					yield return CardDrawPiles3D.Instance.DrawFromSidePile();
				}
			}
		}

		public const string MOD_ID = "gemification";
	}
}
