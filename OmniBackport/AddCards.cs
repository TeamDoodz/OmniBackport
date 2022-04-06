using System;
using System.Collections.Generic;
using DiskCardGame;
using InscryptionAPI.Card;
using TDLib.Config;
using TDLib.GameContent;
using UnityEngine;
using System.IO;
using System.Text;
using OmniBackport.SideDecks;

namespace OmniBackport {
	static class AddCards {
		internal static Dictionary<CardTemple, List<string>> VanillaNonAct1Cards = new Dictionary<CardTemple, List<string>>() {
			{CardTemple.Nature, new List<string>() {
				"Hrokkall",
				"SquirrelBall",
				"Salmon",
			} },
			{CardTemple.Undead, new List<string>() {
				"Banshee",
				"Bonepile",
				"Bonehound",
				"BonelordHorn",
				"CoinLeft",
				"CoinRight",
				"DeadHand",
				"DeadPets",
				"Draugr",
				"DrownedSoul",
				"Family",
				"FrankNStein",
				"GhostShip",
				"Gravedigger",
				"HeadlessHorseman",
				"Mummy",
				"Necromancer",
				"Revenant",
				"Sarcophagus",
				"SkeletonMage", // skelemagus more like skelemogus ඞ
				"TombRobber",
				"Zombie",
			} },
			{CardTemple.Tech, new List<string>() {
				//TODO: Can't find gamblobot or double gunner on the JSONLoader list of cards.
				"AboveCurve",
				"AlarmBot",
				"Amoebot",
				"AttackConduit",
				"Automaton",
				"BatteryBot",
				"BoltHound",
				"Bombbot",
				"BombMaiden",
				"BustedPrinter",
				"CellBuff",
				"CellGift",
				"CellTri",
				"EnergyConduit",
				"EnergyRoller",
				"FactoryConduit",
				"GemExploder",
				"GemRipper",
				"GemsConduit",
				"GemShielder",
				"GiftBot",
				"HealerConduit",
				"Insectodrone",
				"LatcherBomb",
				"LatcherBrittle",
				"LatcherShield",
				"LeapBot",
				"MeatBot", // hehe meat
				"MineCart", // minecfaf?!?!?
				"NullConduit",
				"Ouroboros_Part3",
				"PlasmaGunner",
				"RoboMice",
				"RoboSkeleton",
				"SentryBot",
				"Shieldbot",
				"Shutterbug",
				"Sniper",
				"Steambot",
				"SwapBot",
				"TechMoxTriple",
				"Thickbot",
				"XformerBatBot",
				"XformerGrizzlyBot",
				"XformerPorcupineBot",
			} },
			{CardTemple.Wizard, new List<string>() {
				"BlueMage",
				"FlyingMage",
				"ForceMage",
				"GemFiend",
				"GreenMage",
				"MageKnight",
				"MarrowMage",
				"MasterBleene",
				"MasterGoranj",
				"MasterOrlu",
				"MoxEmerald",
				"MoxRuby",
				"MoxSapphire",
				"MuscleMage",
				"OrangeMage",
				"JuniorSage",
				"PracticeMage",
				"Pupil",
				"RubyGolem",
				"StimMage",
			} },
		};
		internal static Dictionary<CardTemple, string> SideDecks = new Dictionary<CardTemple, string> {
			{ CardTemple.Undead, "Skeleton" },
			{ CardTemple.Tech, "EmptyVessel" },
		};

		private static List<CardMetaCategory> GetCardMetas(bool isRare) {
			List<CardMetaCategory> result = new List<CardMetaCategory>();
			if(isRare) {
				result.Add(CardMetaCategory.Rare);
				return result;
			}
			result.Add(CardMetaCategory.TraderOffer);
			result.Add(CardMetaCategory.ChoiceNode);
			return result;
		}

		private static BasicConfigHelper<bool> SideDeckFindNormally = new BasicConfigHelper<bool>(MainPlugin.cfg, nameof(SideDeckFindNormally), "Allow for side deck cards (squrriel, skeleton, etc) to appear in choice nodes/trader offers.", false, "SideDecks");
		private static BasicConfigHelper<bool> UseCardList = new BasicConfigHelper<bool>(MainPlugin.cfg, nameof(UseCardList), "If this is true, the mod will look for a \"cards.csv\" file that contains a list of act 2/3 cards. Turning this on adds some overhead, so keep it off unless you want to modify the list. If you want to reset the list to factory settings, just delete it.", false);
		public static void AddAllCards(bool IncludeModded) {
			if(UseCardList.GetValue()) GetCardCSV();
			Dictionary<CardTemple, List<string>> AllCards = VanillaNonAct1Cards;
			foreach(var cards in AllCards) {
				foreach(var card in cards.Value) {
					CreateCard(card, cards.Key, true);
				}
			}
		}

		public static void AddAllSideDecks() {
			foreach(var sideDeck in SideDecks) {
				CreateCard(
					sideDeck.Value, 
					sideDeck.Key, 
					SideDeckFindNormally.GetValue(), 
					true,
					MainPlugin.cfg.Bind(
						"SideDecks",
						$"{sideDeck.Value}Points",
						sideDeck.Value == "Skeleton" ? -5 : 0, 
						"Challenge points given to the player when this side deck is used. Use a negative number for this deck to cost points."
					).Value
				);
			}
			// create mox side decks
			{
				CardInfo gencard = CardManager.New($"WizardBackport", "BGMoxDeck", "Bleene's Side Deck", 0, 1);

				gencard.specialAbilities.Add(BGSideDeck.Id);

				gencard.traits.Add(Trait.Terrain);
				gencard.traits.Add(Trait.Gem);

				CardInfo cardInfo = CardLoader.GetCardByName("MoxDualBG");
				gencard.pixelPortrait = cardInfo.pixelPortrait;
				Texture2D texture2D = cardInfo.GetPortrait(true, true);
				gencard.portraitTex = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));

				gencard.SetExtendedProperty("SideDeckValue", MainPlugin.cfg.Bind(
						"SideDecks",
						$"BGMoxDeckPoints",
						0,
						"Challenge points given to the player when this side deck is used. Use a negative number for this deck to cost points."
					).Value);

				gencard.metaCategories.Add(MainPlugin.SIDE_DECK_CATEGORY);
			}
			{
				CardInfo gencard = CardManager.New($"WizardBackport", "GOMoxDeck", "Goranj's Side Deck", 0, 1);

				gencard.specialAbilities.Add(GOSideDeck.Id);

				gencard.traits.Add(Trait.Terrain);
				gencard.traits.Add(Trait.Gem);

				CardInfo cardInfo = CardLoader.GetCardByName("MoxDualGO");
				gencard.pixelPortrait = cardInfo.pixelPortrait;
				Texture2D texture2D = cardInfo.GetPortrait(true, true);
				gencard.portraitTex = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));

				gencard.SetExtendedProperty("SideDeckValue", MainPlugin.cfg.Bind(
						"SideDecks",
						$"GOMoxDeckPoints",
						0,
						"Challenge points given to the player when this side deck is used. Use a negative number for this deck to cost points."
					).Value);

				gencard.metaCategories.Add(MainPlugin.SIDE_DECK_CATEGORY);
			}
			{
				CardInfo gencard = CardManager.New($"WizardBackport", "OBMoxDeck", "Orlu's Side Deck", 0, 1);

				gencard.specialAbilities.Add(OBSideDeck.Id);

				gencard.traits.Add(Trait.Terrain);
				gencard.traits.Add(Trait.Gem);

				CardInfo cardInfo = CardLoader.GetCardByName("MoxDualOB");
				gencard.pixelPortrait = cardInfo.pixelPortrait;
				Texture2D texture2D = cardInfo.GetPortrait(true, true);
				gencard.portraitTex = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));

				gencard.SetExtendedProperty("SideDeckValue", MainPlugin.cfg.Bind(
						"SideDecks",
						$"OBMoxDeckPoints",
						0,
						"Challenge points given to the player when this side deck is used. Use a negative number for this deck to cost points."
					).Value);

				gencard.metaCategories.Add(MainPlugin.SIDE_DECK_CATEGORY);
			}
			{
				CardInfo gencard = CardManager.New($"WizardBackport", "TriMoxDeck", "Magnus Side Deck", 0, 1);

				gencard.specialAbilities.Add(TriSideDeck.Id);

				gencard.traits.Add(Trait.Terrain);
				gencard.traits.Add(Trait.Gem);

				CardInfo cardInfo = CardLoader.GetCardByName("MoxTriple");
				gencard.pixelPortrait = cardInfo.pixelPortrait;
				Texture2D texture2D = cardInfo.GetPortrait(true, true);
				gencard.portraitTex = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));

				gencard.SetExtendedProperty("SideDeckValue", MainPlugin.cfg.Bind(
						"SideDecks",
						$"TriMoxDeckPoints",
						0,
						"Challenge points given to the player when this side deck is used. Use a negative number for this deck to cost points."
					).Value);

				gencard.metaCategories.Add(MainPlugin.SIDE_DECK_CATEGORY);
			}
		}

		private static void CreateCard(string card, CardTemple temple, bool makeObtainable, bool makeSideDeck = false, int sideDeckPoints = 0) {
			try {
				MainPlugin.logger.LogInfo($"Creating card {card} obtainable: {makeObtainable} side deck: {makeSideDeck}");

				CardInfo original = CardLoader.GetCardByName(card);
				CardInfo gencard = CardManager.New($"{temple}Backport", card, original.displayedName, original.baseAttack, original.baseHealth, null);

				// https://answers.unity.com/questions/650552/convert-a-texture2d-to-sprite.html
				Texture2D texture2D = original.GetPortrait(true, true);
				gencard.portraitTex = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
				gencard.pixelPortrait = original.pixelPortrait;

				gencard.abilities = original.abilities;
				gencard.specialAbilities = original.specialAbilities;
				gencard.specialStatIcon = original.specialStatIcon;
				gencard.tribes = original.tribes;

				gencard.cost = original.cost;
				gencard.bonesCost = original.bonesCost;
				gencard.energyCost = original.energyCost;
				gencard.gemsCost = original.gemsCost;

				gencard.iceCubeParams = original.iceCubeParams;
				gencard.evolveParams = original.evolveParams;

				gencard.metaCategories = new List<CardMetaCategory>();
				if(original.metaCategories.Contains(CardMetaCategory.Part3Random)) gencard.metaCategories.Add(CardMetaCategory.Part3Random);

				if(makeObtainable) gencard.metaCategories.AddRange(GetCardMetas(original.metaCategories.Contains(CardMetaCategory.Rare)));
				if(makeSideDeck) {
					gencard.metaCategories.Add(MainPlugin.SIDE_DECK_CATEGORY);
					gencard.SetExtendedProperty("SideDeckValue", -sideDeckPoints);
				}

				gencard.appearanceBehaviour = new List<CardAppearanceBehaviour.Appearance>();
				if(original.metaCategories.Contains(CardMetaCategory.Rare)) {
					gencard.appearanceBehaviour.Add(CardAppearanceBehaviour.Appearance.RareCardBackground);
				}

				gencard.traits = original.traits;
				if(original.traits.Contains(Trait.Terrain)) {
					if(gencard.baseAttack == 0) gencard.appearanceBehaviour.Add(CardAppearanceBehaviour.Appearance.TerrainLayout);
					gencard.appearanceBehaviour.Add(CardAppearanceBehaviour.Appearance.TerrainBackground);
				}
			} catch(Exception e) {
				MainPlugin.logger.LogError($"Error creating card {card}: {e}");
			}
		}

		private static void GetCardCSV() {
			if(!File.Exists(MainPlugin.assets.PathFor("cards","csv"))) {
				using (StreamWriter sw = File.CreateText(MainPlugin.assets.PathFor("cards", "csv"))) {
					sw.Write(GenerateCardCSV());
					sw.Close();
				}
				return;
			}

			VanillaNonAct1Cards = new Dictionary<CardTemple, List<string>>() {
				{CardTemple.Nature, new List<string>() { } },
				{CardTemple.Tech, new List<string>() { } },
				{CardTemple.Undead, new List<string>() { } },
				{CardTemple.Wizard, new List<string>() { } },
			};
			string[] fileData = MainPlugin.assets.LoadCSV("cards");
			foreach(string cardName in fileData) {
				try {
					VanillaNonAct1Cards[CardLoader.GetCardByName(cardName).temple].Add(cardName);
				} catch(Exception e) {
					MainPlugin.logger.LogWarning($"Error when reading card {cardName}: {e}\n\nMake sure the name was typed correctly and that the card exists.");
				}
			}
		}

		private static string GenerateCardCSV() {
			StringBuilder res = new StringBuilder();
			foreach(var cardName in VanillaNonAct1Cards) {
				res.Append(cardName.Value);
				res.Append(",\n");
			}
			res.Remove(res.Length - 2, 2); // remove the last ",\n"
			return res.ToString();
		}
	}
}
