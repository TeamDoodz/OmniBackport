using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Saves;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using TDLib.Collections;
using UnityEngine;

namespace OmniBackport.Patchers {
	[HarmonyPatch]
	public static class CorpseCardsPatch {
		public const string CorpseModID = "CORPSE_CARD";
		public static List<KeyValuePair<string, CardModificationInfo>> BoneLordCards = null;
		public static readonly List<Ability> DoNotNegate = new List<Ability>() { 
			Ability.SplitStrike,
			Ability.TriStrike,
			Ability.CellTriStrike,
			Ability.DoubleStrike
		};

		public static CardModificationInfo CreateCorpseModForCard(CardInfo card) {
			CardModificationInfo outp = new CardModificationInfo();
			outp.negateAbilities = card.Abilities;
			outp.abilities = new List<Ability>() { Ability.Brittle };
			outp.bonesCostAdjustment = Mathf.CeilToInt((card.BonesCost + card.BloodCost * 3f + card.GemsCost.Count * 3f + (card.EnergyCost * 3f) / 2f) / 2f) - card.BonesCost;
			outp.bloodCostAdjustment = -card.BloodCost;
			outp.energyCostAdjustment = -card.EnergyCost;
			outp.nullifyGemsCost = true;
			outp.nameReplacement = $"{card.DisplayedNameLocalized} Corpse";
			outp.singletonId = CorpseModID;
			return outp;
		}

		public static CardInfo GetRandomCorpseCard(int seed) {
			if(BoneLordCards == null) LoadFromDisk();
			KeyValuePair<string, CardModificationInfo> data = BoneLordCards.GetRandom(++seed);
			BoneLordCards.Remove(data);
			CardInfo outp = CardLoader.GetCardByName(data.Key); // GetCardByName already clones things, so we dont need to clone the output
			outp.Mods.Add(data.Value);
			outp.Mods.Add(CreateCorpseModForCard(outp));
			outp.appearanceBehaviour.Add(AppearanceBehaviours.StoneCardBackground.Id);
			return outp;
		}

		public static void AddCardToPool(CardInfo card) {
			MainPlugin.logger.LogInfo($"Adding card {card.name} to pool");
			BoneLordCards.Add(new KeyValuePair<string, CardModificationInfo>(card.name, new CardModificationInfo(card.Attack - card.baseAttack, card.Health - card.baseHealth)));
		}

		#region SaveData

		private static void LoadFromDisk() {
			MainPlugin.logger.LogInfo($"Loading state of {nameof(BoneLordCards)}");
			string data = ModdedSaveManager.RunState.GetValue(MainPlugin.GUID, nameof(BoneLordCards));
			BoneLordCards = DecodeString(data);
			MainPlugin.logger.LogInfo($"There are {BoneLordCards.Count} cards in the pool");
		}
		public static void SaveToDisk() {
			MainPlugin.logger.LogInfo($"Saving state of {nameof(BoneLordCards)}");
			if(BoneLordCards == null) BoneLordCards = new List<KeyValuePair<string, CardModificationInfo>>();
			ModdedSaveManager.RunState.SetValue(MainPlugin.GUID, nameof(BoneLordCards), EncodeString(BoneLordCards));
		}

		private static string EncodeString(List<KeyValuePair<string, CardModificationInfo>> data) {
			if(data.Count == 0) return "";
			StringBuilder sb = new StringBuilder();
			foreach(var entry in data) {
				string cardName = entry.Key;
				int attack = entry.Value.attackAdjustment;
				int health = entry.Value.healthAdjustment;
				sb.Append($"{{{cardName}|{attack}|{health}}},");
			}
			sb.Remove(sb.Length - 1, 1);
			MainPlugin.logger.LogDebug($"Encoding string {sb}");
			return sb.ToString();
		}
		private static List<KeyValuePair<string, CardModificationInfo>> DecodeString(string str) {
			MainPlugin.logger.LogDebug($"Decoding string {str}");
			if(string.IsNullOrEmpty(str)) return new List<KeyValuePair<string, CardModificationInfo>>();

			List<string> split = Regex.Split(str, ",").ToList();
			MainPlugin.logger.LogDebug($"Contents of split by comma:");
			split.ForEach(x => MainPlugin.logger.LogDebug(x));

			List<List<string>> split2 = split.Select(x => Regex.Split(x, @"\|").ToList()).ToList();
			List<List<string>> cleanedSplit2 = split2.Select(x => {
				return x.Select(y => Regex.Replace(y, @"{|}", "")).ToList();
			}).ToList();
			MainPlugin.logger.LogDebug($"Contents of split by pipe:");
			cleanedSplit2.ForEach(x => {
				MainPlugin.logger.LogDebug("Contents of split:");
				x.ForEach(y => MainPlugin.logger.LogDebug(y));
			});

			List<(string, int, int)> split3 = cleanedSplit2.Select(x => {
				string outpName = x[0];
				MainPlugin.logger.LogDebug($"Name: {outpName}");
				int outpAttack = int.Parse(x[1]);
				MainPlugin.logger.LogDebug($"Attack: {outpAttack}");
				int outpHealth = int.Parse(x[2]);
				MainPlugin.logger.LogDebug($"Health: {outpHealth}");
				return (outpName, outpAttack, outpHealth);
			}).ToList();
			return split3.Select(x => {
				return new KeyValuePair<string, CardModificationInfo>(x.Item1, new CardModificationInfo(x.Item2, x.Item3));
			}).ToList();
		}

		#endregion

		#region PatchMethods

		[HarmonyPatch(typeof(Card))]
		[HarmonyPatch("ApplyAppearanceBehaviours")]
		[HarmonyPrefix]
		static void ApplyAppearanceOnCardRender(List<CardAppearanceBehaviour.Appearance> appearances, Card __instance) {
			if(__instance.Info.Mods.Any((x) => x.singletonId == CorpseModID) && !appearances.Contains(AppearanceBehaviours.StoneCardBackground.Id)) {
				MainPlugin.logger.LogDebug($"Adding stone card appearance to card {__instance}");
				appearances.Add(AppearanceBehaviours.StoneCardBackground.Id);
			}
		}

		[HarmonyPatch(typeof(Part1CardChoiceGenerator))]
		[HarmonyPatch("GenerateDirectChoices")]
		[HarmonyPostfix]
		static void AddCorpseCardToChoice(List<CardChoice> __result, int randomSeed) {
			if(SeededRandom.Range(0, 5, randomSeed) == 0) {
				MainPlugin.logger.LogInfo($"Adding corpse card to this choice");
				LoadFromDisk();
				if(BoneLordCards.Count > 0) {
					__result[SeededRandom.Range(0, __result.Count, randomSeed)].CardInfo = GetRandomCorpseCard(randomSeed);
				} else {
					MainPlugin.logger.LogInfo($"Nevermind, there are no corpse cards");
				}
			}
		}

		// Thanks a lot to Aaron from the discord for helping me out with this.
		[HarmonyPatch(typeof(SpecialNodeHandler), nameof(SpecialNodeHandler.StartSpecialNodeSequence))]
		[HarmonyILManipulator]
		private static void KindOfAPostfix(ILContext il) {
			ILCursor c = new ILCursor(il);

			c.GotoNext(MoveType.After,
				x => x.MatchCallOrCallvirt(AccessTools.Method(typeof(CardRemoveSequencer), nameof(CardRemoveSequencer.RemoveSequence)))
			);

			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Ldfld, AccessTools.Field(typeof(SpecialNodeHandler), nameof(SpecialNodeHandler.cardRemoveSequencer)));
			c.EmitDelegate<Func<IEnumerator, CardRemoveSequencer, IEnumerator>>(ActualPostfixKinda);
		}
		private static IEnumerator ActualPostfixKinda(IEnumerator sequence, CardRemoveSequencer __instance) {
			while(sequence.MoveNext()) {
				yield return sequence.Current;
			}
			AddCardToPool(__instance.sacrificeSlot.Card.Info);
		}

		[HarmonyPatch(typeof(SaveManager))]
		[HarmonyPatch("SaveToFile")]
		[HarmonyPrefix]
		static void SaveCorpseCardData() {
			SaveToDisk();
		}

		[HarmonyPatch(typeof(SaveManager))]
		[HarmonyPatch("LoadFromFile")]
		[HarmonyPrefix]
		static void LoadCorpseCardData() {
			LoadFromDisk();
		}

		#endregion

	}
}
