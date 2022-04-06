using System;
using System.Collections.Generic;
using System.Text;
using DiskCardGame;
using HarmonyLib;
using OmniBackport.Nodes.Gemify;
using OmniBackport.Nodes.Overclock;
using TDLib.Attributes;
using TDLib.Collections;
using TDLib.Config;

namespace OmniBackport.Patchers {
	[HarmonyPatch(typeof(Part1CardChoiceGenerator))]
	[HarmonyPatch("GenerateDirectChoices")]
	[AutoInit]
	static class AddModsToChoicesPatch {
		static void Init() {
			// init config
			_ = ModApplyChance.GetValue();
		}
		static BasicConfigHelper<double> ModApplyChance = new BasicConfigHelper<double>(MainPlugin.cfg, nameof(ModApplyChance), "The chance to apply a modifier to cards in choice nodes.", 0.1, "Tweaks");
		static void Postfix(List<CardChoice> __result, int randomSeed) {
			Random rand = new Random(randomSeed);
			foreach(var card in __result) {
				if(rand.NextDouble() < ModApplyChance.GetValue()) {
					card.info.Mods.Add(GetMod(card.info, rand.Next()));
				}
			}
		}
		static CardModificationInfo GetMod(CardInfo card, int seed) {
			ResourceType cost;
			if(card.BloodCost == 0 && card.EnergyCost == 0 && card.BonesCost == 0 && card.GemsCost.Count == 0) {
				cost = ResourceType.None;
			} else {
				List<ResourceType> costs = new List<ResourceType>() { ResourceType.Gems, ResourceType.Bone, ResourceType.Blood, ResourceType.Energy };
				costs.Sort((a, b) => {
					int GetVal(ResourceType v) {
						return v == ResourceType.Blood ? card.BloodCost : v == ResourceType.Bone ? card.BonesCost * 3 : v == ResourceType.Energy ? card.EnergyCost * 2 : v == ResourceType.Gems ? card.GemsCost.Count : 0;
					}
					int valA = GetVal(a);
					int valB = GetVal(b);
					return valA - valB;
				});
			cost = costs[costs.Count - 1];
			}

			switch(cost) {
				case ResourceType.Gems: return GemifySequencer.GetGemifyMod();
				case ResourceType.Energy: return OverclockSequencer.GetOverclockMod();
				default: return new List<CardModificationInfo>() { GemifySequencer.GetGemifyMod(), OverclockSequencer.GetOverclockMod() }.GetRandom(seed);
			}
		}
	}
}
