using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DiskCardGame;
using InscryptionAPI.Card;
using TDLib.Attributes;

namespace OmniBackport.SideDecks {
	public class CustomSideDeck : SpecialCardBehaviour {
		/// <summary>
		/// The cards to draw from the side deck for this battle.
		/// </summary>
		public virtual List<CardInfo> GetCardsToDraw() => new List<CardInfo>();
		public virtual void RemoveFirstCardToDraw() { }

		public override IEnumerator OnDrawn() {
			Card.SetInfo(GetCardForDraw());
			yield break;
		}

		public override bool RespondsToDrawn() {
			return true;
		}

		private CardInfo GetCardForDraw() {
			if(GetCardsToDraw().Count == 0) {
				MainPlugin.logger.LogInfo($"Player drawing a Squirrel");
				return CardLoader.GetCardByName("Squirrel");
			}

			CardInfo outp = GetCardsToDraw()[0];
			RemoveFirstCardToDraw();
			MainPlugin.logger.LogInfo($"Player drawing a {outp.name}");
			return outp;
		}
	}
}
