using System;
using System.Collections.Generic;
using System.Text;
using DiskCardGame;
using TDLib.Attributes;
using TDLib.Collections;
using InscryptionAPI.Card;

namespace OmniBackport.SideDecks {
	[AutoInit]
	public class OBSideDeck : CustomSideDeck {
		private static void Init() {
			TDLib.Events.EventsManager.BattleStarted += (x) => {
				MainPlugin.logger.LogInfo("Battle started; resetting side deck");
				CurrentSideDeck = null;
			};
		}

		public static SpecialTriggeredAbility Id { get; private set; } = SpecialTriggeredAbilityManager.Add(MainPlugin.GUID, nameof(OBSideDeck), typeof(OBSideDeck)).Id;

		private static List<CardInfo> CurrentSideDeck = null;
		public override List<CardInfo> GetCardsToDraw() {
			if(CurrentSideDeck == null) {
				CurrentSideDeck = new List<CardInfo>() { 
					CardLoader.GetCardByName("WizardBackport_MoxSapphire"),
					CardLoader.GetCardByName("WizardBackport_MoxSapphire"),
					CardLoader.GetCardByName("WizardBackport_MoxSapphire"),
					CardLoader.GetCardByName("WizardBackport_MoxSapphire"),
					CardLoader.GetCardByName("WizardBackport_MoxSapphire"),
					CardLoader.GetCardByName("WizardBackport_MoxRuby"),
					CardLoader.GetCardByName("WizardBackport_MoxRuby"),
					CardLoader.GetCardByName("WizardBackport_MoxRuby"),
					CardLoader.GetCardByName("WizardBackport_MoxRuby"),
					CardLoader.GetCardByName("WizardBackport_MoxRuby"),
				};
				CurrentSideDeck = (List<CardInfo>)CurrentSideDeck.Shuffle(SaveManager.SaveFile.GetCurrentRandomSeed());
			MainPlugin.logger.LogDebug($"Current side deck (OBSideDeck):");
			foreach(var card in CurrentSideDeck) {
				MainPlugin.logger.LogDebug(card.name);
			}
			}
			return CurrentSideDeck;
		}
		public override void RemoveFirstCardToDraw() {
			CurrentSideDeck.RemoveAt(0);
		}
	}
}
