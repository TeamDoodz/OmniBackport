using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DiskCardGame;
using HarmonyLib;
using TDLib.GameContent;
using UnityEngine;

namespace OmniBackport.Patchers {
	[HarmonyPatch(typeof(GemsDraw))]
	[HarmonyPatch("OnOtherCardResolve")]
	internal static class GemsDrawFix {
		private static IEnumerator Postfix(IEnumerator sequence, GemsDraw __instance) {
			if(SaveManager.SaveFile.IsPart1) {
				yield return __instance.PreSuccessfulTriggerSequence();
				ViewManager.Instance.SwitchToView(View.Hand, false, false);
				yield return new WaitForSeconds(0.1f);
				int numGems = BoardManager.Instance.PlayerSlotsCopy.FindAll((CardSlot x) => 
					x.Card != null && x.Card.Info.HasTrait(Trait.Gem)
				).Count;
				int num;
				for(int i = 0; i < numGems; i = num + 1) {
					yield return CardDrawPiles3D.Instance.DrawCardFromDeck(); // This method places the card in your hand.
					CardDrawPiles3D.Instance.Pile.Draw(); // This method updates the visuals.
					num = i;
				}
				if(numGems > 0) {
					yield return __instance.LearnAbility(0.5f);
				}
				yield break;
			} else {
				yield return sequence;
			}
		}
	}
}
