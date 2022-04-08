using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DiskCardGame;
using HarmonyLib;

namespace OmniBackport.Patchers {
	[HarmonyPatch(typeof(Loot))]
	[HarmonyPatch("OnDealDamageDirectly")]
	static class LootFix {
		static IEnumerator Postfix(IEnumerator sequence, Loot __instance, int amount) {
			//yield return __instance.PreSuccessfulTriggerSequence();
			ViewManager.Instance.SwitchToView(View.Hand, false, true);
			MainPlugin.logger.LogInfo($"Drawing {amount} cards");
			int num;
			for(int i = 0; i < amount; i = num + 1) {
				if(CardDrawPiles3D.Instance.Pile.NumCards > 0) {
					MainPlugin.logger.LogInfo($"Drawing from main deck");
					CardDrawPiles3D.Instance.Pile.Draw();
					yield return CardDrawPiles3D.Instance.DrawCardFromDeck(null, null);
				} else if (CardDrawPiles3D.Instance.SidePile.NumCards > 0) {
					MainPlugin.logger.LogInfo($"Drawing from side deck");
					yield return CardDrawPiles3D.Instance.DrawFromSidePile();
					CardDrawPiles3D.Instance.SidePile.Draw();
				} else {
					MainPlugin.logger.LogInfo($"Not drawing");
					break;
				}
				num = i;
			}
			ViewManager.Instance.Controller.LockState = ViewLockState.Unlocked;
			yield return __instance.LearnAbility(0f);
			yield break;
		}
	}
}
