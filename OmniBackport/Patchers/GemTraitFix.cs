using System;
using System.Collections.Generic;
using System.Text;
using DiskCardGame;
using HarmonyLib;
using TDLib.GameContent;

namespace OmniBackport.Patchers {
	[HarmonyPatch(typeof(CardInfo))]
	[HarmonyPatch("HasTrait")]
	static class GemTraitFix {
		static void Postfix(CardInfo __instance, bool __result, Trait trait) {
			if(trait == Trait.Gem && __instance.IsGem()) __result = true; 
		}
	}
}
